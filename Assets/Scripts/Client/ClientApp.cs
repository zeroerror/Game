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
using Game.Client.Bussiness.BattleBussiness;
using Game.Client.Bussiness.UIBussiness;
using Game.Client.Bussiness.WorldBussiness;

namespace Game.Client
{

    public class ClientApp : MonoBehaviour
    {

        Thread _loginServClientThread;
        Thread _worldServClientThread;
        Thread _battleServClientThread;
        public string CurrentSceneName { get; private set; }
        PlayerInputComponent playerInputComponent;
        float time;
        bool isStarted;


        void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
            StartAllAsync();
        }

        async void StartAllAsync()
        {
            // ====== Network ======
            AllClientNetwork.Ctor();
            StartAllClient();

            // ======  Asset ======
            await LoadAllAsset();

            // == Load Login Scene ==
            Addressables.LoadSceneAsync("login_scene", LoadSceneMode.Single);
            SceneManager.sceneLoaded -= LoginSceneLoaded;
            SceneManager.sceneLoaded += LoginSceneLoaded;

            // ====== EventCenter ======
            NetworkEventCenter.Ctor();

            // ====== Entry ======
            // Login
            LoginEntry.Ctor();
            LoginEntry.Inject(AllClientNetwork.loginSerClient);
            // World
            WorldEntry.Ctor();
            WorldEntry.Inject(AllClientNetwork.worldSerClient);
            // Battle
            BattleEntry.Ctor();
            playerInputComponent = new PlayerInputComponent();
            BattleEntry.Inject(AllClientNetwork.battleSerClient, playerInputComponent);

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

            isStarted = true;
        }

        void FixedUpdate()
        {
            if (!isStarted) return;

            // == Entry ==
            LoginEntry.Tick();
            WorldEntry.Tick();
            BattleEntry.Tick();
            UIEntry.Tick();

        }

        void Update()
        {
            if (!isStarted) return;

            InputGameSet.Receive_Input(ref playerInputComponent);
            BattleEntry.Update();
        }

        void LoginSceneLoaded(Scene scene, LoadSceneMode sceneMode)
        {
            CurrentSceneName = scene.name;
        }

        void StartAllClient()
        {
            Debug.Log("启动登录服客户端---------------------------------");
            AllClientNetwork.loginSerClient.Connect(NetworkConfig.LOGIN_HOST, NetworkConfig.LOGIN_PORT);
            _loginServClientThread = new Thread(() =>
            {
                while (true)
                {
                    AllClientNetwork.loginSerClient.Tick();
                }
            });
            _loginServClientThread.Start();
            AllClientNetwork.loginSerClient.OnConnectedHandle += () =>
           {
               Debug.Log("连接登录服务器成功*************************************************************");
           };

            Debug.Log("启动世界服客户端---------------------------------");
            _worldServClientThread = new Thread(() =>
            {
                while (true)
                {
                    AllClientNetwork.worldSerClient.Tick();
                }
            });
            _worldServClientThread.Start();
            AllClientNetwork.worldSerClient.OnConnectedHandle += () =>
           {
               Debug.Log("连接世界服务器成功*************************************************************");
               NetworkEventCenter.Invoke_ConnWorSerSuccessHandler();
           };

            Debug.Log("启动战斗服客户端---------------------------------");
            _battleServClientThread = new Thread(() =>
            {
                while (true)
                {
                    AllClientNetwork.battleSerClient.Tick();
                }
            });
            _battleServClientThread.Start();
            AllClientNetwork.battleSerClient.OnConnectedHandle += () =>
           {
               Debug.Log("连接战斗服务器成功*************************************************************");
               NetworkEventCenter.Invoke_BattleSerConnectHandler();
               AllClientNetwork.worldSerClient.Disconnect();//断开和世界服连接 
           };
        }


        async Task LoadAllAsset()
        {
            await UIPanelAssets.LoadAll();
        }

        void OnDestroy()
        {
            LoginEntry.TearDown();
            WorldEntry.TearDown();
            BattleEntry.TearDown();
            UIEntry.TearDown();
            InputGameSet.TearDown();

            _loginServClientThread.Abort();
            _worldServClientThread.Abort();
        }

    }

}

