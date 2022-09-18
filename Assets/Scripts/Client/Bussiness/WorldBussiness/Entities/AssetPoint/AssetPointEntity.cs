using System;
using Game.Client.Bussiness.WorldBussiness.Interface;
using UnityEngine;

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