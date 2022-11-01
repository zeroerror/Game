using UnityEngine;
using Game.Client.Bussiness.BattleBussiness.Facades;

namespace Game.Client.Bussiness.BattleBussiness.Controller.Domain
{

    public class BattleRoleDomain
    {

        BattleFacades battleFacades;

        byte tempRidIndex;

        public BattleRoleDomain()
        {
        }

        public void Inject(BattleFacades facades)
        {
            this.battleFacades = facades;
        }

        public BattleRoleLogicEntity SpawnRoleWithRenderer(byte entityId, bool isOwner)
        {
            var fieldEntity = battleFacades.Repo.FiledRepo.CurFieldEntity;
            var roleRendererDomain = battleFacades.Domain.RoleRendererDomain;

            var roleRenderer = roleRendererDomain.SpawnRoleRenderer(entityId, fieldEntity.Role_Group_Renderer);

            var roleLogic = SpawnRoleLogic(entityId);
            roleLogic.Inject(roleRenderer);

            var fieldCameraComponent = fieldEntity.CameraComponent;
            if (isOwner)
            {
                var roleRepo = battleFacades.Repo.RoleRepo;
                roleRepo.SetOwner(roleLogic);
                fieldCameraComponent.OpenThirdViewCam(roleLogic.roleRenderer);
            }

            return roleLogic;
        }

        public BattleRoleLogicEntity SpawnRoleLogic(int entityId)
        {
            var fieldEntity = battleFacades.Repo.FiledRepo.CurFieldEntity;
            string prefabName = "role_logic";
            if (battleFacades.Assets.BattleRoleAssets.TryGetByName(prefabName, out GameObject prefab))
            {
                prefab = GameObject.Instantiate(prefab, fieldEntity.transform);

                var roleLogic = prefab.GetComponent<BattleRoleLogicEntity>();
                roleLogic.Ctor();
                roleLogic.IDComponent.SetEntityId(entityId);
                roleLogic.IDComponent.SetLeagueId(entityId);
                Reborn(roleLogic);

                var roleRepo = battleFacades.Repo.RoleRepo;
                roleRepo.Add(roleLogic);

                return roleLogic;
            }

            Debug.Log("生成Logic角色失败");
            return null;
        }

        public void Tick_RoleRigidbody(float fixedTime)
        {
            var roleRepo = battleFacades.Repo.RoleRepo;
            roleRepo.Foreach((role) =>
            {
                role.MoveComponent.SimulatePhysics(fixedTime);
            });
        }

        public void RoleMoveActivate(BattleRoleLogicEntity role, Vector3 dir)
        {
        }

        public void Reborn(BattleRoleLogicEntity role)
        {
            role.TearDown();
            role.Reborn(battleFacades.Repo.FiledRepo.CurFieldEntity.BornPos);
        }

        public int TryReceiveDamage(BattleRoleLogicEntity role, int damage)
        {
            return role.TryReceiveDamage(damage);
        }

        public void RoleState_EnterReloading(BattleRoleLogicEntity role)
        {
            role.StateComponent.EnterReloading(40);
        }

        public void RoleState_EnterDead(BattleRoleLogicEntity role)
        {
            role.StateComponent.EnterDead(60);
        }

    }

}