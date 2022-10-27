using Game.Client.Bussiness.BattleBussiness.Generic;

namespace Game.Client.Bussiness.BattleBussiness.Interface
{

    public interface IPickable
    {

        EntityType EntityType { get; }
        int EntityID { get; }

    }

}