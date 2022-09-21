using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Game.Generic;
using System;

namespace Game.Client.Bussiness.BattleBussiness.Assets
{

    public class BattleRoleAssets
    {

        Dictionary<string, GameObject> dic_name;

        public BattleRoleAssets()
        {
            dic_name = new Dictionary<string, GameObject>();
        }

        public void LoadAssets()
        {
            Addressables.LoadAssetsAsync<GameObject>(AssetLabelCollection.RoleAssets, (obj) =>
            {
                dic_name.TryAdd(obj.name, obj);
                Console.WriteLine($"角色资源添加：{obj.name}");
            });
        }

        public bool TryGetByName(string name, out GameObject go)
        {
            return dic_name.TryGetValue(name, out go);
        }


    }

}