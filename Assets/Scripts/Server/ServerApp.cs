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

        void Awake()
        {
            // == Network ==
            allServerNetwork = new AllServerNetwork();
            StartServer();

            // == Entry ==
            // BattleEntry
            battleEntry = new BattleEntry();
            battleEntry.Inject(allServerNetwork.networkServer, UnityEngine.Time.fixedDeltaTime);
            battleEntry.Init();
            // LoginEntry
            loginEntry = new LoginEntry();
            loginEntry.Inject(allServerNetwork.networkServer);

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

        void StartServer()
        {
            Debug.Log("服务端启动！");
            var networkServer = allServerNetwork.networkServer;
            networkServer.StartListen(NetworkConfig.PORT);
            new Thread(() =>
                      {
                          while (true)
                          {
                              networkServer.Tick();
                          }
                      }).Start();

            networkServer.OnConnectedHandle += (connID) =>
            {
                Debug.Log($"服务端: connID:{connID} 客户端连接成功-------------------------");
            };

        }

    }

}

