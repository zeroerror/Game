using UnityEngine;
using Game.Client.Bussiness.BattleBussiness.Facades;
using Game.Client.Bussiness.BattleBussiness.Generic;

namespace Game.Client.Bussiness.BattleBussiness.Controller.Domain
{

    public class WeaponDomain
    {

        BattleFacades battleFacades;

        public WeaponDomain()
        {
        }

        public void Inject(BattleFacades facades)
        {
            this.battleFacades = facades;
        }

        public WeaponEntity SpawnWeapon(WeaponType weaponType, int weaponID, int masterID)
        {
            string prefabName = $"Weapon_{weaponType.ToString()}";

            var asset = battleFacades.Assets.WeaponAsset;
            if (asset.TryGetByName(prefabName, out var prefab))
            {
                var go = GameObject.Instantiate(prefab);
                var weapon = go.GetComponent<WeaponEntity>();
                weapon.Ctor();
                weapon.SetEntityID(weaponID);
                weapon.SetMaster(masterID);
                weapon.SetLeagueID(masterID);

                var repo = battleFacades.Repo;
                var weaponRepo = repo.WeaponRepo;
                weaponRepo.Add(weapon);

                return weapon;
            }

            return null;
        }

        public void TearDownWeapon(WeaponEntity weapon)
        {
            var repo = battleFacades.Repo;
            var weaponRepo = repo.WeaponRepo;
            weaponRepo.TryRemove(weapon);
            GameObject.Destroy(weapon.gameObject);
            GameObject.Destroy(weapon);
        }

        public void TearDownAllWeapons()
        {
            var repo = battleFacades.Repo;
            var weaponRepo = repo.WeaponRepo;
            weaponRepo.ForAll((w) =>
            {
                TearDownWeapon(w);
            });
        }

    }

}