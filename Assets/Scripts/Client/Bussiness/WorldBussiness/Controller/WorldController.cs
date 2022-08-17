using UnityEngine;
using Game.Client.Bussiness.WorldBussiness.Facades;
using Game.Protocol.World;

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
            var req = worldFacades.Network.WorldRoleReqAndRes;
            req.RegistRes_WorldRoleMove(OnWorldRoleMove);
        }

        public void Tick()
        {
            Tick_Input();
        }

        void Tick_Input()
        {
            bool needMove = false;
            Vector3 moveDir = Vector3.zero;
            if (Input.GetKeyDown(KeyCode.W))
            {
                needMove = true;
                moveDir += new Vector3(0, 0, 1);
            }

            if (needMove)
            {
                sbyte rid = worldFacades.Repo.WorldRoleRepo.Owner.Rid;
                worldFacades.Network.WorldRoleReqAndRes.SendReq_WorldRoleMove(1, rid, moveDir);

            }
        }

        void OnWorldRoleMove(FrameResOptMsg msg)
        {
            Debug.Log($"OnWorldRoleMove  frameIndex:{msg.serverFrameIndex}");
        }


    }

}