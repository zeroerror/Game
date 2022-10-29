using System.Collections.Generic;
using UnityEngine;
using Game.Client.Bussiness.BattleBussiness.Generic;

namespace Game.Client.Bussiness.BattleBussiness
{

    public class ItemComponent
    {

        readonly int ITEM_CAPACITY = 9999999;

        public int CurrentCapacity;    //当前武器数量

        List<BulletItemEntity> bulletItemList;

        public ItemComponent()
        {
            bulletItemList = new List<BulletItemEntity>();
        }

        // 拾取
        public void TryCollectItem_Bullet(BulletItemEntity bulletItemEntity)
        {
            // TODO: 根据配置表获取物件单位占用容量
            // TODO: 只收集最大可收集数量
            if (CurrentCapacity >= ITEM_CAPACITY)
            {
                Debug.LogWarning("背包容量已满！ 暂未作处理");
            }

            CurrentCapacity += bulletItemEntity.bulletNum * 1;
            Debug.LogWarning($"收集了子弹包[类型:{bulletItemEntity.bulletType.ToString()} 数量:{bulletItemEntity.bulletNum}]");
            Debug.LogWarning($"当前背包容量 占用 {CurrentCapacity}  剩余 {ITEM_CAPACITY - CurrentCapacity}");
            bulletItemList.Add(bulletItemEntity);
        }

        // 使用
        public int TakeOutItem_Bullet(BulletType bulletType, int num)
        {
            for (int i = 0; i < bulletItemList.Count; i++)
            {
                var bulletItem = bulletItemList[i];
                Debug.Log($"bulletType {bulletType} num {num} ----  bulletItem {bulletItem.bulletType} {bulletItem.bulletNum}");
                if (bulletItem.bulletType != bulletType)
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

        public bool HasItem_Bullet(int num)
        {
            int total = 0;
            var e = bulletItemList.GetEnumerator();
            while (e.MoveNext())
            {

                var bulletPackEntity = e.Current;
                total += bulletPackEntity.bulletNum;
                if (total >= num) return true;
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