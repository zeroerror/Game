using System.Collections.Generic;
using UnityEngine;
using Game.Client.Bussiness.BattleBussiness.Generic;
using Game.Client.Bussiness.BattleBussiness.Interface;

namespace Game.Client.Bussiness.BattleBussiness
{

    public class ItemComponent
    {

        readonly int ITEM_CAPACITY = 300;

        public int CurrentCapacity;    //当前武器数量

        Queue<BulletPackEntity> bulletPackItemQueue;

        public ItemComponent()
        {
        }

        public void Ctor()
        {
            bulletPackItemQueue = new Queue<BulletPackEntity>();
        }

        // 拾取
        public void TryCollectItem_Bullet(BulletPackEntity bulletPackEntity)
        {
            // TODO: 根据配置表获取物件单位占用容量
            // TODO: 只收集最大可收集数量
            if (CurrentCapacity >= 300)
            {
                Debug.LogWarning("背包容量已满！");
            }

            CurrentCapacity += bulletPackEntity.bulletNum * 1;
            Debug.Log($"收集了子弹包[类型:{bulletPackEntity.bulletType.ToString()} 数量:{bulletPackEntity.bulletNum}]");
            Debug.Log($"当前背包容量:{CurrentCapacity}");
            bulletPackItemQueue.Enqueue(bulletPackEntity);
        }

        // 使用
        public int TryTakeOutItem_Bullet(int num)
        {
            if (bulletPackItemQueue.TryPeek(out var bulletPackEntity))
            {
                if (bulletPackEntity.bulletNum >= num)
                {
                    bulletPackEntity.bulletNum -= num;
                    return num;
                }
                else if (bulletPackEntity.bulletNum > 0)
                {
                    int realTakeOut = bulletPackEntity.bulletNum;
                    bulletPackEntity.bulletNum = 0;
                    bulletPackItemQueue.Dequeue();
                    return realTakeOut;
                }
                else
                {
                    GameObject.Destroy(bulletPackEntity);
                    bulletPackItemQueue.Dequeue();
                    return TryTakeOutItem_Bullet(num);
                }
            }

            return 0;
        }

        public bool HasItem_Bullet(int num)
        {
            int total = 0;
            var e = bulletPackItemQueue.GetEnumerator();
            while (e.MoveNext())
            {

                var bulletPackEntity = e.Current;
                total += bulletPackEntity.bulletNum;
                if (total >= num) return true;
            }

            return false;
        }

        //丢弃
        public IPickable[] DropItems(ItemType itemtype, int num)
        {
            IPickable[] items = new IPickable[num];
            switch (itemtype)
            {
                case ItemType.Default:
                    break;
                case ItemType.BulletPack:
                    for (int i = 0; i < num; i++)
                    {
                        items[i] = bulletPackItemQueue.Dequeue();
                    }
                    break;
                case ItemType.Pill:
                    break;
            }

            return items;
        }

    }

}