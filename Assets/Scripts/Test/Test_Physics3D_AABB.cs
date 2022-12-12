using System.Collections.Generic;
using UnityEngine;
using ZeroFrame.AllPhysics;
using ZeroFrame.AllPhysics.Physics3D;
using Game.Generic;

public class Test_Physics3D_AABB : MonoBehaviour
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
            boxes[i] = new Box3D(bcTF.position.ToSysVector3(), 1, 1, 1, bcTF.rotation.eulerAngles.ToSysVector3(), bcTF.localScale.ToSysVector3());
            boxes[i].SetBoxType(BoxType.AABB);
        }
    }

    public void OnDrawGizmos()
    {
        if (!isRun) return;
        if (bcs == null) return;
        if (boxes == null) return;

        // Gizmos.DrawLine(Vector3.zero + Vector3.up * 10f, Vector3.zero + Vector3.down * 10f);
        // Gizmos.DrawLine(Vector3.zero + Vector3.left * 10f, Vector3.zero + Vector3.right * 10f);
        // Gizmos.DrawLine(Vector3.zero + Vector3.forward * 10f, Vector3.zero + Vector3.back * 10f);

        Dictionary<int, Box3D> collisionBoxDic = new Dictionary<int, Box3D>();
        for (int i = 0; i < boxes.Length - 1; i++)
        {
            for (int j = i + 1; j < boxes.Length; j++)
            {
                if (CollisionHelper3D.HasCollision(boxes[i], boxes[j]))
                {
                    collisionBoxDic[i] = boxes[i];
                    if (!collisionBoxDic.ContainsKey(j))
                    {
                        collisionBoxDic[j] = boxes[j];
                    }
                }
            }
        }


        for (int i = 0; i < boxes.Length; i++)
        {
            var bc = bcs[i];
            var box = boxes[i];
            UpdateBox(bc.transform, box);
            Gizmos.color = Color.green;
            DrawBoxPoint(box);
            if (collisionBoxDic.ContainsKey(i))
            {
                Gizmos.color = Color.red;
            }
            DrawBoxBorder(box);
        }

    }

    void UpdateBox(Transform src, Box3D box)
    {
        box.UpdateCenter(src.position.ToSysVector3());
        box.UpdateScale(src.localScale.ToSysVector3());
    }

    void DrawBoxPoint(Box3D box)
    {
        var a = box.A.ToUnityVector3();
        var g = box.G.ToUnityVector3();
        Gizmos.DrawSphere(a, 0.1f);
        Gizmos.DrawSphere(g, 0.1f);
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