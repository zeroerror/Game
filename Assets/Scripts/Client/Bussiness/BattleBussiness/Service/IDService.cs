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

        public void ClearAutoIDByEntityType(EntityType entityType)
        {
            if (entityType == EntityType.BattleRole)
            {
                roleAutoID = 0;
                return;
            }
            if (entityType == EntityType.Bullet)
            {
                bulletAutoID = 0;
                return;
            }
            if (entityType == EntityType.BulletItem)
            {
                bulletItemAutoID = 0;
                return;
            }
            if (entityType == EntityType.Weapon)
            {
                weaponAutoID = 0;
                return;
            }
            if (entityType == EntityType.WeaponItem)
            {
                weaponItemAutoID = 0;
                return;
            }
            if (entityType == EntityType.Armor)
            {
                armorAutoID = 0;
                return;
            }
            if (entityType == EntityType.ArmorItem)
            {
                armorItemAutoID = 0;
                return;
            }
            if (entityType == EntityType.EvolveItem)
            {
                evolveItemAutoID = 0;
                return;
            }

            Debug.LogWarning("未处理情况");
        }
    }

}