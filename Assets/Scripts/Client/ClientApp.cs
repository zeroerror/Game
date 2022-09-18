using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using Game.Infrastructure.Generic;
using Game.Infrastructure.Network.Client.Facades;
using Game.Client.Bussiness.EventCenter;
using Game.Client.Bussiness.LoginBussiness;
using Game.Client.Bussiness.WorldBussiness;
using Game.UI;
using Game.UI.Assets;
using Game.UI.Manager;
using MySql.Data.MySqlClient;
using System.Data;

namespace Game.Client
{

    public class ClientApp : MonoBehaviour
    {

        Thread _clientThread;
        public string CurrentSceneName { get; private set; }
        InputComponent _inputComponent;
        float time;

        void Awake()
        {
            DontDestroyOnLoad(this.gameObject);

            // ====== Network ======
            AllClientNetwork.Ctor();
            StartClient();

            // ====== EventCenter ======
            NetworkEventCenter.Ctor();
            LocalEventCenter.Ctor();

            // ====== Entry ======
            // Login
            LoginEntry.Ctor();
            LoginEntry.Inject(AllClientNetwork.networkClient);
            LoginEntry.Init();
            // World
            WorldEntry.Ctor();
            _inputComponent = new InputComponent();
            WorldEntry.Inject(AllClientNetwork.networkClient, _inputComponent);
            WorldEntry.Init();
            // UI
            UIEntry.Ctor();

            // ====== Manager ======
            UIManager.Ctor();
            CameraManager.Ctor();

            // ======  Asset ======
            Action action = async () =>
            {
                await LoadAllAsset();

                var uiCamTrans = CameraManager.UICamTrans;
                uiCamTrans.SetParent(UIManager.UIRoot.transform, false);
                DontDestroyOnLoad(uiCamTrans);

                // == Load Login Scene ==
                Addressables.LoadSceneAsync("LoginScene", LoadSceneMode.Single);
                SceneManager.sceneLoaded -= LoginSceneLoaded;
                SceneManager.sceneLoaded += LoginSceneLoaded;
            };

            action.Invoke();

            // ======  Input ======
            InputGameSet.Ctor();

            // ====== Physics ======
            Physics.autoSimulation = false;
        }

        void FixedUpdate()
        {
            // == Entry ==
            LoginEntry.Tick();
            WorldEntry.Tick();
            UIEntry.Tick();

            // == EventCenter ==
            NetworkEventCenter.Tick();
            LocalEventCenter.Tick();
        }

        void Update()
        {
            InputGameSet.Receive_Input(ref _inputComponent);
            WorldEntry.Update();
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
            Debug.Log("启动客户端---------------------------------");

            var networkClient = AllClientNetwork.networkClient;

            networkClient.OnConnectedHandle += () =>
            {
                Debug.Log("连接服务器成功*************************************************************");
            };

            networkClient.Connect(NetworkConfig.HOST, NetworkConfig.PORT);

            _clientThread = new Thread(() =>
            {
                while (true)
                {
                    networkClient.Tick();
                }
            });
            _clientThread.Start();
        }

        async Task LoadAllAsset()
        {
            await UIPanelAssets.LoadAll();
        }

        void OnDestroy()
        {
            LoginEntry.TearDown();
            WorldEntry.TearDown();
            UIEntry.TearDown();
            InputGameSet.TearDown();

            _clientThread.Abort();
        }

    }

}

