using System;
using System.Collections.Generic;
using System.Numerics;
using ZeroFrame.AllMath;
using ZeroFrame.ZeroPhysics;

public interface IBounds
{
    Vector2 LTPos { get; }
    Vector2 RBPos { get; }
}

public class Quad<T> where T : IBounds
{

    public ushort nodeIndex;
    public ushort quadIndex;
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
    public Dictionary<uint, Quad<T>> quadDic;
    public Dictionary<ulong, Unit<T>> unitDic;
    Unit<T>[] unitArray;
    int unitCount;

    ushort unitAutoID;

    float sideLen;  // - 矩形边长
    int maxLayer;   // - 最大层数
    int quadUnitLimit;   // - Quad最大包含Unit数量
    public Vector2 rootPos;    // - 根坐标
    public int curLayer;
    public int maxNode;

    public QuadTree(float sideLen, int maxLayer, Vector2 rootPos, int quadUnitLimit = 4, int maxUnit = 1000)
    {
        maxLayer = maxLayer > 8 ? 8 : maxLayer;
        this.sideLen = sideLen;
        this.maxLayer = maxLayer;
        this.curLayer = 0;
        this.quadUnitLimit = quadUnitLimit;
        this.rootPos = rootPos;
        this.maxNode = (int)(AllDigit.Pow(4, maxLayer) - 4) / 3 + 1;
        Console.WriteLine($"Tree MaxNode: {maxNode}");

        quadDic = new Dictionary<uint, Quad<T>>();
        unitDic = new Dictionary<ulong, Unit<T>>();
        unitArray = new Unit<T>[maxUnit];
        unitAutoID = 0;

        var quad = new Quad<T>();
        var halfLen = sideLen / 2f;
        quad.nodeIndex = 0;
        quad.quadIndex = 0;
        quad.ltPos = new Vector2(rootPos.X - halfLen, rootPos.Y + halfLen);
        quad.rbPos = new Vector2(rootPos.X + halfLen, rootPos.Y - halfLen);
        quadDic.TryAdd(QuadTree.GetLocationKey(0, 0), quad);
    }

    public void Tick()
    {
        var values = unitDic.Values;
        Unit<T>[] unitArray = new Unit<T>[values.Count];
        values.CopyTo(unitArray, 0);
        for (int i = 0; i < unitArray.Length; i++)
        {
            var unit = unitArray[i];
            RemoveUnit(unit);
            TickUnitLocation(unit);
        }
    }

    public void Clear()
    {
        curLayer = 0;
    }

    public void ForeachUnit(Action<Unit<T>> action)
    {
        for (int i = 0; i < unitCount; i++)
        {
            action.Invoke(unitArray[i]);
        }
    }

    // 获取坐标区域内所有单位

    /// 获取Unit产生的所有AABB碰撞的Quad
    /// </summary>
    /// <param name="bounds"></param>
    /// <returns></returns>
    public void GetAABBCollsionQuadList(IBounds bounds, List<Quad<T>> quadList, int nodeIndex)
    {
        GetAABBCollsionQuadList(bounds.LTPos, bounds.RBPos, quadList, 0);
    }

    public void GetAABBCollsionQuadList(in Vector2 ltPos, in Vector2 rbPos, List<Quad<T>> quadList, ushort nodeIndex)
    {
        if (nodeIndex < 0 || nodeIndex >= maxNode)
        {
            return;
        }

        ushort quadIndex = (ushort)(nodeIndex % 4);
        if (quadIndex == 0 && nodeIndex != 0)
        {
            quadIndex = 4;
        }

        var fatherNodeIndex = (ushort)((nodeIndex - quadIndex) / 4);
        var key = QuadTree.GetLocationKey(fatherNodeIndex, quadIndex);

        if (!quadDic.TryGetValue(key, out var quad))
        {
            return;
        }

        if (!CollisionHelper.HasAABBCollision(quad.ltPos, quad.rbPos, ltPos, rbPos))
        {
            return;
        }

        if (IsLeaf(nodeIndex))
        {
            quadList.Add(quad);
            return;
        }

        ushort index = (ushort)(4 * nodeIndex);
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

        List<Quad<T>> quadList = new List<Quad<T>>();
        GetAABBCollsionQuadList(val, quadList, 0);

        if (quadList.Count != 0)
        {
            unitArray[unitCount++] = unit;
        }

        quadList.ForEach((quad) =>
        {
            var nodeIndex = quad.nodeIndex;
            var quadIndex = quad.quadIndex;

            var unitKey = QuadTree.GetUnitKey(nodeIndex, quadIndex, unitID);

            // - Check Limit
            var locationKey = QuadTree.GetLocationKey(nodeIndex, quadIndex);
            var unitList = GetUnitList(locationKey);
            if (unitList.Count >= quadUnitLimit && TrySplitQuad(quad))
            {
                Console.WriteLine($"------------------  Reach Quad's Unit Limit.");
                unitList.ForEach((u) =>
                {
                    unitKey = QuadTree.GetUnitKey(nodeIndex, quadIndex, u.UnitID);
                    unitDic.Remove(unitKey);
                    TickUnitLocation(u);
                });
                TickUnitLocation(unit);
                return;
            }

            // - Normal Add
            unitDic.TryAdd(unitKey, unit);
            Console.WriteLine($"------------------ Add Unit: nodeIndex {nodeIndex} quadIndex {quadIndex}");
        });
    }

    public List<Unit<T>> GetUnitList(uint locationKey)
    {
        var unitList = new List<Unit<T>>();
        var e = unitDic.Keys.GetEnumerator();
        while (e.MoveNext())
        {
            ulong unitKey = e.Current;
            var locKey = unitKey >> 32;
            if (locKey == locationKey)
            {
                unitList.Add(unitDic[unitKey]);
            }
        }
        return unitList;
    }

    public List<ulong> GetUnitKeyList(uint locationKey)
    {
        var unitKeyList = new List<ulong>();
        var e = unitDic.Keys.GetEnumerator();
        while (e.MoveNext())
        {
            ulong unitKey = e.Current;
            var locKey = unitKey >> 32;
            if (locKey == locationKey)
            {
                unitKeyList.Add(unitKey);
            }
        }
        return unitKeyList;
    }

    int GetLayer(int nodeIndex)
    {
        for (int curLayer = 1; curLayer <= maxLayer; curLayer++)
        {
            int n = (int)(AllDigit.Pow(4, curLayer) - 4) / 3;
            if (nodeIndex <= n)
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
        quadList.ForEach((quad) =>
        {
            var nodeIndex = quad.nodeIndex;
            var quadIndex = quad.quadIndex;

            // - Check Limit Again
            var locationKey = QuadTree.GetLocationKey(nodeIndex, quadIndex);
            var unitList = GetUnitList(locationKey);
            if (unitList.Count >= quadUnitLimit && TrySplitQuad(quad))
            {
                Console.WriteLine($"------------------  Reach Quad's Unit Limit.");
                unitList.ForEach((u) =>
                {
                    var unitKey = QuadTree.GetUnitKey(nodeIndex, quadIndex, u.UnitID);
                    unitDic.Remove(unitKey);
                    TickUnitLocation(u);
                });
                TickUnitLocation(unit);
                return;
            }

            // - Final Set
            var newUnitKey = QuadTree.GetUnitKey(nodeIndex, quadIndex, unit.UnitID);
            unitDic.TryAdd(newUnitKey, unit);
            Console.WriteLine($"------------------ TickUnitLocation[{unit.UnitID}]: nodeIndex {nodeIndex} quadIndex {quadIndex}");
        });
    }

    void RemoveUnit(Unit<T> unit)
    {
        var unitID = unit.UnitID;
        var keys = unitDic.Keys;
        var array = new ulong[keys.Count];
        keys.CopyTo(array, 0);
        for (int i = 0; i < array.Length; i++)
        {
            var key = array[i];
            var uid = (ushort)key;
            if (uid == unitID)
            {
                unitDic.Remove(key);
            }
        }
    }

    bool TrySplitQuad(Quad<T> quad)
    {
        var quadNodeIndex = quad.nodeIndex;
        if (GetLayer(quadNodeIndex) >= maxLayer)
        {
            Console.WriteLine($"########## Quad Split Max");
            return false;
        }

        SplitQuad(quad.ltPos, quad.rbPos, quadNodeIndex);
        return true;
    }

    void SplitQuad(Vector2 ltPos, Vector2 rbPos, ushort nodeIndex)
    {
        var len = (rbPos.X - ltPos.X);
        var halfLen = len / 2f;
        var index = 4 * nodeIndex;

        var key = QuadTree.GetLocationKey(nodeIndex, 1);
        quadDic.TryAdd(key, new Quad<T> { nodeIndex = (ushort)(index + 1), quadIndex = 1, ltPos = ltPos, rbPos = ltPos + new Vector2(halfLen, -halfLen) });

        key = QuadTree.GetLocationKey(nodeIndex, 2);
        quadDic.TryAdd(key, new Quad<T> { nodeIndex = (ushort)(index + 2), quadIndex = 2, ltPos = ltPos + new Vector2(halfLen, 0), rbPos = ltPos + new Vector2(len, -halfLen) });

        key = QuadTree.GetLocationKey(nodeIndex, 3);
        quadDic.TryAdd(key, new Quad<T> { nodeIndex = (ushort)(index + 3), quadIndex = 3, ltPos = ltPos + new Vector2(0, -halfLen), rbPos = ltPos + new Vector2(halfLen, -len) });

        key = QuadTree.GetLocationKey(nodeIndex, 4);
        quadDic.TryAdd(key, new Quad<T> { nodeIndex = (ushort)(index + 4), quadIndex = 4, ltPos = ltPos + new Vector2(halfLen, -halfLen), rbPos = ltPos + new Vector2(len, -len) });
    }

    bool IsLeaf(ushort nodeIndex)
    {
        return !quadDic.TryGetValue(QuadTree.GetLocationKey(nodeIndex, 1), out var quad)
        && !quadDic.TryGetValue(QuadTree.GetLocationKey(nodeIndex, 2), out quad)
        && !quadDic.TryGetValue(QuadTree.GetLocationKey(nodeIndex, 3), out quad)
        && !quadDic.TryGetValue(QuadTree.GetLocationKey(nodeIndex, 4), out quad);
    }

}

public static class QuadTree
{

    public static ulong GetUnitKey(ushort nodeIndex, ushort quadIndex, ushort unitID)
    {
        ulong key = (ulong)nodeIndex << 48;
        key |= (ulong)quadIndex << 32;
        key |= (ulong)unitID;
        return key;
    }

    public static uint GetLocationKey(ushort nodeIndex, ushort quadIndex)
    {
        uint key = (uint)nodeIndex << 16;
        key |= quadIndex;
        return key;
    }

}
