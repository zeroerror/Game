using UnityEngine;
using UnityEditor;

public static class EditorHelper
{
    [MenuItem("GameObject/禁用所有碰撞体")]
    static void DisableAllCollider()
    {
        var chosenGo = Selection.activeGameObject;
        var allColliders = chosenGo.transform.GetComponentsInChildren<Collider>();
        for (int i = 0; i < allColliders.Length; i++)
        {
            var collider = allColliders[i];
            collider.enabled = false;
        }
        Debug.Log($"禁用碰撞盒个数：{allColliders.Length}");
    }

    [MenuItem("GameObject/启用所有碰撞体")]
    static void EnableAllCollider()
    {
        var chosenGo = Selection.activeGameObject;
        var allColliders = chosenGo.transform.GetComponentsInChildren<Collider>();
        for (int i = 0; i < allColliders.Length; i++)
        {
            var collider = allColliders[i];
            collider.enabled = true;
        }
        Debug.Log($"启用碰撞盒个数：{allColliders.Length}");
    }

    [MenuItem("GameObject/启用所有MeshCollider的Convex")]
    static void EnableAllMeshConvex()
    {
        var chosenGo = Selection.activeGameObject;
        var allColliders = chosenGo.transform.GetComponentsInChildren<MeshCollider>();
        for (int i = 0; i < allColliders.Length; i++)
        {
            var meshCollider = allColliders[i];
            meshCollider.convex = true;
        }
        Debug.Log($"启用Convex个数：{allColliders.Length}");
    }

}
