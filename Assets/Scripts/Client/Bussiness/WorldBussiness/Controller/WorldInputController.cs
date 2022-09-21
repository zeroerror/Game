using UnityEngine;
using Game.Client.Bussiness.WorldBussiness.Facades;
using Game.Client.Bussiness.WorldBussiness.Interface;
using Game.Client.Bussiness.WorldBussiness.Generic;

namespace Game.Client.Bussiness.WorldBussiness.Controller
{

    public class WorldInputController
    {

        WorldFacades worldFacades;
        float fixedDeltaTime => UnityEngine.Time.fixedDeltaTime;

        public void Inject(WorldFacades worldFacades)
        {
            this.worldFacades = worldFacades;
        }

        public void Tick()
        {
            Tick_Input();
        }

        void Tick_Input()
        {
            //没有角色就没有移动
            var owner = worldFacades.Repo.RoleRepo.Owner;
            if (owner == null || owner.IsDead) return;

            var input = worldFacades.InputComponent;
            if (input.isPressJump)
            {
                byte rid = owner.EntityId;
                worldFacades.Network.WorldRoleReqAndRes.SendReq_WRoleJump(rid);
            }
            if (input.isPressSwitchView)
            {
                //打开第一人称视角
                // TODO: 加切换视角的判定条件
                var fieldCameraComponent = worldFacades.Repo.FiledRepo.CurFieldEntity.CameraComponent;
                if (fieldCameraComponent.CurrentCameraView == CameraView.ThirdView) fieldCameraComponent.OpenFirstViewCam(owner.roleRenderer);
                else if (fieldCameraComponent.CurrentCameraView == CameraView.FirstView) fieldCameraComponent.OpenThirdViewCam(owner.roleRenderer);
            }
            if (input.isPressPickUpItem)
            {
                // 拾取物品
                var domain = worldFacades.Domain.PhysicsDomain;
                var nearItemList = domain.GetHitItem_ColliderList(owner);
                float closestDis = float.MaxValue;
                Collider closestCollider = null;
                IPickable closestPickable = null;
                Vector3 ownerPos = owner.MoveComponent.CurPos;
                Debug.Log($"想要拾取物品，周围可拾取数量为:{nearItemList.Count}");

                nearItemList.ForEach((item) =>
                {
                    if (item.isEnter == CollisionStatus.Exit) return;

                    var pickable = item.collider.GetComponentInParent<IPickable>();
                    if (pickable == null) return;

                    Debug.Log($"item:{item.collider.transform.parent.name}");

                    var collider = item.collider;
                    var dis = Vector3.Distance(collider.transform.position, ownerPos);
                    if (dis < closestDis)
                    {
                        closestDis = dis;
                        closestCollider = item.collider;
                        closestPickable = pickable;
                    }
                });

                if (closestCollider != null)
                {
                    var rqs = worldFacades.Network.ItemReqAndRes;
                    rqs.SendReq_ItemPickUp(owner.EntityId, closestPickable.ItemType, closestPickable.EntityId);
                }
            }
            if (input.isPressShoot)
            {
                // 射击前 
                // 1.客户端判断流程
                var weaponComponent = owner.WeaponComponent;
                Debug.Assert(weaponComponent.CurrentWeapon != null, "当前武器为空，无法射击");
                Debug.Assert(!weaponComponent.IsReloading, "当前武器换弹中，无法射击");
                if (weaponComponent.CurrentWeapon != null && !weaponComponent.IsReloading)
                {
                    var curCamView = worldFacades.Repo.FiledRepo.CurFieldEntity.CameraComponent.CurrentCameraView;
                    var inputDomain = worldFacades.Domain.WorldInputDomain;
                    Vector3 targetPos = inputDomain.GetShotPointByCameraView(curCamView, owner);
                    // 2.服务端流程
                    var rqs = worldFacades.Network.WeaponReqAndRes;
                    rqs.SendReq_WeaponShoot(owner.EntityId, targetPos);
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
                    weaponComponent.SetReloading(true);
                    var rqs = worldFacades.Network.WeaponReqAndRes;
                    rqs.SendReq_WeaponReload(owner);
                }
            }
            if (input.isPressDropWeapon)
            {
                // 丢弃武器前判断流程 TODO:由服务端鉴定
                var rqs = worldFacades.Network.WeaponReqAndRes;
                rqs.SendReq_WeaponDrop(owner);
            }

            if (input.moveAxis != Vector3.zero)
            {
                var moveAxis = input.moveAxis;

                var cameraView = worldFacades.Repo.FiledRepo.CurFieldEntity.CameraComponent.CurrentCameraView;
                Vector3 moveDir = worldFacades.Domain.WorldInputDomain.GetMoveDirByCameraView(owner, moveAxis, cameraView);
                owner.MoveComponent.FaceTo(moveDir);


                if (!WillHitOtherRole(owner, moveDir))
                {
                    var rqs = worldFacades.Network.WorldRoleReqAndRes;
                    if (owner.MoveComponent.IsEulerAngleNeedFlush())
                    {
                        owner.MoveComponent.FlushEulerAngle();
                        //客户端鉴权旋转角度同步
                        rqs.SendReq_WRoleRotate(owner);
                    }

                    byte rid = owner.EntityId;
                    rqs.SendReq_WRoleMove(rid, moveDir);
                }
            }

            input.Reset();

            if (owner.MoveComponent.IsEulerAngleNeedFlush())
            {
                owner.MoveComponent.FlushEulerAngle();
                //客户端鉴权旋转角度同步
                var rqs = worldFacades.Network.WorldRoleReqAndRes;
                rqs.SendReq_WRoleRotate(owner);
            }

        }

        bool WillHitOtherRole(WorldRoleLogicEntity roleEntity, Vector3 moveDir)
        {
            var roleRepo = worldFacades.Repo.RoleRepo;
            var array = roleRepo.GetAll();
            for (int i = 0; i < array.Length; i++)
            {
                var r = array[i];
                if (r.EntityId == roleEntity.EntityId) continue;

                var pos1 = r.MoveComponent.CurPos;
                var pos2 = roleEntity.MoveComponent.CurPos;
                if (Vector3.Distance(pos1, pos2) < 1f)
                {
                    var betweenV = pos1 - pos2;
                    betweenV.Normalize();
                    moveDir.Normalize();
                    var cosVal = Vector3.Dot(moveDir, betweenV);
                    Debug.Log(cosVal);
                    if (cosVal > 0) return true;
                }
            }

            return false;
        }

    }


}