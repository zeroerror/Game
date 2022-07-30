using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using ZeroFrame.Network;
using ZeroFrame.Protocol;

namespace Game.Client
{

    public class ClientApp : MonoBehaviour
    {

        NetworkServer networkServer;
        NetworkClient networkClient;
        List<ChatSendReqMessage> serverRevQueue = new List<ChatSendReqMessage>();
        List<ChatSendReqMessage> clientRevQueue = new List<ChatSendReqMessage>();

        const string host = "localhost";
        const int port = 4000;
        void Awake()
        {
            StartServer();
        }

        void Start()
        {
            StartClient();
        }

        bool isSend = false;
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F) && networkClient.Connected)
            {
                var msg = new ChatSendReqMessage();
                msg.msg = "了解你的捍卫者";
                networkClient.SendMsg(1, 1, msg);
                Debug.Log($"Client: 发送信息:{msg.msg}");
            }

            if (serverRevQueue.Count > 0)
            {
                ChatSendReqMessage revMsg = (ChatSendReqMessage)serverRevQueue[serverRevQueue.Count - 1];
                Debug.Log($"Server: 收到客户端消息：{revMsg.msg}");
                ChatSendReqMessage sendMsg = new ChatSendReqMessage();
                sendMsg.msg = "这是你的捍卫者：微笑机器人 level 4 20 4000";
                networkServer.SendMsg(1, 1, sendMsg, 1);
                serverRevQueue.RemoveAt(serverRevQueue.Count - 1);
            }

            if (clientRevQueue.Count > 0)
            {
                ChatSendReqMessage revMsg = (ChatSendReqMessage)clientRevQueue[clientRevQueue.Count - 1];
                Debug.Log($"Client: 收到了服务器的回复信息：{revMsg.msg}");
                clientRevQueue.RemoveAt(clientRevQueue.Count - 1);
            }

        }

        void StartServer()
        {
            networkServer = new NetworkServer(4096);
            networkServer.StartListen(port);
            networkServer.OnConnectedHandle += (connId) =>
            {
                Debug.Log($"Server: connId:{connId} 客户端连接成功！");
            };
            networkServer.OnDisconnectedHandle += (connId) =>
            {
                Debug.Log($"Server: connId:{connId} 客户端断开连接！");
            };
            networkServer.AddRegister(1, 1, () => new ChatSendReqMessage(), (connID, msg) =>
            {
                lock (serverRevQueue)
                {
                    serverRevQueue.Add(msg);
                }
            });

            Thread thead = new Thread(ServerTick);
            thead.Start();
        }

        void ServerTick()
        {
            while (true)
            {
                networkServer.Tick();
            }
        }

        void StartClient()
        {
            networkClient = new NetworkClient(1026);
            networkClient.Connect(host, port);
            networkClient.OnConnectedHandle += () =>
            {
                Debug.Log($"Client: 客户端连接 host:{host} port:{port} 成功！");
            };
            networkClient.AddRegister(1, 1, () => new ChatSendReqMessage(), (msg) =>
            {
                clientRevQueue.Add(msg);
            });
            Thread thead = new Thread(ClientTick);
            thead.Start();

        }

        void ClientTick()
        {
            while (true)
            {
                networkClient.Tick();
            }
        }


    }

}


