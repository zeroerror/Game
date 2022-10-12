using Game.Client.Bussiness.BattleBussiness.Generic;
using UnityEngine;

namespace Game.Client.Bussiness.BattleBussiness.Interface
{

    public interface IPickable
    {
        ItemType ItemType { get; }

        int EntityId { get; }

        int MasterId { get; }

    }


}