using UnityEngine;

namespace Game.Client.Bussiness.WorldBussiness.Interface
{

    public enum ItemType
    {
        Default,
        Weapon,
        Bullet,
        Pill    // heal or speedup ,etc.
    }

    public interface IPickable
    {
        ItemType ItemType { get; }
        void SetItemType(ItemType itemType);

        ushort EntityId { get; }
        void SetEntityId(ushort entityId);

        byte MasterId { get; }
        void SetMasterId(byte masterWRid);

    }


}