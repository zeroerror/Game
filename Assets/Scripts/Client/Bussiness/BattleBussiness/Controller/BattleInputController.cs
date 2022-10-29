using UnityEngine;
using Game.Client.Bussiness.BattleBussiness.Facades;
using Game.Client.Bussiness.EventCenter;
using Game.Client.Bussiness.BattleBussiness.Generic;
using Game.Client.Bussiness.BattleBussiness.Interface;

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
            UIEventCenter.ShootAction += ((fireDir) =>
            {
                battleFacades.InputComponent.isPressShoot = true;
                battleFacades.InputComponent.fireDir = fireDir;
            });
            UIEventCenter.StopShootAction += (() =>
            {
                battleFacades.InputComponent.isPressShoot = false;
            });
            UIEventCenter.ReloadAction += (() => battleFacades.InputComponent.isPressWeaponReload = true);
            UIEventCenter.JumpAction += (() => battleFacades.InputComponent.isPressRoll = true);
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
            TickInput_Move();
            TickInput_Rotate();
            TickInput_Roll();
            TickInput_SwitchingView();
            TickInput_Pick();
            TickInput_Shoot();
            TickInput_Reload();
            TickInput_DropWeapon();

            battleFacades.InputComponent.Reset();
        }

        void TickInput_Move()
        {
            var owner = battleFacades.Repo.RoleRepo.Owner;
            if (owner == null || owner.IsDead) return;

            var input = battleFacades.InputComponent;
            if (input.moveAxis != Vector3.zero)
            {
                var moveAxis = input.moveAxis;

                var cameraView = battleFacades.Repo.FiledRepo.CurFieldEntity.CameraComponent.CurrentCameraView;
                Vector3 moveDir = battleFacades.Domain.InputDomain.GetMoveDirByCameraView(owner, moveAxis, cameraView);
                owner.MoveComponent.FaceTo(moveDir);

                if (!WillHitOtherRole(owner, moveDir))
                {
                    var rqs = battleFacades.Network.RoleReqAndRes;
                    rqs.SendReq_RoleMove(owner.IDComponent.EntityID, moveDir);
                }
            }
        }

        void TickInput_Roll()
        {
            var owner = battleFacades.Repo.RoleRepo.Owner;
            if (owner == null || owner.IsDead) return;

            var input = battleFacades.InputComponent;
            if (input.isPressRoll)
            {
                battleFacades.Network.RoleReqAndRes.SendReq_RoleRoll(owner);
            }
        }

        void TickInput_SwitchingView()
        {
            var owner = battleFacades.Repo.RoleRepo.Owner;
            if (owner == null || owner.IsDead) return;

            var input = battleFacades.InputComponent;
            if (input.isPressSwitchView)
            {
                //打开第一人称视角
                // TODO: 加切换视角的判定条件
                var fieldCameraComponent = battleFacades.Repo.FiledRepo.CurFieldEntity.CameraComponent;
                if (fieldCameraComponent.CurrentCameraView == CameraView.ThirdView) fieldCameraComponent.OpenFirstViewCam(owner.roleRenderer);
                else if (fieldCameraComponent.CurrentCameraView == CameraView.FirstView) fieldCameraComponent.OpenThirdViewCam(owner.roleRenderer);
            }
        }

        void TickInput_Pick()
        {
            var owner = battleFacades.Repo.RoleRepo.Owner;
            if (owner == null || owner.IsDead) return;

            var input = battleFacades.InputComponent;
            if (input.isPressPick)
            {
                // 拾取物品
                var domain = battleFacades.Domain.PhysicsDomain;
                var nearItemList = domain.GetHitItem_ColliderList(owner);
                float closestDis = float.MaxValue;
                GameObject closestGo = null;
                IPickable pick = null;
                Vector3 ownerPos = owner.MoveComponent.Position;

                nearItemList.ForEach((item) =>
                {
                    if (item.status == CollisionStatus.Exit) return;

                    var idc = item.gameObject.GetComponentInParent<IPickable>();
                    if (idc == null)
                    {
                        return;
                    }

                    var collider = item.GetCollider();
                    var dis = Vector3.Distance(collider.transform.position, ownerPos);
                    if (dis < closestDis)
                    {
                        closestDis = dis;
                        closestGo = item.gameObject;
                        pick = idc;
                    }
                });

                if (closestGo != null)
                {
                    var rqs = battleFacades.Network.ItemReqAndRes;
                    rqs.SendReq_ItemPickUp(owner.IDComponent.EntityID, pick.EntityType, pick.EntityID);
                }
            }
        }

        void TickInput_Shoot()
        {
            var owner = battleFacades.Repo.RoleRepo.Owner;
            if (owner == null || owner.IsDead) return;

            var input = battleFacades.InputComponent;
            if (input.isPressShoot)
            {
                var curWeapon = owner.WeaponComponent.CurrentWeapon;
                if (curWeapon == null)
                {
                    return;
                }

                // - 默认射击方向为角色前方
                if (input.fireDir == Vector2.zero)
                {
                    var forward = owner.transform.forward;
                    input.fireDir = new Vector2(forward.x, forward.z);
                }
                else
                {
                    var faceDir = new Vector3(input.fireDir.x, 0, input.fireDir.y);
                    owner.transform.forward = faceDir;
                    owner.MoveComponent.FaceTo(faceDir);
                }

                var rqs = battleFacades.Network.WeaponReqAndRes;
                rqs.SendReq_WeaponShoot(owner.IDComponent.EntityID, curWeapon.ShootPointPos, input.fireDir);
            }
        }

        void TickInput_Reload()
        {
            var owner = battleFacades.Repo.RoleRepo.Owner;
            if (owner == null || owner.IsDead) return;

            var input = battleFacades.InputComponent;
            if (input.isPressWeaponReload)
            {
                // 换弹前判断流程
                var weaponComponent = owner.WeaponComponent;
                var animatorComponent = owner.roleRenderer.AnimatorComponent;
                Debug.Assert(weaponComponent.CurrentWeapon != null, "当前武器为空");
                if (owner.CanWeaponReload())
                {
                    var rqs = battleFacades.Network.WeaponReqAndRes;
                    rqs.SendReq_WeaponReload(owner);
                }
            }
        }

        void TickInput_DropWeapon()
        {
            var owner = battleFacades.Repo.RoleRepo.Owner;
            if (owner == null || owner.IsDead) return;

            var input = battleFacades.InputComponent;
            if (input.isPressDropWeapon)
            {
                // 丢弃武器前判断流程 TODO:由服务端鉴定
                var rqs = battleFacades.Network.WeaponReqAndRes;
                rqs.SendReq_WeaponDrop(owner);
            }
        }

        void TickInput_Rotate()
        {
            var owner = battleFacades.Repo.RoleRepo.Owner;
            if (owner == null || owner.IsDead) return;

            //客户端鉴权旋转角度同步
            var rqs = battleFacades.Network.RoleReqAndRes;
            rqs.SendReq_RoleRotate(owner);
        }

        bool WillHitOtherRole(BattleRoleLogicEntity roleEntity, Vector3 moveDir)
        {
            var roleRepo = battleFacades.Repo.RoleRepo;
            var array = roleRepo.GetAll();
            for (int i = 0; i < array.Length; i++)
            {
                var r = array[i];
                if (r.IDComponent.EntityID == roleEntity.IDComponent.EntityID) continue;

                var pos1 = r.MoveComponent.Position;
                var pos2 = roleEntity.MoveComponent.Position;
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