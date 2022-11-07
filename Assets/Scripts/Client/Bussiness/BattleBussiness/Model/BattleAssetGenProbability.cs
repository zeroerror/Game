using System;
using Game.Client.Bussiness.BattleBussiness.Generic;

namespace Game.Client.Bussiness.BattleBussiness
{

    [Serializable]
    public struct BattleAssetGenProbability
    {
        public EntityType entityType;
        public byte subType;
        public float weight;
    }

}