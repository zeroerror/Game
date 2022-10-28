using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Game.Generic;
using System;

namespace Game.Client.Bussiness.BattleBussiness.Assets
{

    public class WeaponAsset
    {

        Dictionary<string, GameObject> dic_name;

        public WeaponAsset()
        {
            dic_name = new Dictionary<string, GameObject>();
        }

        public void LoadAssets()
        {
            Addressables.LoadAssetsAsync<GameObject>(AssetLabelCollection.WeaponAsset, (obj) =>
            {
                dic_name.TryAdd(obj.name, obj);
            });
        }

        public bool TryGetByName(string name, out GameObject go)
        {
            return dic_name.TryGetValue(name, out go);
        }

    }

}