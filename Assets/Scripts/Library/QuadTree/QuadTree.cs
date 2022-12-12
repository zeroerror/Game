using System;
using System.Collections.Generic;
using System.Numerics;
using ZeroFrame.AllMath;
using ZeroFrame.AllPhysics.Physics2D;

public interface IBounds
{
    Vector2 LTPos { get; }
    Vector2 RBPos { get; }
}

public class Quad<T> where T : IBounds
{

    public ushort quadID;
    public Vector2 ltPos;
    public Vector2 rbPos;

    public Quad() { }

}

public class Unit<T> where T : IBounds
{

    ushort unitID;
    public ushort UnitID => unitID;
    public void SetUnitID(ushort v) => unitID = v;

    T value;
    public T Value => value;
    public void SetValue(T v) => value = v;

}

public class QuadTree<T> where T : IBounds
{

    // -------------------------- Quad And Unit--------------------------
    public Dictionary<ushort, Quad<T>> quadDic;
    public Dictionary<ushort, int> quadUnitCountDic;
    public Dictionary<ushort, List<Unit<T>>> quadUnitDic;

    public Dictionary<ushort, Unit<T>> unitDic;
    public Dictionary<ushort, List<Quad<T>>> unitQuadDic;


    ushort unitAutoID;

    float sideLen;  // - 矩形边长
    int maxLayer;   // - 最大层数
    int quadUnitLimit;   // - Quad最大包含Unit数量
    public Vector2 rootPos;    // - 根坐标
    public int maxNode;

    ulong[] removeKeyArray;

    public QuadTree(float sideLen, int maxLayer, Vector2 rootPos, int quadUnitLimit = 4, int maxUnit = 1000)
    {
        maxLayer = maxLayer > 8 ? 8 : maxLayer;
        this.sideLen = sideLen;
        this.maxLayer = maxLayer;
        this.quadUnitLimit = quadUnitLimit;
        this.rootPos = rootPos;
        this.maxNode = (int)(AllDigit.Pow(4, maxLayer) - 4) / 3 + 1;
        this.unitAutoID = 0;

        quadDic = new Dictionary<ushort, Quad<T>>();
        quadUnitDic = new Dictionary<ushort, List<Unit<T>>>();
        quadUnitCountDic = new Dictionary<ushort, int>();

        unitDic = new Dictionary<ushort, Unit<T>>();
        unitQuadDic = new Dictionary<ushort, List<Quad<T>>>();

        var quad = new Quad<T>();
        var halfLen = sideLen / 2f;
        quad.quadID = 0;
        quad.ltPos = new Vector2(rootPos.X - halfLen, rootPos.Y + halfLen);
        quad.rbPos = new Vector2(rootPos.X + halfLen, rootPos.Y - halfLen);

        quadDic.TryAdd(0, quad);
        quadUnitDic.TryAdd(0, new List<Unit<T>>(10));
        quadUnitCountDic.Add(0, 0);

        removeKeyArray = new ulong[maxNode];
    }

    public void Tick()
    {
        Clear();
        var e = unitDic.Values.GetEnumerator();
        while (e.MoveNext())
        {
            var unit = e.Current;
            TickUnitLocation(unit);
        }

        TickQuadSplit();
    }

    public void Clear()
    {
        var keys = quadUnitCountDic.Keys;
        ushort[] keyArray = new ushort[keys.Count];
        keys.CopyTo(keyArray, 0);
        for (int i = 0; i < keyArray.Length; i++)
        {
            var key = keyArray[i];
            quadUnitCountDic[key] = 0;
        }

        var e1 = quadUnitDic.Keys.GetEnumerator();
        while (e1.MoveNext())
        {
            var key = e1.Current;
            quadUnitDic[key].Clear();
        }

        var e2 = unitQuadDic.Keys.GetEnumerator();
        while (e2.MoveNext())
        {
            var key = e2.Current;
            unitQuadDic[key].Clear();
        }
    }

    public void ForeachUnit(Action<Unit<T>> action)
    {
        var e = unitDic.Values.GetEnumerator();
        while (e.MoveNext())
        {
            action.Invoke(e.Current);
        }
    }

    public bool TryGetUnits(Quad<T> quad, out List<Unit<T>> list)
    {
        List<Unit<T>> units = new List<Unit<T>>();
        var qid = quad.quadID;
        return quadUnitDic.TryGetValue(qid, out list);
    }

    // 获取坐标区域内所有单位

    /// 获取Unit产生的所有AABB碰撞的Quad
    /// </summary>
    /// <param name="bounds"></param>
    /// <returns></returns>
    public void GetAABBCollsionQuadList(IBounds bounds, List<Quad<T>> quadList, int quadID)
    {
        GetAABBCollsionQuadList(bounds.LTPos, bounds.RBPos, quadList, 0);
    }

    public void GetAABBCollsionQuadList(in Vector2 ltPos, in Vector2 rbPos, List<Quad<T>> quadList, ushort quadID)
    {
        if (quadID < 0 || quadID >= maxNode)
        {
            return;
        }

        ushort quadIndex = (ushort)(quadID % 4);
        if (quadIndex == 0 && quadID != 0)
        {
            quadIndex = 4;
        }

        var fatherQuadID = (ushort)((quadID - quadIndex) / 4);

        if (!quadDic.TryGetValue(quadID, out var quad))
        {
            return;
        }

        if (!CollisionHelper2D.HasCollision_AABB(quad.ltPos, quad.rbPos, ltPos, rbPos))
        {
            return;
        }

        if (IsLeaf(quadID))
        {
            quadList.Add(quad);
            return;
        }

        ushort index = (ushort)(4 * quadID);
        GetAABBCollsionQuadList(ltPos, rbPos, quadList, (ushort)(index + 1));
        GetAABBCollsionQuadList(ltPos, rbPos, quadList, (ushort)(index + 2));
        GetAABBCollsionQuadList(ltPos, rbPos, quadList, (ushort)(index + 3));
        GetAABBCollsionQuadList(ltPos, rbPos, quadList, (ushort)(index + 4));
    }

    public void AddUnit(T val)
    {
        // - Create Unit
        Unit<T> unit = new Unit<T>();
        unit.SetValue(val);
        ushort unitID = unitAutoID++;
        unit.SetUnitID(unitID);
        unitDic.Add(unitID, unit);
        unitQuadDic.Add(unitID, new List<Quad<T>>());

        List<Quad<T>> quadList = new List<Quad<T>>();
        GetAABBCollsionQuadList(val, quadList, 0);
        quadList.ForEach((quad) =>
        {
            quadUnitDic.TryGetValue(quad.quadID, out var unitList);
            unitList.Add(unit);
        });
    }

    int GetLayer(int quadID)
    {
        for (int curLayer = 1; curLayer <= maxLayer; curLayer++)
        {
            int n = (int)(AllDigit.Pow(4, curLayer) - 4) / 3;
            if (quadID <= n)
            {
                return curLayer;
            }
        }

        return 0;
    }

    void TickUnitLocation(Unit<T> unit)
    {
        List<Quad<T>> quadList = new List<Quad<T>>();
        GetAABBCollsionQuadList(unit.Value, quadList, 0);
        var uid = unit.UnitID;
        for (int i = 0; i < quadList.Count; i++)
        {
            var quad = quadList[i];
            var qid = quad.quadID;
            quadUnitDic.TryGetValue(qid, out var unitList);
            unitList.Add(unit);
            quadUnitCountDic[qid]++;

            unitQuadDic[uid].Add(quad);
        }
    }

    void TickQuadSplit()
    {
        var keys = quadUnitCountDic.Keys;
        ushort[] keyArray = new ushort[keys.Count];
        keys.CopyTo(keyArray, 0);
        for (int i = 0; i < keyArray.Length; i++)
        {
            var k = keyArray[i];
            var v = quadUnitCountDic[k];
            if (v <= 4)
            {
                continue;
            }

            var quad = quadDic[k];
            TrySplitQuad(quad);
            quadUnitCountDic[k] = 0;
        }
    }

    bool TrySplitQuad(Quad<T> quad)
    {
        var quadQuadID = quad.quadID;
        if (GetLayer(quadQuadID) >= maxLayer)
        {
            // Console.WriteLine($"########## Quad Split Max");
            return false;
        }

        SplitQuad(quad.ltPos, quad.rbPos, quadQuadID);
        return true;
    }

    void SplitQuad(Vector2 ltPos, Vector2 rbPos, ushort quadID)
    {
        var len = (rbPos.X - ltPos.X);
        var halfLen = len / 2f;
        var index = 4 * quadID;

        var qID = (ushort)(index + 1);
        quadDic.TryAdd(qID, new Quad<T> { quadID = qID, ltPos = ltPos, rbPos = ltPos + new Vector2(halfLen, -halfLen) });
        quadUnitCountDic.Add(qID, 0);
        quadUnitDic.TryAdd(qID, new List<Unit<T>>(10));

        qID = (ushort)(index + 2);
        quadDic.TryAdd(qID, new Quad<T> { quadID = qID, ltPos = ltPos + new Vector2(halfLen, 0), rbPos = ltPos + new Vector2(len, -halfLen) });
        quadUnitCountDic.Add(qID, 0);
        quadUnitDic.TryAdd(qID, new List<Unit<T>>(10));

        qID = (ushort)(index + 3);
        quadDic.TryAdd(qID, new Quad<T> { quadID = qID, ltPos = ltPos + new Vector2(0, -halfLen), rbPos = ltPos + new Vector2(halfLen, -len) });
        quadUnitCountDic.Add(qID, 0);
        quadUnitDic.TryAdd(qID, new List<Unit<T>>(10));

        qID = (ushort)(index + 4);
        quadDic.TryAdd(qID, new Quad<T> { quadID = qID, ltPos = ltPos + new Vector2(halfLen, -halfLen), rbPos = ltPos + new Vector2(len, -len) });
        quadUnitCountDic.Add(qID, 0);
        quadUnitDic.TryAdd(qID, new List<Unit<T>>(10));
    }

    bool IsLeaf(ushort quadID)
    {
        ushort qid = (ushort)(quadID * 4);
        return !quadDic.TryGetValue((ushort)(qid + 1), out var quad)
        && !quadDic.TryGetValue((ushort)(qid + 2), out quad)
        && !quadDic.TryGetValue((ushort)(qid + 3), out quad)
        && !quadDic.TryGetValue((ushort)(qid + 4), out quad);
    }

}

public static class QuadTree
{

    public static uint GetQuadUnitKey(ushort quadID, ushort unitID)
    {
        uint key = (uint)quadID << 16;
        key |= (uint)unitID;
        return key;
    }

}
