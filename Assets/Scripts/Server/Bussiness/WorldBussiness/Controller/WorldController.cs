using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using Game.Protocol.Client2World;
using Game.Server.Bussiness.WorldBussiness.Facades;
using Game.Server.Bussiness.EventCenter;

namespace Game.Server.Bussiness.WorldBussiness.Controller
{

    public class WorldController
    {

        WorldFacades worldFacades;

        public WorldController()
        {
            NetworkEventCenter.Regist_NewWorldConnection(OnNewWorldConnection);
        }

        public void Inject(WorldFacades worldFacades)
        {
            this.worldFacades = worldFacades;

            var rqs = worldFacades.Network.WorldReqAndRes;
            rqs.RegistReq_WorldConnection(OnWorldConnection);
        }

        public void Tick()
        {

        }


        void OnNewWorldConnection(int connId)
        {
            Debug.Log($"[世界服]: connID:{connId} 客户端连接成功-------------------------");
        }

        #region [Client Request]
        void OnWorldConnection(int connId, WolrdEnterReqMessage msg)
        {
            var rqs = worldFacades.Network.WorldReqAndRes;
            rqs.SendRes_WorldConnection(connId, msg.account);
            Debug.Log($"[世界服]: connId:{connId} 客户端请求进入世界，已回复-------------------------");
        }
        #endregion

    }

}