using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Game.Protocol.Client2World;
using Game.Infrastructure.Generic;
using Game.Infrastructure.Network.Server.Facades;

namespace Game.Server
{

    public class ServerApp : MonoBehaviour
    {
        public class LoginEvent
        {
            public int connID;
            public int status;
            public string userToken;
        }
        public List<LoginEvent> loginEventList;
        void Awake()
        {
            DontDestroyOnLoad(this.gameObject);

            loginEventList = new List<LoginEvent>();

            AllServerNetwork.Ctor();
            StartServer();
        }

        void Update()
        {
            if (loginEventList.GetEnumerator().MoveNext())
            {
                var ev = loginEventList[0];
                var networkServer = AllServerNetwork.networkServer;
                networkServer.SendMsg<LoginResMessage>(1, 1, ev.connID, new LoginResMessage
                {
                    status = 1,
                    userToken = "testusertoken"
                });
                loginEventList.Remove(ev);
            }
        }

        void StartServer()
        {
            var networkServer = AllServerNetwork.networkServer;

            networkServer.OnConnectedHandle += (connID) =>
            {
                Debug.Log($"服务端: connID:{connID} 客户端连接成功-------------------------");
            };

            networkServer.RegistMsg<LoginReqMessage>(1, 1, (connId, msg) =>
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

