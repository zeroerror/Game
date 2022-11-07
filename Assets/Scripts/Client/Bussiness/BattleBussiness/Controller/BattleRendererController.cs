


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
            battleFacades.LogicTriggerAPI.damageRecordAction += DamageRecord;
        }

        public void Tick(float fixedDeltaTime)
        {
        }

        public void Update(float deltaTime)
        {
            Update_RoleRenderer(deltaTime);
            Update_BulletRenderer(deltaTime);
            Update_Camera();
        }

        #region [Renderer Update]

        void Update_RoleRenderer(float deltaTime)
        {
            var roleRendererDomain = battleFacades.Domain.RoleRendererDomain;
            roleRendererDomain.Update_WorldUI();
            roleRendererDomain.Update_RoleRenderer(deltaTime);
        }

        void Update_BulletRenderer(float deltaTime)
        {
            var bulletRendererDomain = battleFacades.Domain.BulletRendererDomain;
            bulletRendererDomain.Update_BulletRenderer(deltaTime);
        }

        void Update_Camera()
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
            Debug.Log($"args {atkEntityType.ToString()} {atkEntityID.ToString()} {vicEntityType.ToString()} {vicEntityID.ToString()}");

            if (atkEntityType == EntityType.Bullet)
            {
                var repo = battleFacades.Repo;
                var roleRendererRepo = repo.RoleRendererRepo;
                var vicRenderer = roleRendererRepo.Get(vicEntityID);
                vicRenderer.SetDamageText(damage.ToString());

                // - UI
                var BulletRepo = repo.BulletRepo;
                var bullet = BulletRepo.Get(atkEntityID);
                var WeaponRepo = repo.WeaponRepo;
                var roleRepo = repo.RoleLogicRepo;
                if (WeaponRepo.TryGet(bullet.WeaponID, out var weapon)
                && roleRepo.TryGet(weapon.MasterID, out var atkRole)
                && roleRepo.IsOwner(atkRole.IDComponent.EntityID))
                {
                    var arbitService = battleFacades.ArbitrationService; ;
                    arbitService.GetAtkerTotalKillAndCauseDamage(atkEntityType, atkEntityID, out var totalKill, out var totalDamage);
                    UIEventCenter.KillAndDamageInfoUpdateAction.Invoke(totalKill, (int)totalDamage);
                }
                return;
            }

            Debug.LogError("未处理情况 DamageRecord ");
        }

        #endregion

    }


}



