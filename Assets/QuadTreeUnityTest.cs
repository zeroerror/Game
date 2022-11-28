using System.Collections.Generic;
using UnityEngine;
using ZeroFrame.QuadTree;

public class QuadTreeUnityTest : MonoBehaviour
{

    struct Unit : IBounds
    {

        public GameObject go;

        public System.Numerics.Vector2 LTPos => new System.Numerics.Vector2(go.transform.position.x, go.transform.position.y);
        public System.Numerics.Vector2 RBPos => new System.Numerics.Vector2(go.transform.position.x, go.transform.position.y);

    }

    QuadTree<Unit> qTree;
    public float quadLen;
    public int quadTreeLayer;
    public int searchLayer;
    public GameObject billboard;

    void Start()
    {
        qTree = new QuadTree<Unit>(quadLen, quadTreeLayer, System.Numerics.Vector2.Zero);
        billboard.transform.localScale = new Vector3(quadLen, quadLen, 1);
        Camera.main.orthographicSize = quadLen / 2f;
    }

    int index = 0;
    void Update()
    {
        isRunning = true;
        qTree.Tick();

        if (Input.GetMouseButtonDown(0) && Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 1000))
        {
            Unit unit;
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            var pos = hit.point;
            pos.z = 0;
            go.transform.position = pos;
            go.transform.name = $"unit_{index++}";
            unit.go = go;
            qTree.AddUnit(unit);
        }
    }

    bool isRunning = false;
    void OnDrawGizmos()
    {
        if (!isRunning)
        {
            return;
        }

        DrawQuadTree();
    }

    void DrawQuadTree()
    {
        Gizmos.color = Color.red;
        var e = qTree.quadDic.GetEnumerator();
        while (e.MoveNext())
        {
            var kvp = e.Current;
            var key = kvp.Key;
            var quad = kvp.Value;
            DrawQuad(quad);
        }

    }

    static void DrawQuad(Quad<Unit> quad)
    {
        var ltp = quad.ltPos;
        var rbp = quad.rbPos;
        Vector3 ltPos = new Vector3(ltp.X, ltp.Y, 0);
        Vector3 rbPos = new Vector3(rbp.X, rbp.Y, 0);
        var len = rbp.X - ltp.X;

        Vector3 rtPos = ltPos;
        rtPos.x += len;
        Vector3 lbPos = rbPos;
        lbPos.x -= len;

        Gizmos.color = Color.green;
        Gizmos.DrawLine(ltPos, rtPos);
        Gizmos.DrawLine(lbPos, rbPos);
        Gizmos.DrawLine(ltPos, lbPos);
        Gizmos.DrawLine(rtPos, rbPos);
    }

}
