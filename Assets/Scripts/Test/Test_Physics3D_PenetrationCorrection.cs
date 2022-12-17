using UnityEngine;
using ZeroFrame.AllPhysics.Physics3D;
using Game.Generic;
using ZeroFrame.AllPhysics;
using ZeroFrame.AllMath;

public class Test_Physics3D_PenetrationCorrection : MonoBehaviour
{

    public BoxType boxType;

    public Transform Boxes;
    Box3D[] boxes;
    Transform[] boxTfs;

    int[] collsionArray;
    bool canRun = false;

    void Start()
    {
        canRun = Boxes != null;
        if (!canRun) return;
        InitBox3Ds();
    }


    public void OnDrawGizmos()
    {
        if (!canRun) return;
        if (boxTfs == null) return;
        if (boxes == null) return;

        // - Collision 
        for (int i = 0; i < collsionArray.Length; i++) { collsionArray[i] = 0; }
        for (int i = 0; i < boxes.Length - 1; i++)
        {
            for (int j = i + 1; j < boxes.Length; j++)
            {
                if (CollisionHelper3D.HasCollision(boxes[i], boxes[j])) { collsionArray[i] = 1; collsionArray[j] = 1; }
            }
        }

        // - Projection
        Axis3D axis3D = new Axis3D();
        axis3D.origin = System.Numerics.Vector3.Zero;
        axis3D.dir = System.Numerics.Vector3.UnitX;
        Gizmos.DrawLine((axis3D.origin - 100f * axis3D.dir).ToUnityVector3(), (axis3D.origin + 100f * axis3D.dir).ToUnityVector3());

        // - Update And DrawBox
        for (int i = 0; i < boxes.Length; i++)
        {
            var bc = boxTfs[i];
            var box = boxes[i];
            UpdateBox(bc.transform, box);
            Gizmos.color = Color.green;
            DrawBoxPoint(box);
            if (collsionArray[i] == 1) Gizmos.color = Color.red;
            DrawBoxBorder(box);
            DrawProjectionSub(axis3D, box);
        }
    }

    void DrawProjectionSub(Axis3D axis3D, Box3D box)
    {
        var proj = box.GetProjectionSub(axis3D);
        Gizmos.color = Color.white;
        Gizmos.color = Color.black;
        axis3D.dir.Normalize();
        Gizmos.DrawLine((axis3D.dir * proj.X + axis3D.origin).ToUnityVector3(), (axis3D.dir * proj.Y + axis3D.origin).ToUnityVector3());
    }

    void OnGUI()
    {
        if (GUILayout.Button("穿透恢复"))
        {
            UpdateAllBoxes();
            CollisionUpdate();
            for (int i = 0; i < boxes.Length - 1; i++)
            {
                var c1 = collsionArray[i];
                var b1 = boxes[i];
                var btf1 = boxTfs[i];
                for (int j = i + 1; j < boxes.Length; j++)
                {
                    var btf2 = boxTfs[j];
                    var c2 = collsionArray[j];
                    var b2 = boxes[j];
                    if (c1 == 1 && c2 == 1)
                    {
                        // - Penetration
                        CollisionHelper3D.PenetrationCorrection(b1, 0.5f, b2, 0.5f);
                        btf1.position = b1.Center.ToUnityVector3();
                        btf2.position = b2.Center.ToUnityVector3();
                    }
                }
            }
        }
    }

    void InitBox3Ds()
    {
        var bcCount = Boxes.childCount;
        collsionArray = new int[bcCount];
        boxTfs = new Transform[bcCount];
        for (int i = 0; i < bcCount; i++)
        {
            var bc = Boxes.GetChild(i);
            boxTfs[i] = bc;
        }

        boxes = new Box3D[bcCount];
        for (int i = 0; i < bcCount; i++)
        {
            var bcTF = boxTfs[i].transform;
            boxes[i] = new Box3D(bcTF.position.ToSysVector3(), 1, 1, 1, bcTF.rotation.eulerAngles.ToSysVector3(), bcTF.localScale.ToSysVector3());
            boxes[i].SetBoxType(boxType);
            Debug.Log($"BoxType: {boxes[i].BoxType}");
        }
        Debug.Log($"Total Box: {bcCount}");
    }

    void UpdateAllBoxes()
    {
        for (int i = 0; i < boxes.Length; i++)
        {
            var bc = boxTfs[i];
            var box = boxes[i];
            UpdateBox(bc.transform, box);
        }
    }

    void CollisionUpdate()
    {
        for (int i = 0; i < collsionArray.Length; i++) { collsionArray[i] = 0; }
        for (int i = 0; i < boxes.Length - 1; i++)
        {
            for (int j = i + 1; j < boxes.Length; j++)
            {
                if (CollisionHelper3D.HasCollision(boxes[i], boxes[j]))
                {
                    collsionArray[i] = 1;
                    collsionArray[j] = 1;
                }
            }
        }
    }

    void UpdateBox(Transform src, Box3D box)
    {
        box.UpdateCenter(src.position.ToSysVector3());
        box.UpdateScale(src.localScale.ToSysVector3());
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