using System.Collections.Generic;
using UnityEngine;
using ZeroFrame.AllPhysics;
using Game.Generic;

public class Test_Phsics2D_OBB : MonoBehaviour
{

    bool isRun = false;

    Box2D[] boxes;
    BoxCollider[] bcs;
    public Transform Boxes;

    public void Start()
    {
        if (Boxes == null) return;
        isRun = true;

        var bcCount = Boxes.childCount;
        bcs = new BoxCollider[bcCount];
        for (int i = 0; i < bcCount; i++)
        {
            var bc = Boxes.GetChild(i);
            bcs[i] = bc.GetComponent<BoxCollider>();
        }

        boxes = new Box2D[bcCount];
        for (int i = 0; i < bcCount; i++)
        {
            var bcTF = bcs[i].transform;
            boxes[i] = new Box2D(bcTF.position.ToSysVector2(), bcTF.localScale.x, bcTF.localScale.y, bcTF.rotation.eulerAngles.z);
        }
    }

    public void OnDrawGizmos()
    {
        if (!isRun) return;
        if (bcs == null) return;
        if (boxes == null) return;

        Dictionary<int, Box2D> collisionBoxDic = new Dictionary<int, Box2D>();
        for (int i = 0; i < boxes.Length - 1; i++)
        {
            for (int j = i + 1; j < boxes.Length; j++)
            {
                if (CollisionHelper2D.HasCollision(boxes[i], boxes[j]))
                {
                    collisionBoxDic[i] = boxes[i];
                    if (!collisionBoxDic.ContainsKey(j))
                    {
                        collisionBoxDic[j] = boxes[j];
                    }
                }
            }
        }

        Gizmos.DrawLine(Vector3.zero + Vector3.up * 10f, Vector3.zero + Vector3.down * 10f);
        Gizmos.DrawLine(Vector3.zero + Vector3.left * 10f, Vector3.zero + Vector3.right * 10f);

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

    void DrawBoxBorder(Box2D box)
    {
        Gizmos.DrawLine(box.A.ToUnityVector3(), (box.B.ToUnityVector3()));
        Gizmos.DrawLine(box.B.ToUnityVector3(), (box.C.ToUnityVector3()));
        Gizmos.DrawLine(box.C.ToUnityVector3(), (box.D.ToUnityVector3()));
        Gizmos.DrawLine(box.D.ToUnityVector3(), (box.A.ToUnityVector3()));
    }

    void DrawBoxPoint(Box2D box)
    {
        var a = box.A;
        var b = box.B;
        var c = box.C;
        var d = box.D;
        Gizmos.DrawSphere(a.ToUnityVector3(), 0.1f);
        Gizmos.DrawSphere(b.ToUnityVector3(), 0.1f);
        Gizmos.DrawSphere(c.ToUnityVector3(), 0.1f);
        Gizmos.DrawSphere(d.ToUnityVector3(), 0.1f);
    }

    void UpdateBox(Transform src, Box2D box)
    {
        box.UpdateCenter(src.position.ToSysVector2());
        box.UpdateRotAngle(src.rotation.eulerAngles.z);
    }
}