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
        Queue<FrameBulletHitRoleResMsg> bulletHitRoleQueue;
        Queue<FrameBulletHitFieldResMsg> bulletHitFieldQueue;
        Queue<FrameBulletLifeOverResMsg> bulletLifeOverQueue;

        public BattleBulletController()
        {
        }

        public void Inject(BattleFacades battleFacades)
        {
            this.battleFacades = battleFacades;

            var bulletRqs = battleFacades.Network.BulletReqAndRes;
            bulletRqs.RegistRes_BulletSpawn(OnBulletSpawn);
            bulletRqs.RegistRes_BulletHitRole(OnBulletHitRole);
            bulletRqs.RegistRes_BulletHitField(OnBulletHitField);
            bulletRqs.RegistRes_BulletTearDown(OnBulletLifeOver);
            bulletSpawnQueue = new Queue<FrameBulletSpawnResMsg>();
            bulletHitRoleQueue = new Queue<FrameBulletHitRoleResMsg>();
            bulletHitFieldQueue = new Queue<FrameBulletHitFieldResMsg>();
            bulletLifeOverQueue = new Queue<FrameBulletLifeOverResMsg>();
        }

        public void Tick(float fixedDeltaTime)
        {
            TickSerRes_All(fixedDeltaTime);
        }

        #region [Tick]

        public void TickSerRes_All(float fixedDeltaTime)
        {
            Tick_BulletSpawn(fixedDeltaTime);
            Tick_BulletHitRole(fixedDeltaTime);
            Tick_BulletHitWall(fixedDeltaTime);
            Tick_BulletLifeOver(fixedDeltaTime);
        }

        void Tick_BulletSpawn(float fixedDeltaTime)
        {
            while (bulletSpawnQueue.TryPeek(out var msg))
            {
                bulletSpawnQueue.Dequeue();

                var bulletId = msg.bulletEntityId;
                var bulletTypeByte = msg.bulletType;
                var bulletType = (BulletType)bulletTypeByte;

                Vector3 startPos = new Vector3(msg.startPosX / 10000f, msg.startPosY / 10000f, msg.startPosZ / 10000f);
                Vector3 fireDir = new Vector3(msg.fireDirX / 100f, 0, msg.fireDirZ / 100f);

                var bulletRepo = battleFacades.Repo.BulletRepo;

                var bulletLogic = battleFacades.Domain.BulletLogicDomain.SpawnBulletLogic(bulletType, msg.bulletEntityId, msg.masterEntityId, startPos, fireDir);
                var bulletRenderer = battleFacades.Domain.BulletRendererDomain.SpawnBulletRenderer(bulletLogic.BulletType, bulletLogic.IDComponent.EntityID);
                bulletRenderer.SetPosition(bulletLogic.Position);
                bulletRenderer.SetRotation(bulletLogic.Rotation);

                Debug.Log($"生成子弹帧 {msg.serverFrame}: MasterId:{bulletLogic.MasterEntityID} 起点位置：{startPos}  飞行方向{fireDir}");

            }
        }

        void Tick_BulletHitRole(float fixedDeltaTime)
        {
            var domain = battleFacades.Domain;
            var roleDomain = domain.RoleDomain;
            var hitDomain = domain.HitDomain;

            while (bulletHitRoleQueue.TryDequeue(out var msg))
            {
                var role = battleFacades.Repo.RoleLogicRepo.Get(msg.roleEntityId);
                var bullet = battleFacades.Repo.BulletRepo.Get(msg.bulletEntityId);
                var atkIDC = bullet.IDComponent;
                var vicIDC = role.IDComponent;
                hitDomain.TryHitActor(atkIDC, vicIDC, bullet.HitPowerModel);
            }
        }

        void Tick_BulletHitWall(float fixedDeltaTime)
        {
            while (bulletHitFieldQueue.TryDequeue(out var msg))
            {
                // - 同步子弹位置
                Vector3 pos = new Vector3(msg.posX / 10000f, msg.posY / 10000f, msg.posZ / 10000f);
                var bullet = battleFacades.Repo.BulletRepo.Get(msg.bulletEntityID);
                bullet.LocomotionComponent.SetPosition(pos);
            }
        }

        void Tick_BulletLifeOver(float fixedDeltaTime)
        {
            while (bulletLifeOverQueue.TryDequeue(out var msg))
            {
                var bulletRepo = battleFacades.Repo.BulletRepo;
                var entityID = msg.bulletEntityID;
                var bulletEntity = bulletRepo.Get(entityID);

                Vector3 pos = new Vector3(msg.posX / 10000f, msg.posY / 10000f, msg.posZ / 10000f);
                TearDownBulletLogicAndRenderer(pos, bulletEntity);
            }
        }

        public void TearDownBulletLogicAndRenderer(Vector3 pos, BulletEntity bulletLogic)
        {
            bulletLogic.LocomotionComponent.SetPosition(pos);

            var domain = battleFacades.Domain;

            var bulletLogicDomain = domain.BulletLogicDomain;
            bulletLogicDomain.TearDownBulletLogic(bulletLogic);

            var bulletRendererDomain = domain.BulletRendererDomain;
            bulletRendererDomain.TearDownBulletRenderer(bulletLogic.IDComponent.EntityID);

        }

        #endregion

        #region [Regist]

        void OnBulletSpawn(FrameBulletSpawnResMsg msg)
        {
            Debug.Log($"加入子弹生成队列");
            bulletSpawnQueue.Enqueue(msg);
        }

        void OnBulletHitRole(FrameBulletHitRoleResMsg msg)
        {
            Debug.Log("加入子弹击中角色队列");
            bulletHitRoleQueue.Enqueue(msg);
        }

        void OnBulletHitField(FrameBulletHitFieldResMsg msg)
        {
            Debug.Log("加入子弹击中墙壁队列");
            bulletHitFieldQueue.Enqueue(msg);
        }

        void OnBulletLifeOver(FrameBulletLifeOverResMsg msg)
        {
            Debug.Log($"{msg.bulletType.ToString()} 加入子弹销毁队列");
            bulletLifeOverQueue.Enqueue(msg);
        }

        #endregion

    }

}