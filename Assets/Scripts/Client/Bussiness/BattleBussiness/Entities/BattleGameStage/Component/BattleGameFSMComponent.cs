using System.Collections.Generic;
using Game.Client.Bussiness.BattleBussiness.Generic;

namespace Game.Client.Bussiness.BattleBussiness
{

    public class BattleGameFSMComponent
    {

        BattleState battleState;
        public BattleState BattleState => battleState;

        BattleStateSpawningFieldMod spawningFieldMod;
        public BattleStateSpawningFieldMod SpawningFieldMod => spawningFieldMod;

        BattleStatePreparingMod preparingMod;
        public BattleStatePreparingMod PreparingMod => preparingMod;

        BattleStateFightingMod fightingMod;
        public BattleStateFightingMod FightingMod => fightingMod;

        BattleStateSettlementMod settlementMod;
        public BattleStateSettlementMod SettlementMod => settlementMod;

        public BattleGameFSMComponent()
        {
            spawningFieldMod = new BattleStateSpawningFieldMod();
            preparingMod = new BattleStatePreparingMod();
            fightingMod = new BattleStateFightingMod();
            settlementMod = new BattleStateSettlementMod();
        }

        public void EnterGameState_BattleSpawningField(BattleStage stage)
        {
            spawningFieldMod.isFirstEnter = true;
            spawningFieldMod.stage = stage;
            battleState = BattleState.SpawningField;
        }

        public void EnterGameState_BattlePreparing(int maintainFrame)
        {
            preparingMod.isFirstEnter = true;
            preparingMod.maintainFrame = maintainFrame;
            battleState = BattleState.Preparing;
        }

        public void EnterGameState_BattleFighting(int maintainFrame)
        {
            fightingMod.isFirstEnter = true;
            fightingMod.maintainFrame = maintainFrame;
            battleState = BattleState.Fighting;
        }

        public void EnterGameState_BattleSettlement(int maintainFrame)
        {
            settlementMod.isFirstEnter = true;
            settlementMod.maintainFrame = maintainFrame;
            battleState = BattleState.Settlement;
        }

    }

}