using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Game.Generic;

namespace Game.UI.Assets
{

    public class UIAssets
    {

        Dictionary<string, GameObject> all;

        public UIAssets()
        {
            all = new Dictionary<string, GameObject>();
        }

        public async Task LoadAll()
        {
            IList<GameObject> list = await Addressables.LoadAssetsAsync<GameObject>(AssetLabelCollection.UIAssets, null).Task;
            IEnumerator<GameObject> enumerator = list.GetEnumerator();
            while (enumerator.MoveNext())
            {
                GameObject go = enumerator.Current;
                all.Add(go.name, go);
            }
        }

        public GameObject Get(string name)
        {
            all.TryGetValue(name, out GameObject go);
            return go;
        }

    }

}

