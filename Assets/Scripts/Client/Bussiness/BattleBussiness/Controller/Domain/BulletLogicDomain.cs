using System.Collections.Generic;
using UnityEngine;
using Game.Client.Bussiness.BattleBussiness.Facades;
using Game.Client.Bussiness.BattleBussiness.Generic;

namespace Game.Client.Bussiness.BattleBussiness.Controller.Domain
{

    public class BulletLogicDomain
    {

        BattleFacades battleFacades;

        public BulletLogicDomain()
        {
        }

        public void Inject(BattleFacades facades)
        {
            this.battleFacades = facades;
        }

        public BulletEntity Spawn(BulletType bulletType, int bulletEntityID, int weaponEntityID, Vector3 startPos, Vector3 fireDir)
        {
            string bulletPrefabName = bulletType.ToString() + "_Logic";

            if (battleFacades.Assets.BulletAsset.TryGetByName(bulletPrefabName, out GameObject prefabAsset))
            {
                var repo = battleFacades.Repo;
                var parent = repo.FieldRepo.CurFieldEntity.transform;
                prefabAsset = GameObject.Instantiate(prefabAsset, parent);

                var bulletEntity = prefabAsset.GetComponent<BulletEntity>();

                bulletEntity.Ctor();
                bulletEntity.gameObject.SetActive(true);
                bulletEntity.SetBulletType(bulletType);
                bulletEntity.SetEntityID(bulletEntityID);
                bulletEntity.SetWeaponID(weaponEntityID);
                var weapon = repo.WeaponRepo.Get(weaponEntityID);
                bulletEntity.SetLeagueID(weapon.IDComponent.LeagueId);
                bulletEntity.SetPosition(startPos);
                bulletEntity.FaceTo(fireDir);
                bulletEntity.LocomotionComponent.ApplyMoveVelocity(fireDir);

                battleFacades.Repo.BulletRepo.Add(bulletEntity);

                if (bulletEntity is HookerEntity hookerEntity)
                {
                    hookerEntity.SetMasterGrabPoint(weapon.transform);
                }

                return bulletEntity;
            }

            return null;
        }

        public void TearDown(BulletEntity bullet)
        {
            if (bullet.BulletType == BulletType.DefaultBullet)
            {
                bullet.TearDown();
            }

            if (bullet is GrenadeEntity grenadeEntity)
            {
                var bulletDomain = battleFacades.Domain.BulletLogicDomain;
                bulletDomain.GrenadeExplode(grenadeEntity);
            }

            if (bullet is HookerEntity hookerEntity)
            {
                hookerEntity.TearDown();
            }

            battleFacades.Repo.BulletRepo.TryRemove(bullet);
        }

        public void Tick_Physics_All(float fixedDeltaTime)
        {
            // - Normal Physics
            var bulletRepo = battleFacades.Repo.BulletRepo;
            bulletRepo.Foreach((bullet) =>
            {
                bullet.LocomotionComponent.Tick_AllPhysics(fixedDeltaTime);
            });

            // - Hooker Physics
            Tick_Physics_AllHookers(fixedDeltaTime);
        }

        public List<BulletEntity> Tick_LifeTime_All(float deltaTime)
        {
            var bulletRepo = battleFacades.Repo.BulletRepo;
            List<BulletEntity> removeList = new List<BulletEntity>();
            bulletRepo.Foreach((bulletEntity) =>
            {
                if (bulletEntity.LifeTime <= 0)
                {
                    removeList.Add(bulletEntity);
                }
                bulletEntity.ReduceLifeTime(deltaTime);
            });

            return removeList;
        }

        public List<HitModel> Tick_HitModels_All(EntityType hitEntityType, float fixedDeltaTime)
        {
            List<HitModel> hitModelList = new List<HitModel>();
            var physicsDomain = battleFacades.Domain.PhysicsDomain;
            var bulletRepo = battleFacades.Repo.BulletRepo;

            bulletRepo.ForAll((bullet) =>
            {
                List<CollisionExtra> hitCEList = null;
                if (hitEntityType == EntityType.BattleRole)
                {
                    hitCEList = physicsDomain.GetHitRole_ColliderList(bullet);
                }
                else if (hitEntityType == EntityType.Aridrop)
                {
                    hitCEList = physicsDomain.GetHitAirdrop_ColliderList(bullet);
                }

                Transform hookedTrans = null;
                bool hashit = false;

                hitCEList.ForEach((ce) =>
                {
                    if (bullet.BulletType == BulletType.Grenade)
                    {
                        // - Grenade Ignore
                        return;
                    }

                    if (hookedTrans == null)
                    {
                        hookedTrans = ce.gameObject.transform;
                    }

                    IDComponent victimIDC = null;
                    if (hitEntityType == EntityType.BattleRole)
                    {
                        var role = ce.gameObject.GetComponentInParent<BattleRoleLogicEntity>();
                        victimIDC = role.IDComponent;
                    }
                    else if (hitEntityType == EntityType.Aridrop)
                    {
                        var airdrop = ce.gameObject.GetComponentInParent<BattleAirdropEntity>();
                        victimIDC = airdrop.IDComponent;
                    }
                    else
                    {
                        Debug.LogError("未处理情况");
                    }

                    var hitDomain = battleFacades.Domain.HitDomain;
                    var bulletIDC = bullet.IDComponent;
                    HitPowerModel hitPowerModel = bullet.HitPowerModel;
                    if (!hitDomain.TryHitActor(bulletIDC, victimIDC, in hitPowerModel))
                    {
                        return;
                    }

                    HitModel model = new HitModel();
                    model.attackerIDC = bulletIDC;
                    model.victimIDC = victimIDC;
                    hitModelList.Add(model);
                    hashit = true;
                });

                if (hashit)
                {
                    ApplyHitEffector(bullet, hookedTrans);
                }
            });

            return hitModelList;
        }

        public void ApplyHitEffector(BulletEntity bullet, Transform hitTF)
        {
            // 普通子弹
            if (bullet.BulletType == BulletType.DefaultBullet)
            {
                TearDown(bullet);
            }
            // 爪钩
            if (bullet is HookerEntity hookerEntity)
            {
                hookerEntity.TryGrabSomthing(hitTF);
            }
            // 手雷
            else if (bullet is GrenadeEntity grenadeEntity)
            {
                var moveComponent = grenadeEntity.LocomotionComponent;
                moveComponent.SetStatic();

                var tf = grenadeEntity.transform;
                var collider = tf.GetComponent<Collider>();
                collider.enabled = false;
            }
        }

        #region [Grenade]

        public void GrenadeExplode(GrenadeEntity grenadeEntity)
        {
            Debug.Log("爆炸");
            grenadeEntity.isExploded = true;

            var roleRepo = battleFacades.Repo.RoleLogicRepo;
            roleRepo.Foreach((role) =>
            {
                // - 根据距离HitActor
                var dis = Vector3.Distance(role.LocomotionComponent.Position, grenadeEntity.LocomotionComponent.Position);
                if (dis < grenadeEntity.ExplosionRadius)
                {
                    HitPowerModel hitPowerModel = grenadeEntity.HitPowerModel;

                    var hitDomain = battleFacades.Domain.HitDomain;
                    if (hitDomain.TryHitActor(grenadeEntity.IDComponent, role.IDComponent, hitPowerModel))
                    {
                        var roleDomain = battleFacades.Domain.RoleLogicDomain;
                        roleDomain.TryReceiveDamage(role, hitPowerModel.damage);
                        if (role.HealthComponent.CheckIsDead())
                        {
                            roleDomain.RoleState_EnterDead(role);
                        }
                    }
                }
            });

            grenadeEntity.TearDown();
        }

        #endregion

        #region [Hooker]

        void Tick_Physics_AllHookers(float fixedDeltaTime)
        {
            var activeHookers = battleFacades.Domain.BulletLogicDomain.GetActivatedHookerList();
            var rqs = battleFacades.Network.RoleReqAndRes;
            activeHookers.ForEach((hooker) =>
            {
                var master = battleFacades.Repo.RoleLogicRepo.Get(hooker.WeaponID);
                if (!hooker.TickHooker(out float force))
                {
                    master.StateComponent.SetRoleState(RoleState.Normal);
                    return;
                }

                var masterMC = master.LocomotionComponent;
                var hookerEntityMC = hooker.LocomotionComponent;
                var dir = hookerEntityMC.Position - masterMC.Position;
                var dis = Vector3.Distance(hookerEntityMC.Position, masterMC.Position);
                dir.Normalize();
                var v = dir * force * fixedDeltaTime;
                masterMC.AddExtraVelocity(v);
            });
        }

        List<HookerEntity> GetActivatedHookerList()
        {
            List<HookerEntity> hookerEntities = new List<HookerEntity>();
            var bulletRepo = battleFacades.Repo.BulletRepo;
            bulletRepo.Foreach((bullet) =>
            {
                if (bullet is HookerEntity hookerEntity && hookerEntity.GrabPoint != null)
                {
                    hookerEntities.Add(hookerEntity);
                }
            });

            return hookerEntities;
        }


        #endregion

    }


}