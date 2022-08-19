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

        void Awake()
        {
            DontDestroyOnLoad(this.gameObject);

            // MySQL
            string connStr = "server=127.0.0.1;port=3306;user=root;password=123456; database=game;";
            string sql = "select * from students";
            var firstRow = MySqlHelper.ExecuteDataRow(connStr, sql);
            var rows = firstRow.Table.Rows;
            Debug.Log("MySQL查询Start=============================================");
            foreach (DataRow row in rows)
            {
                Debug.Log($"{row.ItemArray[0]},{row.ItemArray[1]},{row.ItemArray[2]},{row.ItemArray[3]},{row.ItemArray[4]}");
            }
            Debug.Log("MySQL查询End=============================================");

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
            WorldEntry.Inject(AllClientNetwork.networkClient);
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
        }

        void Update()
        {
            // == Entry ==
            LoginEntry.Tick();
            WorldEntry.Tick();
            UIEntry.Tick();

            // == EventCenter ==
            NetworkEventCenter.Tick();
            LocalEventCenter.Tick();
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

