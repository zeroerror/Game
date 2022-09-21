using Game.Client.Bussiness.BattleBussiness.Generic;
using UnityEngine;

namespace Game.Client.Bussiness.BattleBussiness.Interface
{

    public interface IPickable
    {
        ItemType ItemType { get; }

        ushort EntityId { get; }

        byte MasterId { get; }

    }


}