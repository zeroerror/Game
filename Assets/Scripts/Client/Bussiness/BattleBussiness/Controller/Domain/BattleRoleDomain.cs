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
                RoleReborn(roleLogic);

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
                role.MoveComponent.Tick_Friction(fixedTime);
                role.MoveComponent.Tick_Gravity(fixedTime);
                role.MoveComponent.Tick_Rigidbody(fixedTime);
            });
        }

        public void RoleMoveActivate(BattleRoleLogicEntity role, Vector3 dir)
        {
            role.MoveComponent.ActivateMoveVelocity(dir);
        }

        public bool TryRoleRoll(BattleRoleLogicEntity role, Vector3 dir)
        {
            var moveComponent = role.MoveComponent;
            if (!moveComponent.IsGrounded)
            {
                return false;
            }

            role.Roll(dir);
            role.StateComponent.EnterRolling(30);

            return true;
        }

        public void RoleReborn(BattleRoleLogicEntity role)
        {
            role.TearDown();
            role.Reborn(battleFacades.Repo.FiledRepo.CurFieldEntity.BornPos);
        }

        public int RoleTryReceiveDamage(BattleRoleLogicEntity role, int damage)
        {
            return role.TryReceiveDamage(damage);
        }

        public void RoleStateEnterReloading(BattleRoleLogicEntity role)
        {
            role.StateComponent.EnterReloading(40);
        }

        public void RoleStateEnterDead(BattleRoleLogicEntity role)
        {
            role.StateComponent.EnterDead(60);
        }

        public bool CanRoleShoot(BattleRoleLogicEntity role)
        {
            var weaponComponent = role.WeaponComponent;
            var curWeapon = weaponComponent.CurrentWeapon;

            if (curWeapon == null)
            {
                Debug.LogWarning("当前武器为空，无法射击");
                return false;
            }

            if (weaponComponent.IsReloading)
            {
                Debug.LogWarning("换弹中，无法射击");
                return false;
            }

            var stateComponent = role.StateComponent;
            Debug.LogWarning($"stateComponent.RoleState  {stateComponent.RoleState.ToString()}");
            Debug.LogWarning($"stateComponent.ShootingMod.maintainFrame  {stateComponent.ShootingMod.maintainFrame}");
            if (stateComponent.RoleState == RoleState.Shoot && stateComponent.ShootingMod.maintainFrame > 3)
            {
                Debug.LogWarning("射击CD未结束");
                return false;
            }

            return true;
        }

    }

}