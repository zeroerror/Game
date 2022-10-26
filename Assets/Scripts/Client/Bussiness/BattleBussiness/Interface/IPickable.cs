using Game.Client.Bussiness.BattleBussiness.Generic;
using UnityEngine;

namespace Game.Client.Bussiness.BattleBussiness.Interface
{

    public interface IPickable
    {
        EntityType EntityType { get; }

        int EntityID { get; }

        int MasterId { get; }

    }


}