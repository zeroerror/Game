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

    public class Pickable : MonoBehaviour
    {
        public ItemType ItemType { get; private set; }
        protected void SetItemType(ItemType itemType) => ItemType = itemType;

        public ushort EntityId { get; private set; }
        protected void SetEntityId(ushort entityId) => EntityId = entityId;

        public byte MasterWRid { get; private set; }
        protected void SetMasterWRid(byte masterWRid) => MasterWRid = masterWRid;

    }


}