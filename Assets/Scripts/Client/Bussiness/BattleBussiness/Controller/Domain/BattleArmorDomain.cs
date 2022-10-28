using UnityEngine;
using Game.Client.Bussiness.BattleBussiness.Facades;
using Game.Client.Bussiness.BattleBussiness.Generic;

namespace Game.Client.Bussiness.BattleBussiness.Controller.Domain
{

    public class BattleArmorDomain
    {

        BattleFacades battleFacades;

        public BattleArmorDomain()
        {
        }

        public void Inject(BattleFacades facades)
        {
            this.battleFacades = facades;
        }

        public BattleArmorEntity SpawnArmor(ArmorType armorType, int entityID)
        {
            string prefabName = $"Armor_{armorType.ToString()}";

            var asset = battleFacades.Assets.ArmorAsset;
            if (asset.TryGetByName(prefabName, out var prefab))
            {
                var go = GameObject.Instantiate(prefab);
                var armor = go.GetComponent<BattleArmorEntity>();
                armor.Ctor();
                armor.IDComponent.SetEntityId(entityID);

                var repo = battleFacades.Repo;
                var armorRepo = repo.ArmorRepo;
                armorRepo.Add(armor);

                return armor;
            }

            return null;
        }

    }

}