using Game.Infrastructure.Input;
using Game.Client.Bussiness.BattleBussiness.Controller.Domain;
using Game.Infrastructure.Network.Client;
using Game.Infrastructure.Network.Server;
using System;
using Game.Protocol.Client2World;
using UnityEngine;

namespace Game.Server.Bussiness.WorldBussiness.Facades
{

    public class WorldReqAndRes
    {

        NetworkServer _worldServer;

        public WorldReqAndRes()
        {
        }

        public void Inject(NetworkServer _server)
        {
            this._worldServer = _server;
        }


        // ====== Send ======

        public void SendRes_WorldConnection(int connId, string account)
        {
            WolrdEnterResMessage msg = new WolrdEnterResMessage
            {
                account = account
            };

            _worldServer.SendMsg(connId, msg);
        }

        // ====== Regist ======
        public void RegistReq_WorldConnection(Action<int, WolrdEnterReqMessage> action)
        {
            _worldServer.AddRegister(action);
        }

    }

}