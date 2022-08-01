using UnityEngine;
using UnityEngine.SceneManagement;
using Game.Manager;
using Game.Facades;

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

        }

    }

}

