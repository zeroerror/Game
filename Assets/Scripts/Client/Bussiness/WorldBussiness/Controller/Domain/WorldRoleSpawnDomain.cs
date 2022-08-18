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

        byte tempRidIndex;

        public WorldRoleSpawnDomain()
        {
        }

        public void Inject(WorldFacades facades)
        {
            this.worldFacades = facades;
        }


        public void Tick()
        {

        }

        public WorldRoleEntity SpawnWorldRole(Transform parent)
        {
            if (worldFacades.Assets.WorldRoleAssets.TryGetByName("player", out GameObject prefabAsset))
            {
                prefabAsset = GameObject.Instantiate(prefabAsset, parent);
                var entity = prefabAsset.GetComponent<WorldRoleEntity>();
                entity.SetRid(++tempRidIndex);

                return entity;
            }

            return null;

        }

    }

}