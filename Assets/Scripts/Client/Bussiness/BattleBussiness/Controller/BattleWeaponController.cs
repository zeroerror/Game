using UnityEngine;
using Game.Client.Bussiness.BattleBussiness.Facades;
using Game.Protocol.Battle;
using System.Collections.Generic;

namespace Game.Client.Bussiness.BattleBussiness.Controller
{

    public class BattleWeaponController
    {

        BattleFacades battleFacades;

        // 服务器下发的武器射击队列
        Queue<BattleWeaponShootResMsg> weaponShootQueue;
        // 服务器下发的武器换弹队列
        Queue<BattleWeaponReloadResMsg> weaponReloadQueue;
        // 服务器下发的武器丢弃队列
        Queue<BattleWeaponDropResMsg> weaponDropQueue;

        public BattleWeaponController()
        {
            weaponShootQueue = new Queue<BattleWeaponShootResMsg>();
            weaponReloadQueue = new Queue<BattleWeaponReloadResMsg>();
            weaponDropQueue = new Queue<BattleWeaponDropResMsg>();
        }

        public void Inject(BattleFacades battleFacades)
        {
            this.battleFacades = battleFacades;


            var weaponRqs = battleFacades.Network.WeaponReqAndRes;
            weaponRqs.RegistRes_WeaponShoot(OnWeaponShoot);
            weaponRqs.RegistRes_WeaponReload(OnWeaponReload);
            weaponRqs.RegistRes_WeaponDrop(OnWeaponDrop);
        }

        public void Tick(float fixedDeltaTime)
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

                var repo = battleFacades.Repo;

                var weaponRepo = repo.WeaponRepo;
                var weapon = weaponRepo.Get(msg.weaponID);

                var roleRepo = repo.RoleLogicRepo;
                var master = roleRepo.Get(weapon.MasterID);

                var weaponComponent = master.WeaponComponent;
                if (weapon != weaponComponent.CurWeapon)
                {
                    Debug.LogError("客服不一致!");
                }

                if (weaponComponent.TryWeaponShoot())
                {
                    Debug.Log($"角色:{master.IDComponent.EntityID} 射击");
                    var curWeapon = weaponComponent.CurWeapon;
                    curWeapon.PlayShootAudio();
                }
            }
        }

        void Tick_WeaponReload()
        {
            while (weaponReloadQueue.TryPeek(out var msg))
            {
                weaponReloadQueue.Dequeue();

                var roleRepo = battleFacades.Repo.RoleLogicRepo;
                var master = roleRepo.Get(msg.masterId);
                var reloadBulletNum = msg.reloadBulletNum;
                master.WeaponComponent.FinishReloading(reloadBulletNum);
            }
        }

        void Tick_WeaponDrop()
        {
            while (weaponDropQueue.TryPeek(out var msg))
            {
                weaponDropQueue.Dequeue();
                var master = battleFacades.Repo.RoleLogicRepo.Get(msg.masterID);
                master.WeaponComponent.TryDropWeapon(msg.weaponID, out var weapon);

                var itemDomain = battleFacades.Domain.ItemDomain;
                itemDomain.DropWeaponToItem(weapon);
            }
        }

        #endregion

        #region [Server]

        void OnWeaponShoot(BattleWeaponShootResMsg msg)
        {
            weaponShootQueue.Enqueue(msg);
        }

        void OnWeaponReload(BattleWeaponReloadResMsg msg)
        {
            weaponReloadQueue.Enqueue(msg);
        }

        void OnWeaponDrop(BattleWeaponDropResMsg msg)
        {
            weaponDropQueue.Enqueue(msg);
        }

        #endregion

    }


}