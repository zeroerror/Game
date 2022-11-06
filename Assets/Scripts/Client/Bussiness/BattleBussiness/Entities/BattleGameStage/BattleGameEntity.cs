using Game.Client.Bussiness.BattleBussiness.Generic;

namespace Game.Client.Bussiness.BattleBussiness
{

    public class BattleGameEntity
    {

        BattleStage stage;
        public BattleStage Stage => stage;

        public BattleGameFSMComponent FSMComponent { get; private set; }

        public BattleGameEntity()
        {
            FSMComponent = new BattleGameFSMComponent();
        }

        public void AddStage(BattleStage v)
        {
            stage = stage.AddStage(v);
        }

        public void RemoveStage(BattleStage v)
        {
            stage = stage.RemoveStage(v);
        }

    }

}