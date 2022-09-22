using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Game.Generic;

namespace Game.Bussiness.UIBussiness
{

    public static class UIPanelAssets
    {

        static Dictionary<string, GameObject> all;

        public static async Task LoadAll()
        {
            all = new Dictionary<string, GameObject>();
            IList<GameObject> list = await Addressables.LoadAssetsAsync<GameObject>(AssetLabelCollection.UIAssets, null).Task;
            IEnumerator<GameObject> enumerator = list.GetEnumerator();
            while (enumerator.MoveNext())
            {
                GameObject go = enumerator.Current;
                all.Add(go.name, go);
            }
        }

        public static void TearDown()
        {
            all = null;
        }

        public static GameObject Get(string name)
        {
            all.TryGetValue(name, out GameObject go);
            return go;
        }

    }

}

