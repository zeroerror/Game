using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game.Client
{

    public class ClientApp : MonoBehaviour
    {

        void Awake()
        {
            //Asset Load
            Addressables.LoadAssetAsync<GameObject>("UI").Completed += ((handle) =>
              {
                  Debug.Log($"handle.Result.name  : {handle.Result.name}");
              });

            //Open LoginScene
        }

        void Update()
        {
            // Tick
        }

    }

}

