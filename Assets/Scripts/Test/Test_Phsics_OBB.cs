using UnityEngine;

public class Test_Phsics_OBB : MonoBehaviour
{
    public Transform tf1;
    public Transform tf2;

    public bool isRun = false;

    public void OnDrawGizmos()
    {
        if (!isRun) return;
        if (tf1 == null || tf2 == null) return;

        var a1_tf = tf1.GetChild(0);
        var b1_tf = tf1.GetChild(1);
        var c1_tf = tf1.GetChild(2);
        var d1_tf = tf1.GetChild(3);

        var a2_tf = tf2.GetChild(0);
        var b2_tf = tf2.GetChild(1);
        var c2_tf = tf2.GetChild(2);
        var d2_tf = tf2.GetChild(3);
        if (a1_tf == null || b1_tf == null || c1_tf == null || d1_tf == null) return;
        if (a2_tf == null || b2_tf == null || c2_tf == null || d2_tf == null) return;

        // - Cude 1
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(a1_tf.position, b1_tf.position);
        Gizmos.DrawLine(b1_tf.position, c1_tf.position);
        Gizmos.DrawLine(c1_tf.position, d1_tf.position);
        Gizmos.DrawLine(d1_tf.position, a1_tf.position);

        var rotAngle1 = tf1.rotation.eulerAngles.z;
        Vector3 leftPos = Vector3.zero;
        Vector3 rightPos = Vector3.zero;
        Vector3 topPos = Vector3.zero;
        Vector3 downPos = Vector3.zero;
        GetQuadLRTDPos(a1_tf.position, b1_tf.position, c1_tf.position, d1_tf.position, rotAngle1,
        ref leftPos, ref rightPos, ref topPos, ref downPos);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(leftPos + Vector3.up * 10f, leftPos + Vector3.down * 10f);
        Gizmos.DrawLine(rightPos + Vector3.up * 10f, rightPos + Vector3.down * 10f);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(topPos + Vector3.left * 10f, topPos + Vector3.right * 10f);
        Gizmos.DrawLine(downPos + Vector3.left * 10f, downPos + Vector3.right * 10f);

        // - Cude 2
        Gizmos.color = Color.red;
        Gizmos.DrawLine(a2_tf.position, b2_tf.position);
        Gizmos.DrawLine(b2_tf.position, c2_tf.position);
        Gizmos.DrawLine(c2_tf.position, d2_tf.position);
        Gizmos.DrawLine(d2_tf.position, a2_tf.position);

        var rotAngle2 = tf2.rotation.eulerAngles.z;
        GetQuadLRTDPos(a2_tf.position, b2_tf.position, c2_tf.position, d2_tf.position, rotAngle2,
        ref leftPos, ref rightPos, ref topPos, ref downPos);

        Gizmos.color = Color.black;
        Gizmos.DrawLine(leftPos + Vector3.up * 10f, leftPos + Vector3.down * 10f);
        Gizmos.DrawLine(rightPos + Vector3.up * 10f, rightPos + Vector3.down * 10f);
        Gizmos.color = Color.white;
        Gizmos.DrawLine(topPos + Vector3.left * 10f, topPos + Vector3.right * 10f);
        Gizmos.DrawLine(downPos + Vector3.left * 10f, downPos + Vector3.right * 10f);
    }

    void GetQuadLRTDPos(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float rotAngle,
    ref Vector3 leftPos, ref Vector3 rightPos, ref Vector3 topPos, ref Vector3 downPos)
    {
        var count = (int)(rotAngle / 90);
        bool isPositive = rotAngle > 0;
        if (count == 0)
        {
            if (isPositive)
            {
                leftPos = a;
                rightPos = c;
                topPos = b;
                downPos = d;
            }
            else
            {
                leftPos = d;
                rightPos = b;
                topPos = a;
                downPos = c;
            }
        }
        else if (count == 1)
        {
            if (isPositive)
            {
                leftPos = b;
                rightPos = d;
                topPos = c;
                downPos = a;
            }
            else
            {
                leftPos = c;
                rightPos = a;
                topPos = d;
                downPos = b;
            }
        }
        else if (count == 2)
        {
            if (isPositive)
            {
                leftPos = c;
                rightPos = a;
                topPos = d;
                downPos = b;
            }
            else
            {
                leftPos = b;
                rightPos = d;
                topPos = c;
                downPos = a;
            }
        }
        else if (count == 3)
        {
            if (isPositive)
            {
                leftPos = d;
                rightPos = b;
                topPos = a;
                downPos = c;
            }
            else
            {
                leftPos = a;
                rightPos = c;
                topPos = b;
                downPos = d;
            }
        }
    }

    float GetProjection(Vector2 a, Vector2 b)
    {
        return Vector2.Dot(a, b.normalized);
    }

}