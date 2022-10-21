using System.Collections.Generic;
using UnityEngine;
using Game.Client.Bussiness.BattleBussiness.Generic;
using Game.Client.Bussiness.BattleBussiness.Interface;

namespace Game.Client.Bussiness.BattleBussiness
{

    public class IDComponent
    {

        EntityType entityType;
        public EntityType EntityType => entityType;
        public void SetEntityType(EntityType entityType) => this.entityType = entityType;

        int entityId;
        public int EntityId => entityId;
        public void SetEntityId(int entityId) => this.entityId = entityId;

        int leagueId;
        public int LeagueId => leagueId;
        public void SetLeagueId(int leagueId) => this.leagueId = leagueId;

        public IDComponent()
        {
        }

        public bool HasLeague()
        {
            return leagueId >= 10000;
        }

    }

}