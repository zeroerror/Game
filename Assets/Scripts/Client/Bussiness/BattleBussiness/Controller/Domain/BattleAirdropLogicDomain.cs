using UnityEngine;
using Game.Client.Bussiness.BattleBussiness.Facades;
using Game.Client.Bussiness.BattleBussiness.Generic;

namespace Game.Client.Bussiness.BattleBussiness.Controller.Domain
{

    public class BattleAirdropLogicDomain
    {

        BattleFacades battleFacades;

        public BattleAirdropLogicDomain()
        {
        }

        public void Inject(BattleFacades facades)
        {
            this.battleFacades = facades;
        }

        public BattleAirdropEntity SpawnLogic(BattleStage curLevelStage, int entityID, Vector3 spawnPos, Transform root = null)
        {
            string prefabName = $"Airdrop_Logic_{curLevelStage.ToString()}";
            var itemAsset = battleFacades.Assets.ItemAsset;
            if (!itemAsset.TryGetByName(prefabName, out var prefab))
            {
                return null;
            }

            var go = GameObject.Instantiate(prefab);
            var entity = go.GetComponent<BattleAirdropEntity>();
            entity.Ctor();
            entity.IDComponent.SetEntityID(entityID);
            entity.IDComponent.SetLeagueID(-1);
            entity.LocomotionComponent.SetPosition(spawnPos);

            var repo = battleFacades.Repo;
            var airDropRepo = repo.AirdropLogicRepo;
            airDropRepo.Add(entity);

            Debug.Log($"生成空投 {prefabName} 位置 {spawnPos}");
            return entity;
        }

        public void TearDownLogic(BattleAirdropEntity airdrop)
        {
            var repo = battleFacades.Repo;
            var airDropRepo = repo.AirdropLogicRepo;
            airDropRepo.Remove(airdrop);
            airdrop.TearDown();
        }

        public float TryReceiveDamage(BattleAirdropEntity airdrop, float damage)
        {
            return airdrop.HealthComponent.TryReiveDamage(damage);
        }

        public void Tick_Physics_AllAirdrops(float fixedDeltaTime)
        {
            var airdropRepo = battleFacades.Repo.AirdropLogicRepo;
            airdropRepo.Foreach((airdrop) =>
            {
                airdrop.LocomotionComponent.Tick_AllPhysics(fixedDeltaTime);
            });
        }

    }

}