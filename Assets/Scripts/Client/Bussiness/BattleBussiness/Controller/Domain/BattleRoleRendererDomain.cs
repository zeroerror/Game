using UnityEngine;
using Game.Client.Bussiness.BattleBussiness.Facades;

namespace Game.Client.Bussiness.BattleBussiness.Controller.Domain
{

    public class BattleRoleRendererDomain
    {

        BattleFacades battleFacades;

        byte tempRidIndex;

        public BattleRoleRendererDomain()
        {
        }

        public void Inject(BattleFacades facades)
        {
            this.battleFacades = facades;
        }

        public void TickRoleWorldUI()
        {
            var roleRepo = battleFacades.Repo.RoleRepo;
            roleRepo.Foreach((role) =>
            {
                var bloodSlider = role.roleRenderer.BloodSlider;
                if (role.StateComponent.RoleState == RoleState.Dead || role.StateComponent.RoleState == RoleState.Reborn)
                {
                    bloodSlider.value = 0;
                    bloodSlider.gameObject.SetActive(false);
                    return;
                }

                bloodSlider.gameObject.SetActive(true);
                role.roleRenderer.BloodSlider.maxValue = role.HealthComponent.MaxHealth;
                role.roleRenderer.BloodSlider.value = role.HealthComponent.Health;
            });
        }


        public BattleRoleRendererEntity SpawnRoleRenderer(int entityId, Transform parent)
        {
            string rolePrefabName = "role_renderer";
            Debug.Log("生成" + rolePrefabName);
            if (battleFacades.Assets.BattleRoleAssets.TryGetByName(rolePrefabName, out GameObject prefabAsset))
            {
                prefabAsset = GameObject.Instantiate(prefabAsset, parent);
                var roleRenderer = prefabAsset.GetComponentInChildren<BattleRoleRendererEntity>();
                roleRenderer.SetEntityId(entityId);
                roleRenderer.Ctor();

                return roleRenderer;
            }

            Debug.Log("生成Renderer角色失败");
            return null;
        }

    }

}