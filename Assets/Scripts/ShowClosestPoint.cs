using UnityEngine;

// Attach this to a GameObject that has a Collider component attached
public class ShowClosestPoint : MonoBehaviour
{
    public Transform location;

    public void OnDrawGizmos()
    {
        var collider = GetComponent<Collider>();

        if (!collider)
            return; // nothing to do without a collider

        if (location == null) return;
        Vector3 closestPoint = collider.ClosestPoint(location.position);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(location.position, 0.1f);
        Gizmos.DrawWireSphere(closestPoint, 0.1f);
    }
}