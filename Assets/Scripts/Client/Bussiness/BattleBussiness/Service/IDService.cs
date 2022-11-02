using System.Collections.Generic;
using UnityEngine;
using Game.Client.Bussiness.BattleBussiness.Generic;

namespace Game.Client.Bussiness.BattleBussiness
{

    public class IDService
    {

        public int roleAutoID;
        public int bulletAutoID;
        public int bulletItemAutoID;
        public int armorAutoID;
        public int armorItemAutoID;
        public int weaponAutoID;
        public int weaponItemAutoID;
        public int evolveItemAutoID;
        
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
            if (entityType == EntityType.BulletItem)
            {
                return ++bulletItemAutoID;
            }
            if (entityType == EntityType.Weapon)
            {
                return ++weaponAutoID;
            }
            if (entityType == EntityType.WeaponItem)
            {
                return ++weaponItemAutoID;
            }
            if (entityType == EntityType.Armor)
            {
                return ++armorAutoID;
            }
            if (entityType == EntityType.ArmorItem)
            {
                return ++armorItemAutoID;
            }
            if (entityType == EntityType.EvolveItem)
            {
                return ++evolveItemAutoID;
            }

            Debug.LogWarning("未处理情况");
            return -1;
        }

    }

}