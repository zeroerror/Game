using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using Game.Infrastructure.Input;
using Game.Client.Bussiness.EventCenter;
using Game.Client.Bussiness.WorldBussiness.Facades;


namespace Game.Client.Bussiness.WorldBussiness.Controller
{

    public class WorldController
    {
        WorldFacades worldFacades;

        public WorldController()
        {
        }

        public void Inject(WorldFacades worldFacades)
        {
            this.worldFacades = worldFacades;
        }

        public void Tick()
        {

        }
        
    }

}