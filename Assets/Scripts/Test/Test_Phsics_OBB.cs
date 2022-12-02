using UnityEngine;
using ZeroFrame.AllPhysics;
using Game.Generic;

public class Test_Phsics_OBB : MonoBehaviour
{
    public bool isRun = false;

    Box2D box1;
    Box2D box2;
    public BoxCollider bc1;
    public BoxCollider bc2;

    public void Start()
    {
        var bc1TF = bc1.transform;
        box1 = new Box2D(bc1TF.position.ToSysVector2(), bc1TF.localScale.x, bc1TF.localScale.y, bc1TF.rotation.eulerAngles.z);
        var bc2TF = bc2.transform;
        box2 = new Box2D(bc2TF.position.ToSysVector2(), bc2TF.localScale.x, bc2TF.localScale.y, bc2TF.rotation.eulerAngles.z);
    }

    public void OnDrawGizmos()
    {
        if (!isRun) return;
        if (bc1 == null) return;
        if (bc2 == null) return;
        if (box1 == null) return;

        UpdateBox(bc1.transform, box1);
        UpdateBox(bc2.transform, box2);

        Gizmos.color = Color.green;
        DrawBoxPoint(box1);
        Gizmos.color = Color.green;
        DrawBoxPoint(box2);

        Gizmos.DrawLine(Vector3.zero + Vector3.up * 10f, Vector3.zero + Vector3.down * 10f);
        Gizmos.DrawLine(Vector3.zero + Vector3.left * 10f, Vector3.zero + Vector3.right * 10f);

        if (CollisionHelper2D.HasCollision_OBB(box1, box2)) Gizmos.color = Color.red;
        DrawBoxBorder(box1);
        DrawBoxBorder(box2);
    }

    void DrawBoxBorder(Box2D box)
    {
        Gizmos.DrawLine(box.GetA().ToUnityVector3(), (box.GetB().ToUnityVector3()));
        Gizmos.DrawLine(box.GetB().ToUnityVector3(), (box.GetC().ToUnityVector3()));
        Gizmos.DrawLine(box.GetC().ToUnityVector3(), (box.GetD().ToUnityVector3()));
        Gizmos.DrawLine(box.GetD().ToUnityVector3(), (box.GetA().ToUnityVector3()));
    }

    void DrawBoxPoint(Box2D box)
    {
        var a = box.GetA();
        var b = box.GetB();
        var c = box.GetC();
        var d = box.GetD();
        Gizmos.DrawSphere(a.ToUnityVector3(), 0.1f);
        Gizmos.DrawSphere(b.ToUnityVector3(), 0.1f);
        Gizmos.DrawSphere(c.ToUnityVector3(), 0.1f);
        Gizmos.DrawSphere(d.ToUnityVector3(), 0.1f);
    }

    void UpdateBox(Transform src, Box2D box)
    {
        box.SetCenter(src.position.ToSysVector2());
        box.SetRotAngle(src.rotation.eulerAngles.z);
    }
}