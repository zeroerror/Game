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

        public BattleAirdropEntity SpawnBattleAirDrop(Vector3 spawnPos, int entityID, BattleStage stage)
        {
            string prefabName = $"Item_Airdrop_{stage.ToString()}";
            var itemAsset = battleFacades.Assets.ItemAsset;
            if (!itemAsset.TryGetByName(prefabName, out var prefab))
            {
                return null;
            }

            var airdropEntity = GameObject.Instantiate(prefab).GetComponent<BattleAirdropEntity>();
            airdropEntity.Ctor();
            airdropEntity.SetEntityID(entityID);
            airdropEntity.LocomotionComponent.SetPosition(spawnPos);

            var repo = battleFacades.Repo;
            var airDropRepo = repo.AirdropRepo;
            airDropRepo.Add(airdropEntity);

            Debug.Log($"生成空投 {prefabName} 位置 {spawnPos}");
            return airdropEntity;
        }

        public void Tick_Physics_AllAirdrops(float fixedDeltaTime)
        {
            var airdropRepo = battleFacades.Repo.AirdropRepo;
            airdropRepo.Foreach((airdrop) =>
            {
                airdrop.LocomotionComponent.Tick_AllPhysics(fixedDeltaTime);
            });
        }

    }

}