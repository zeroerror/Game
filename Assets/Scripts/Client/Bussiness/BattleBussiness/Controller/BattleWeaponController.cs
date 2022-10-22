using UnityEngine;
using Game.Client.Bussiness.BattleBussiness.Facades;
using Game.Protocol.Battle;
using System.Collections.Generic;

namespace Game.Client.Bussiness.BattleBussiness.Controller
{

    public class BattleWeaponController
    {

        BattleFacades battleFacades;
        float fixedDeltaTime => UnityEngine.Time.fixedDeltaTime;


        // 服务器下发的武器射击队列
        Queue<FrameWeaponShootResMsg> weaponShootQueue;
        // 服务器下发的武器换弹队列
        Queue<FrameWeaponReloadResMsg> weaponReloadQueue;
        // 服务器下发的武器丢弃队列
        Queue<FrameWeaponDropResMsg> weaponDropQueue;

        public BattleWeaponController()
        {
            weaponShootQueue = new Queue<FrameWeaponShootResMsg>();
            weaponReloadQueue = new Queue<FrameWeaponReloadResMsg>();
            weaponDropQueue = new Queue<FrameWeaponDropResMsg>();
        }

        public void Inject(BattleFacades battleFacades)
        {
            this.battleFacades = battleFacades;


            var weaponRqs = battleFacades.Network.WeaponReqAndRes;
            weaponRqs.RegistRes_WeaponShoot(OnWeaponShoot);
            weaponRqs.RegistRes_WeaponReload(OnWeaponReload);
            weaponRqs.RegistRes_WeaponDrop(OnWeaponDrop);
        }

        public void Tick()
        {
            Tick_WeaponShoot();
            Tick_WeaponReload();
            Tick_WeaponDrop();
        }

        #region [Tick]

        void Tick_WeaponShoot()
        {
            while (weaponShootQueue.TryPeek(out var msg))
            {
                weaponShootQueue.Dequeue();

                var roleRepo = battleFacades.Repo.RoleRepo;
                var master = roleRepo.GetByEntityId(msg.masterId);
                if (master.WeaponComponent.TryWeaponShoot())
                {
                    Debug.Log($"角色:{msg.masterId}射击");
                }
            }
        }

        void Tick_WeaponReload()
        {
            while (weaponReloadQueue.TryPeek(out var msg))
            {
                weaponReloadQueue.Dequeue();

                var roleRepo = battleFacades.Repo.RoleRepo;
                var master = roleRepo.GetByEntityId(msg.masterId);
                var reloadBulletNum = msg.reloadBulletNum;
                master.WeaponComponent.FinishReloading(reloadBulletNum);
            }
        }

        void Tick_WeaponDrop()
        {
            while (weaponDropQueue.TryPeek(out var msg))
            {
                weaponDropQueue.Dequeue();
                var master = battleFacades.Repo.RoleRepo.GetByEntityId(msg.masterId);
                master.WeaponComponent.TryDropWeapon(msg.entityId, out var weaponEntity);
                battleFacades.Domain.WeaponDomain.ReuseWeapon(weaponEntity, master.MoveComponent.Position);
            }
        }

        #endregion

        #region [Server]

        void OnWeaponShoot(FrameWeaponShootResMsg msg)
        {
            Debug.Log($"加入武器射击队列");
            weaponShootQueue.Enqueue(msg);
        }

        void OnWeaponReload(FrameWeaponReloadResMsg msg)
        {
            Debug.Log($"加入武器换弹结束队列");
            weaponReloadQueue.Enqueue(msg);
        }

        void OnWeaponDrop(FrameWeaponDropResMsg msg)
        {
            Debug.Log($"加入武器丢弃队列");
            weaponDropQueue.Enqueue(msg);
        }

        #endregion

    }


}