using System.Threading;
using UnityEngine;
using Game.Infrastructure.Generic;
using Game.Infrastructure.Network.Server.Facades;
using Game.Server.Bussiness.LoginBussiness;
using Game.Server.Bussiness.BattleBussiness;

namespace Game.Server
{

    public class ServerApp : MonoBehaviour
    {

        // Network
        AllServerNetwork allServerNetwork;

        // Entry
        BattleEntry battleEntry;
        LoginEntry loginEntry;

        Thread _loginThread;
        Thread _worldServerThread;

        void Awake()
        {
            // == Network ==
            allServerNetwork = new AllServerNetwork();
            StartLoginServer();
            StartWorldServer();

            // == Entry ==
            // BattleEntry
            battleEntry = new BattleEntry();
            battleEntry.Inject(allServerNetwork.LoginServer, UnityEngine.Time.fixedDeltaTime);
            battleEntry.Init();
            // LoginEntry
            loginEntry = new LoginEntry();
            loginEntry.Inject(allServerNetwork.LoginServer);

            DontDestroyOnLoad(this.gameObject);

            // == Physics ==
            Physics.autoSimulation = false;
        }

        void FixedUpdate()
        {
            // == Entry ==
            battleEntry.Tick();
            loginEntry.Tick();

        }

        bool isCancel;
        void StartLoginServer()
        {
            Debug.Log("登录服启动！");
            var networkServer = allServerNetwork.LoginServer;
            networkServer.StartListen(NetworkConfig.LOGIN_PORT);
            _loginThread = new Thread(() =>
                        {
                            while (!isCancel)
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
            Debug.Log("世界服启动！");
            var worldServer = allServerNetwork.WorldServer;
            worldServer.StartListen(NetworkConfig.WORLDSERVER_PORT[0]);
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
                Debug.Log($"[世界服]: connID:{connID} 客户端连接成功-------------------------");
            };
        }

    }

}

