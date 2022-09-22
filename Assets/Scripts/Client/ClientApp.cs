using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using Game.Infrastructure.Generic;
using Game.Infrastructure.Network.Client.Facades;
using Game.Client.Bussiness;
using Game.Client.Bussiness.EventCenter;
using Game.Client.Bussiness.LoginBussiness;
using Game.Client.Bussiness.BattleBussiness;
using Game.Bussiness.UIBussiness;

namespace Game.Client
{

    public class ClientApp : MonoBehaviour
    {

        Thread _clientThread;
        public string CurrentSceneName { get; private set; }
        InputComponent _inputComponent;
        float time;
        bool isStarted;


        void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
            StartAllAsync();
        }

        async void StartAllAsync()
        {
            // ======  Asset ======
            await LoadAllAsset();

            // == Load Login Scene ==
            Addressables.LoadSceneAsync("LoginScene", LoadSceneMode.Single);
            SceneManager.sceneLoaded -= LoginSceneLoaded;
            SceneManager.sceneLoaded += LoginSceneLoaded;

            // ====== EventCenter ======
            NetworkEventCenter.Ctor();
            LocalEventCenter.Ctor();

            // ====== Network ======
            AllClientNetwork.Ctor();

            // ====== Entry ======
            // Login
            LoginEntry.Ctor();
            LoginEntry.Inject(AllClientNetwork.networkClient);
            LoginEntry.Init();
            // Battle
            BattleEntry.Ctor();
            _inputComponent = new InputComponent();
            BattleEntry.Inject(AllClientNetwork.networkClient, _inputComponent);
            BattleEntry.Init();

            // ====== Manager ======
            UIManager.Ctor();
            CameraManager.Ctor();

            // ======  UI ======
            UIEntry.Ctor();
            var uiCamTrans = CameraManager.UICamTrans;
            uiCamTrans.SetParent(UIManager.UIRoot.transform, false);

            // ======  Input ======
            InputGameSet.Ctor();

            // ====== Physics ======
            Physics.autoSimulation = false;

            StartClient();
            isStarted = true;
        }

        void FixedUpdate()
        {
            if (!isStarted) return;

            // == Entry ==
            LoginEntry.Tick();
            BattleEntry.Tick();
            UIEntry.Tick();

            // == EventCenter ==
            NetworkEventCenter.Tick();
            LocalEventCenter.Tick();
        }

        void Update()
        {
            if (!isStarted) return;

            InputGameSet.Receive_Input(ref _inputComponent);
            BattleEntry.Update();
        }

        void LoginSceneLoaded(Scene scene, LoadSceneMode sceneMode)
        {
            CurrentSceneName = scene.name;
        }

        void StartClient()
        {
            Debug.Log("启动客户端---------------------------------");

            var networkClient = AllClientNetwork.networkClient;

            networkClient.Connect(NetworkConfig.LOCAL_HOST, NetworkConfig.LOGIN_PORT);

            _clientThread = new Thread(() =>
            {
                while (true)
                {
                    networkClient.Tick();
                }
            });
            _clientThread.Start();

            networkClient.OnConnectedHandle += () =>
            {
                Debug.Log("连接登录服务器成功*************************************************************");
            };
        }

        async Task LoadAllAsset()
        {
            await UIPanelAssets.LoadAll();
        }

        void OnDestroy()
        {
            LoginEntry.TearDown();
            BattleEntry.TearDown();
            UIEntry.TearDown();
            InputGameSet.TearDown();

            _clientThread.Abort();
        }

    }

}

