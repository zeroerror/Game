namespace Game.Client.Bussiness.BattleBussiness.Generic
{

    public enum BattleStage : int
    {
        None = 0,
        Level1 = 1 << 0,
        Level2 = 1 << 1,
        Level3 = 1 << 2,
        Prepared = 1 << 3,
        GameOver = 1 << 4,
    }

    public static class BattleStageExtensions
    {

        public static BattleStage GetCurLevelStage(this BattleStage self)
        {
            if (self.HasStage(BattleStage.Level3))
            {
                return BattleStage.Level3;
            }
            if (self.HasStage(BattleStage.Level2))
            {
                return BattleStage.Level2;
            }

            return BattleStage.Level1;
        }

        public static bool HasStage(this BattleStage self, BattleStage tar)
        {
            return (self & tar) != 0;
        }

        public static BattleStage AddStage(this BattleStage self, BattleStage tar)
        {
            return self |= tar;
        }

        public static BattleStage RemoveStage(this BattleStage self, BattleStage tar)
        {
            return self & (self ^ tar);
        }

        public static int CompareStage(this BattleStage self, BattleStage tar, BattleStage flag)
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