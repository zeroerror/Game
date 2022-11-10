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
        Queue<BattleBulletSpawnResMsg> bulletSpawnQueue;
        Queue<BattleBulletHitEntityResMsg> bulletHitEntityQueue;
        Queue<BattleBulletHitFieldResMsg> bulletHitFieldQueue;
        Queue<BattleBulletLifeTimeOverResMsg> bulletLifeOverQueue;

        public BattleBulletController()
        {
            bulletSpawnQueue = new Queue<BattleBulletSpawnResMsg>();
            bulletHitEntityQueue = new Queue<BattleBulletHitEntityResMsg>();
            bulletHitFieldQueue = new Queue<BattleBulletHitFieldResMsg>();
            bulletLifeOverQueue = new Queue<BattleBulletLifeTimeOverResMsg>();
        }

        public void Inject(BattleFacades battleFacades)
        {
            this.battleFacades = battleFacades;
            var bulletRqs = battleFacades.Network.BulletReqAndRes;
            bulletRqs.RegistRes_BulletSpawn(OnBulletSpawn);
            bulletRqs.RegistRes_BulletHitEntity(OnBulletHitEntity);
            bulletRqs.RegistRes_BulletHitField(OnBulletHitField);
            bulletRqs.RegistRes_BulletLifeOver(OnBulletLifeOver);

            var logicEventCenter = battleFacades.LogicEventCenter;
            logicEventCenter.Regist_BulletHitFieldAction(LogicEvent_BulletHitField);
        }

        public void Tick(float fixedDeltaTime)
        {
            Tick_BulletSpawn();
            Tick_BulletHitEntity();
            Tick_BulletHitField();
            Tick_BulletLifeOver();
        }

        void Tick_BulletSpawn()
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

        void OnBulletSpawn(BattleBulletSpawnResMsg msg)
        {
            bulletSpawnQueue.Enqueue(msg);
        }

        void Tick_BulletHitEntity()
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

        void OnBulletHitEntity(BattleBulletHitEntityResMsg msg)
        {
            bulletHitEntityQueue.Enqueue(msg);
        }

        void Tick_BulletHitField()
        {
            while (bulletHitFieldQueue.TryDequeue(out var msg))
            {
                var bulleID = msg.bulletEntityID;
                var pos = new Vector3(msg.posX / 10000f, msg.posY / 10000f, msg.posZ / 10000f);
                var bulletLogicDomain = battleFacades.Domain.BulletLogicDomain;
                bulletLogicDomain.ApplyEffector_BulletHitField(bulleID, pos);

                var bulletRendererDomain = battleFacades.Domain.BulletRendererDomain;
                bulletRendererDomain.ApplyEffector_BulletHitField(bulleID, pos);
            }
        }

        void OnBulletHitField(BattleBulletHitFieldResMsg msg)
        {
            bulletHitFieldQueue.Enqueue(msg);
        }

        void Tick_BulletLifeOver()
        {
            while (bulletLifeOverQueue.TryDequeue(out var msg))
            {
                var bulletID = msg.entityID;
                var bulletPos = new Vector3(msg.posX / 10000f, msg.posY / 10000f, msg.posZ / 10000f);

                var bulletLogicDomain = battleFacades.Domain.BulletLogicDomain;
                bulletLogicDomain.LifeTimeOver(bulletID, bulletPos);
                var bulletRendererDomain = battleFacades.Domain.BulletRendererDomain;
                bulletRendererDomain.LifeTimeOver(bulletID, bulletPos);
            }
        }

        void OnBulletLifeOver(BattleBulletLifeTimeOverResMsg msg)
        {
            bulletLifeOverQueue.Enqueue(msg);
        }

        void LogicEvent_BulletHitField(int bulletID, Vector3 hitPos, Transform hitTF)
        {
            var bulletRenderer = battleFacades.Repo.BulletRendererRepo.Get(bulletID);
            var allDomains = battleFacades.Domain;
            var bulletRendererDomain = allDomains.BulletRendererDomain;
            bulletRendererDomain.ApplyEffector_BulletHitField(bulletRenderer, hitPos);
        }

    }

}