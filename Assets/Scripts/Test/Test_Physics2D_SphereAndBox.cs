using System.Collections.Generic;
using UnityEngine;
using ZeroFrame.AllPhysics;
using ZeroFrame.AllMath;
using Game.Generic;

public class Test_Physics2D_SphereAndBox : MonoBehaviour
{

    bool isRun = false;

    Box2D[] allBoxes;
    Sphere2D[] allSpheres;
    Transform[] tfs;
    public Transform spheresAndBoxes;

    int[] collsionArray;

    public void Start()
    {
        if (spheresAndBoxes == null) return;
        isRun = true;

        var bcCount = spheresAndBoxes.childCount;
        collsionArray = new int[bcCount];
        tfs = new Transform[bcCount];
        for (int i = 0; i < bcCount; i++)
        {
            var bc = spheresAndBoxes.GetChild(i);
            tfs[i] = bc;
        }

        allBoxes = new Box2D[bcCount];
        allSpheres = new Sphere2D[bcCount];
        int boxCount = 0;
        int sphereCount = 0;
        for (int i = 0; i < bcCount; i++)
        {
            var bcTF = tfs[i].transform;
            if (bcTF.GetComponent<BoxCollider>())
            {
                allBoxes[i] = new Box2D(bcTF.position.ToSysVector2(), bcTF.localScale.x, bcTF.localScale.y, bcTF.rotation.eulerAngles.z);
                allBoxes[i].SetBoxType(BoxType.OBB);
                boxCount++;
            }
            else if (bcTF.GetComponent<SphereCollider>())
            {
                allSpheres[i] = new Sphere2D(bcTF.position.ToSysVector2(), bcTF.GetComponent<SphereCollider>().radius, bcTF.rotation.eulerAngles.z);
                sphereCount++;
            }

        }
        Debug.Log($"Total Box: {boxCount}");
        Debug.Log($"Total Sphere: {sphereCount}");
    }

    public void OnDrawGizmos()
    {
        if (!isRun) return;
        if (tfs == null) return;
        if (allBoxes == null) return;

        for (int i = 0; i < collsionArray.Length; i++)
        {
            collsionArray[i] = 0;
        }

        for (int i = 0; i < allBoxes.Length - 1; i++)
        {
            var box1 = allBoxes[i];
            if (box1 == null) continue;
            for (int j = i + 1; j < allBoxes.Length; j++)
            {
                var box2 = allBoxes[j];
                if (box2 == null) continue;
                if (CollisionHelper2D.HasCollision_OBB(box1, box2))
                {
                    collsionArray[i] = 1;
                    collsionArray[j] = 1;
                }
            }
        }

        for (int i = 0; i < allSpheres.Length - 1; i++)
        {
            var sphere1 = allSpheres[i];
            if (sphere1 == null) continue;
            for (int j = i + 1; j < allSpheres.Length; j++)
            {
                var sphere2 = allSpheres[j];
                if (sphere2 == null) continue;
                if (CollisionHelper2D.HasCollision(sphere1, sphere2))
                {
                    collsionArray[i] = 1;
                    collsionArray[j] = 1;
                }
            }
        }

        for (int i = 0; i < allSpheres.Length; i++)
        {
            var sphere = allSpheres[i];
            if (sphere == null) continue;
            for (int j = 0; j < allBoxes.Length; j++)
            {
                var box = allBoxes[j];
                if (box == null) continue;
                if (CollisionHelper2D.HasCollision(sphere, box))
                {
                    collsionArray[i] = 1;
                    collsionArray[j] = 1;
                }
            }
        }

        for (int i = 0; i < allBoxes.Length; i++)
        {
            var bc = tfs[i];
            var box = allBoxes[i];
            if (box == null) continue;
            UpdateBox(bc.transform, box);
            Gizmos.color = Color.green;
            DrawBoxPoint(box);
            if (collsionArray[i] == 1) { Gizmos.color = Color.red; DrawBoxBorder(box); }
        }

        for (int i = 0; i < allSpheres.Length; i++)
        {
            var bc = tfs[i];
            var sphere = allSpheres[i];
            if (sphere == null) continue;
            UpdateSphere(bc.transform, sphere);
            Gizmos.color = Color.green;
            if (collsionArray[i] == 1) Gizmos.color = Color.red;
            DrawSphereBorder(sphere);
        }

    }

    void DrawProjectionSub(Axis2D axis2D, Box2D box)
    {
        var proj = box.GetProjectionSub(axis2D);
        Gizmos.color = Color.white;
        Gizmos.color = Color.black;
        Gizmos.DrawLine((axis2D.dir.Normalize() * proj.X + axis2D.center).ToUnityVector3(), (axis2D.dir.Normalize() * proj.Y + axis2D.center).ToUnityVector3());
    }

    void UpdateBox(Transform src, Box2D box)
    {
        box.SetCenter(src.position.ToSysVector2());
        box.SetRotAngle(src.rotation.eulerAngles.z);
    }

    void UpdateSphere(Transform src, Sphere2D sphere)
    {
        sphere.SetCenter(src.position.ToSysVector2());
        sphere.SetRotAngle(src.rotation.eulerAngles.z);
    }

    void DrawBoxPoint(Box2D box)
    {
        var a = box.GetA().ToUnityVector3();
        var b = box.GetB().ToUnityVector3();
        var c = box.GetC().ToUnityVector3();
        var d = box.GetD().ToUnityVector3();
        Gizmos.color = Color.red;
        float size = 0.02f;
        Gizmos.DrawSphere(a, size);
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(b, size);
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(c, size);
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(d, size);
        Gizmos.color = Color.red;
    }

    void DrawBoxBorder(Box2D box)
    {
        var a = box.GetA().ToUnityVector3();
        var b = box.GetB().ToUnityVector3();
        var c = box.GetC().ToUnityVector3();
        var d = box.GetD().ToUnityVector3();
        Gizmos.DrawLine(a, b);
        Gizmos.DrawLine(b, c);
        Gizmos.DrawLine(c, d);
        Gizmos.DrawLine(d, a);
    }

    void DrawSphereBorder(Sphere2D sphere)
    {
        Gizmos.DrawSphere(sphere.Center.ToUnityVector3(), sphere.Radius + 0.02f);
    }

}