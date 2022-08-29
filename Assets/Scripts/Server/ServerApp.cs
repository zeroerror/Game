using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Game.Protocol.Client2World;
using Game.Infrastructure.Generic;
using Game.Infrastructure.Network.Server.Facades;
using Game.Server.Bussiness.LoginBussiness;
using Game.Server.Bussiness.WorldBussiness;
using Game.Server.Bussiness.WorldBussiness.Facades;

namespace Game.Server
{

    public class ServerApp : MonoBehaviour
    {

        // Network
        AllServerNetwork allServerNetwork;

        // Entry
        WorldEntry worldEntry;
        LoginEntry loginEntry;

        void Awake()
        {
            // == Network ==
            allServerNetwork = new AllServerNetwork();
            StartServer();

            // == Entry ==
            // WorldEntry
            worldEntry = new WorldEntry();
            worldEntry.Inject(allServerNetwork.networkServer);
            worldEntry.Init();
            // LoginEntry
            loginEntry = new LoginEntry();
            loginEntry.Inject(allServerNetwork.networkServer);

            DontDestroyOnLoad(this.gameObject);




        }

        void FixedUpdate()
        {
            // == Entry ==
            worldEntry.Tick();
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

