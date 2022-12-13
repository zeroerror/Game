using UnityEngine;
using ZeroFrame.AllPhysics;
using ZeroFrame.AllPhysics.Physics3D;
using Game.Generic;
using System.Collections.Generic;

public class Test_Physics3D_Raycast : MonoBehaviour
{

    public Transform rayStart;
    public Transform rayEnd;

    public Transform boxRoot;
    public BoxType boxType;
    Transform[] box_tfs;
    Box3D[] boxes;

    public Transform sphereRoot;
    Transform[] sphere_tfs;
    Sphere3D[] spheres;

    Ray3D ray;

    void Start()
    {
        if (boxRoot == null) return;
        if (sphereRoot == null) return;
        isRun = true;

        var count = boxRoot.childCount;
        box_tfs = new Transform[count];
        for (int i = 0; i < count; i++)
        {
            box_tfs[i] = boxRoot.GetChild(i);
        }
        boxes = new Box3D[count];
        for (int i = 0; i < count; i++)
        {
            var tf = box_tfs[i].transform;
            var pos = tf.position.ToSysVector3();
            var eulerAngles = tf.rotation.eulerAngles.ToSysVector3();
            var localScale = tf.localScale.ToSysVector3();
            boxes[i] = new Box3D(pos, 1, 1, 1, eulerAngles, localScale);
            boxes[i].SetBoxType(boxType);
        }
        Debug.Log($"Total Box: {count}");

        count = sphereRoot.childCount;
        sphere_tfs = new Transform[count];
        for (int i = 0; i < count; i++)
        {
            sphere_tfs[i] = sphereRoot.GetChild(i);
        }
        spheres = new Sphere3D[count];
        for (int i = 0; i < count; i++)
        {
            var tf = sphere_tfs[i].transform;
            var pos = tf.position.ToSysVector3();
            var eulerAngles = tf.rotation.eulerAngles.ToSysVector3();
            var localScale = tf.localScale.ToSysVector3();
            spheres[i] = new Sphere3D(pos, 0.5f, localScale.X);
        }
        Debug.Log($"Total Sphere: {count}");

        var rayStartPos = rayStart.position;
        var rayEndPos = rayEnd.position;
        var posDiff = (rayEndPos - rayStartPos);
        ray = new Ray3D(rayStartPos.ToSysVector3(), posDiff.normalized.ToSysVector3(), posDiff.magnitude);
    }

    bool isRun = false;
    bool[] collisionList_box = new bool[100];
    bool[] collisionList_sphere = new bool[100];
    List<Vector3> hitPointList = new List<Vector3>();

    public void OnDrawGizmos()
    {
        if (!isRun) return;
        for (int i = 0; i < collisionList_box.Length; i++) { collisionList_box[i] = false; }
        for (int i = 0; i < collisionList_sphere.Length; i++) { collisionList_sphere[i] = false; }
        hitPointList.Clear();

        var rayStartPos = rayStart.position;
        var rayEndPos = rayEnd.position;
        ray.origin = rayStartPos.ToSysVector3();
        var posDiff = (rayEndPos - rayStartPos);
        ray.dir = posDiff.normalized.ToSysVector3();
        ray = new Ray3D(rayStartPos.ToSysVector3(), posDiff.normalized.ToSysVector3(), posDiff.magnitude);

        bool hasCollision = false;

        // - Box
        for (int i = 0; i < boxes.Length; i++)
        {
            var b = boxes[i];
            UpdateBox(box_tfs[i], b);
            if (CollisionHelper3D.HasCollision(b, ray, out var hps))
            {
                collisionList_box[i] = true;
                hps.ForEach((p) =>
                {
                    hitPointList.Add(p.ToUnityVector3());
                });
                hasCollision = true;
            }
        }
        for (int i = 0; i < boxes.Length; i++)
        {
            Gizmos.color = Color.green;
            if (collisionList_box[i]) Gizmos.color = Color.red;
            DrawBoxBorders(boxes[i]);
            // DrawBoxPoints(boxes[i]);
        }

        // - Sphere
        for (int i = 0; i < spheres.Length; i++)
        {
            var s = spheres[i];
            UpdateSphere3D(sphere_tfs[i], s);
            if (CollisionHelper3D.HasCollision(s, ray, out var hps))
            {
                collisionList_sphere[i] = true;
                hps.ForEach((p) =>
                {
                    hitPointList.Add(p.ToUnityVector3());
                });
                hasCollision = true;
            }
        }

        Gizmos.color = Color.red;
        for (int i = 0; i < spheres.Length; i++)
        {
            if (collisionList_sphere[i])
            {
                DrawSphere3D(spheres[i]);
            }
        }

        // - Ray
        Gizmos.color = Color.green;
        if (hasCollision) Gizmos.color = Color.red;
        Gizmos.DrawLine(ray.origin.ToUnityVector3(), ray.origin.ToUnityVector3() + (ray.dir.ToUnityVector3() * ray.length));

        // - Hit Points
        Gizmos.color = Color.white;
        hitPointList.ForEach((p) =>
        {
            Gizmos.DrawSphere(p, 0.08f);
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

    void DrawSphere3D(Sphere3D sphere)
    {
        Gizmos.DrawSphere(sphere.Center.ToUnityVector3(), sphere.Radius_scaled);
    }

    void UpdateSphere3D(Transform src, Sphere3D sphere)
    {
        sphere.UpdateCenter(src.position.ToSysVector3());
        sphere.UpdateScale(src.localScale.x);
    }

}