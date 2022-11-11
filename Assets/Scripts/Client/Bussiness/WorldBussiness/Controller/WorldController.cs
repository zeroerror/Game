using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using Game.Client.Bussiness.EventCenter;
using Game.Client.Bussiness.WorldBussiness.Facades;
using Game.Protocol.Client2World;
using System.Collections.Generic;
using Game.Protocol.Login;
using Game.Protocol.World;

namespace Game.Client.Bussiness.WorldBussiness.Controller
{

    public class WorldController
    {

        Queue<WolrdEnterResMessage> worldEnterQueue;
        Queue<WolrdLeaveResMsg> worldLeaveQueue;
        Queue<WorldCreateRoomResMsg> worldLRoomCreateQueue;
        Queue<WorldRoomDismissResMsg> worldLRoomDismissQueue;
        Queue<WorldAllRoomsBacisInfoResMsg> worldAllRoomsBacisInfoQueue;

        WorldFacades worldFacades;

        public WorldController()
        {
            NetworkEventCenter.Regist_LoginSuccess(OnLoginSuccess);
            NetworkEventCenter.Regist_ConnWorSerSuccess(OnConnWorSerSuccess);

            UIEventCenter.World_ConAction += SendConnWorSer;
            UIEventCenter.World_CreateRoomAction += SendReq_CreateWorldRoom;
            UIEventCenter.World_ReqAllRoomsBasicInfoAction += SendReq_GetAllWorldRoomsBasicInfo;

            worldEnterQueue = new Queue<WolrdEnterResMessage>();
            worldLeaveQueue = new Queue<WolrdLeaveResMsg>();
            worldLRoomCreateQueue = new Queue<WorldCreateRoomResMsg>();
            worldLRoomDismissQueue = new Queue<WorldRoomDismissResMsg>();
            worldAllRoomsBacisInfoQueue = new Queue<WorldAllRoomsBacisInfoResMsg>();

        }

        public void Inject(WorldFacades worldFacades)
        {
            this.worldFacades = worldFacades;
            worldFacades.Network.WorldReqAndRes.RegistRes_WorldEnter(OnEnterWorldRes);
            worldFacades.Network.WorldReqAndRes.RegistRes_WorldLeave(OnLeaveWorldRes);
            worldFacades.Network.WorldReqAndRes.RegistRes_WorldGetAllRoomsBacisInfo(OnWorldGetAllRoomsBasicInfo);
            worldFacades.Network.WorldReqAndRes.RegistRes_WorldRoomCreate(OnWorldRoomCreate);
            worldFacades.Network.WorldReqAndRes.RegistRes_WorldRoomDismiss(OnWorldRoomADismiss);
        }

        public void Tick()
        {
            Tick_WorldEnter();
            Tick_WorldLeave();
            Tick_WorldRoomCreate();
            Tick_WorldRoomDismiss();
            Tick_WorldGetAllRoomsBasicInfo();
        }

        void OnLoginSuccess(string account, string[] worldSerHosts, ushort[] ports)
        {

            worldFacades.Repo.WorldRoleRepo.SetAccount(account);

            // UI
            object[] args = { account, worldSerHosts, ports };
            UIEventCenter.AddToTearDown("Home_LoginPanel");
            UIEventCenter.AddToOpen(new OpenEventModel { uiName = "Home_WorldServerPanel", args = args });

            // Scene
            SpawnScene("world_choose_scene");
        }

        #region [SEND]

        void SendConnWorSer(string host, ushort port)
        {
            var rqs = worldFacades.Network.WorldReqAndRes;
            rqs.ConnWorldServer(host, port);
        }

        void OnConnWorSerSuccess()
        {
            var rqs = worldFacades.Network.WorldReqAndRes;
            rqs.SendReq_EnterWorld(worldFacades.Repo.WorldRoleRepo.Account);
        }

        void SendReq_CreateWorldRoom(string roomName)
        {
            var rqs = worldFacades.Network.WorldReqAndRes;
            rqs.SendReq_CreateWorldRoom(roomName);
        }

        void SendReq_GetAllWorldRoomsBasicInfo()
        {
            var rqs = worldFacades.Network.WorldReqAndRes;
            rqs.SendReq_GetAllWorldRoomsBasicInfo();
        }

        #endregion

        void Tick_WorldEnter()
        {
            while (worldEnterQueue.TryDequeue(out var msg))
            {
                var entityId = msg.entityId;
                var account = msg.account;
                WorldRoleEntity roleEntity = new WorldRoleEntity();
                roleEntity.SetAccount(msg.account);
                roleEntity.SetEntityId(entityId);
                var roleRepo = worldFacades.Repo.WorldRoleRepo;
                roleRepo.Add(roleEntity);
                if (msg.isOwner) roleRepo.SetOwner(roleEntity);

                UIEventCenter.AddToTearDown("Home_WorldServerPanel");
                UIEventCenter.AddToOpen(new OpenEventModel { uiName = "Home_WorldRoomPanel" });
                SpawnScene("world_scene");

                Debug.Log($"entityId:{entityId}  account:{account} 进入世界 当前在线人数:{roleRepo.Count}");
            }
        }

        void OnEnterWorldRes(WolrdEnterResMessage msg)
        {
            worldEnterQueue.Enqueue(msg);
        }

        void Tick_WorldLeave()
        {
            while (worldLeaveQueue.TryDequeue(out var msg))
            {
                UIEventCenter.AddToTearDown("Home_WorldServerPanel");
                var entityId = msg.entityId;
                var account = msg.account;
                var roleRepo = worldFacades.Repo.WorldRoleRepo;
                roleRepo.RemoveByEntityId(entityId);
                Debug.Log($"entityId:{entityId}  account:{account} 离开世界 当前在线人数:{roleRepo.Count}");
            }
        }

        void OnLeaveWorldRes(WolrdLeaveResMsg msg)
        {
            worldLeaveQueue.Enqueue(msg);
        }

        void Tick_WorldRoomCreate()
        {
            while (worldLRoomCreateQueue.TryDequeue(out var msg))
            {
                NetworkEventCenter.Invoke_WorldRoomCreate(msg);
            }
        }

        void OnWorldRoomCreate(WorldCreateRoomResMsg msg)
        {
            worldLRoomCreateQueue.Enqueue(msg);
        }

        void Tick_WorldRoomDismiss()
        {
            while (worldLRoomDismissQueue.TryDequeue(out var msg))
            {
                NetworkEventCenter.Invoke_WorldRoomDismiss(msg);
            }
        }

        void OnWorldRoomADismiss(WorldRoomDismissResMsg msg)
        {
            Debug.Log("OnWorldRoomADismiss");
            worldLRoomDismissQueue.Enqueue(msg);
        }

        void Tick_WorldGetAllRoomsBasicInfo()
        {
            while (worldAllRoomsBacisInfoQueue.TryDequeue(out var msg))
            {
                var wRoleRepo = worldFacades.Repo.WorldRoleRepo;
                var allWRoles = wRoleRepo.GetAll();
                var length = allWRoles.Length;
                string[] accountArray = new string[length];
                for (int i = 0; i < length; i++)
                {
                    accountArray[i] = allWRoles[i].Account;
                }

                NetworkEventCenter.Invoke_AllWorldRoomsBasicInfo(msg, accountArray);
            }
        }

        void OnWorldGetAllRoomsBasicInfo(WorldAllRoomsBacisInfoResMsg msg)
        {
            worldAllRoomsBacisInfoQueue.Enqueue(msg);
        }

        public async void SpawnScene(string sceneName)
        {
            Debug.Log($"开始加载世界:{sceneName}");
            var result = await Addressables.LoadSceneAsync(sceneName, LoadSceneMode.Single).Task;
            Debug.Log($"加载世界完成:{sceneName}");
            return;
        }

    }

}