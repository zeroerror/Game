using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sample : MonoBehaviour
{

    public Transform a1;
    public Transform a2;
    public Transform b1;
    public Transform b2;

    void OnDrawGizmos()
    {
        // Gizmos.DrawSphere();

        // Init
        a1.localScale = b1.localScale * 1;
        a1.rotation = Quaternion.identity * b1.rotation;
        a1.position = b1.position * 1;
        Debug.Assert(a1.position == b1.position);
        Debug.Assert(a1.rotation == b1.rotation);
        Debug.Assert(a1.localScale == b1.localScale);

        var worldA2Scale = Vector3.Scale(b1.localScale, b2.localScale);
        a2.localScale = worldA2Scale;
        a2.rotation = b1.rotation * b2.localRotation;
        a2.position = b1.position + b2.localRotation * b2.localPosition;
        Debug.Assert(a2.localScale == b2.localScale);
        Debug.Assert(a2.rotation == b2.rotation);
        Debug.Assert(a2.position == b2.position, $"a2.position:{a2.position}  b2.position:{b2.position}");

        // == S



        // == R



        //== T

    }
}
