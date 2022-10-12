using UnityEngine;
using Game.Client.Bussiness.BattleBussiness.Network;
using Game.Client.Bussiness.BattleBussiness.Facades;
using Game.Client.Bussiness.EventCenter;

namespace Game.Client.Bussiness.BattleBussiness.Controller.Domain
{

    public class BattleRoleDomain
    {

        BattleFacades battleFacades;

        BattleRoleReqAndRes battleRoleReqAndRes;

        byte tempRidIndex;

        public BattleRoleDomain()
        {
        }

        public void Inject(BattleFacades facades)
        {
            this.battleFacades = facades;
        }

        public BattleRoleLogicEntity SpawnRoleLogic(Transform parent)
        {
            string rolePrefabName = "role_logic";
            if (battleFacades.Assets.BattleRoleAssets.TryGetByName(rolePrefabName, out GameObject prefabAsset))
            {
                prefabAsset = GameObject.Instantiate(prefabAsset, parent);
                var roleLogic = prefabAsset.GetComponent<BattleRoleLogicEntity>();
                return roleLogic;
            }

            Debug.Log("生成角色失败");
            return null;
        }

        public BattleRoleRendererEntity SpawnRoleRenderer(Transform parent)
        {
            string rolePrefabName = "role_renderer";
            Debug.Log("生成" + rolePrefabName);
            if (battleFacades.Assets.BattleRoleAssets.TryGetByName(rolePrefabName, out GameObject prefabAsset))
            {
                prefabAsset = GameObject.Instantiate(prefabAsset, parent);
                var roleRenderer = prefabAsset.GetComponentInChildren<BattleRoleRendererEntity>();
                return roleRenderer;
            }

            return null;
        }

        public void RebornRole(BattleRoleLogicEntity role)
        {
            role.TearDown();
            role.Reborn(battleFacades.Repo.FiledRepo.CurFieldEntity.BornPos);
        }

        public void Tick_RoleRigidbody(float fixedTime)
        {
            var roleRepo = battleFacades.Repo.RoleRepo;
            roleRepo.Foreach((role) =>
            {
                role.MoveComponent.Tick_Friction(fixedTime);
                role.MoveComponent.Tick_GravityVelocity(fixedTime);
                role.MoveComponent.Tick_Rigidbody(fixedTime);
            });
        }

        public void Update_RoleRenderer(float deltaTime)
        {
            var roleRepo = battleFacades.Repo.RoleRepo;
            roleRepo.Foreach((role) =>
            {
                var roleRenderer = role.roleRenderer;
                roleRenderer.transform.position = Vector3.Lerp(roleRenderer.transform.position, role.MoveComponent.Position, deltaTime * roleRenderer.posAdjust);
                roleRenderer.transform.rotation = Quaternion.Lerp(roleRenderer.transform.rotation, role.MoveComponent.Rotation, deltaTime * roleRenderer.rotAdjust);

                var animatorComponent = roleRenderer.AnimatorComponent;
                var weaponComponent = role.WeaponComponent;

                var inputComponent = battleFacades.InputComponent;

                var owner = battleFacades.Repo.RoleRepo.Owner;
                bool isOwner = owner.IDComponent.EntityId == role.IDComponent.EntityId;

                if (isOwner && inputComponent.moveAxis != Vector3.zero)
                {
                    if (weaponComponent.CurrentWeapon == null) animatorComponent.PlayRun();
                    else animatorComponent.PlayRunWithGun();
                    return;
                }

                if (role.MoveComponent.Velocity.magnitude < 0.1f)
                {
                    roleRenderer.noMoveTime += deltaTime;
                    if (roleRenderer.noMoveTime > 0.2f && !roleRenderer.AnimatorComponent.IsInState("Shoot"))
                    {
                        bool hasGun = role.WeaponComponent.CurrentWeapon != null;
                        if (hasGun) roleRenderer.AnimatorComponent.PlayIdleWithGun();
                        else roleRenderer.AnimatorComponent.PlayIdle();
                    }
                    return;
                }

                role.roleRenderer.noMoveTime = 0;
            });
        }


    }

}