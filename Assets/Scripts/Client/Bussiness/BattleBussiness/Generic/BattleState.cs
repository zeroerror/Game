namespace Game.Client.Bussiness.BattleBussiness.Generic
{

    public enum BattleState : byte
    {
        None,
        SpawningField,
        SpawningItem,
        Preparing,
        Fighting,
        Settlement
    }

    public static class BattleStateExtensions
    {
        public static bool CanBattleLoop(this BattleState state)
        {
            return state != BattleState.None && state != BattleState.SpawningField;
        }
    }


}