using System.Collections.Generic;
using UnityEngine;
using Game.Client.Bussiness.WorldBussiness.Facades;
using Game.Infrastructure.Generic;

namespace Game.Client.Bussiness.WorldBussiness.Controller.Domain
{

    public class WeaponDomain
    {

        WorldFacades worldFacades;

        public WeaponDomain()
        {
        }

        public void Inject(WorldFacades facades)
        {
            this.worldFacades = facades;
        }

        public GameObject SpawnWeapon(WeaponType weaponType)
        {
            var weaponAssets = worldFacades.Assets.WeaponAsset;
            weaponAssets.TryGetByName(weaponType.ToString(), out GameObject weapon);
            weapon = GameObject.Instantiate(weapon);
            weapon.SetActive(true);

            return weapon;
        }

    }

}