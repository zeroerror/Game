using UnityEngine;
using Game.Client.Bussiness.BattleBussiness.Facades;
using Game.Client.Bussiness.BattleBussiness.Generic;

namespace Game.Client.Bussiness.BattleBussiness.Controller.Domain
{

    public class BattleArmorEvolveItemDomain
    {

        BattleFacades battleFacades;

        public BattleArmorEvolveItemDomain()
        {
        }

        public void Inject(BattleFacades facades)
        {
            this.battleFacades = facades;
        }

        public BattleArmorEvolveItemEntity SpawnBattleArmorEvolveItem(GameObject entityGo, int entityID)
        {
            var armorItem = entityGo.GetComponent<BattleArmorEvolveItemEntity>();
            armorItem.Ctor();
            armorItem.IDComponent.SetEntityId(entityID);

            var repo = battleFacades.Repo;
            var armorEvolveItemRepo = repo.ArmorEvolveItemRepo;
            armorEvolveItemRepo.Add(armorItem);
            
            return armorItem;
        }

        public void TearDownArmorEvolveItem(BattleArmorEvolveItemEntity weaponItem)
        {
            var repo = battleFacades.Repo;
            var armorEvolveItemRepo = repo.ArmorEvolveItemRepo;
            armorEvolveItemRepo.TryRemove(weaponItem);
            GameObject.Destroy(weaponItem.gameObject);
            GameObject.Destroy(weaponItem);
        }

    }

}