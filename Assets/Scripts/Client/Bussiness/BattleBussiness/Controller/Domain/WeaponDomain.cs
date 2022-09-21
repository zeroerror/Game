using UnityEngine;
using Game.Client.Bussiness.BattleBussiness.Facades;

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

        public void ReuseWeapon(WeaponEntity weapon, Vector3 dropPos)
        {
            var weaponRepo = battleFacades.Repo.WeaponRepo;
            var roleRepo = battleFacades.Repo.RoleRepo;
            weapon.transform.SetParent(null);
            weapon.transform.position = dropPos;
            weapon.ClearMaster();
            weaponRepo.Add(weapon);
        }

    }

}