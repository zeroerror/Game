using UnityEngine;
using Game.Client.Bussiness.BattleBussiness.Facades;
using UnityEngine.UI;

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
                var armorSlider = role.roleRenderer.ArmorSlider;
                if (role.StateComponent.RoleState == RoleState.Dead || role.StateComponent.RoleState == RoleState.Reborn)
                {
                    bloodSlider.value = 0;
                    bloodSlider.gameObject.SetActive(false);
                    armorSlider.value = 0;
                    armorSlider.gameObject.SetActive(false);
                    return;
                }

                var healthComponent = role.HealthComponent;
                bloodSlider.maxValue = healthComponent.MaxHealth;
                bloodSlider.value = healthComponent.Health;

                var armor = role.Armor;
                if (armor == null)
                {
                    armorSlider.maxValue = 0;
                    armorSlider.value = 0;
                }
                else
                {
                    armorSlider.maxValue = armor.MaxHealth;
                    armorSlider.value = armor.CurHealth;
                }

                bloodSlider.gameObject.SetActive(true);
                armorSlider.gameObject.SetActive(true);
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

        public void HUD_ShowDamageText(BattleRoleLogicEntity role, int damage)
        {
            var roleRenderer = role.roleRenderer;
            var textTF = roleRenderer.GetDamageTextTF();
            var text = textTF.GetComponent<Text>();
            text.text = damage.ToString();
            textTF.gameObject.SetActive(true);
        }

    }

}