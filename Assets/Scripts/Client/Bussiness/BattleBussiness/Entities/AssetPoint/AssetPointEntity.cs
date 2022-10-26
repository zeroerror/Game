using System;
using UnityEngine;
using Game.Client.Bussiness.BattleBussiness.Generic;

namespace Game.Client.Bussiness.BattleBussiness
{
    [Serializable]
    public struct AssetGenProbability
    {
        public EntityType entityType;
        public byte subType;
        public float weight;
    }

    public class AssetPointEntity : MonoBehaviour
    {
        public AssetGenProbability[] itemGenProbabilityArray;
    }

}