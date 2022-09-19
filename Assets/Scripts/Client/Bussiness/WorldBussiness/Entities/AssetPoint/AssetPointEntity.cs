using System;
using UnityEngine;
using Game.Client.Bussiness.WorldBussiness.Generic;

namespace Game.Client.Bussiness.WorldBussiness
{
    [Serializable]
    public struct ItemGenProbability
    {
        public ItemType itemType;
        public byte subType;
        public float weight;
    }

    public class AssetPointEntity : MonoBehaviour
    {
        public ItemGenProbability[] itemGenProbabilityArray;
    }

}