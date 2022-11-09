


using UnityEngine;
using Game.Client.Bussiness.EventCenter;
using Game.Client.Bussiness.BattleBussiness.Facades;
using Game.Client.Bussiness.BattleBussiness.Generic;

namespace Game.Client.Bussiness.BattleBussiness.Controller
{

    public class BattleRendererController
    {

        BattleFacades battleFacades;

        public BattleRendererController() { }

        public void Inject(BattleFacades facades)
        {
            battleFacades = facades;
            battleFacades.LogicTriggerEvent.Regist_BattleDamageRecordAction(DamageRecord);
        }

        public void Tick(float fixedDeltaTime)
        {
        }

        public void Update(float deltaTime)
        {
            Update_AllRoleRenderer(deltaTime);
            Update_AllAirdropRenderer(deltaTime);
            Update_AllBulletRenderer(deltaTime);
            Update_CameraRenderer();
        }

        #region [Renderer Update]

        void Update_AllRoleRenderer(float deltaTime)
        {
            var roleRendererDomain = battleFacades.Domain.RoleRendererDomain;
            roleRendererDomain.Update_AllRolesRendererAndHUD(deltaTime);
        }

        void Update_AllAirdropRenderer(float deltaTime)
        {
            var airdropRendererDomain = battleFacades.Domain.AirdropRendererDomain;
            airdropRendererDomain.Update_AllAirdropsRendererAndHUD(deltaTime);
        }

        void Update_AllBulletRenderer(float deltaTime)
        {
            var bulletRendererDomain = battleFacades.Domain.BulletRendererDomain;
            bulletRendererDomain.Update_AllBulletRenderer(deltaTime);
        }

        void Update_CameraRenderer()
        {
            Vector2 inputAxis = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            var inputDomain = battleFacades.Domain.InputDomain;
            inputDomain.UpdateCameraByCameraView(inputAxis);
        }

        void DamageRecord(DamageRecordArgs args)
        {
            var atkEntityType = args.atkEntityType;
            var atkEntityID = args.atkEntityID;
            var vicEntityType = args.vicEntityType;
            var vicEntityID = args.vicEntityID;
            var damage = args.damage;
            Debug.Log($"DamageRecord {atkEntityType.ToString()} {atkEntityID.ToString()} {vicEntityType.ToString()} {vicEntityID.ToString()}");

            // - HUD 
            ShowVictimHUD(vicEntityType, vicEntityID, damage);

            // - UI
            if (IsAtkByOwner(atkEntityType, atkEntityID))
            {
                var arbitService = battleFacades.ArbitrationService; ;
                arbitService.GetAtkerTotalKillAndCauseDamage(atkEntityType, atkEntityID, out var totalKill, out var totalDamage);
                UIEventCenter.KillAndDamageInfoUpdateAction.Invoke(totalKill, (int)totalDamage);
            }

        }

        void ShowVictimHUD(EntityType vicEntityType, int vicEntityID, float damage)
        {
            var repo = battleFacades.Repo;
            if (vicEntityType == EntityType.BattleRole)
            {
                var roleRendererRepo = repo.RoleRendererRepo;
                var vicRenderer = roleRendererRepo.Get(vicEntityID);
                vicRenderer.SetDamageText(damage.ToString());
                return;
            }

            if (vicEntityType == EntityType.Aridrop)
            {
                var airdropRendererRepo = repo.AirdropRendererRepo;
                var airdropRenderer = airdropRendererRepo.Get(vicEntityID);
                airdropRenderer.SetDamageText(damage.ToString());
                return;
            }

            Debug.LogError("Not Handle");
        }

        bool IsAtkByOwner(EntityType atkEntityType, int atkEntityID)
        {
            var repo = battleFacades.Repo;
            if (atkEntityType == EntityType.Bullet)
            {
                var bulletRepo = repo.BulletLogicRepo;
                var bullet = bulletRepo.Get(atkEntityID);
                var WeaponRepo = repo.WeaponRepo;
                var roleRepo = repo.RoleLogicRepo;
                if (WeaponRepo.TryGet(bullet.WeaponID, out var weapon)
                && roleRepo.TryGet(weapon.MasterID, out var atkRole)
                && roleRepo.IsOwner(atkRole.IDComponent.EntityID))
                {
                    return true;
                }
                return false;
            }

            Debug.LogError("未处理情况");
            return false;
        }

        #endregion

    }


}



