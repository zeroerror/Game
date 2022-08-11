using UnityEngine;
using Game.Client.Bussiness.WorldBussiness.Network;
using Game.Client.Bussiness.WorldBussiness.Facades;
using Game.Client.Bussiness.EventCenter;

namespace Game.Client.Bussiness.WorldBussiness.Controller.Domain
{

    public class WorldRoleSpawnDomain
    {

        WorldFacades worldFacades;

        WorldRoleReqAndRes _worldRoleReqAndRes;

        public WorldRoleSpawnDomain()
        {
            LocalEventCenter.Regist_SceneLoadedHandler(OnSceneLoaded);
        }

        public void Inject(WorldFacades facades)
        {
            this.worldFacades = facades;
        }


        public void Tick()
        {

        }

        public void OnSceneLoaded(string name)
        {
            if (name == "WorldChooseScene")
            {
                Debug.Log("生成角色");
                worldFacades.Assets.WorldRoleAssets.TryGetByName("player", out GameObject go);
                go = GameObject.Instantiate(go);
                var entity = go.GetComponent<WorldRoleEntity>();
                var repo = worldFacades.Repo.WorldRoleRepo;
                repo.Add(entity);
                worldFacades.CinemachineExtra.FollowSolo(entity.CamTrackingObj, 3f);
                worldFacades.CinemachineExtra.LookAtSolo(entity.CamTrackingObj, 3f);

                // LocalEventCenter.Invoke_WorldRoleSpawnHandler(entity);
            }
        }

    }

}