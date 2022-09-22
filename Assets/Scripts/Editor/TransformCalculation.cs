using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TransformCalculation : EditorWindow
{

    struct BoxPoints
    {
        public Vector3 p1;
        public Vector3 p2;
        public Vector3 p3;
        public Vector3 p4;
        public Vector3 p5;
        public Vector3 p6;
        public Vector3 p7;
        public Vector3 p8;

        public Vector3 Center => (p1 + p2 + p3 + p4 + p5 + p6 + p7 + p8) / 8;
    }

    GameObject targetGo;

    static GameObject chosenGo;

    struct Point
    {
        public GameObject pointGo;
        public Vector3 targetPos;
    }

    static List<Point> pointList;

    [MenuItem("GameObject/Transform计算窗口")]
    public static void Init()
    {
        chosenGo = Selection.activeGameObject;
        TransformCalculation window = (TransformCalculation)EditorWindow.GetWindow(typeof(TransformCalculation));
        window.Show();
        pointList = new List<Point>();
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
            p.transform.position = Vector3.Lerp(p.transform.position, pTargetPos, 0.2f);
        });
    }

    void OnGUI()
    {
        if (chosenGo == null)
            return;

        GUI.Label(new Rect(100, 50, 200, 50), targetGo == null ? "请选择目标物体" : "当前选择目标物体为");
        if (targetGo != null)
        {
            GUI.color = Color.green;
            GUI.Label(new Rect(120, 80, 200, 50), targetGo.name);
            GUI.color = Color.white;
        }

        if (GUI.Button(new Rect(100, 150, 200, 50), "移动到目标物体位置"))
        {
            if (targetGo == null)
            {
                Debug.Log($"请选择场景物体");
            }
            else
            {
                GetBoxPoints(targetGo.transform, out BoxPoints boxPoints);
                Quaternion quaternion = targetGo.transform.rotation;
                GetMoveAfterPoints(targetGo.transform, ref boxPoints);
                var targetPos = targetGo.transform.position;

                pointList.ForEach((point) =>
                {
                    GameObject.DestroyImmediate(point.pointGo);
                });
                pointList.Clear();

                GameObject p1Go = CreateCube(boxPoints.p1);
                GameObject p2Go = CreateCube(boxPoints.p2);
                GameObject p3Go = CreateCube(boxPoints.p3);
                GameObject p4Go = CreateCube(boxPoints.p4);
                GameObject p5Go = CreateCube(boxPoints.p5);
                GameObject p6Go = CreateCube(boxPoints.p6);
                GameObject p7Go = CreateCube(boxPoints.p7);
                GameObject p8Go = CreateCube(boxPoints.p8);

                Vector3 centerPos = boxPoints.Center;
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

    void OnDestroy()
    {
        pointList.ForEach((point) =>
        {
            GameObject.DestroyImmediate(point.pointGo);
        });
        pointList.Clear();
    }

    void GetMoveAfterPoints(Transform target, ref BoxPoints boxPoints)
    {
        Debug.Log($"移动到 {target.name} 所在位置----------------");
        var targetQuaternion = target.localRotation;

        // //获取缩放
        boxPoints.p1 = Vector3.Scale(boxPoints.p1, target.localScale);
        boxPoints.p2 = Vector3.Scale(boxPoints.p2, target.localScale);
        boxPoints.p3 = Vector3.Scale(boxPoints.p3, target.localScale);
        boxPoints.p4 = Vector3.Scale(boxPoints.p4, target.localScale);
        boxPoints.p5 = Vector3.Scale(boxPoints.p5, target.localScale);
        boxPoints.p6 = Vector3.Scale(boxPoints.p6, target.localScale);
        boxPoints.p7 = Vector3.Scale(boxPoints.p7, target.localScale);
        boxPoints.p8 = Vector3.Scale(boxPoints.p8, target.localScale);

        // 获取旋转后的顶点
        boxPoints.p1 = targetQuaternion * boxPoints.p1;
        boxPoints.p2 = targetQuaternion * boxPoints.p2;
        boxPoints.p3 = targetQuaternion * boxPoints.p3;
        boxPoints.p4 = targetQuaternion * boxPoints.p4;
        boxPoints.p5 = targetQuaternion * boxPoints.p5;
        boxPoints.p6 = targetQuaternion * boxPoints.p6;
        boxPoints.p7 = targetQuaternion * boxPoints.p7;
        boxPoints.p8 = targetQuaternion * boxPoints.p8;


        // 过程必须是从子物体开始一直到父物体，记住这一点
        var parent = target.transform.parent;
        if (parent != null)
        {
            GetMoveAfterPoints(parent, ref boxPoints);
        }
    }

    static void GetBoxPoints(Transform src, out BoxPoints boxPoints)
    {
        boxPoints = new BoxPoints();
        var srcCollider = src.GetComponent<BoxCollider>();
        var originalSize = srcCollider.size;
        var center = srcCollider.center;
        boxPoints.p1 = center;
        boxPoints.p1.x -= originalSize.x / 2;
        boxPoints.p1.y += originalSize.y / 2;
        boxPoints.p1.z += originalSize.z / 2;
        boxPoints.p2 = center;
        boxPoints.p2.x += originalSize.x / 2;
        boxPoints.p2.y += originalSize.y / 2;
        boxPoints.p2.z += originalSize.z / 2;
        boxPoints.p3 = center;
        boxPoints.p3.x -= originalSize.x / 2;
        boxPoints.p3.y += originalSize.y / 2;
        boxPoints.p3.z -= originalSize.z / 2;
        boxPoints.p4 = center;
        boxPoints.p4.x += originalSize.x / 2;
        boxPoints.p4.y += originalSize.y / 2;
        boxPoints.p4.z -= originalSize.z / 2;
        boxPoints.p5 = center;
        boxPoints.p5.x -= originalSize.x / 2;
        boxPoints.p5.y -= originalSize.y / 2;
        boxPoints.p5.z += originalSize.z / 2;
        boxPoints.p6 = center;
        boxPoints.p6.x += originalSize.x / 2;
        boxPoints.p6.y -= originalSize.y / 2;
        boxPoints.p6.z += originalSize.z / 2;
        boxPoints.p7 = center;
        boxPoints.p7.x -= originalSize.x / 2;
        boxPoints.p7.y -= originalSize.y / 2;
        boxPoints.p7.z -= originalSize.z / 2;
        boxPoints.p8 = center;
        boxPoints.p8.x += originalSize.x / 2;
        boxPoints.p8.y -= originalSize.y / 2;
        boxPoints.p8.z -= originalSize.z / 2;
    }

    static GameObject CreateCube(Vector3 p)
    {
        return CreatePrimitiveType(p, PrimitiveType.Cube, new Vector3(0.3f, 0.3f, 0.3f));
    }

    static GameObject CreateSphere(Vector3 p)
    {
        return CreatePrimitiveType(p, PrimitiveType.Sphere, new Vector3(0.3f, 0.3f, 0.3f));
    }

    static GameObject CreatePrimitiveType(Vector3 p, PrimitiveType type, Vector3 size)
    {
        GameObject go = GameObject.CreatePrimitive(type);
        go.transform.position = p;
        go.transform.localScale = size;
        return go;
    }

}
