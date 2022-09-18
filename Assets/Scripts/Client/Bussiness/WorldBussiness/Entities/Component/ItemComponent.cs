using System.Collections.Generic;
using Game.Client.Bussiness.WorldBussiness.Interface;
using UnityEngine;

namespace Game.Client.Bussiness.WorldBussiness
{

    public class ItemComponent
    {

        readonly int ITEM_CAPACITY = 300;

        public int CurrentCapacity;    //当前武器数量

        Queue<BulletEntity> bulletItemQueue;

        public ItemComponent()
        {
        }

        public void Ctor()
        {
            bulletItemQueue = new Queue<BulletEntity>();
        }

        // 拾取
        public void TryCollectItem_Bullet(BulletEntity bulletEntity)
        {
            Debug.Log($"收集了子弹");
            bulletItemQueue.Enqueue(bulletEntity);
        }

        // 使用
        public bool TryTakeOutItem_Bullet(out BulletEntity bulletEntity)
        {
            bulletEntity = bulletItemQueue.Dequeue();
            return bulletEntity != null;
        }

        //丢弃
        public IPickable[] DropItems(ItemType itemtype, int num)
        {
            IPickable[] items = new IPickable[num];
            switch (itemtype)
            {
                case ItemType.Default:
                    break;
                case ItemType.Bullet:
                    for (int i = 0; i < num; i++)
                    {
                        items[i] = bulletItemQueue.Dequeue();
                    }
                    break;
                case ItemType.Pill:
                    break;
            }

            return items;
        }

    }

}