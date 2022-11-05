using Game.Client.Bussiness.BattleBussiness.Generic;

namespace Game.Client.Bussiness.BattleBussiness
{

    public class BattleGameEntity
    {

        BattleGameStage gameStage;
        public BattleGameStage GameStage => gameStage;
        public void SetGameStageFlag(BattleGameStage v) => gameStage = v;

        public BattleGameFSMComponent FSMComponent { get; private set; }
        public BattleGameEntity()
        {
            FSMComponent = new BattleGameFSMComponent();
        }

    }

}