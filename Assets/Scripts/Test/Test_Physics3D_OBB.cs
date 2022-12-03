using System.Collections.Generic;
using UnityEngine;
using ZeroFrame.AllPhysics;
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
    }

    public void OnDrawGizmos()
    {
        if (!isRun) return;
        if (bcs == null) return;
        if (boxes == null) return;

        Dictionary<int, Box3D> collisionBoxDic = new Dictionary<int, Box3D>();
        // for (int i = 0; i < boxes.Length - 1; i++)
        // {
        //     for (int j = i + 1; j < boxes.Length; j++)
        //     {
        //         if (CollisionHelper3D.HasCollision_AABB(boxes[i], boxes[j]))
        //         {
        //             collisionBoxDic[i] = boxes[i];
        //             if (!collisionBoxDic.ContainsKey(j))
        //             {
        //                 collisionBoxDic[j] = boxes[j];
        //             }
        //         }
        //     }
        // }

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
        Gizmos.DrawSphere(a, 0.1f);
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(b, 0.1f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(c, 0.1f);
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(d, 0.1f);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(e, 0.1f);
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(f, 0.1f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(g, 0.1f);
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(h, 0.1f);
    }

    void DrawBoxBorder(Box3D box)
    {

    }

}