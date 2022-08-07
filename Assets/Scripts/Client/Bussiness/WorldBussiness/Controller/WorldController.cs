using Game.Client.Bussiness.WorldBussiness.Network;
using Game.Infrastructure.Network.Client;
using Game.Client.Bussiness.EventCenter.Facades;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace Game.Client.Bussiness.WorldBussiness.Controller
{

    public static class WorldController
    {

        static WorldReqAndRes _worldReqAndRes;

        public static void Inject(NetworkClient client, WorldReqAndRes worldReqAndRes)
        {
            _worldReqAndRes = worldReqAndRes;
            _worldReqAndRes.Inject(client);
            _worldReqAndRes.RegistWorldEnterRes();
            _worldReqAndRes.RegistWorldEnterRes();
        }

        public static void Tick()
        {
            OnLogin2WorldEvent();
        }

        static void OnLogin2WorldEvent()
        {
            var ev = AllBussinessEvent.LoginToWorldEvent;
            if (!ev.IsTrigger) return;
            ev.SetIsTrigger(false);

            Addressables.LoadSceneAsync("WorldChooseScene", LoadSceneMode.Single);
        }

    }

}