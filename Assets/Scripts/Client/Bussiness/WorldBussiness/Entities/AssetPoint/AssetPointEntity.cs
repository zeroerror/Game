using System;
using UnityEngine;

namespace Game.Client.Bussiness.WorldBussiness
{

    [Serializable]
    public struct WeaponGenProbability
    {
        public WeaponType weaponType;
        public float weight;
    }

    public class AssetPointEntity : MonoBehaviour
    {
        public WeaponGenProbability[] weaponGenProbability;
    }

}