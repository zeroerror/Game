using System;
using UnityEngine;
using Game.Infrastructure.Network.Server;
using Game.Protocol.World;
using Game.Client.Bussiness.WorldBussiness;

namespace Game.Server.Bussiness.WorldBussiness.Network
{

    public class WeaponReqAndRes
    {
        NetworkServer _server;

        public WeaponReqAndRes()
        {

        }

        public void Inject(NetworkServer server)
        {
            _server = server;
        }

        public void SendRes_WeaponAssetsSpawn(int connId, int frameIndex, WeaponType[] weaponTypeArray, ushort[] weaponIdArray)
        {
            byte[] wta = new byte[weaponTypeArray.Length];
            for (int i = 0; i < weaponTypeArray.Length; i++)
            {
                wta[i] = (byte)weaponTypeArray[i];
            }

            FrameWeaponAssetsSpawnResMsg msg = new FrameWeaponAssetsSpawnResMsg
            {
                serverFrame = frameIndex,
                weaponTypeArray = wta,
                weaponIdArray = weaponIdArray
            };

            _server.SendMsg(connId, msg);
        }

    }

}