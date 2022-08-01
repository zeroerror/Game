using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using ZeroFrame.Network;
using ZeroFrame.Protocol;
using Game.Network;

namespace Game.Client
{

    public class Test : MonoBehaviour
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
                msg.msg = "�˽���ĺ�����";
                networkClient.SendMsg(1, 1, msg);
                Debug.Log($"Client: ������Ϣ:{msg.msg}");
            }

            if (serverRevQueue.Count > 0)
            {
                ChatSendReqMessage revMsg = (ChatSendReqMessage)serverRevQueue[serverRevQueue.Count - 1];
                Debug.Log($"Server: �յ��ͻ�����Ϣ��{revMsg.msg}");
                ChatSendReqMessage sendMsg = new ChatSendReqMessage();
                sendMsg.msg = "������ĺ����ߣ�΢Ц������ level 4 20 4000";
                networkServer.SendMsg(1, 1, sendMsg, 1);
                serverRevQueue.RemoveAt(serverRevQueue.Count - 1);
            }

            if (clientRevQueue.Count > 0)
            {
                ChatSendReqMessage revMsg = (ChatSendReqMessage)clientRevQueue[clientRevQueue.Count - 1];
                Debug.Log($"Client: �յ��˷������Ļظ���Ϣ��{revMsg.msg}");
                clientRevQueue.RemoveAt(clientRevQueue.Count - 1);
            }

        }

        void StartServer()
        {
            networkServer = new NetworkServer(4096);
            networkServer.StartListen(port);
            networkServer.OnConnectedHandle += (connId) =>
            {
                Debug.Log($"Server: connId:{connId} �ͻ������ӳɹ���");
            };
            networkServer.OnDisconnectedHandle += (connId) =>
            {
                Debug.Log($"Server: connId:{connId} �ͻ��˶Ͽ����ӣ�");
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
                Debug.Log($"Client: �ͻ������� host:{host} port:{port} �ɹ���");
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


