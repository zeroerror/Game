using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using Game.Client.Bussiness.EventCenter;
using Game.Client.Bussiness.WorldBussiness.Facades;
using Game.Protocol.Client2World;
using System.Collections.Generic;
using Game.Protocol.Login;

namespace Game.Client.Bussiness.WorldBussiness.Controller
{

    public class WorldController
    {

        Queue<WolrdEnterResMessage> worldEnterQueue;
        Queue<WolrdLeaveResMessage> worldLeaveQueue;

        WorldFacades worldFacades;

        public WorldController()
        {
            NetworkEventCenter.RegistLoginSuccess(OnLoginSuccess);
            NetworkEventCenter.RegistConnWorSerSuccess(SendWorldEnterReq);
            UIEventCenter.ConnWorSerAction += SendConnWorSer;

            worldEnterQueue = new Queue<WolrdEnterResMessage>();
            worldLeaveQueue = new Queue<WolrdLeaveResMessage>();

        }

        public void Inject(WorldFacades worldFacades)
        {
            this.worldFacades = worldFacades;
            worldFacades.Network.WorldReqAndRes.RegistRes_WorldEnter(OnEnterWorldRes);
            worldFacades.Network.WorldReqAndRes.RegistRes_WorldLeave(OnLeaveWorldRes);
        }

        public void Tick()
        {
            while (worldEnterQueue.TryDequeue(out var msg))
            {
                UIEventCenter.EnqueueTearDownQueue("Home_WorldServerPanel");
                var entityId = msg.entityId;
                var account = msg.account;
                WorldRoleEntity roleEntity = new WorldRoleEntity();
                roleEntity.SetAccount(msg.account);
                roleEntity.SetEntityId(entityId);
                var roleRepo = worldFacades.Repo.WorldRoleRepo;
                roleRepo.Add(roleEntity);
                if (msg.isOwner) roleRepo.SetOwner(roleEntity);
                SpawnScene("world_scene");

                Debug.Log($"entityId:{entityId}  account:{account} 进入世界 当前在线人数:{roleRepo.Count}");
            }

            while (worldLeaveQueue.TryDequeue(out var msg))
            {
                UIEventCenter.EnqueueTearDownQueue("Home_WorldServerPanel");
                var entityId = msg.entityId;
                var account = msg.account;
                var roleRepo = worldFacades.Repo.WorldRoleRepo;
                roleRepo.RemoveByEntityId(entityId);
                Debug.Log($"entityId:{entityId}  account:{account} 离开世界 当前在线人数:{roleRepo.Count}");
            }
        }

        void OnLoginSuccess(string account, string[] worldSerHosts, ushort[] ports)
        {

            worldFacades.Repo.WorldRoleRepo.SetAccount(account);

            // UI
            object[] args = { account, worldSerHosts, ports };
            UIEventCenter.EnqueueTearDownQueue("Home_LoginPanel");
            UIEventCenter.EnqueueOpenQueue(new OpenEventModel { uiName = "Home_WorldServerPanel", args = args });

            // Scene
            SpawnScene("world_choose_scene");
        }



        #region [SEND]

        void SendConnWorSer(string host, ushort port)
        {
            var rqs = worldFacades.Network.WorldReqAndRes;
            rqs.ConnWorldServer(host, port);
        }

        void SendWorldEnterReq()
        {
            var rqs = worldFacades.Network.WorldReqAndRes;
            rqs.SendReq_WorldEnterMsg(worldFacades.Repo.WorldRoleRepo.Account);
        }

        #endregion

        #region [RESPONSE]

        void OnEnterWorldRes(WolrdEnterResMessage msg)
        {
            worldEnterQueue.Enqueue(msg);
        }

        void OnLeaveWorldRes(WolrdLeaveResMessage msg)
        {
            worldLeaveQueue.Enqueue(msg);
        }

        #endregion

        #region [Private Func]
        public async void SpawnScene(string sceneName)
        {
            Debug.Log($"开始加载世界:{sceneName}");
            var result = await Addressables.LoadSceneAsync(sceneName, LoadSceneMode.Single).Task;
            Debug.Log($"加载世界完成:{sceneName}");
            return;
        }
        #endregion

    }

}