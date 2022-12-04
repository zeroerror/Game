using System.Collections.Generic;
using UnityEngine;
using ZeroFrame.AllPhysics;
using ZeroFrame.AllMath;
using Game.Generic;

public class Test_Physics3D_OBB : MonoBehaviour
{

    bool isRun = false;

    Box3D[] boxes;
    Transform[] bcs;
    public Transform Boxes;

    public void Start()
    {
        if (Boxes == null) return;
        isRun = true;

        var bcCount = Boxes.childCount;
        bcs = new Transform[bcCount];
        for (int i = 0; i < bcCount; i++)
        {
            var bc = Boxes.GetChild(i);
            bcs[i] = bc;
        }

        boxes = new Box3D[bcCount];
        for (int i = 0; i < bcCount; i++)
        {
            var bcTF = bcs[i].transform;
            boxes[i] = new Box3D(bcTF.position.ToSysVector3(), bcTF.localScale.x, bcTF.localScale.y, bcTF.localScale.z, bcTF.rotation.eulerAngles.ToSysVector3());
            boxes[i].SetBoxType(BoxType.OBB);
        }
        Debug.Log($"Total Box: {bcCount}");
    }

    public void OnDrawGizmos()
    {
        if (!isRun) return;
        if (bcs == null) return;
        if (boxes == null) return;

        Dictionary<int, Box3D> collisionBoxDic = new Dictionary<int, Box3D>();
        for (int i = 0; i < boxes.Length - 1; i++)
        {
            for (int j = i + 1; j < boxes.Length; j++)
            {
                if (CollisionHelper3D.HasCollision_OBB(boxes[i], boxes[j]))
                {
                    collisionBoxDic[i] = boxes[i];
                    if (!collisionBoxDic.ContainsKey(j))
                    {
                        collisionBoxDic[j] = boxes[j];
                    }
                }
            }
        }

        Axis3D axis3D = new Axis3D();
        axis3D.center = System.Numerics.Vector3.Zero;
        axis3D.dir = System.Numerics.Vector3.UnitX;
        Gizmos.DrawLine((axis3D.center - 100f * axis3D.dir).ToUnityVector3(), (axis3D.center + 100f * axis3D.dir).ToUnityVector3());
        for (int i = 0; i < boxes.Length; i++)
        {
            var bc = bcs[i];
            var box = boxes[i];
            UpdateBox(bc.transform, box);
            Gizmos.color = Color.green;
            DrawBoxPoint(box);
            if (collisionBoxDic.ContainsKey(i)) Gizmos.color = Color.red;
            DrawBoxBorder(box);
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
        box.SetCenter(src.position.ToSysVector3());
        box.SetRotAngle(src.rotation.eulerAngles.ToSysVector3());
    }

    void DrawBoxPoint(Box3D box)
    {
        var a = box.GetA().ToUnityVector3();
        var b = box.GetB().ToUnityVector3();
        var c = box.GetC().ToUnityVector3();
        var d = box.GetD().ToUnityVector3();
        var e = box.GetE().ToUnityVector3();
        var f = box.GetF().ToUnityVector3();
        var g = box.GetG().ToUnityVector3();
        var h = box.GetH().ToUnityVector3();
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
        var a = box.GetA().ToUnityVector3();
        var b = box.GetB().ToUnityVector3();
        var c = box.GetC().ToUnityVector3();
        var d = box.GetD().ToUnityVector3();
        var e = box.GetE().ToUnityVector3();
        var f = box.GetF().ToUnityVector3();
        var g = box.GetG().ToUnityVector3();
        var h = box.GetH().ToUnityVector3();
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