using System;

namespace Game.Client.Bussiness.BattleBussiness
{

    public struct HitModel
    {
        public IDComponent attackerIDC;
        public IDComponent victimIDC;
        public int damage;
    }

}