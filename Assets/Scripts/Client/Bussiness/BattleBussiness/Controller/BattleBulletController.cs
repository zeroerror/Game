using System.Collections.Generic;
using UnityEngine;
using Game.Protocol.Battle;
using Game.Client.Bussiness.BattleBussiness.Facades;
using Game.Client.Bussiness.BattleBussiness.Generic;

namespace Game.Client.Bussiness.BattleBussiness.Controller
{

    public class BattleBulletController
    {
        BattleFacades battleFacades;

        // 事件队列
        Queue<FrameBulletSpawnResMsg> bulletSpawnQueue;
        Queue<FrameBulletHitEntityResMsg> bulletHitEntityQueue;
        Queue<FrameBulletHitFieldResMsg> bulletHitFieldQueue;

        public BattleBulletController()
        {
            bulletSpawnQueue = new Queue<FrameBulletSpawnResMsg>();
            bulletHitEntityQueue = new Queue<FrameBulletHitEntityResMsg>();
            bulletHitFieldQueue = new Queue<FrameBulletHitFieldResMsg>();
        }

        public void Inject(BattleFacades battleFacades)
        {
            this.battleFacades = battleFacades;
            var bulletRqs = battleFacades.Network.BulletReqAndRes;
            bulletRqs.RegistRes_BulletSpawn(OnBulletSpawn);
            bulletRqs.RegistRes_BulletHitEntity(OnBulletHitEntity);
            bulletRqs.RegistRes_BulletHitField(OnBulletHitField);

            var logicEventCenter = battleFacades.LogicEventCenter;
            logicEventCenter.Regist_BulletHitFieldAction(LogicEvent_BulletHitField);
        }

        public void Tick(float fixedDeltaTime)
        {
            TickSerRes_All(fixedDeltaTime);
        }

        #region [Tick]

        public void TickSerRes_All(float fixedDeltaTime)
        {
            Tick_BulletSpawn(fixedDeltaTime);
            Tick_BulletHitEntity(fixedDeltaTime);
            Tick_BulletHitField(fixedDeltaTime);
        }

        void Tick_BulletSpawn(float fixedDeltaTime)
        {
            while (bulletSpawnQueue.TryPeek(out var msg))
            {
                bulletSpawnQueue.Dequeue();

                int bulletID = msg.bulletID;
                BulletType bulletType = (BulletType)msg.bulletType;
                int weaponID = msg.weaponID;
                Vector3 pos = new Vector3(msg.startPosX / 10000f, msg.startPosY / 10000f, msg.startPosZ / 10000f);
                Vector3 fireDir = new Vector3(msg.fireDirX / 100f, 0, msg.fireDirZ / 100f);

                var bulletLogicDomain = battleFacades.Domain.BulletLogicDomain;
                var bulletLogic = bulletLogicDomain.SpawnLogic(bulletType, msg.bulletID, pos);
                bulletLogicDomain.ShootByWeapon(bulletLogic, weaponID, fireDir);

                var bulletRendererDomain = battleFacades.Domain.BulletRendererDomain;
                var bulletRenderer = bulletRendererDomain.SpawnBulletRenderer(bulletLogic.BulletType, bulletLogic.IDComponent.EntityID);
                bulletRenderer.SetPosition(bulletLogic.Position);
                bulletRenderer.SetRotation(bulletLogic.Rotation);

                Debug.Log($"生成子弹帧 {msg.serverFrame}: 起点位置：{pos}  飞行方向{fireDir}");
            }
        }

        void Tick_BulletHitEntity(float fixedDeltaTime)
        {
            var domain = battleFacades.Domain;
            var roleDomain = domain.RoleLogicDomain;
            var hitDomain = domain.HitDomain;

            while (bulletHitEntityQueue.TryDequeue(out var msg))
            {
                var bullet = battleFacades.Repo.BulletLogicRepo.Get(msg.bulletEntityID);
                var atkIDC = bullet.IDComponent;
                var hitPowerModel = bullet.HitPowerModel;

                var victimEntityType = (EntityType)msg.entityType;
                IDComponent vicIDC = null;
                if (victimEntityType == EntityType.BattleRole)
                {
                    var role = battleFacades.Repo.RoleLogicRepo.Get(msg.entityID);
                    vicIDC = role.IDComponent;
                }
                else if (victimEntityType == EntityType.Aridrop)
                {
                    var airdrop = battleFacades.Repo.AirdropLogicRepo.Get(msg.entityID);
                    vicIDC = airdrop.IDComponent;
                }
                else
                {
                    Debug.LogError("未处理情况!");
                }

                hitDomain.TryHitActor(atkIDC, vicIDC, hitPowerModel);
            }
        }

        void Tick_BulletHitField(float fixedDeltaTime)
        {
            while (bulletHitFieldQueue.TryDequeue(out var msg))
            {
                var bulleID = msg.bulletEntityID;
                var bulletLogicDomain = battleFacades.Domain.BulletLogicDomain;
                bulletLogicDomain.TearDown(bulleID);
                var bulletRendererDomain = battleFacades.Domain.BulletRendererDomain;
                bulletRendererDomain.TearDown(bulleID);
            }
        }

        #endregion

        #region [Regist]

        void OnBulletSpawn(FrameBulletSpawnResMsg msg)
        {
            Debug.Log($"加入子弹生成队列");
            bulletSpawnQueue.Enqueue(msg);
        }

        void OnBulletHitEntity(FrameBulletHitEntityResMsg msg)
        {
            Debug.Log("加入子弹击中实体队列");
            bulletHitEntityQueue.Enqueue(msg);
        }

        void OnBulletHitField(FrameBulletHitFieldResMsg msg)
        {
            Debug.Log("加入子弹击中墙壁队列");
            bulletHitFieldQueue.Enqueue(msg);
        }

        #endregion

        void LogicEvent_BulletHitField(int bulletID, Transform hitTF)
        {
            var bulletRenderer = battleFacades.Repo.BulletRendererRepo.Get(bulletID);
            var allDomains = battleFacades.Domain;
            var bulletRendererDomain = allDomains.BulletRendererDomain;
            bulletRendererDomain.ApplyEffector_BulletHitField(bulletRenderer, hitTF);

            // - vfx
            var vfxGo = GameObject.Instantiate(bulletRenderer.vfxPrefab_hitField);
            vfxGo.GetComponent<ParticleSystem>().Play();
        }

    }

}