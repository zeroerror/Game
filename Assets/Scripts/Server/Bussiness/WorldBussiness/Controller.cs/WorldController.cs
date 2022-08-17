using System;
using Game.Server.Bussiness.WorldBussiness.Facades;
using Game.Protocol.World;
using UnityEngine;

namespace Game.Server.Bussiness.WorldBussiness
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

            var rqs = worldFacades.AllWorldNetwork.WorldRoleReqAndRes;
            rqs.RegistReq_WorldRoleMove(OnWoldRoleMove);
        }

        public void Tick()
        {

        }



        void OnWoldRoleMove(int connId, FrameReqOptMsg msg)
        {
            var rqs = worldFacades.AllWorldNetwork.WorldRoleReqAndRes;
            rqs.SendRes_WorldRoleMove(connId);
            Debug.Log($"服务端：回复帧消息---->确认人物移动  clientFrameIndex:{msg.clientFrameIndex} rid:{(byte)msg.msg>>24}  x:{(byte)msg.msg>>16}  y:{(byte)msg.msg>>8}  z:{(byte)msg.msg}");
        }

    }

}