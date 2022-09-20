using UnityEngine;
using Game.Client.Bussiness.WorldBussiness.Facades;

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

        public void ReuseWeapon(WeaponEntity weapon, Vector3 dropPos)
        {
            var weaponRepo = worldFacades.Repo.WeaponRepo;
            var roleRepo = worldFacades.Repo.RoleRepo;
            weapon.transform.SetParent(null);
            weapon.transform.position = dropPos;
            weapon.ClearMaster();
            weaponRepo.Add(weapon);
        }

    }

}