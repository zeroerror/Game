using Game.Client.Bussiness.BattleBussiness.Generic;

namespace Game.Client.Bussiness.BattleBussiness
{

    public class BattleGameEntity
    {

        BattleGameStage clientStage;
        public BattleGameStage ClientStage => clientStage;
        public void SetClientStage(BattleGameStage v) => clientStage = v;

        BattleGameStage serStage;
        public BattleGameStage SerStage => serStage;
        public void SetSerStage(BattleGameStage v) => serStage = v;

        public BattleGameFSMComponent FSMComponent { get; private set; }
        public BattleGameEntity()
        {
            FSMComponent = new BattleGameFSMComponent();
        }

    }

}