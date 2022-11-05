using UnityEngine;
using Game.Client.Bussiness.BattleBussiness.Generic;

namespace Game.Client.Bussiness.BattleBussiness
{

    public class BattleGameFSMComponent
    {

        BattleGameState gameState;
        public BattleGameState GameState => gameState;

        GameStateBattleLoadingMod loadingMod;
        public GameStateBattleLoadingMod LoadingMod => loadingMod;

        GameStateBattlePreparingMod preparingMod;
        public GameStateBattlePreparingMod PreparingMod => preparingMod;

        GameStateBattleFightingMod fightingMod;
        public GameStateBattleFightingMod FightingMod => fightingMod;

        GameStateBattleSettlementMod settlementMod;
        public GameStateBattleSettlementMod SettlementMod => settlementMod;

        public BattleGameFSMComponent()
        {
            loadingMod = new GameStateBattleLoadingMod();
            preparingMod = new GameStateBattlePreparingMod();
            fightingMod = new GameStateBattleFightingMod();
            settlementMod = new GameStateBattleSettlementMod();
        }

        public void EnterGameState_BattleLoading()
        {
            loadingMod.isFirstEnter = true;
            gameState = BattleGameState.Loading;
        }

        public void EnterGameState_BattlePreparing(int maintainFrame)
        {
            preparingMod.isFirstEnter = true;
            preparingMod.maintainFrame = maintainFrame;
            gameState = BattleGameState.Preparing;
        }

        public void EnterGameState_BattleFighting(int maintainFrame)
        {
            fightingMod.isFirstEnter = true;
            fightingMod.maintainFrame = maintainFrame;
            gameState = BattleGameState.Fighting;
        }

        public void EnterGameState_BattleSettlement(int maintainFrame)
        {
            settlementMod.isFirstEnter = true;
            settlementMod.maintainFrame = maintainFrame;
            gameState = BattleGameState.Settlement;
        }

    }

}