using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Test : EditorWindow
{

    GameObject targetGo;

    static GameObject chosenGo;

    struct Point
    {
        public GameObject pointGo;
        public Vector3 targetPos;
    }

    static List<Point> pointList;

    [MenuItem("GameObject/打开窗口")]
    public static void Init()
    {
        chosenGo = Selection.activeGameObject;
        Test window = (Test)EditorWindow.GetWindow(typeof(Test));
        window.Show();
        pointList = new List<Point>();
    }

    public void Awake()
    {

    }

    void OnDestroy()
    {
        pointList.ForEach((point) =>
        {
            GameObject.DestroyImmediate(point.pointGo);
        });
        pointList.Clear();
    }

    void OnInspectorUpdate()
    {
        // Call Repaint on OnInspectorUpdate as it repaints the windows
        // less times as if it was OnGUI/Update
        targetGo = Selection.activeGameObject;
        Repaint();

        pointList.ForEach((point) =>
        {
            var p = point.pointGo;
            var pTargetPos = point.targetPos;
            p.transform.position = Vector3.Lerp(p.transform.position, pTargetPos, 0.1f);
        });
    }

    void OnGUI()
    {
        if (chosenGo == null)
            return;

        GUI.Label(new Rect(100, 20, 200, 50), $"当前需移动物体为:{chosenGo.name}");
        GUI.Label(new Rect(100, 50, 200, 50), targetGo == null ? "请选择目标物体" : $"当前选择目标物体为:{targetGo.name}");
        if (GUI.Button(new Rect(100, 150, 200, 50), "移动到目标物体位置"))
        {
            if (targetGo == null)
            {
                Debug.Log($"请选择场景物体");
            }
            else
            {
                Vector3 p1 = Vector3.zero;
                Vector3 p2 = Vector3.zero;
                Vector3 p3 = Vector3.zero;
                Vector3 p4 = Vector3.zero;
                Vector3 p5 = Vector3.zero;
                Vector3 p6 = Vector3.zero;
                Vector3 p7 = Vector3.zero;
                Vector3 p8 = Vector3.zero;
                GetMoveAfterPoints(targetGo.transform, ref p1, ref p2, ref p3, ref p4, ref p5, ref p6, ref p7, ref p8);
                var targetPos = targetGo.transform.position;

                pointList.ForEach((point) =>
                {
                    GameObject.DestroyImmediate(point.pointGo);
                });
                pointList.Clear();

                GameObject p1Go = CreateCube(p1);
                GameObject p2Go = CreateCube(p2);
                GameObject p3Go = CreateCube(p3);
                GameObject p4Go = CreateCube(p4);
                GameObject p5Go = CreateCube(p5);
                GameObject p6Go = CreateCube(p6);
                GameObject p7Go = CreateCube(p7);
                GameObject p8Go = CreateCube(p8);

                Vector3 centerPos = (p1 + p2 + p3 + p4 + p5 + p6 + p7 + p8) / 8;
                GameObject centerGo = CreateSphere(centerPos);
                centerGo.transform.name = "中心";

                pointList.Add(new Point { pointGo = p1Go, targetPos = p1Go.transform.position + targetPos });
                pointList.Add(new Point { pointGo = p2Go, targetPos = p2Go.transform.position + targetPos });
                pointList.Add(new Point { pointGo = p3Go, targetPos = p3Go.transform.position + targetPos });
                pointList.Add(new Point { pointGo = p4Go, targetPos = p4Go.transform.position + targetPos });
                pointList.Add(new Point { pointGo = p5Go, targetPos = p5Go.transform.position + targetPos });
                pointList.Add(new Point { pointGo = p6Go, targetPos = p6Go.transform.position + targetPos });
                pointList.Add(new Point { pointGo = p7Go, targetPos = p7Go.transform.position + targetPos });
                pointList.Add(new Point { pointGo = p8Go, targetPos = p8Go.transform.position + targetPos });
                pointList.Add(new Point { pointGo = centerGo, targetPos = centerGo.transform.position + targetPos });
            }
        }
    }

    void GetMoveAfterPoints(Transform target, ref Vector3 p1, ref Vector3 p2, ref Vector3 p3, ref Vector3 p4, ref Vector3 p5, ref Vector3 p6, ref Vector3 p7, ref Vector3 p8)
    {
        if (target.transform.parent != null)
        {
            GetMoveAfterPoints(target.transform.parent, ref p1, ref p2, ref p3, ref p4, ref p5, ref p6, ref p7, ref p8);
        }

        Debug.Log($"移动到 {target.name} 所在位置----------------");

        var targetQuaternion = target.rotation;
        var targetAngle = targetGo.transform.rotation.eulerAngles;

        // 获取Box的8个顶点
        GetBoxPoints(target, out p1, out p2, out p3, out p4, out p5, out p6, out p7, out p8);
        // 获取旋转后的顶点
        p1 = targetQuaternion * p1;
        p2 = targetQuaternion * p2;
        p3 = targetQuaternion * p3;
        p4 = targetQuaternion * p4;
        p5 = targetQuaternion * p5;
        p6 = targetQuaternion * p6;
        p7 = targetQuaternion * p7;
        p8 = targetQuaternion * p8;

    }

    private static void GetBoxPoints(Transform src, out Vector3 p1, out Vector3 p2, out Vector3 p3, out Vector3 p4, out Vector3 p5, out Vector3 p6, out Vector3 p7, out Vector3 p8)
    {
        var srcCollider = src.GetComponent<BoxCollider>();
        var realScale = srcCollider.transform.localScale;
        var parent = src.transform.parent;
        while (parent != null)
        {
            realScale.x *= parent.localScale.x;
            realScale.y *= parent.localScale.y;
            realScale.z *= parent.localScale.z;
            parent = src.transform.parent;
        }

        var realSize = srcCollider.size;
        realSize.x *= realScale.x;
        realSize.y *= realScale.y;
        realSize.z *= realScale.z;
        var center = srcCollider.center;
        p1 = center;
        p1.x -= realSize.x / 2;
        p1.y += realSize.y / 2;
        p1.z += realSize.z / 2;
        p2 = center;
        p2.x += realSize.x / 2;
        p2.y += realSize.y / 2;
        p2.z += realSize.z / 2;
        p3 = center;
        p3.x -= realSize.x / 2;
        p3.y += realSize.y / 2;
        p3.z -= realSize.z / 2;
        p4 = center;
        p4.x += realSize.x / 2;
        p4.y += realSize.y / 2;
        p4.z -= realSize.z / 2;
        p5 = center;
        p5.x -= realSize.x / 2;
        p5.y -= realSize.y / 2;
        p5.z += realSize.z / 2;
        p6 = center;
        p6.x += realSize.x / 2;
        p6.y -= realSize.y / 2;
        p6.z += realSize.z / 2;
        p7 = center;
        p7.x -= realSize.x / 2;
        p7.y -= realSize.y / 2;
        p7.z -= realSize.z / 2;
        p8 = center;
        p8.x += realSize.x / 2;
        p8.y -= realSize.y / 2;
        p8.z -= realSize.z / 2;
    }

    static GameObject CreateCube(Vector3 p)
    {
        return CreatePrimitiveType(p, PrimitiveType.Cube, new Vector3(0.1f, 0.1f, 0.1f));
    }

    static GameObject CreateSphere(Vector3 p)
    {
        return CreatePrimitiveType(p, PrimitiveType.Sphere, new Vector3(0.1f, 0.1f, 0.1f));
    }

    static GameObject CreatePrimitiveType(Vector3 p, PrimitiveType type, Vector3 size)
    {
        GameObject go = GameObject.CreatePrimitive(type);
        go.transform.position = p;
        go.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        return go;
    }

}
