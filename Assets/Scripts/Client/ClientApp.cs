using System;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using Game.UI.Facades;
using Game.Infrastructure.Generic;
using Game.Infrastructure.Network.Client.Facades;
using Game.Client.Bussiness.LoginBussiness;
using Game.Client.Bussiness.WorldBussiness;
using Game.Client.Bussiness.EventCenter.Facades;
using Game.Manager;
using Game.UI;

namespace Game.Client
{

    public class ClientApp : MonoBehaviour
    {
        public string CurrentSceneName { get; private set; }
        void Awake()
        {
            DontDestroyOnLoad(this.gameObject);

            // == Network ==
            AllClientNetwork.Ctor();
            AllBussinessEvent.Ctor();
            StartClient();

            // == Entry ==
            // Login
            LoginEntry.Ctor();
            LoginEntry.Inject(AllClientNetwork.networkClient);
            LoginEntry.Init();
            // World
            WorldEntry.Ctor();
            WorldEntry.Inject(AllClientNetwork.networkClient);
            WorldEntry.Init();

            Action action = async () =>
            {
                // == Manager Init ==
                // UI
                UIManager.Ctor();
                AllUIAssets.Ctor();
                await AllUIAssets.LoadAll();
                // Camera
                CameraManager.Ctor();
                var uiCamTrans = CameraManager.UICamTrans;
                uiCamTrans.SetParent(UIManager.UIRoot.transform, false);
                DontDestroyOnLoad(uiCamTrans);
                // == Load Login Scene ==
                Addressables.LoadSceneAsync("LoginScene", LoadSceneMode.Single);
                SceneManager.sceneLoaded -= LoginSceneLoaded;
                SceneManager.sceneLoaded += LoginSceneLoaded;
            };

            action.Invoke();
        }

        void Update()
        {
            LoginEntry.Tick();
            WorldEntry.Tick();
        }

        void LoginSceneLoaded(Scene scene, LoadSceneMode sceneMode)
        {
            CurrentSceneName = scene.name;
            if (CurrentSceneName == "LoginScene")
            {
                UIManager.OpenUI("Home_LoginPanel");
            }
        }

        void StartClient()
        {
            Debug.Log("Start Client---------------------------------");

            var networkClient = AllClientNetwork.networkClient;

            networkClient.OnConnectedHandle += () =>
            {
                Debug.Log("Connect Success");
            };

            networkClient.Connect(NetworkConfig.host, NetworkConfig.port);

            new Thread(() =>
            {
                while (true)
                {
                    networkClient.Tick();
                }
            }).Start();

        }

        void OnDestroy()
        {
            WorldEntry.TearDown();
        }

    }

}

