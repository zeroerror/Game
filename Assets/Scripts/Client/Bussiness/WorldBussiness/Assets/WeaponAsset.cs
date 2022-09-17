using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Game.Generic;
using System;

namespace Game.Client.Bussiness.WorldBussiness.Assets
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
            Addressables.LoadAssetsAsync<GameObject>(AssetLabelCollection.WeaponAssets, (obj) =>
            {
                dic_name.TryAdd(obj.name, obj);
                Console.WriteLine($"武器资源添加：{obj.name}");
            });
        }

        public bool TryGetByName(string name, out GameObject go)
        {
            return dic_name.TryGetValue(name, out go);
        }

    }

}