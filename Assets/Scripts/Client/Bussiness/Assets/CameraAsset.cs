using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Game.Generic;

namespace Game.Client.Bussiness.Assets
{

    public class CameraAsset
    {

        Dictionary<string, GameObject> dic_name;

        public CameraAsset()
        {
            dic_name = new Dictionary<string, GameObject>();
        }

        public void LoadAssets()
        {
            Addressables.LoadAssetsAsync<GameObject>(AssetLabelCollection.CameraAssets, (obj) =>
            {
                dic_name.Add(obj.name, obj);
            });
        }

        public bool TryGetByName(string name, out GameObject go)
        {
            return dic_name.TryGetValue(name, out go);
        }


    }

}