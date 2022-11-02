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

        public void Update_RoleRenderer(float deltaTime)
        {
            var roleStateRendererDomain = battleFacades.Domain.RoleStateRendererDomain;
            roleStateRendererDomain.ApplyRoleState(deltaTime);

            var roleRepo = battleFacades.Repo.RoleLogicRepo;
            roleRepo.Foreach((role) =>
            {
                var roleRenderer = role.roleRenderer;
                var moveComponent = role.LocomotionComponent;
                roleRenderer.transform.position = Vector3.Lerp(roleRenderer.transform.position, moveComponent.Position, deltaTime * roleRenderer.posAdjust);
                roleRenderer.transform.rotation = Quaternion.Lerp(roleRenderer.transform.rotation, moveComponent.Rotation, deltaTime * roleRenderer.rotAdjust);
            });
        }

        public void Update_WorldUI()
        {
            var roleRepo = battleFacades.Repo.RoleLogicRepo;
            roleRepo.Foreach((role) =>
            {
                var bloodSlider = role.roleRenderer.BloodSlider;
                var armorSlider = role.roleRenderer.ArmorSlider;
                if (role.StateComponent.RoleState == RoleState.Dying || role.StateComponent.RoleState == RoleState.Reborning)
                {
                    bloodSlider.value = 0;
                    bloodSlider.gameObject.SetActive(false);
                    armorSlider.value = 0;
                    armorSlider.gameObject.SetActive(false);
                    return;
                }

                var healthComponent = role.HealthComponent;
                bloodSlider.maxValue = healthComponent.MaxHealth;
                bloodSlider.value = healthComponent.CurHealth;

                var armor = role.Armor;
                if (armor == null)
                {
                    armorSlider.value = 0;
                }
                else
                {
                    armorSlider.maxValue = armor.MaxHealth;
                    armorSlider.value = armor.CurHealth;
                    Debug.Log($"armor.CurHealth {armor.CurHealth}");
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
                roleRenderer.SetEntityID(entityId);
                roleRenderer.Ctor();

                var repo = battleFacades.Repo;
                var roleRendererRepo = repo.RoleRendererRepo;
                roleRendererRepo.Add(roleRenderer);

                return roleRenderer;
            }

            Debug.Log("生成Renderer角色失败");
            return null;
        }

        public void HUD_ShowDamageText(BattleRoleLogicEntity role, float damage)
        {
            var roleRenderer = role.roleRenderer;
            var textTF = roleRenderer.GetDamageTextTF();
            var text = textTF.GetComponent<Text>();
            text.text = damage.ToString();
            textTF.gameObject.SetActive(true);
        }

    }

}