using System.Collections.Generic;
using UnityEngine;
using Game.Client.Bussiness.BattleBussiness.Facades;
using Game.Client.Bussiness.BattleBussiness.Generic;
using Game.Protocol.Battle;

namespace Game.Client.Bussiness.BattleBussiness.Controller.Domain
{

    public class BulletDomain
    {

        BattleFacades battleFacades;

        // 事件队列
        Queue<FrameBulletSpawnResMsg> bulletSpawnQueue;
        Queue<FrameBulletHitRoleResMsg> bulletHitRoleQueue;
        Queue<FrameBulletHitFieldResMsg> bulletHitFieldQueue;
        Queue<FrameBulletLifeOverResMsg> bulletLifeOverQueue;

        public BulletDomain()
        {
        }

        public void Inject(BattleFacades facades)
        {
            this.battleFacades = facades;
        }

        public BulletEntity SpawnBulletLogic(BulletType bulletType, int bulletEntityID, int weaponEntityID, Vector3 startPos, Vector3 fireDir)
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

        public void TearDownBulletLogic(BulletEntity bullet)
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

        public List<BulletEntity> Tick_BulletLifeTime(float deltaTime)
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

        public void Tick_Physics_AllBullets(float fixedDeltaTime)
        {
            var bulletRepo = battleFacades.Repo.BulletRepo;
            bulletRepo.Foreach((bullet) =>
            {
                bullet.LocomotionComponent.Tick_AllPhysics(fixedDeltaTime);
            });
        }

        public List<HitModel> Tick_And_Get_BulletHitRoleHitModel(float fixedDeltaTime)
        {
            List<HitModel> attackList = new List<HitModel>();
            var physicsDomain = battleFacades.Domain.PhysicsDomain;
            var bulletRepo = battleFacades.Repo.BulletRepo;

            bulletRepo.Foreach((bullet) =>
            {
                var hitRoleList = physicsDomain.GetHitRole_ColliderList(bullet);
                Transform hookedTrans = null;
                bool hashit = false;

                hitRoleList.ForEach((ce) =>
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

                    var role = ce.gameObject.GetComponentInParent<BattleRoleLogicEntity>();
                    if (role.HealthComponent.CheckIsDead())
                    {
                        Debug.LogWarning($"TryHitActor 角色已死亡! return");
                        return;
                    }

                    var hitDomain = battleFacades.Domain.HitDomain;
                    var roleIDC = role.IDComponent;
                    var bulletIDC = bullet.IDComponent;
                    HitPowerModel hitPowerModel = bullet.HitPowerModel;
                    if (!hitDomain.TryHitActor(bulletIDC, roleIDC, in hitPowerModel))
                    {
                        Debug.LogWarning($"TryHitActor 失败! return");
                        return;
                    }

                    HitModel model = new HitModel();
                    model.attackerIDC = bulletIDC;
                    model.victimIDC = roleIDC;
                    attackList.Add(model);
                    hashit = true;
                });

                if (hashit)
                {
                    ApplyBulletHitEffector(bullet, hookedTrans);
                }
            });

            return attackList;
        }

        public void ApplyBulletHitEffector(BulletEntity bullet, Transform hitTF)
        {
            // 普通子弹
            if (bullet.BulletType == BulletType.DefaultBullet)
            {
                bullet.SetLifeTime(0);
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
                        var roleDomain = battleFacades.Domain.RoleDomain;
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

        // 获得当前所有已激活的爪钩
        public List<HookerEntity> GetActiveHookerList()
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

        public void Tick_ActiveHookerDraging(float fixedDeltaTime)
        {
            var activeHookers = battleFacades.Domain.BulletLogicDomain.GetActiveHookerList();
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

        #endregion

    }


}