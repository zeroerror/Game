using UnityEngine;
using ZeroFrame.AllPhysics;
using ZeroFrame.AllPhysics.Physics3D;
using Game.Generic;
using System.Collections.Generic;

public class Test_Physics3D_Raycast : MonoBehaviour
{

    public Transform rayStart;
    public Transform rayEnd;

    Ray3D ray;
    Box3D[] boxes;
    Transform[] tfs;
    public Transform Boxes;

    public BoxType boxType;

    void Start()
    {
        if (Boxes == null) return;
        isRun = true;

        var bcCount = Boxes.childCount;
        tfs = new Transform[bcCount];
        for (int i = 0; i < bcCount; i++)
        {
            tfs[i] = Boxes.GetChild(i);
        }

        boxes = new Box3D[bcCount];
        for (int i = 0; i < bcCount; i++)
        {
            var bcTF = tfs[i].transform;
            var pos = bcTF.position.ToSysVector3();
            var eulerAngles = bcTF.rotation.eulerAngles.ToSysVector3();
            var localScale = bcTF.localScale.ToSysVector3();
            boxes[i] = new Box3D(pos, 1, 1, 1, eulerAngles, localScale);
            boxes[i].SetBoxType(boxType);
        }
        Debug.Log($"Total Box: {bcCount}");

        var rayStartPos = rayStart.position;
        var rayEndPos = rayEnd.position;
        var posDiff = (rayEndPos - rayStartPos);
        ray = new Ray3D(rayStartPos.ToSysVector3(), posDiff.normalized.ToSysVector3(), posDiff.magnitude);
    }

    bool isRun = false;
    bool[] collisionArray = new bool[100];
    List<Vector3> hitPoints = new List<Vector3>();

    public void OnDrawGizmos()
    {
        if (!isRun) return;
        for (int i = 0; i < collisionArray.Length; i++)
        {
            collisionArray[i] = false;
        }
        hitPoints.Clear();

        var rayStartPos = rayStart.position;
        var rayEndPos = rayEnd.position;
        ray.origin = rayStartPos.ToSysVector3();
        var posDiff = (rayEndPos - rayStartPos);
        ray.dir = posDiff.normalized.ToSysVector3();
        ray = new Ray3D(rayStartPos.ToSysVector3(), posDiff.normalized.ToSysVector3(), posDiff.magnitude);

        bool hasCollision = false;
        for (int i = 0; i < boxes.Length; i++)
        {
            var b = boxes[i];
            UpdateBox(tfs[i], b);
            if (b.HasCollision(ray, out var hps))
            {
                collisionArray[i] = true;
                hps.ForEach((p) =>
                {
                    hitPoints.Add(p.ToUnityVector3());
                });
                hasCollision = true;
            }
        }

        Gizmos.color = Color.green;
        if (hasCollision) Gizmos.color = Color.red;
        Gizmos.DrawLine(ray.origin.ToUnityVector3(), ray.origin.ToUnityVector3() + (ray.dir.ToUnityVector3() * ray.length));

        for (int i = 0; i < boxes.Length; i++)
        {
            Gizmos.color = Color.green;
            if (collisionArray[i]) Gizmos.color = Color.red;
            DrawBoxBorders(boxes[i]);
            // DrawBoxPoints(boxes[i]);
        }

        Gizmos.color = Color.magenta;
        hitPoints.ForEach((p) =>
        {
            Gizmos.DrawSphere(p, 0.07f);
        });
    }

    void DrawBoxBorders(Box3D box)
    {
        var a = box.A.ToUnityVector3();
        var b = box.B.ToUnityVector3();
        var c = box.C.ToUnityVector3();
        var d = box.D.ToUnityVector3();
        var e = box.E.ToUnityVector3();
        var f = box.F.ToUnityVector3();
        var g = box.G.ToUnityVector3();
        var h = box.H.ToUnityVector3();
        Gizmos.DrawLine(a, b);
        Gizmos.DrawLine(b, c);
        Gizmos.DrawLine(c, d);
        Gizmos.DrawLine(d, a);
        Gizmos.DrawLine(e, f);
        Gizmos.DrawLine(f, g);
        Gizmos.DrawLine(g, h);
        Gizmos.DrawLine(h, e);
        Gizmos.DrawLine(a, e);
        Gizmos.DrawLine(b, f);
        Gizmos.DrawLine(c, g);
        Gizmos.DrawLine(d, h);
    }

    void DrawBoxPoints(Box3D box)
    {
        var a = box.A.ToUnityVector3();
        var b = box.B.ToUnityVector3();
        var c = box.C.ToUnityVector3();
        var d = box.D.ToUnityVector3();
        var e = box.E.ToUnityVector3();
        var f = box.F.ToUnityVector3();
        var g = box.G.ToUnityVector3();
        var h = box.H.ToUnityVector3();
        Gizmos.color = Color.red;
        float size = 0.1f;
        Gizmos.DrawSphere(a, size);
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(b, size);
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(c, size);
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(d, size);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(e, size);
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(f, size);
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(g, size);
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(h, size);
    }

    void UpdateBox(Transform src, Box3D box)
    {
        box.UpdateCenter(src.position.ToSysVector3());
        box.UpdateScale(src.localScale.ToSysVector3());
        box.UpdateRotAngle(src.rotation.eulerAngles.ToSysVector3());
    }

}