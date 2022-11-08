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
            Tick_BulletHitWall(fixedDeltaTime);
        }

        void Tick_BulletSpawn(float fixedDeltaTime)
        {
            while (bulletSpawnQueue.TryPeek(out var msg))
            {
                bulletSpawnQueue.Dequeue();

                var bulletID = msg.bulletID;
                var bulletTypeByte = msg.bulletType;
                var bulletType = (BulletType)bulletTypeByte;

                Vector3 startPos = new Vector3(msg.startPosX / 10000f, msg.startPosY / 10000f, msg.startPosZ / 10000f);
                Vector3 fireDir = new Vector3(msg.fireDirX / 100f, 0, msg.fireDirZ / 100f);

                var bulletRepo = battleFacades.Repo.BulletLogicRepo;

                var bulletLogic = battleFacades.Domain.BulletLogicDomain.Spawn(bulletType, msg.bulletID, msg.weaponID, startPos, fireDir);
                var bulletRenderer = battleFacades.Domain.BulletRendererDomain.SpawnBulletRenderer(bulletLogic.BulletType, bulletLogic.IDComponent.EntityID);
                bulletRenderer.SetPosition(bulletLogic.Position);
                bulletRenderer.SetRotation(bulletLogic.Rotation);

                Debug.Log($"生成子弹帧 {msg.serverFrame}: MasterId:{bulletLogic.WeaponID} 起点位置：{startPos}  飞行方向{fireDir}");

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

        void Tick_BulletHitWall(float fixedDeltaTime)
        {
            while (bulletHitFieldQueue.TryDequeue(out var msg))
            {
                // - 同步子弹位置
                Vector3 pos = new Vector3(msg.posX / 10000f, msg.posY / 10000f, msg.posZ / 10000f);
                var bullet = battleFacades.Repo.BulletLogicRepo.Get(msg.bulletEntityID);
                bullet.LocomotionComponent.SetPosition(pos);
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

    }

}