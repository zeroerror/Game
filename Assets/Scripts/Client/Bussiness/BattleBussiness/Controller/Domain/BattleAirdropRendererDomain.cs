using UnityEngine;
using Game.Client.Bussiness.BattleBussiness.Facades;
using Game.Client.Bussiness.BattleBussiness.Generic;

namespace Game.Client.Bussiness.BattleBussiness.Controller.Domain
{

    public class BattleAirdropRendererDomain
    {

        BattleFacades battleFacades;

        public BattleAirdropRendererDomain()
        {
        }

        public void Inject(BattleFacades facades)
        {
            this.battleFacades = facades;
        }

        public void Update_AllAirdropsRendererAndHUD(float deltaTime)
        {
            Update_AllAirdropsRenderer(deltaTime);
            Update_AllAirdropsHUD(deltaTime);
        }

        public void Update_AllAirdropsRenderer(float deltaTime)
        {
            // - Locomotion
            var repo = battleFacades.Repo;
            var airdropRendererRepo = repo.AirdropRendererRepo;
            var airdropLogicRepo = repo.AirdropLogicRepo;
            airdropLogicRepo.Foreach((airdropLogic) =>
            {
                var entityID = airdropLogic.IDComponent.EntityID;
                var moveComponent = airdropLogic.LocomotionComponent;

                var airdropRenderer = airdropRendererRepo.Get(entityID);
                airdropRenderer.transform.position = Vector3.Lerp(airdropLogic.transform.position, moveComponent.Position, deltaTime * airdropRenderer.posAdjust);
                airdropRenderer.transform.rotation = Quaternion.Lerp(airdropLogic.transform.rotation, moveComponent.Rotation, deltaTime * airdropRenderer.rotAdjust);
            });
        }

        public void Update_AllAirdropsHUD(float deltaTime)
        {
            var repo = battleFacades.Repo;
            var airdropRendererRepo = repo.AirdropRendererRepo;
            var airdropLogicRepo = repo.AirdropLogicRepo;
            airdropLogicRepo.Foreach((airdropLogic) =>
            {
                var entityID = airdropLogic.IDComponent.EntityID;
                var healthComponent = airdropLogic.HealthComponent;

                var airdropRenderer = airdropRendererRepo.Get(entityID);
                var bloodSlider = airdropRenderer.BloodSlider;
                bloodSlider.maxValue = healthComponent.MaxHealth;
                bloodSlider.value = healthComponent.CurHealth;
            });
        }

        public BattleAirdropRendererEntity SpawnRenderer(int entityId, BattleStage stage, Vector3 pos, Transform root = null)
        {
            string prefabName = $"Airdrop_Renderer_{stage.ToString()}";
            if (battleFacades.Assets.ItemAsset.TryGetByName(prefabName, out GameObject prefab))
            {
                var go = GameObject.Instantiate(prefab, root);
                var airdropRenderer = go.GetComponentInChildren<BattleAirdropRendererEntity>();
                airdropRenderer.SetEntityID(entityId);
                airdropRenderer.Ctor();

                var repo = battleFacades.Repo;
                var airdropRendererRepo = repo.AirdropRendererRepo;
                airdropRendererRepo.Add(airdropRenderer);

                airdropRenderer.transform.position = pos;

                Debug.Log($"生成 {prefabName}");
                return airdropRenderer;
            }

            Debug.LogError($"生成失败 {prefabName}");
            return null;
        }

        public void TearDownRenderer(int entityID)
        {
            var repo = battleFacades.Repo;
            var airdropRendererRepo = repo.AirdropRendererRepo;
            var airdropRenderer = airdropRendererRepo.Get(entityID);
            airdropRenderer.TearDown();
            airdropRendererRepo.Remove(airdropRenderer);
        }

    }

}