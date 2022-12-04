using System.Collections.Generic;
using UnityEngine;
using ZeroFrame.AllPhysics;
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
            boxes[i] = new Box3D(bcTF.position.ToSysVector3(), 1, 1, bcTF.localScale.z, bcTF.rotation.eulerAngles.ToSysVector3(), bcTF.localScale.ToSysVector3());
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
                if (CollisionHelper3D.HasCollision_AABB(boxes[i], boxes[j]))
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
        var a = box.GetMaxPos().ToUnityVector3();
        var b = box.GetMinPos().ToUnityVector3();
        Gizmos.DrawSphere(a, 0.1f);
        Gizmos.DrawSphere(b, 0.1f);
    }

    void DrawBoxBorder(Box3D box)
    {
        var a = box.GetMaxPos().ToUnityVector3();
        Gizmos.DrawLine(a, a + Vector3.right * box.Width);
        Gizmos.DrawLine(a, a + Vector3.back * box.length);
        Gizmos.DrawLine(a, a + Vector3.down * box.height);

        var b = box.GetMinPos().ToUnityVector3();
        Gizmos.DrawLine(b, b + Vector3.left * box.Width);
        Gizmos.DrawLine(b, b + Vector3.forward * box.length);
        Gizmos.DrawLine(b, b + Vector3.up * box.height);

        var c = a + Vector3.right * box.Width;
        Gizmos.DrawLine(c, c + Vector3.left * box.Width);
        Gizmos.DrawLine(c, c + Vector3.back * box.length);
        Gizmos.DrawLine(c, c + Vector3.down * box.height);

        var d = b + Vector3.left * box.Width;
        Gizmos.DrawLine(d, d + Vector3.right * box.Width);
        Gizmos.DrawLine(d, d + Vector3.forward * box.length);
        Gizmos.DrawLine(d, d + Vector3.up * box.height);
    }

}