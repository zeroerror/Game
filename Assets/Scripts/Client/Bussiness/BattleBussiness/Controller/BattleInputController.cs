using UnityEngine;
using Game.Client.Bussiness.BattleBussiness.Facades;
using Game.Client.Bussiness.BattleBussiness.Interface;
using Game.Client.Bussiness.EventCenter;

namespace Game.Client.Bussiness.BattleBussiness.Controller
{

    public class BattleInputController
    {

        BattleFacades battleFacades;
        float fixedDeltaTime => UnityEngine.Time.fixedDeltaTime;

        public BattleInputController()
        {
            UIEventCenter.MoveAction += ((moveAxis) => battleFacades.InputComponent.moveAxis = new Vector3(moveAxis.x, 0, moveAxis.y));
            UIEventCenter.PickAction += (() => battleFacades.InputComponent.isPressPick = true);
            UIEventCenter.FireAction += (() => battleFacades.InputComponent.isPressFire = true);
            UIEventCenter.ReloadAction += (() => battleFacades.InputComponent.isPressWeaponReload = true);
            UIEventCenter.JumpAction += (() => battleFacades.InputComponent.isPressJump = true);
            UIEventCenter.DropWeaponAction += (() => battleFacades.InputComponent.isPressDropWeapon = true);
        }

        public void Inject(BattleFacades battleFacades)
        {
            this.battleFacades = battleFacades;
        }

        public void Tick()
        {
            Tick_Input();
        }

        void Tick_Input()
        {
            //没有角色就没有移动
            var owner = battleFacades.Repo.RoleRepo.Owner;
            if (owner == null || owner.IsDead) return;

            var input = battleFacades.InputComponent;
            if (input.isPressJump)
            {
                battleFacades.Network.RoleReqAndRes.SendReq_RoleJump(owner.IDComponent.EntityId);
            }
            if (input.isPressSwitchView)
            {
                //打开第一人称视角
                // TODO: 加切换视角的判定条件
                var fieldCameraComponent = battleFacades.Repo.FiledRepo.CurFieldEntity.CameraComponent;
                if (fieldCameraComponent.CurrentCameraView == CameraView.ThirdView) fieldCameraComponent.OpenFirstViewCam(owner.roleRenderer);
                else if (fieldCameraComponent.CurrentCameraView == CameraView.FirstView) fieldCameraComponent.OpenThirdViewCam(owner.roleRenderer);
            }
            if (input.isPressPick)
            {
                // 拾取物品
                var domain = battleFacades.Domain.PhysicsDomain;
                var nearItemList = domain.GetHitItem_ColliderList(owner);
                float closestDis = float.MaxValue;
                GameObject closestGo = null;
                IPickable closestPickable = null;
                Vector3 ownerPos = owner.MoveComponent.CurPos;
                Debug.Log($"想要拾取物品，周围可拾取数量为:{nearItemList.Count}");

                nearItemList.ForEach((item) =>
                {
                    if (item.status == CollisionStatus.Exit) return;

                    var pickable = item.gameObject.GetComponentInParent<IPickable>();
                    if (pickable == null) return;

                    var collider = item.Collider;
                    Debug.Log($"item:{collider.transform.parent.name}");

                    var dis = Vector3.Distance(collider.transform.position, ownerPos);
                    if (dis < closestDis)
                    {
                        closestDis = dis;
                        closestGo = item.gameObject;
                        closestPickable = pickable;
                    }
                });

                if (closestGo != null)
                {
                    var rqs = battleFacades.Network.ItemReqAndRes;
                    rqs.SendReq_ItemPickUp(owner.IDComponent.EntityId, closestPickable.ItemType, closestPickable.EntityId);
                }
            }
            if (input.isPressFire)
            {
                // 射击前 
                // 1.客户端判断流程
                var weaponComponent = owner.WeaponComponent;
                Debug.Assert(weaponComponent.CurrentWeapon != null, "当前武器为空，无法射击");
                Debug.Assert(!weaponComponent.IsReloading, "换弹中，无法射击");
                if (weaponComponent.CurrentWeapon != null && !weaponComponent.IsReloading)
                {
                    var curCamView = battleFacades.Repo.FiledRepo.CurFieldEntity.CameraComponent.CurrentCameraView;
                    var inputDomain = battleFacades.Domain.BattleInputDomain;
                    Vector3 targetPos = inputDomain.GetShotPointByCameraView(curCamView, owner);
                    // 2.服务端流程
                    var rqs = battleFacades.Network.WeaponReqAndRes;
                    rqs.SendReq_WeaponShoot(owner.IDComponent.EntityId, targetPos);
                }

            }
            if (input.isPressWeaponReload)
            {
                // 换弹前判断流程
                var weaponComponent = owner.WeaponComponent;
                Debug.Assert(weaponComponent.CurrentWeapon != null, "当前武器为空");
                // Debug.Assert(!weaponComponent.IsReloading, "当前武器已经在换弹中");
                if (owner.CanWeaponReload())
                {
                    weaponComponent.BeginReloading();
                    var rqs = battleFacades.Network.WeaponReqAndRes;
                    rqs.SendReq_WeaponReload(owner);
                }
            }
            if (input.isPressDropWeapon)
            {
                // 丢弃武器前判断流程 TODO:由服务端鉴定
                var rqs = battleFacades.Network.WeaponReqAndRes;
                rqs.SendReq_WeaponDrop(owner);
            }

            if (input.moveAxis != Vector3.zero)
            {
                var moveAxis = input.moveAxis;

                var cameraView = battleFacades.Repo.FiledRepo.CurFieldEntity.CameraComponent.CurrentCameraView;
                Vector3 moveDir = battleFacades.Domain.BattleInputDomain.GetMoveDirByCameraView(owner, moveAxis, cameraView);
                owner.MoveComponent.FaceTo(moveDir);


                if (!WillHitOtherRole(owner, moveDir))
                {
                    var rqs = battleFacades.Network.RoleReqAndRes;
                    if (owner.MoveComponent.IsEulerAngleNeedFlush())
                    {
                        owner.MoveComponent.FlushEulerAngle();
                        //客户端鉴权旋转角度同步
                        rqs.SendReq_RoleRotate(owner);
                    }

                    rqs.SendReq_RoleMove(owner.IDComponent.EntityId, moveDir);
                }
            }

            input.Reset();

            if (owner.MoveComponent.IsEulerAngleNeedFlush())
            {
                owner.MoveComponent.FlushEulerAngle();
                //客户端鉴权旋转角度同步
                var rqs = battleFacades.Network.RoleReqAndRes;
                rqs.SendReq_RoleRotate(owner);
            }

        }

        bool WillHitOtherRole(BattleRoleLogicEntity roleEntity, Vector3 moveDir)
        {
            var roleRepo = battleFacades.Repo.RoleRepo;
            var array = roleRepo.GetAll();
            for (int i = 0; i < array.Length; i++)
            {
                var r = array[i];
                if (r.IDComponent.EntityId == roleEntity.IDComponent.EntityId) continue;

                var pos1 = r.MoveComponent.CurPos;
                var pos2 = roleEntity.MoveComponent.CurPos;
                if (Vector3.Distance(pos1, pos2) < 1f)
                {
                    var betweenV = pos1 - pos2;
                    betweenV.Normalize();
                    moveDir.Normalize();
                    var cosVal = Vector3.Dot(moveDir, betweenV);
                    if (cosVal > 0) return true;
                }
            }

            return false;
        }

    }


}