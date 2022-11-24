using System.Collections.Generic;
using UnityEngine;
using ZeroFrame.QuadTree;

public class QuadTreeUnityTest : MonoBehaviour
{

    struct Unit : IPos
    {

        public GameObject go;

        public float PosX => go.transform.position.x;

        public float PosY => go.transform.position.y;

    }

    QuadTree<Unit> qTree;
    public float quadLen;
    public int quadTreeLayer;
    public int searchLayer;
    public GameObject billboard;

    void Start()
    {
        qTree = new QuadTree<Unit>(quadLen, quadTreeLayer);
        billboard.transform.localScale = new Vector3(quadLen, quadLen, 1);
        Camera.main.orthographicSize = quadLen / 2f;
    }

    int index = 0;

    float deltaTime = 0;
    void Update()
    {
        isRunning = true;
        qTree.Tick();
        // if (deltaTime > 1f)
        // {
        //     qTree.Tick();
        //     deltaTime = 0;
        // }

        deltaTime += UnityEngine.Time.deltaTime;

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
        DrawUnitQuad();
    }

    void DrawQuadTree()
    {
        Gizmos.color = Color.red;
        var keys = qTree.origin_Dic.Keys;
        var e = keys.GetEnumerator();
        while (e.MoveNext())
        {
            var key = e.Current;
            var quadOrigin = qTree.origin_Dic[key];
            var posX = quadOrigin.posX;
            var posY = quadOrigin.posY;
            Vector3 pos = new Vector3(posX, posY, 0);
            var sideLen = quadOrigin.sideLength;
            var leftPos = pos + new Vector3(-sideLen, 0, 0);
            var rightPos = pos + new Vector3(sideLen, 0, 0);
            var upPos = pos + new Vector3(0, sideLen, 0);
            var downPos = pos + new Vector3(0, -sideLen, 0);
            Gizmos.DrawLine(leftPos, rightPos);
            Gizmos.DrawLine(upPos, downPos);
        }

        var halfLen = quadLen / 2f;
        Gizmos.DrawLine(new Vector3(-halfLen, halfLen), new Vector3(halfLen, halfLen));
        Gizmos.DrawLine(new Vector3(-halfLen, -halfLen), new Vector3(halfLen, -halfLen));
        Gizmos.DrawLine(new Vector3(-halfLen, halfLen), new Vector3(-halfLen, -halfLen));
        Gizmos.DrawLine(new Vector3(halfLen, halfLen), new Vector3(halfLen, -halfLen));
    }

    void DrawUnitQuad()
    {
        var unitDic = qTree.unitDic;
        var origin_Dic = qTree.origin_Dic;
        var values = qTree.unitDic.Values;
        foreach (var list in values)
        {
            list.ForEach((unit) =>
            {
                var posX = unit.PosX;
                var posY = unit.PosY;
                // - Self Quad
                Gizmos.color = Color.green;
                if (qTree.TryGetQuadOriginAndQuadIndex(posX, posY, out var quadOrigin, out var quadIndex))
                {
                    DrawQuad(quadOrigin, quadIndex);
                }
                // - Neighbor Quad
                Gizmos.color = Color.yellow;
                var originList = qTree.SearchQuad(posX, posY, searchLayer);
                originList.ForEach((quad) =>
                {
                    DrawQuad(quad.quadOrigin, quad.quadIndex);
                });
            });
        }
    }

    void DrawQuad(QuadOrigin origin, int quadIndex)
    {
        Vector3 ltPos = Vector3.zero;
        Vector3 rtPos = Vector3.zero;
        Vector3 lbPos = Vector3.zero;
        Vector3 rbPos = Vector3.zero;
        var originPos = new Vector3(origin.posX, origin.posY, 0);
        var sideLength = origin.sideLength;
        if (quadIndex == 1)
        {
            ltPos = originPos + new Vector3(-sideLength, sideLength);
            rtPos = originPos + new Vector3(0, sideLength);
            lbPos = originPos + new Vector3(-sideLength, 0);
            rbPos = originPos;
        }
        else if (quadIndex == 2)
        {
            ltPos = originPos + new Vector3(0, sideLength);
            rtPos = originPos + new Vector3(sideLength, sideLength); ;
            lbPos = originPos;
            rbPos = originPos + new Vector3(sideLength, 0);
        }
        else if (quadIndex == 3)
        {
            ltPos = originPos + new Vector3(-sideLength, 0);
            rtPos = originPos;
            lbPos = originPos + new Vector3(-sideLength, -sideLength);
            rbPos = originPos + new Vector3(0, -sideLength);
        }
        else if (quadIndex == 4)
        {
            ltPos = originPos;
            rtPos = originPos + new Vector3(sideLength, 0);
            lbPos = originPos + new Vector3(0, -sideLength);
            rbPos = originPos + new Vector3(sideLength, -sideLength);
        }

        Gizmos.DrawLine(ltPos, rtPos);
        Gizmos.DrawLine(rtPos, rbPos);
        Gizmos.DrawLine(rbPos, lbPos);
        Gizmos.DrawLine(lbPos, ltPos);
    }

}
