using System.Collections.Generic;
using UnityEngine;
using Game.Protocol.Battle;
using Game.Server.Bussiness.BattleBussiness.Facades;
using Game.Client.Bussiness.BattleBussiness;
using Game.Client.Bussiness.BattleBussiness.Generic;

namespace Game.Server.Bussiness.BattleBussiness
{

    public class BattleWeaponController
    {

        BattleServerFacades serverFacades;

        // NetWorkd Info
        public int ServeFrame => serverFacades.Network.ServeFrame;
        List<int> ConnIdList => serverFacades.Network.connIdList;

        // 记录所有武器射击帧
        Dictionary<long, BattleWeaponShootReqMsg> weaponShootMsgDic;

        // 记录所有武器装弹帧
        Dictionary<long, BattleWeaponReloadReqMsg> weaponReloadMsgDic;

        // 记录所有武器丢弃帧
        Dictionary<long, BattleWeaponDropReqMsg> weaponDropMsgDic;

        public BattleWeaponController()
        {
            weaponShootMsgDic = new Dictionary<long, BattleWeaponShootReqMsg>();
            weaponReloadMsgDic = new Dictionary<long, BattleWeaponReloadReqMsg>();
            weaponDropMsgDic = new Dictionary<long, BattleWeaponDropReqMsg>();
        }

        public void Inject(BattleServerFacades battleFacades)
        {
            this.serverFacades = battleFacades;

            var weaponRqs = battleFacades.Network.WeaponReqAndRes;
            weaponRqs.RegistReq_WeaponShoot(OnWeaponShoot);
            weaponRqs.RegistReq_WeaponReload(OnWeaponReload);
            weaponRqs.RegistReq_WeaponDrop(OnWeaponDrop);
        }

        public void Tick(float fixedDeltaTime)
        {
            Tick_WeaponShoot();
            Tick_WeaponReloadBegin();
            Tick_ReloadingFrame();
            Tick_WeaponDrop();

            var allRole = serverFacades.BattleFacades.Repo.RoleLogicRepo;
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
                var key = GetKey(connId);

                if (weaponShootMsgDic.TryGetValue(key, out var msg))
                {
                    var battleFacades = serverFacades.BattleFacades;
                    var weaponRepo = battleFacades.Repo.WeaponRepo;

                    var weaponRqs = serverFacades.Network.WeaponReqAndRes;
                    var bulletRqs = serverFacades.Network.BulletReqAndRes;

                    var weaponID = msg.weaponID;
                    var firePointPosX = msg.firePointPosX;
                    var firePointPosY = msg.firePointPosY;
                    var firePointPosZ = msg.firePointPosZ;
                    var fireDirX = msg.fireDirX;
                    var fireDirZ = msg.fireDirZ;

                    if (weaponRepo.TryGet(weaponID, out var weapon))
                    {
                        var roleRepo = battleFacades.Repo.RoleLogicRepo;
                        var master = roleRepo.Get(weapon.MasterID);
                        if (!master.CanWeaponShoot())
                        {
                            return;
                        }

                        var weaponComponent = master.WeaponComponent;
                        if (weaponComponent.TryWeaponShoot())    //TODO: 逻辑应该在状态机判断
                        {
                            Vector3 fireDir = new Vector3(fireDirX / 100f, 0, fireDirZ / 100f);
                            var startPos = new Vector3(firePointPosX / 10000f, firePointPosY / 10000f, firePointPosZ / 10000f);
                            var curWeapon = weaponComponent.CurWeapon;
                            var bulletType = curWeapon.BulletType;
                            var idService = battleFacades.IDService;
                            var bulletEntityId = idService.GetAutoIDByEntityType(EntityType.Bullet);
                            var weaponEntityID = weapon.IDComponent.EntityID;

                            var bulletLogicDomain = battleFacades.Domain.BulletLogicDomain;
                            var bulletLogic = bulletLogicDomain.SpawnLogic(bulletType, bulletEntityId, startPos);
                            bulletLogicDomain.ShootByWeapon(bulletLogic, weaponID, fireDir);

                            master.InputComponent.SetShootDir(fireDir);
                            master.StateComponent.EnterShooting(curWeapon.FreezeMaintainFrame, curWeapon.BreakFrame);

                            ConnIdList.ForEach((connId) =>
                            {
                                weaponRqs.SendRes_WeaponShoot(connId, weaponID);
                                bulletRqs.SendRes_BulletSpawn(connId, bulletLogic);
                            });
                        }
                    }
                }

            });

        }

        void Tick_WeaponReloadBegin()
        {
            ConnIdList.ForEach((connId) =>
            {
                var key = GetKey(connId);

                if (weaponReloadMsgDic.TryGetValue(key, out var msg))
                {
                    var weaponRepo = serverFacades.BattleFacades.Repo.WeaponRepo;
                    var roleRepo = serverFacades.BattleFacades.Repo.RoleLogicRepo;
                    var masterId = msg.masterId;

                    if (roleRepo.TryGet(masterId, out var master) && master.CanWeaponReload())
                    {
                        master.InputComponent.pressReload = true;
                    }
                }
            });
        }

        void Tick_ReloadingFrame()
        {
            var allRole = serverFacades.BattleFacades.Repo.RoleLogicRepo;
            var rqs = serverFacades.Network.WeaponReqAndRes;
            allRole.Foreach((role) =>
            {
                if (role.WeaponComponent.IsReloading)
                {
                    var curWeapon = role.WeaponComponent.CurWeapon;
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
                var key = GetKey(connId);

                if (weaponDropMsgDic.TryGetValue(key, out var msg))
                {
                    var weaponRepo = serverFacades.BattleFacades.Repo.WeaponRepo;
                    var roleRepo = serverFacades.BattleFacades.Repo.RoleLogicRepo;
                    var rqs = serverFacades.Network.WeaponReqAndRes;
                    var entityID = msg.entityID;
                    var masterId = msg.masterId;
                    if (roleRepo.TryGet(masterId, out var master)
                        && master.WeaponComponent.TryDropWeapon(entityID, out var weapon))
                    {
                        var itemDomain = serverFacades.BattleFacades.Domain.ItemDomain;
                        itemDomain.DropWeaponToItem(weapon);

                        ConnIdList.ForEach((connId) =>
                        {
                            rqs.SendRes_WeaponDrop(connId, ServeFrame, masterId, entityID);
                        });
                    }
                }
            });
        }

        #endregion

        #region [Req]
        void OnWeaponShoot(int connId, BattleWeaponShootReqMsg msg)
        {
            lock (weaponShootMsgDic)
            {
                var key = GetKey(connId);

                if (!weaponShootMsgDic.TryGetValue(key, out var _))
                {
                    weaponShootMsgDic[key] = msg;
                }
            }
        }

        void OnWeaponReload(int connId, BattleWeaponReloadReqMsg msg)
        {
            lock (weaponReloadMsgDic)
            {
                long key = (long)ServeFrame << 32;
                key |= (long)connId;

                if (!weaponReloadMsgDic.TryGetValue(key, out var _))
                {
                    weaponReloadMsgDic[key] = msg;
                }
            }
        }

        void OnWeaponDrop(int connId, BattleWeaponDropReqMsg msg)
        {
            lock (weaponDropMsgDic)
            {
                var key = GetKey(connId);

                if (!weaponDropMsgDic.TryGetValue(key, out var _))
                {
                    weaponDropMsgDic[key] = msg;
                }

                Debug.Log("收到武器丢弃请求");
            }
        }

        #endregion

        long GetKey(int connId)
        {
            long key = (long)ServeFrame << 32;
            key |= (long)connId;
            return key;
        }

    }

}
