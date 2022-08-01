using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Game.Manager;
using Game.Facades;
using Game.Client.Network;

namespace Game.Client
{

    public class ClientApp : MonoBehaviour
    {

        async void Awake()
        {
            Debug.Log("Asset Load---------------");
            // Asset Load
            AllAssets.Ctor();
            await AllAssets.LoadAll();

            Debug.Log("Manager Init---------------");
            // Manager Init

            // == CameraMgr
            CameraMgr.Init();
            var uiCamTrans = CameraMgr.UICamTrans;
            DontDestroyOnLoad(uiCamTrans);

            // == UIMgr
            UIMgr.Init();

            Debug.Log("Load Login Scene---------------");
            // Load Login Scene
            SceneManager.LoadSceneAsync("LoginScene", LoadSceneMode.Single);
            SceneManager.sceneLoaded -= LoginSceneLoaded;
            SceneManager.sceneLoaded += LoginSceneLoaded;

            //Network
            StartClient();

        }

        void Update()
        {
            // Tick


        }

        void LoginSceneLoaded(Scene scene, LoadSceneMode sceneMode)
        {
            UIMgr.OpenUI("Home_LoginPanel");
        }

        void StartServer()
        {
        }

        void StartClient()
        {
            Debug.Log("StartClient");
            string host = "localhost";
            int port = 4000;
            AllNetwork.Ctor();
            var networkClient = AllNetwork.networkClient;
            var networkServer = AllNetwork.networkServer;

            networkServer.OnConnectedHandle += (connID) =>
                    {
                        Debug.Log($"Server: connID:{connID} 客户端连接成功");
                    };


            networkClient.OnConnectedHandle += () =>
            {
                Debug.Log("Client: 客户端连接成功");
            };


            networkServer.StartListen(port);
            networkClient.Connect(host, port);

            new Thread(() =>
            {
                while (true)
                {
                    networkServer.Tick();
                }
            }).Start();

            new Thread(() =>
            {
                while (true)
                {
                    networkClient.Tick();
                }
            }).Start();

        }

    }

}

