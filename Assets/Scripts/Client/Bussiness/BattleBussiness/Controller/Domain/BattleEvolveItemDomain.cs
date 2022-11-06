using UnityEngine;
using Game.Client.Bussiness.BattleBussiness.Facades;
using Game.Client.Bussiness.BattleBussiness.Generic;

namespace Game.Client.Bussiness.BattleBussiness.Controller.Domain
{

    public class BattleEvolveItemDomain
    {

        BattleFacades battleFacades;

        public BattleEvolveItemDomain()
        {
        }

        public void Inject(BattleFacades facades)
        {
            this.battleFacades = facades;
        }

        public BattleEvolveItemEntity SpawnEvolveItem(GameObject entityGo, int entityID)
        {
            var armorItem = entityGo.GetComponent<BattleEvolveItemEntity>();
            armorItem.Ctor();
            armorItem.SetEntityID(entityID);

            var repo = battleFacades.Repo;
            var armorEvolveItemRepo = repo.EvolveItemRepo;
            armorEvolveItemRepo.Add(armorItem);
            
            return armorItem;
        }

        public void TearDownEvolveItem(BattleEvolveItemEntity weaponItem)
        {
            var repo = battleFacades.Repo;
            var armorEvolveItemRepo = repo.EvolveItemRepo;
            armorEvolveItemRepo.TryRemove(weaponItem);
            GameObject.Destroy(weaponItem.gameObject);
            GameObject.Destroy(weaponItem);
        }

    }

}