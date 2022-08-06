using System;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using Game.Manager;
using Game.Facades;
using Game.Infrastructure.Network.Client.Facades;

namespace Game.Client
{

    public class ClientApp : MonoBehaviour
    {
        string host = "localhost";
        int port = 4000;
        async void Awake()
        {
            DontDestroyOnLoad(this.gameObject);

            Debug.Log("Asset Load---------------");
            // Asset Load
            AllAssets.Ctor();
            await AllAssets.LoadAll();

            Debug.Log("Manager Init---------------");

            // ==Manager Init
            // CameraMgr
            CameraMgr.Init();
            var uiCamTrans = CameraMgr.UICamTrans;
            DontDestroyOnLoad(uiCamTrans);

            // UIMgr
            UIMgr.Init();

            // Load Login Scene
            SceneManager.LoadSceneAsync("LoginScene", LoadSceneMode.Single);
            SceneManager.sceneLoaded -= LoginSceneLoaded;
            SceneManager.sceneLoaded += LoginSceneLoaded;

            //Network
            AllClientNetwork.Ctor();
            StartClient();
        }

        void Update()
        {

        }

        void LoginSceneLoaded(Scene scene, LoadSceneMode sceneMode)
        {
            UIMgr.OpenUI("Home_LoginPanel");
            Debug.Log($"[Scene Loaded]: {scene.name}");
        }

        void StartClient()
        {
            Debug.Log("Start Client---------------------------------");

            var networkClient = AllClientNetwork.networkClient;

            networkClient.OnConnectedHandle += () =>
            {
                Debug.Log("客户端: 连接成功");
            };

            networkClient.Connect(host, port);

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

