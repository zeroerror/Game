using System;
using UnityEngine;
using Game.Client.Bussiness.BattleBussiness.Facades;
using Game.Client.Bussiness.BattleBussiness.Generic;

namespace Game.Client.Bussiness.BattleBussiness.Controller.Domain
{

    public class BattleGameStateDomain
    {
        BattleFacades battleFacades;

        public Action gameStateChangeHandler;

        public BattleGameStateDomain()
        {
        }

        public void Inject(BattleFacades battleFacades)
        {
            this.battleFacades = battleFacades;
        }

        public void ApplyGameState()
        {
            ApplyGameState_Any();
            ApplyGameState_Loading();
            ApplyGameState_Preparing();
            ApplyGameState_Fighting();
            ApplyGameState_BattleSettlement();
        }

        void ApplyGameState_Any()
        {
        }

        void ApplyGameState_Loading()
        {
            var gameEntity = battleFacades.GameEntity;
            var fsm = gameEntity.FSMComponent;
            var gameState = fsm.GameState;

            if (gameState != BattleGameState.Loading)
            {
                return;
            }

            var stateMod = fsm.LoadingMod;
            if (stateMod.isFirstEnter)
            {
                stateMod.isFirstEnter = false;
                var fieldDomain = battleFacades.Domain.FieldDomain;
                fieldDomain.SpawBattleScene();
                Debug.Log($"进入 加载阶段");
            }

            var field = battleFacades.Repo.FiledRepo.CurFieldEntity;
            if (field != null)
            {
                // - Stage
                var gameStage = gameEntity.ClientStage;
                gameStage.AddStage(BattleGameStage.Loaded);

                // State
                fsm.EnterGameState_BattlePreparing(300);
                gameStateChangeHandler.Invoke();
            }
        }

        void ApplyGameState_Preparing()
        {
            var gameEntity = battleFacades.GameEntity;
            var fsm = gameEntity.FSMComponent;
            var gameState = fsm.GameState;

            if (gameState != BattleGameState.Preparing)
            {
                return;
            }

            var stateMod = fsm.PreparingMod;
            if (stateMod.isFirstEnter)
            {
                stateMod.isFirstEnter = false;
                Debug.Log($"进入 准备阶段 持续 {stateMod.maintainFrame} 帧");
            }

            if (stateMod.maintainFrame > 0)
            {
                stateMod.maintainFrame--;
            }
            else
            {
                // - Stage
                var gameStage = gameEntity.ClientStage;
                gameStage.AddStage(BattleGameStage.Prepared);

                // - State 
                fsm.EnterGameState_BattleFighting(300);
                gameStateChangeHandler.Invoke();
            }

        }

        void ApplyGameState_Fighting()
        {
            var gameEntity = battleFacades.GameEntity;
            var fsm = gameEntity.FSMComponent;
            var gameState = fsm.GameState;

            if (gameState != BattleGameState.Fighting)
            {
                return;
            }

            var stateMod = fsm.FightingMod;
            if (stateMod.isFirstEnter)
            {
                stateMod.isFirstEnter = false;
                Debug.Log($"进入 战斗阶段 持续 {stateMod.maintainFrame} 帧");
            }

            if (stateMod.maintainFrame > 0)
            {
                stateMod.maintainFrame--;
            }
            else
            {
                // - Stage
                var gameStage = gameEntity.ClientStage;
                gameStage.AddStage(BattleGameStage.GameOver);

                // - State 
                fsm.EnterGameState_BattleSettlement(150);
                gameStateChangeHandler.Invoke();
            }
        }

        void ApplyGameState_BattleSettlement()
        {
            var gameEntity = battleFacades.GameEntity;
            var fsm = gameEntity.FSMComponent;
            var gameState = fsm.GameState;

            if (gameState != BattleGameState.Settlement)
            {
                return;
            }

            var stateMod = fsm.SettlementMod;
            if (stateMod.isFirstEnter)
            {
                stateMod.isFirstEnter = false;
                Debug.Log($"进入战斗结算阶段 持续 {stateMod.maintainFrame} 帧");
            }

            if (stateMod.maintainFrame > 0)
            {
                stateMod.maintainFrame--;
            }
            else
            {
                // - Stage
                var gameStage = gameEntity.ClientStage;
                gameStage.RemoveStage(BattleGameStage.GameOver);

                // - State 
                fsm.EnterGameState_BattlePreparing(300);
                gameStateChangeHandler.Invoke();
            }
        }

    }

}