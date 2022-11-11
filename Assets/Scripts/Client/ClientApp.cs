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

        BattleEntry battleEntry;
        LoginEntry loginEntry;
        WorldEntry worldEntry;
        UIEntry uiEntry;

        Thread _loginServClientThread;
        Thread _worldServClientThread;
        Thread _battleServClientThread;
        public string CurrentSceneName { get; private set; }
        PlayerInputComponent playerInputComponent;
        float time;
        bool isStarted;
        float fixedDeltaTime;

        void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
            fixedDeltaTime = UnityEngine.Time.fixedDeltaTime;
            StartAll();
        }

        async void StartAll()
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

            // ====== InputComponent ======
            playerInputComponent = new PlayerInputComponent();

            // ====== Entry ======
            // Login
            loginEntry = new LoginEntry();
            loginEntry.Ctor();
            loginEntry.Inject(AllClientNetwork.loginSerClient);

            // World
            worldEntry = new WorldEntry();
            worldEntry.Ctor();
            worldEntry.Inject(AllClientNetwork.worldSerClient);

            // Battle
            battleEntry = new BattleEntry();
            battleEntry.Ctor();
            battleEntry.Inject(AllClientNetwork.battleSerClient, playerInputComponent);

            uiEntry = new UIEntry();
            uiEntry.Ctor();

            // ====== Manager ======
            CameraManager.Ctor();

            // ======  UI ======
            UIManager.Ctor();
            var uiCamTrans = CameraManager.UICamTrans;
            uiCamTrans.SetParent(UIManager.UICanvasGo.transform, false);

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
            loginEntry.Tick();
            worldEntry.Tick();
            battleEntry.Tick(fixedDeltaTime);
            uiEntry.Tick();

        }

        void Update()
        {
            if (!isStarted) return;

            InputGameSet.Receive_Input(ref playerInputComponent);
            battleEntry.Update();
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
                NetworkEventCenter.Invoke_ConnWorSerSuccessAction();
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
                NetworkEventCenter.Invoke_BattleSerConnectAction();
                AllClientNetwork.worldSerClient.Disconnect();//断开和世界服连接 
            };
        }

        async Task LoadAllAsset()
        {
            await UIPanelAssets.LoadAll();
        }

        void OnDestroy()
        {
            loginEntry.TearDown();
            worldEntry.TearDown();
            battleEntry.TearDown();
            uiEntry.TearDown();
            InputGameSet.TearDown();

            _loginServClientThread.Abort();
            _worldServClientThread.Abort();
        }

    }

}

