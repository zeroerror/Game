using UnityEngine;
using Game.Client.Bussiness.BattleBussiness.Facades;
using Game.Client.Bussiness.BattleBussiness.Generic;

namespace Game.Client.Bussiness.BattleBussiness.Controller.Domain
{

    public class BattleAirdropDomain
    {

        BattleFacades battleFacades;

        public BattleAirdropDomain()
        {
        }

        public void Inject(BattleFacades facades)
        {
            this.battleFacades = facades;
        }

        public BattleAirdropEntity SpawnBattleAirDrop(Vector3 spawnPos)
        {
            string prefabName = "Item_Airdrop_Level1";
            var itemAsset = battleFacades.Assets.ItemAsset;
            if (!itemAsset.TryGetByName(prefabName, out var prefab))
            {
                return null;
            }

            var airdropEntity = GameObject.Instantiate(prefab).GetComponent<BattleAirdropEntity>();
            airdropEntity.Ctor();
            airdropEntity.LocomotionComponent.SetPosition(spawnPos);

            var repo = battleFacades.Repo;
            var airDropRepo = repo.AirdropRepo;
            airDropRepo.Add(airdropEntity);

            Debug.Log($"生辰空投 prefabName {prefabName}  spawnPos {spawnPos}");
            return airdropEntity;
        }

    }

}