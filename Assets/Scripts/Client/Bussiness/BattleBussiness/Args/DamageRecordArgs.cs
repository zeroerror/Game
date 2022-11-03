using Game.Client.Bussiness.BattleBussiness.Generic;

namespace Game.Client.Bussiness.BattleBussiness
{

    public struct DamageRecordArgs
    {
        public EntityType atkEntityType;
        public int atkEntityID;
        public EntityType vicEntityType;
        public int vicEntityID;
        public float damage;
    }

}