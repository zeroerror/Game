using Game.Client.Bussiness.BattleBussiness.Generic;

namespace Game.Client.Bussiness.BattleBussiness
{

    public class IDComponent
    {

        EntityType entityType;
        public EntityType EntityType => entityType;
        public void SetEntityType(EntityType entityType) => this.entityType = entityType;

        byte subType;
        public byte SubType => subType;
        public void SetSubType(byte val) => subType = val;

        int entityId;
        public int EntityID => entityId;
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