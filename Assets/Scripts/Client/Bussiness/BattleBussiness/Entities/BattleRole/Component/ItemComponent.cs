using System.Collections.Generic;
using UnityEngine;
using Game.Client.Bussiness.BattleBussiness.Generic;

namespace Game.Client.Bussiness.BattleBussiness
{

    public class ItemComponent
    {

        readonly int CAPACITY = 9999999;

        public int takenCapacity;

        List<BulletItemEntity> bulletItemList;

        public ItemComponent()
        {
            bulletItemList = new List<BulletItemEntity>();
        }

        public void Reset()
        {
            bulletItemList.Clear();
            takenCapacity = 0;
        }

        // 拾取
        public bool TryCollectItem_Bullet(BulletItemEntity bulletItem)
        {
            // TODO: 根据配置表获取物件单位占用容量
            // TODO: 只收集最大可收集数量
            if (takenCapacity >= CAPACITY)
            {
                Debug.LogWarning("背包容量已满！ 暂未作处理");
                return false;
            }

            takenCapacity += bulletItem.bulletNum * 1;
            Debug.LogWarning($"收集了子弹包[类型:{bulletItem.BulletType.ToString()} 数量:{bulletItem.bulletNum}]");
            Debug.LogWarning($"当前背包容量 占用 {takenCapacity}  剩余 {CAPACITY - takenCapacity}");
            bulletItemList.Add(bulletItem);
            return true;
        }

        // 使用
        public int TakeOutItem_Bullet(BulletType bulletType, int num)
        {
            for (int i = 0; i < bulletItemList.Count; i++)
            {
                var bulletItem = bulletItemList[i];
                if (bulletItem.BulletType != bulletType)
                {
                    continue;
                }

                if (bulletItem.bulletNum >= num)
                {
                    bulletItem.bulletNum -= num;
                    return num;
                }
                else if (bulletItem.bulletNum > 0)
                {
                    int realTakeOut = bulletItem.bulletNum;
                    bulletItem.bulletNum = 0;
                    bulletItemList.Remove(bulletItem);
                    return realTakeOut + TakeOutItem_Bullet(bulletType, num - realTakeOut);
                }
                else
                {
                    GameObject.Destroy(bulletItem);
                    bulletItemList.Remove(bulletItem);
                    return TakeOutItem_Bullet(bulletType, num);
                }
            }

            return 0;
        }

        public bool HasItem_Bullet(BulletType bulletType, int num)
        {
            int total = 0;
            var e = bulletItemList.GetEnumerator();
            while (e.MoveNext())
            {
                var bulletPackEntity = e.Current;
                if (bulletType != bulletPackEntity.BulletType)
                {
                    continue;
                }

                total += bulletPackEntity.bulletNum;
                if (total >= num)
                {
                    return true;
                }
            }

            return false;
        }

        //丢弃
        public List<IDComponent> DropItems(EntityType entityType, int num)
        {
            List<IDComponent> itemList = new List<IDComponent>();
            switch (entityType)
            {
                case EntityType.BulletItem:
                    for (int i = 0; i < bulletItemList.Count; i++)
                    {
                        if (num <= i)
                        {
                            break;
                        }

                        itemList.Add(itemList[i]);
                    }

                    break;
                case EntityType.Armor:
                    break;
            }

            return itemList;
        }

    }

}