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

        public WeaponEntity Spawn(WeaponType weaponType, int weaponID)
        {
            string prefabName = $"Weapon_{weaponType.ToString()}";
            var asset = battleFacades.Assets.WeaponAsset;
            if (!asset.TryGetByName(prefabName, out var prefab))
            {
                Debug.LogError($"{prefabName} Spawn Failed!");
                return null;
            }

            var go = GameObject.Instantiate(prefab);
            var entity = go.GetComponent<WeaponEntity>();
            entity.Ctor();
            entity.SetEntityID(weaponID);

            var repo = battleFacades.Repo;
            var weaponRepo = repo.WeaponRepo;
            weaponRepo.Add(entity);

            return entity;
        }

        public void SetMaster(WeaponEntity weapon, int masterEntityID)
        {
            weapon.SetMaster(masterEntityID);
            weapon.SetLeagueID(masterEntityID);
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