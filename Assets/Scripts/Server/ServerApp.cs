using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Game.Protocol.Client2World;
using Game.Infrastructure.Generic;
using Game.Infrastructure.Network.Server.Facades;
using Game.Server.Bussiness.WorldBussiness;
using Game.Server.Bussiness.WorldBussiness.Facades;

namespace Game.Server
{

    public class ServerApp : MonoBehaviour
    {

        // Network
        AllServerNetwork allServerNetwork;

        WorldEntry worldEntry;

        public class LoginEvent
        {
            public int connID;
            public int status;
            public string userToken;
        }
        public List<LoginEvent> loginEventList;

        void Awake()
        {
            // == Network ==
            allServerNetwork = new AllServerNetwork();
            StartServer();

            // == Entry ==
            // WorldEntry
            worldEntry = new WorldEntry();
            worldEntry.Inject(allServerNetwork.networkServer);

            DontDestroyOnLoad(this.gameObject);
            loginEventList = new List<LoginEvent>();

        }

        void FixedUpdate()
        {
            if (loginEventList.GetEnumerator().MoveNext())
            {
                var ev = loginEventList[0];
                var networkServer = allServerNetwork.networkServer;
                networkServer.SendMsg<LoginResMessage>(ev.connID, new LoginResMessage
                {
                    status = 1,
                    userToken = "testusertoken"
                });
                loginEventList.Remove(ev);
            }

            // == Entry ==
            worldEntry.Tick();

        }

        void StartServer()
        {
            Debug.Log("服务端启动！");
            var networkServer = allServerNetwork.networkServer;
            networkServer.OnConnectedHandle += (connID) =>
            {
                Debug.Log($"服务端: connID:{connID} 客户端连接成功-------------------------");
            };

            networkServer.AddRegister<LoginReqMessage>((connId, msg) =>
            {
                Debug.Log($"服务端: 账户登录请求 connId:{connId}  account:{msg.account}  pwd:{msg.pwd}");

                lock (loginEventList)
                {
                    loginEventList.Add(new LoginEvent
                    {
                        connID = connId,
                        status = 1,
                        userToken = "Test Token"
                    });
                }
            });

            networkServer.StartListen(NetworkConfig.port);
            new Thread(() =>
            {
                while (true)
                {
                    networkServer.Tick();
                }
            }).Start();
        }

    }

}

