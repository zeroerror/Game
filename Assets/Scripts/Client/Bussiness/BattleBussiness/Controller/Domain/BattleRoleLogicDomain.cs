using UnityEngine;
using Game.Client.Bussiness.BattleBussiness.Facades;
using Game.Client.Bussiness.BattleBussiness.Generic;

namespace Game.Client.Bussiness.BattleBussiness.Controller.Domain
{

    public class BattleRoleLogicDomain
    {

        BattleFacades battleFacades;

        byte tempRidIndex;

        public BattleRoleLogicDomain()
        {
        }

        public void Inject(BattleFacades facades)
        {
            this.battleFacades = facades;
        }

        public BattleRoleLogicEntity SpawnRoleWithRenderer(int entityId, ControlType controlType)
        {
            var repo = battleFacades.Repo;
            var fieldEntity = repo.FieldRepo.CurFieldEntity;
            var roleRendererDomain = battleFacades.Domain.RoleRendererDomain;

            var roleRenderer = roleRendererDomain.SpawnRenderer(entityId, fieldEntity.Role_Group_Renderer);

            var roleLogic = SpawnRoleLogic(entityId);
            roleLogic.Inject(roleRenderer);

            var fieldCameraComponent = fieldEntity.CameraComponent;

            if (controlType == ControlType.Owner)
            {
                var roleRepo = battleFacades.Repo.RoleLogicRepo;
                roleRepo.SetOwner(roleLogic);
                fieldCameraComponent.OpenThirdViewCam(roleLogic.roleRenderer);
            }

            return roleLogic;
        }

        public BattleRoleLogicEntity SpawnRoleLogic(int entityId)
        {
            var fieldEntity = battleFacades.Repo.FieldRepo.CurFieldEntity;
            string prefabName = "role_logic";
            if (battleFacades.Assets.BattleRoleAssets.TryGetByName(prefabName, out GameObject prefab))
            {
                prefab = GameObject.Instantiate(prefab, fieldEntity.transform);

                var roleLogic = prefab.GetComponent<BattleRoleLogicEntity>();
                roleLogic.Ctor();
                roleLogic.SetEntityID(entityId);
                roleLogic.SetLeagueID(entityId);
                Reborn(roleLogic);

                var roleRepo = battleFacades.Repo.RoleLogicRepo;
                roleRepo.Add(roleLogic);

                return roleLogic;
            }

            Debug.Log("生成Logic角色失败");
            return null;
        }

        public void Tick_Physics_AllRoles(float fixedTime)
        {
            var roleRepo = battleFacades.Repo.RoleLogicRepo;
            roleRepo.Foreach((role) =>
            {
                role.LocomotionComponent.Tick_AllPhysics(fixedTime);
            });
        }

        public void RoleMoveActivate(BattleRoleLogicEntity role, Vector3 dir)
        {
        }

        public void Reborn(BattleRoleLogicEntity role)
        {
            var repo = battleFacades.Repo;
            var fieldRepo = repo.FieldRepo;
            var curField = fieldRepo.CurFieldEntity;
            var pos = curField.UseRandomBornPos();
            role.Reborn(pos);
        }

        public float TryReceiveDamage(BattleRoleLogicEntity role, float damage)
        {
            return role.TryReceiveDamage(damage);
        }

        // --- State
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