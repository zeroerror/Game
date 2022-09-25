using System.Threading;
using UnityEngine;
using Game.Infrastructure.Generic;
using Game.Infrastructure.Network.Server.Facades;
using Game.Server.Bussiness.LoginBussiness;
using Game.Server.Bussiness.BattleBussiness;
using Game.Protocol.Client2World;
using Game.Server.Bussiness.WorldBussiness;
using Game.Server.Bussiness.EventCenter;

namespace Game.Server
{

    public class ServerApp : MonoBehaviour
    {

        // Network
        AllServerNetwork allServerNetwork;

        // Entry
        BattleEntry battleEntry;
        LoginEntry loginEntry;
        WorldEntry worldEntry;

        Thread _loginThread;
        Thread _worldServerThread;
        Thread _battleServerThread;

        void Awake()
        {
            // == Network ==
            allServerNetwork = new AllServerNetwork();
            StartLoginServer();
            StartWorldServer();
            //战斗服的启动是由客户端在世界服候创建战斗对局决定启动

            // == Event Center ==
            ServerNetworkEventCenter.Ctor();
            ServerNetworkEventCenter.Regist_BattleServerNeedCreate(StartBatllteServer);

            // == Entry ==
            // LoginEntry
            loginEntry = new LoginEntry();
            loginEntry.Inject(allServerNetwork.LoginServer);
            // WorldEntry
            worldEntry = new WorldEntry();
            worldEntry.Inject(allServerNetwork.WorldServer);
            // BattleEntry
            battleEntry = new BattleEntry();
            battleEntry.Inject(allServerNetwork.BattleServer, UnityEngine.Time.fixedDeltaTime);


            DontDestroyOnLoad(this.gameObject);

            // == Physics ==
            Physics.autoSimulation = false;
        }

        void FixedUpdate()
        {
            // == Entry ==
            loginEntry.Tick();
            worldEntry.Tick();
            battleEntry.Tick();
        }

        void OnDestroy()
        {
            _loginThread.Abort();
            _worldServerThread.Abort();
        }

        void StartLoginServer()
        {
            var port = NetworkConfig.LOGIN_PORT;
            Debug.Log($"登录服启动！端口:{port}");
            var networkServer = allServerNetwork.LoginServer;
            networkServer.StartListen(port);
            _loginThread = new Thread(() =>
                        {
                            while (true)
                            {
                                networkServer.Tick();
                            }
                        });
            _loginThread.Start();
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
            _worldServerThread = new Thread(() =>
                        {
                            while (true)
                            {
                                worldServer.Tick();
                            }
                        });
            _worldServerThread.Start();

            worldServer.OnConnectedHandle += (connID) =>
            {
                ServerNetworkEventCenter.Invoke_WorldConnection(connID);
            };

            worldServer.OnDisconnectedHandle += (connID) =>
            {
                ServerNetworkEventCenter.Invoke_WorldDisconnection(connID);
            };
        }

        void StartBatllteServer()
        {
            var port = NetworkConfig.BATTLESERVER_PORT[0];
            Debug.Log($"战斗服启动！端口:{port}");
            var battleServer = allServerNetwork.BattleServer;
            battleServer.StartListen(port);
            _battleServerThread = new Thread(() =>
                        {
                            while (true)
                            {
                                battleServer.Tick();
                            }
                        });
            _battleServerThread.Start();

            battleServer.OnConnectedHandle += (connID) =>
            {
                Debug.Log($"[战斗服]: connID:{connID} 客户端连接成功-------------------------");
                ServerNetworkEventCenter.battleSerConnect.Invoke(connID);
            };
        }

    }

}

