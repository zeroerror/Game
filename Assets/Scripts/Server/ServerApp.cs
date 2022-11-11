using System.Threading;
using System.Collections.Generic;
using UnityEngine;
using Game.Infrastructure.Generic;
using Game.Infrastructure.Network.Server.Facades;
using Game.Server.Bussiness.LoginBussiness;
using Game.Server.Bussiness.LoginBussiness.Facades;
using Game.Server.Bussiness.BattleBussiness;
using Game.Server.Bussiness.BattleBussiness.Facades;
using Game.Server.Bussiness.WorldBussiness;
using Game.Server.Bussiness.WorldBussiness.Facades;
using Game.Server.Bussiness.EventCenter;
using Game.Client.Bussiness.BattleBussiness.Generic;
using Game.Infrastructure.Network.Server;

namespace Game.Server
{

    public class ServerApp : MonoBehaviour
    {

        // ------ Network
        AllServerNetwork allServerNetwork;
        float fixedDeltaTime;

        // ------ Entry
        LoginEntry loginEntry;
        Thread loginThread;
        ServerLoginFacades loginFacades;

        WorldEntry worldEntry;
        Thread worldServerThread;
        ServerWorldFacades worldFacades;

        List<BattleEntry> battleEntryList;
        List<Thread> battleServerThreadList;

        void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
            fixedDeltaTime = UnityEngine.Time.fixedDeltaTime;
            battleEntryCreateQueue = new Queue<BattleEntryCtorModel>();

            // == Network ==
            allServerNetwork = new AllServerNetwork();
            StartLoginServer();
            StartWorldServer();

            // == Event Center ==
            ServerNetworkEventCenter.Ctor();
            ServerNetworkEventCenter.Regist_StartBattleServer(StartBattleServer);
            ServerNetworkEventCenter.Regist_BattleServerConnHandler((connID, networkServer) =>
            {
                battleEntryCreateQueue.Enqueue(new BattleEntryCtorModel { connID = connID, networkServer = networkServer });
            });

            // == Entry ==
            // - Login Entry
            loginFacades = new ServerLoginFacades();
            loginFacades.Inject(allServerNetwork.LoginServer);
            loginEntry = new LoginEntry();
            loginEntry.Inject(loginFacades);

            // - World Entry
            worldFacades = new ServerWorldFacades();
            worldFacades.Inject(allServerNetwork.WorldServer);
            worldEntry = new WorldEntry();
            worldEntry.Inject(worldFacades);

            // - Battle Entry
            battleEntryList = new List<BattleEntry>();
            battleServerThreadList = new List<Thread>();

            // == Physics ==
            Physics.autoSimulation = false;
        }

        struct BattleEntryCtorModel
        {
            public int connID;
            public NetworkServer networkServer;
        }

        Queue<BattleEntryCtorModel> battleEntryCreateQueue;

        void FixedUpdate()
        {
            // == Entry ==
            loginEntry.Tick();
            worldEntry.Tick();
            battleEntryList.ForEach((battleEntry) =>
            {
                battleEntry.Tick(fixedDeltaTime);
            });

            while (battleEntryCreateQueue.TryDequeue(out var model))
            {
                var connID = model.connID;
                var networkServer = model.networkServer;
                Debug.Log($"battleEntryCreateQueue connID: {connID}");

                // - Facades
                var facades = new ServerBattleFacades();
                facades.Inject(networkServer);
                facades.Network.connIdList.Add(connID);

                // - Entry
                BattleEntry battleEntry = new BattleEntry();
                battleEntry.Inject(facades);
                battleEntryList.Add(battleEntry);

                var battleFacades = facades.BattleFacades;
                var gameEntity = battleFacades.GameEntity;
                var gameStage = gameEntity.Stage;
                var fsm = gameEntity.FSMComponent;
                var gameState = fsm.BattleState;
                if (!gameStage.HasStage(BattleStage.Level1) && gameState != BattleState.SpawningField)
                {
                    fsm.EnterGameState_BattleSpawningField(BattleStage.Level1);
                }
            }

        }

        void OnDestroy()
        {
            loginThread.Abort();
            worldServerThread.Abort();
        }

        void StartLoginServer()
        {
            var port = NetworkConfig.LOGIN_PORT;
            Debug.Log($"登录服启动！端口:{port}");
            var networkServer = allServerNetwork.LoginServer;
            networkServer.StartListen(port);
            loginThread = new Thread(() =>
                        {
                            while (true)
                            {
                                networkServer.Tick();
                            }
                        });
            loginThread.Start();
            networkServer.OnConnectedHandle += (connID) =>
            {
                Debug.Log($"[登录服]: connID:{connID} 客户端连接成功-------------------------");
            };

        }

        void StartWorldServer()
        {
            var port = NetworkConfig.WORLDSERVER_PORT[0];
            Debug.Log($"世界服启动！端口:{port}");
            var worldServer = allServerNetwork.WorldServer;
            worldServer.StartListen(port);
            worldServerThread = new Thread(() =>
                        {
                            while (true)
                            {
                                worldServer.Tick();
                            }
                        });
            worldServerThread.Start();

            worldServer.OnConnectedHandle += (connID) =>
            {
                ServerNetworkEventCenter.Invoke_WorldConnection(connID);
            };

            worldServer.OnDisconnectedHandle += (connID) =>
            {
                ServerNetworkEventCenter.Invoke_WorldDisconnection(connID);
            };
        }

        void StartBattleServer(string host, ushort port)
        {
            if (battleServerThreadList.Count >= NetworkConfig.BATTLE_SERVER_MAX)
            {
                Debug.LogWarning($"战斗服达到最大数量[{NetworkConfig.BATTLE_SERVER_MAX}]限制");
                return;
            }

            Debug.Log($"战斗服启动 Host: {host} Port: {port}");
            var battleServer = allServerNetwork.BattleServerQueue.Dequeue();
            battleServer.StartListen(port);
            var thread = new Thread(() =>
            {
                while (true)
                {
                    battleServer.Tick();
                }
            });
            thread.Start();
            battleServerThreadList.Add(thread);

            battleServer.OnConnectedHandle += (connID) =>
            {
                Debug.Log($"战斗服启动 Host: {host} Port: {port}  客户端[{connID}]连接成功-------------------------");
                ServerNetworkEventCenter.Invoke_BattleServerConnect(connID, battleServer);
            };
        }
    }

}

