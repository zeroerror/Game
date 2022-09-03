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
        public string CurrentSceneName { get; private set; }
        InputComponent InputComponent;

        void Awake()
        {
            DontDestroyOnLoad(this.gameObject);

            // == Network ==
            AllClientNetwork.Ctor();
            StartClient();

            // == EventCenter ==
            NetworkEventCenter.Ctor();
            LocalEventCenter.Ctor();

            // == Entry ==
            // Login
            LoginEntry.Ctor();
            LoginEntry.Inject(AllClientNetwork.networkClient);
            LoginEntry.Init();
            // World
            WorldEntry.Ctor();
            InputComponent = new InputComponent();
            WorldEntry.Inject(AllClientNetwork.networkClient, InputComponent);
            WorldEntry.Init();
            // UI
            UIEntry.Ctor();

            // == Manager ==
            UIManager.Ctor();
            CameraManager.Ctor();

            Action action = async () =>
            {
                // == All Asset ==
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

            // == Physics ==
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
            Tick_Input();
        }

        void Tick_Input()
        {
            if (Input.GetKey(KeyCode.W))
            {
                InputComponent.moveAxis.z = 1;
            }
            if (Input.GetKey(KeyCode.S))
            {
                InputComponent.moveAxis.z = -1;
            }
            if (Input.GetKey(KeyCode.A))
            {
                InputComponent.moveAxis.x = -1;
            }
            if (Input.GetKey(KeyCode.D))
            {
                InputComponent.moveAxis.x = 1;
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                InputComponent.pressJump = true;
            }
            if (Input.GetKeyDown(KeyCode.G))
            {
                var mainCam = Camera.main;
                if (mainCam != null)
                {
                    var ray = mainCam.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out RaycastHit hit))
                    {
                        InputComponent.grenadeThrowPoint = hit.point;
                    }
                }
            }
            if (Input.GetMouseButtonDown(0))
            {
                var mainCam = Camera.main;
                if (mainCam != null)
                {
                    var ray = mainCam.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out RaycastHit hit))
                    {
                        InputComponent.shootPoint = hit.point;
                    }
                }
            }
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

            networkClient.Connect(NetworkConfig.HOST, NetworkConfig.PORT);

            new Thread(() =>
            {
                while (true)
                {
                    networkClient.Tick();
                }
            }).Start();

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
        }

    }

}

