using Game.Client.Bussiness.WorldBussiness.Generic;
using UnityEngine;

namespace Game.Client.Bussiness.WorldBussiness.Interface
{

    public interface IPickable
    {
        ItemType ItemType { get; }

        ushort EntityId { get; }

        byte MasterId { get; }

    }


}