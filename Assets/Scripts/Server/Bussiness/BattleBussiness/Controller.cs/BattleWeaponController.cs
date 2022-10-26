using System.Collections.Generic;
using UnityEngine;
using Game.Protocol.Battle;
using Game.Server.Bussiness.BattleBussiness.Facades;
using Game.Client.Bussiness.BattleBussiness.Repo;
using Game.Client.Bussiness.BattleBussiness;

namespace Game.Server.Bussiness.BattleBussiness
{

    public class BattleWeaponController
    {

        BattleServerFacades battleFacades;

        // NetWorkd Info
        public int ServeFrame => battleFacades.Network.ServeFrame;
        List<int> ConnIdList => battleFacades.Network.connIdList;

        // 记录所有武器射击帧
        Dictionary<long, FrameWeaponShootReqMsg> weaponShootMsgDic;

        // 记录所有武器装弹帧
        Dictionary<long, FrameWeaponReloadReqMsg> weaponReloadMsgDic;

        // 记录所有武器丢弃帧
        Dictionary<long, FrameWeaponDropReqMsg> weaponDropMsgDic;

        public BattleWeaponController()
        {
            weaponShootMsgDic = new Dictionary<long, FrameWeaponShootReqMsg>();
            weaponReloadMsgDic = new Dictionary<long, FrameWeaponReloadReqMsg>();
            weaponDropMsgDic = new Dictionary<long, FrameWeaponDropReqMsg>();
        }

        public void Inject(BattleServerFacades battleFacades, float fixedDeltaTime)
        {
            this.battleFacades = battleFacades;

            var weaponRqs = battleFacades.Network.WeaponReqAndRes;
            weaponRqs.RegistReq_WeaponShoot(OnWeaponShoot);
            weaponRqs.RegistReq_WeaponReload(OnWeaponReload);
            weaponRqs.RegistReq_WeaponDrop(OnWeaponDrop);
        }

        public void Tick()
        {
            Tick_WeaponShoot();
            Tick_WeaponReloadBegin();
            Tick_ReloadingFrame();
            Tick_WeaponDrop();

            var allRole = battleFacades.BattleFacades.Repo.RoleRepo;
            allRole.Foreach((role) =>
            {
                var WeaponComponent = role.WeaponComponent;
                if (WeaponComponent.IsReloading)
                {

                }
            });

        }

        #region [Tick]

        void Tick_WeaponShoot()
        {
            ConnIdList.ForEach((connId) =>
            {
                long key = (long)ServeFrame << 32;
                key |= (long)connId;

                if (weaponShootMsgDic.TryGetValue(key, out var msg))
                {
                    var clientFacades = battleFacades.BattleFacades;
                    var weaponRepo = clientFacades.Repo.WeaponRepo;
                    var roleRepo = clientFacades.Repo.RoleRepo;

                    var weaponRqs = battleFacades.Network.WeaponReqAndRes;
                    var bulletRqs = battleFacades.Network.BulletReqAndRes;

                    var masterId = msg.masterId;
                    var firePointPosX = msg.firePointPosX;
                    var firePointPosY = msg.firePointPosY;
                    var firePointPosZ = msg.firePointPosZ;
                    var fireDirX = msg.fireDirX;
                    var fireDirZ = msg.fireDirZ;

                    if (roleRepo.TryGetByEntityId(masterId, out var master) && clientFacades.Domain.RoleDomain.CanRoleShoot(master))
                    {
                        if (master.WeaponComponent.TryWeaponShoot())    //TODO: 逻辑应该在状态机判断
                        {
                            Vector3 fireDir = new Vector3(fireDirX / 100f, 0, fireDirZ / 100f);
                            master.InputComponent.SetShootDir(fireDir);
                            master.StateComponent.EnterFiring(5);

                            var bulletType = master.WeaponComponent.CurrentWeapon.bulletType;
                            var bulletEntityId = clientFacades.Repo.BulletRepo.AutoEntityID;
                            var startPos = new Vector3(firePointPosX / 10000f, firePointPosY / 10000f, firePointPosZ / 10000f);

                            var bulletEntity = clientFacades.Domain.BulletDomain.SpawnBullet(bulletType, bulletEntityId, masterId, startPos, fireDir);

                            ConnIdList.ForEach((connId) =>
                            {
                                weaponRqs.SendRes_WeaponShoot(connId, masterId);
                                bulletRqs.SendRes_BulletSpawn(connId,bulletEntity);
                            });
                            Debug.Log($"生成子弹bulletType:{bulletType.ToString()} bulletId:{bulletEntityId}  MasterWRid:{masterId}  起点：{startPos} 飞行方向:{fireDir}");
                        }
                    }
                }

            });

        }

        void Tick_WeaponReloadBegin()
        {
            ConnIdList.ForEach((connId) =>
            {
                long key = (long)ServeFrame << 32;
                key |= (long)connId;

                if (weaponReloadMsgDic.TryGetValue(key, out var msg))
                {
                    var weaponRepo = battleFacades.BattleFacades.Repo.WeaponRepo;
                    var roleRepo = battleFacades.BattleFacades.Repo.RoleRepo;
                    var masterId = msg.masterId;

                    if (roleRepo.TryGetByEntityId(masterId, out var master) && master.CanWeaponReload())
                    {
                        master.InputComponent.pressReload = true;
                    }
                }
            });
        }

        void Tick_ReloadingFrame()
        {
            var allRole = battleFacades.BattleFacades.Repo.RoleRepo;
            var rqs = battleFacades.Network.WeaponReqAndRes;
            allRole.Foreach((role) =>
            {
                if (role.WeaponComponent.IsReloading)
                {
                    var curWeapon = role.WeaponComponent.CurrentWeapon;
                    if (curWeapon.CurReloadingFrame > 0)
                    {
                        curWeapon.ReduceCurReloadingFrame();
                    }
                    else
                    {
                        int reloadBulletNum = role.ReloadBulletsToWeapon(curWeapon);
                        ConnIdList.ForEach((connId) =>
                        {
                            rqs.SendRes_WeaponReloaded(connId, ServeFrame, role.IDComponent.EntityID, reloadBulletNum);
                        });
                    }
                }
            });
        }

        void Tick_WeaponDrop()
        {
            ConnIdList.ForEach((connId) =>
            {
                long key = (long)ServeFrame << 32;
                key |= (long)connId;

                if (weaponDropMsgDic.TryGetValue(key, out var msg))
                {
                    var weaponRepo = battleFacades.BattleFacades.Repo.WeaponRepo;
                    var roleRepo = battleFacades.BattleFacades.Repo.RoleRepo;
                    var rqs = battleFacades.Network.WeaponReqAndRes;
                    var entityId = msg.entityId;
                    var masterId = msg.masterId;
                    if (roleRepo.TryGetByEntityId(masterId, out var master)
                        && master.WeaponComponent.TryDropWeapon(entityId, out var weapon))
                    {
                        // 服务器逻辑
                        battleFacades.BattleFacades.Domain.WeaponDomain.ReuseWeapon(weapon, master.MoveComponent.Position);

                        ConnIdList.ForEach((connId) =>
                        {
                            rqs.SendRes_WeaponDrop(connId, ServeFrame, masterId, entityId);
                        });
                    }
                }
            });
        }

        #endregion

        #region [Req]
        void OnWeaponShoot(int connId, FrameWeaponShootReqMsg msg)
        {
            lock (weaponShootMsgDic)
            {
                long key = (long)ServeFrame << 32;
                key |= (long)connId;

                if (!weaponShootMsgDic.TryGetValue(key, out var _))
                {
                    weaponShootMsgDic[key] = msg;
                }
                Debug.Log("收到武器射击请求");
            }
        }

        void OnWeaponReload(int connId, FrameWeaponReloadReqMsg msg)
        {
            lock (weaponReloadMsgDic)
            {
                long key = (long)ServeFrame << 32;
                key |= (long)connId;

                if (!weaponReloadMsgDic.TryGetValue(key, out var _))
                {
                    weaponReloadMsgDic[key] = msg;
                }

                Debug.Log("收到武器换弹请求");
            }
        }

        void OnWeaponDrop(int connId, FrameWeaponDropReqMsg msg)
        {
            lock (weaponDropMsgDic)
            {
                long key = (long)ServeFrame << 32;
                key |= (long)connId;

                if (!weaponDropMsgDic.TryGetValue(key, out var _))
                {
                    weaponDropMsgDic[key] = msg;
                }

                Debug.Log("收到武器丢弃请求");
            }
        }

        #endregion

    }


}
