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

    [MenuItem("GameObject/启用所有MeshCollider (Convex:On)")]
    static void EnableAllMeshConvex()
    {
        var chosenGo = Selection.activeGameObject;
        var allMeshFilter = chosenGo.transform.GetComponentsInChildren<MeshFilter>();
        for (int i = 0; i < allMeshFilter.Length; i++)
        {
            var meshFilter = allMeshFilter[i];
            var meshCollider = meshFilter.transform.GetComponent<MeshCollider>();
            if (meshCollider == null)
            {
                meshCollider = meshFilter.gameObject.AddComponent<MeshCollider>();
            }

            meshCollider.enabled = true;
            meshCollider.convex = true;
        }
        Debug.Log($"启用Convex个数：{allMeshFilter.Length}");
    }

    [MenuItem("GameObject/启用所有MeshCollider (Convex:Off)")]
    static void DisableAllMeshConvex()
    {
        var chosenGo = Selection.activeGameObject;
        var allMeshFilter = chosenGo.transform.GetComponentsInChildren<MeshFilter>();
        for (int i = 0; i < allMeshFilter.Length; i++)
        {
            var meshFilter = allMeshFilter[i];
            var meshCollider = meshFilter.transform.GetComponent<MeshCollider>();
            if (meshCollider == null)
            {
                meshCollider = meshFilter.gameObject.AddComponent<MeshCollider>();
            }

            meshCollider.enabled = true;
            meshCollider.convex = false;
        }
        Debug.Log($"关闭Convex个数：{allMeshFilter.Length}");
    }

    [MenuItem("GameObject/移除所有Collider")]
    static void RmoveAllCollider()
    {
        var chosenGo = Selection.activeGameObject;
        var allColliders = chosenGo.transform.GetComponentsInChildren<Collider>();
        for (int i = 0; i < allColliders.Length; i++)
        {
            GameObject.DestroyImmediate(allColliders[i]);
        }
        Debug.Log($"移除Collider个数：{allColliders.Length}");
    }

    [MenuItem("GameObject/获取当前路径")]
    static void GetPath()
    {
        var chosenGo = Selection.activeGameObject;
        string path = chosenGo.transform.name;
        var parent = chosenGo.transform.parent;
        while (parent != null && parent.GetComponent<Canvas>() == null)
        {
            path = $"{parent.name}/{path}";
            parent = parent.parent;
        }
        Debug.Log($"path：{path}");
        TextEditor textEditor = new TextEditor();
        textEditor.text = path;
        textEditor.SelectAll();
        textEditor.Copy();
    }

}
