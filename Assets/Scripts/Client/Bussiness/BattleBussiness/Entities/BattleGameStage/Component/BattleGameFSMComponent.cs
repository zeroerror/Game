using UnityEngine;
using Game.Client.Bussiness.BattleBussiness.Generic;

namespace Game.Client.Bussiness.BattleBussiness
{

    public class BattleGameFSMComponent
    {

        BattleState state;
        public BattleState State => state;

        BattleStateLoadingMod loadingMod;
        public BattleStateLoadingMod LoadingMod => loadingMod;

        BattleStatePreparingMod preparingMod;
        public BattleStatePreparingMod PreparingMod => preparingMod;

        BattleStateFightingMod fightingMod;
        public BattleStateFightingMod FightingMod => fightingMod;

        BattleStateSettlementMod settlementMod;
        public BattleStateSettlementMod SettlementMod => settlementMod;

        public BattleGameFSMComponent()
        {
            loadingMod = new BattleStateLoadingMod();
            preparingMod = new BattleStatePreparingMod();
            fightingMod = new BattleStateFightingMod();
            settlementMod = new BattleStateSettlementMod();
        }

        public void EnterGameState_BattleLoading(BattleStage stage)
        {
            loadingMod.isFirstEnter = true;
            loadingMod.stage = stage;
            state = BattleState.Loading;
        }

        public void EnterGameState_BattlePreparing(int maintainFrame)
        {
            preparingMod.isFirstEnter = true;
            preparingMod.maintainFrame = maintainFrame;
            state = BattleState.Preparing;
        }

        public void EnterGameState_BattleFighting(int maintainFrame)
        {
            fightingMod.isFirstEnter = true;
            fightingMod.maintainFrame = maintainFrame;
            state = BattleState.Fighting;
        }

        public void EnterGameState_BattleSettlement(int maintainFrame)
        {
            settlementMod.isFirstEnter = true;
            settlementMod.maintainFrame = maintainFrame;
            state = BattleState.Settlement;
        }

    }

}