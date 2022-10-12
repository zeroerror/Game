using System.Collections.Generic;
using UnityEngine;
using Game.Client.Bussiness.BattleBussiness.Generic;
using Game.Client.Bussiness.BattleBussiness.Interface;

namespace Game.Client.Bussiness.BattleBussiness
{

    public class IDComponent
    {
        public EntityType EntityType => EntityType.BattleRole;

        short entityId;
        public int EntityId => entityId;
        public void SetEntityId(short entityId) => this.entityId = entityId;

        int leagueId;
        public int LeagueId => leagueId;
        public void SetLeagueId(int leagueId) => this.leagueId = leagueId;

        int connId;
        public int ConnId => connId;
        public void SetConnId(int connId) => this.connId = connId;

        public IDComponent()
        {
        }

        public bool HasLeague()
        {
            return leagueId != 0;
        }

    }

}