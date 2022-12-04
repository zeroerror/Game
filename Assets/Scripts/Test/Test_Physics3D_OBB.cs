using System.Collections.Generic;
using UnityEngine;
using ZeroFrame.AllPhysics;
using ZeroFrame.AllMath;
using Game.Generic;

public class Test_Physics3D_OBB : MonoBehaviour
{

    bool isRun = false;

    Box3D[] boxs;
    Transform[] bcs;
    public Transform Boxes;

    int[] collsionArray;

    public void Start()
    {
        if (Boxes == null) return;
        isRun = true;

        var bcCount = Boxes.childCount;
        collsionArray = new int[bcCount];
        bcs = new Transform[bcCount];
        for (int i = 0; i < bcCount; i++)
        {
            var bc = Boxes.GetChild(i);
            bcs[i] = bc;
        }

        boxs = new Box3D[bcCount];
        for (int i = 0; i < bcCount; i++)
        {
            var bcTF = bcs[i].transform;
            boxs[i] = new Box3D(bcTF.position.ToSysVector3(), bcTF.localScale.x, bcTF.localScale.y, bcTF.localScale.z, bcTF.rotation.eulerAngles.ToSysVector3());
            boxs[i].SetBoxType(BoxType.OBB);
        }
        Debug.Log($"Total Box: {bcCount}");
    }

    public void OnDrawGizmos()
    {
        if (!isRun) return;
        if (bcs == null) return;
        if (boxs == null) return;

        for (int i = 0; i < collsionArray.Length; i++)
        {
            collsionArray[i] = 0;
        }

        for (int i = 0; i < boxs.Length - 1; i++)
        {
            for (int j = i + 1; j < boxs.Length; j++)
            {
                if (CollisionHelper3D.HasCollision_OBB(boxs[i], boxs[j]))
                {
                    collsionArray[i] = 1;
                    collsionArray[j] = 1;
                }
            }
        }

        Axis3D axis3D = new Axis3D();
        axis3D.center = System.Numerics.Vector3.Zero;
        axis3D.dir = System.Numerics.Vector3.UnitX;
        Gizmos.DrawLine((axis3D.center - 100f * axis3D.dir).ToUnityVector3(), (axis3D.center + 100f * axis3D.dir).ToUnityVector3());
        for (int i = 0; i < boxs.Length; i++)
        {
            var bc = bcs[i];
            var box = boxs[i];
            UpdateBox(bc.transform, box);
            Gizmos.color = Color.green;
            DrawBoxPoint(box);
            if (collsionArray[i] == 1)
            {
                Gizmos.color = Color.red;
                DrawBoxBorder(box);
            }

            DrawProjectionSub(axis3D, box);
        }

    }

    void DrawProjectionSub(Axis3D axis3D, Box3D box)
    {
        var proj = box.GetProjectionSub(axis3D);
        Gizmos.color = Color.white;
        Gizmos.color = Color.black;
        Gizmos.DrawLine((axis3D.dir.Normalize() * proj.X + axis3D.center).ToUnityVector3(), (axis3D.dir.Normalize() * proj.Y + axis3D.center).ToUnityVector3());
    }

    void UpdateBox(Transform src, Box3D box)
    {
        box.UpdateCenter(src.position.ToSysVector3());
        box.UpdateRotAngle(src.rotation.eulerAngles.ToSysVector3());
    }

    void DrawBoxPoint(Box3D box)
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
        float size = 0.02f;
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


    void DrawBoxBorder(Box3D box)
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

}