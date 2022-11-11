using System.Collections.Generic;
using UnityEngine;
using Game.Client.Bussiness.BattleBussiness.Facades;
using Game.Client.Bussiness.BattleBussiness.Generic;

namespace Game.Client.Bussiness.BattleBussiness.Controller.Domain
{

    public class BattleBulletLogicDomain
    {

        BattleFacades battleFacades;

        public BattleBulletLogicDomain()
        {
        }

        public void Inject(BattleFacades facades)
        {
            this.battleFacades = facades;
        }

        public BulletLogicEntity SpawnLogic(BulletType bulletType, int bulletEntityID, Vector3 pos)
        {
            string prefabName = bulletType.ToString() + "_Logic";
            if (!battleFacades.Assets.BulletAsset.TryGetByName(prefabName, out GameObject go))
            {
                Debug.LogError($"{prefabName} Spawn Failed!");
                return null;
            }

            var repo = battleFacades.Repo;
            var parent = repo.FieldRepo.CurFieldEntity.transform;
            go = GameObject.Instantiate(go, parent);

            var entity = go.GetComponent<BulletLogicEntity>();
            entity.Ctor();
            entity.gameObject.SetActive(true);
            entity.SetBulletType(bulletType);
            entity.SetEntityID(bulletEntityID);
            entity.SetPosition(pos);

            repo.BulletLogicRepo.Add(entity);

            return entity;

        }

        public void ShootByWeapon(BulletLogicEntity bulletEntity, int weaponEntityID, Vector3 fireDir)
        {
            var repo = battleFacades.Repo;
            bulletEntity.SetWeaponID(weaponEntityID);
            var weapon = repo.WeaponRepo.Get(weaponEntityID);
            bulletEntity.SetLeagueID(weapon.IDComponent.LeagueId);
            bulletEntity.FaceTo(fireDir);
            bulletEntity.LocomotionComponent.ApplyMoveVelocity(fireDir);
            if (bulletEntity is HookerLogicEntity hookerEntity)
            {
                hookerEntity.SetMasterGrabPoint(weapon.transform);
            }
        }

        public void TearDown(BulletLogicEntity bullet)
        {
            if (bullet == null)
            {
                return;
            }

            if (bullet is GrenadeLogicEntity grenadeEntity)
            {
                var bulletDomain = battleFacades.Domain.BulletLogicDomain;
                bulletDomain.GrenadeExplodeTearDown(grenadeEntity);
            }
            else
            {
                bullet.TearDown();
            }

            battleFacades.Repo.BulletLogicRepo.Remove(bullet);
        }

        public void TearDown(int entityID)
        {
            var repo = battleFacades.Repo;
            var bulletLogicRepo = repo.BulletLogicRepo;
            var bulletLogic = bulletLogicRepo.Get(entityID);
            TearDown(bulletLogic);
        }

        public void LifeTimeOver(BulletLogicEntity bullet, Vector3 pos)
        {
            if (bullet == null)
            {
                return;
            }

            // bullet.LocomotionComponent.SetPosition(pos);

            var bulletType = bullet.BulletType;
            if (bulletType == BulletType.DefaultBullet)
            {
                bullet.TearDown();
            }
            else if (bullet is GrenadeLogicEntity grenadeEntity)
            {
                GrenadeExplodeTearDown(grenadeEntity);
            }
            else if (bullet is HookerLogicEntity hookerEntity)
            {
                hookerEntity.TearDown();
            }

            var bulletRepo = battleFacades.Repo.BulletLogicRepo;
            bulletRepo.TryRemove(bullet);
            Debug.Log($"Bullet LifeOver: {bullet.IDComponent.EntityID}");
        }

        public void LifeTimeOver(int bulletID, Vector3 pos)
        {
            var bullet = battleFacades.Repo.BulletLogicRepo.Get(bulletID);
            LifeTimeOver(bullet, pos);
        }

        public void Tick_Physics_All(float fixedDeltaTime)
        {
            // - Normal Physics
            var bulletRepo = battleFacades.Repo.BulletLogicRepo;
            bulletRepo.Foreach((bullet) =>
            {
                bullet.LocomotionComponent.Tick_AllPhysics(fixedDeltaTime);
            });

            // - Hooker Physics
            Tick_Physics_AllHookers(fixedDeltaTime);
        }

        public List<BulletLogicEntity> Tick_LifeTime_All(float deltaTime)
        {
            var bulletRepo = battleFacades.Repo.BulletLogicRepo;
            List<BulletLogicEntity> removeList = new List<BulletLogicEntity>();
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
            var bulletRepo = battleFacades.Repo.BulletLogicRepo;

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
                    Vector3 hitPos = Vector3.zero;
                    if (hitEntityType == EntityType.BattleRole)
                    {
                        var role = ce.gameObject.GetComponentInParent<BattleRoleLogicEntity>();
                        victimIDC = role.IDComponent;
                        hitPos = role.LocomotionComponent.Position;
                    }
                    else if (hitEntityType == EntityType.Aridrop)
                    {
                        var airdrop = ce.gameObject.GetComponentInParent<BattleAirdropEntity>();
                        victimIDC = airdrop.IDComponent;
                        hitPos = airdrop.LocomotionComponent.Position;
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
                    ApplyEffector_BulletHitField(bullet, hitPos, hookedTrans);
                });

            });

            return hitModelList;
        }

        public void ApplyEffector_BulletHitField(BulletLogicEntity bullet, Vector3 hitPos, Transform hitTF = null)
        {
            if (bullet == null)
            {
                return;
            }

            // var lc = bullet.LocomotionComponent;
            // lc.SetPosition(hitPos);

            // 普通子弹
            if (bullet.BulletType == BulletType.DefaultBullet)
            {
                TearDown(bullet);
            }
            // 爪钩
            if (bullet is HookerLogicEntity hookerEntity)
            {
                hookerEntity.TryGrabSomthing(hitTF);
            }
            // 手雷
            else if (bullet is GrenadeLogicEntity grenadeEntity)
            {
                var moveComponent = grenadeEntity.LocomotionComponent;
                moveComponent.SetStatic();

                var tf = grenadeEntity.transform;
                var collider = tf.GetComponent<Collider>();
                collider.enabled = false;
            }
        }

        public void ApplyEffector_BulletHitField(int bulletID, Vector3 hitPos, Transform hitTF = null)
        {
            var bullet = battleFacades.Repo.BulletLogicRepo.Get(bulletID);
            ApplyEffector_BulletHitField(bullet, hitPos, hitTF);
        }

        #region [Grenade]

        public void GrenadeExplodeTearDown(GrenadeLogicEntity grenadeEntity)
        {
            Debug.Log("爆炸");
            grenadeEntity.SetIsExploded(true);

            var allDomains = battleFacades.Domain;
            var allRepo = battleFacades.Repo;
            // - Role
            var roleRepo = allRepo.RoleLogicRepo;
            roleRepo.Foreach((role) =>
            {
                // - 根据距离HitActor
                var dis = Vector3.Distance(role.LocomotionComponent.Position, grenadeEntity.LocomotionComponent.Position);
                if (dis < grenadeEntity.ExplosionRadius)
                {
                    HitPowerModel hitPowerModel = grenadeEntity.HitPowerModel;

                    var hitDomain = allDomains.HitDomain;
                    if (hitDomain.TryHitActor(grenadeEntity.IDComponent, role.IDComponent, hitPowerModel))
                    {
                        var roleDomain = allDomains.RoleLogicDomain;
                        roleDomain.TryReceiveDamage(role, hitPowerModel.damage);
                        if (role.HealthComponent.CheckIsDead())
                        {
                            roleDomain.RoleState_EnterDead(role);
                        }
                    }
                }
            });

            // - Airdrop
            var airdropLogicRepo = battleFacades.Repo.AirdropLogicRepo;
            airdropLogicRepo.Foreach((airdrop) =>
            {
                // - 根据距离HitActor
                var dis = Vector3.Distance(airdrop.LocomotionComponent.Position, grenadeEntity.LocomotionComponent.Position);
                if (dis < grenadeEntity.ExplosionRadius)
                {
                    HitPowerModel hitPowerModel = grenadeEntity.HitPowerModel;

                    var hitDomain = allDomains.HitDomain;
                    if (hitDomain.TryHitActor(grenadeEntity.IDComponent, airdrop.IDComponent, hitPowerModel))
                    {
                        var airdropLogicDomain = allDomains.AirdropLogicDomain;
                        airdropLogicDomain.TryReceiveDamage(airdrop, hitPowerModel.damage);
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

        List<HookerLogicEntity> GetActivatedHookerList()
        {
            List<HookerLogicEntity> hookerEntities = new List<HookerLogicEntity>();
            var bulletRepo = battleFacades.Repo.BulletLogicRepo;
            bulletRepo.Foreach((bullet) =>
            {
                if (bullet is HookerLogicEntity hookerEntity && hookerEntity.GrabPoint != null)
                {
                    hookerEntities.Add(hookerEntity);
                }
            });

            return hookerEntities;
        }


        #endregion

    }


}