using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Game.Protocol.Client2World;
using Game.Infrastructure.Generic;
using Game.Infrastructure.Network.Server.Facades;
using Game.Server.Bussiness.LoginBussiness;
using Game.Server.Bussiness.BattleBussiness;
using Game.Server.Bussiness.BattleBussiness.Facades;

namespace Game.Server.Bussiness.Center
{

    public static class EventCenter
    {

        public static bool stopPhyscisOneFrame;

    }

}