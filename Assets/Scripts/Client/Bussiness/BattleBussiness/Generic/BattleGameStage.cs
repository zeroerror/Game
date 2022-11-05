namespace Game.Client.Bussiness.BattleBussiness.Generic
{

    public enum BattleGameStage
    {
        None = 0,
        Loaded = 1 << 0,
        Prepared = 1 << 1,
        GameOver = 1 << 2,
    }

    public static class BattleGameStageExtensions
    {

        public static bool HasStage(this BattleGameStage self, BattleGameStage tar)
        {
            return (self & tar) != 0;
        }

        public static void AddStage(this BattleGameStage self, BattleGameStage tar)
        {
            self |= tar;
        }

        public static void RemoveStage(this BattleGameStage self, BattleGameStage tar)
        {
            self = self & (self ^ tar);
        }

        public static int CompareStageFlag(this BattleGameStage self, BattleGameStage tar, BattleGameStage flag)
        {
            if (!self.HasStage(flag) && tar.HasStage(flag))
            {
                // - Off -> On
                return 1;
            }

            if (self.HasStage(flag) && !tar.HasStage(flag))
            {
                // - On -> Off
                return -1;
            }

            // - Same
            return 0;
        }

    }

}