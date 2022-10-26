using System.Collections.Generic;
using UnityEngine;
using Game.Client.Bussiness.BattleBussiness.Generic;

namespace Game.Client.Bussiness.BattleBussiness
{

    public class IDService
    {

        public int roleAutoID;
        public int bulletAutoID;
        public int bulletPackAutoID;
        public int armorAutoID;
        public int weaponAutoID;

        public IDService()
        {
        }

        public int GetAutoIDByEntityType(EntityType entityType)
        {
            if (entityType == EntityType.BattleRole)
            {
                return ++roleAutoID;
            }
            if (entityType == EntityType.Bullet)
            {
                return ++bulletAutoID;
            }
            if (entityType == EntityType.BulletPack)
            {
                return ++bulletPackAutoID;
            }
            if (entityType == EntityType.Weapon)
            {
                return ++weaponAutoID;
            }
            if (entityType == EntityType.Armor)
            {
                return ++armorAutoID;
            }

            Debug.LogWarning("未处理情况");
            return -1;
        }

    }

}