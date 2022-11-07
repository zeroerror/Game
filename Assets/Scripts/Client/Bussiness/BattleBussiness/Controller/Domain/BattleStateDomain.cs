using System;
using UnityEngine;
using Game.Client.Bussiness.BattleBussiness.Facades;
using Game.Client.Bussiness.BattleBussiness.Generic;

namespace Game.Client.Bussiness.BattleBussiness.Controller.Domain
{

    public class BattleStateDomain
    {
        BattleFacades battleFacades;

        Action stateAndStageChangeHandler;
        public void RegistStateAndStageChangeHandler(Action action) => stateAndStageChangeHandler += action;

        Action airDropHandler;
        public void RegistAirDropHandler(Action action) => airDropHandler += action;

        public BattleStateDomain()
        {
        }

        public void Inject(BattleFacades battleFacades)
        {
            this.battleFacades = battleFacades;
        }

        public void ApplyGameState()
        {
            ApplyGameState_Any();
            ApplyGameState_SpawningField();
            ApplyGameState_Preparing();
            ApplyGameState_Fighting();
            ApplyGameState_BattleSettlement();
        }

        public int GetCurMainTainFrame()
        {
            var gameEntity = battleFacades.GameEntity;
            var fsm = gameEntity.FSMComponent;
            var state = fsm.BattleState;
            int curMaintainFrame = 0;
            if (state == BattleState.Preparing)
            {
                curMaintainFrame = fsm.PreparingMod.maintainFrame;
            }
            else if (state == BattleState.Fighting)
            {
                curMaintainFrame = fsm.FightingMod.maintainFrame;
            }
            else if (state == BattleState.Settlement)
            {
                curMaintainFrame = fsm.SettlementMod.maintainFrame;
            }

            return curMaintainFrame;
        }

        void ApplyGameState_Any()
        {
        }

        void ApplyGameState_SpawningField()
        {
            var gameEntity = battleFacades.GameEntity;
            var fsm = gameEntity.FSMComponent;
            var battleState = fsm.BattleState;

            if (battleState != BattleState.SpawningField)
            {
                return;
            }

            var stateMod = fsm.SpawningFieldMod;
            var stage = stateMod.stage;
            if (stateMod.isFirstEnter)
            {
                stateMod.isFirstEnter = false;

                // - 场景加载
                if (!gameEntity.Stage.HasStage(stage))
                {
                    var domain = battleFacades.Domain;
                    var fieldDomain = domain.FieldDomain;
                    var fieldName = stage.ToString().ToLower();
                    fieldDomain.SpawBattleField(fieldName);
                }

                Debug.Log($"进入 加载阶段 {stage}");
            }

            var field = battleFacades.Repo.FieldRepo.CurFieldEntity;
            if (field != null)
            {
                // --- 清场
                var domain = battleFacades.Domain;
                var commonDomain = domain.CommonDomain;
                commonDomain.ClearBattleField();

                // - Stage
                gameEntity.AddStage(stage);

                // State
                fsm.EnterGameState_BattlePreparing(300);
                stateAndStageChangeHandler.Invoke();
            }
        }

        void ApplyGameState_Preparing()
        {
            var gameEntity = battleFacades.GameEntity;
            var fsm = gameEntity.FSMComponent;
            var gameState = fsm.BattleState;

            if (gameState != BattleState.Preparing)
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
                gameEntity.AddStage(BattleStage.Prepared);

                // - State 
                fsm.EnterGameState_BattleFighting(18000);
                stateAndStageChangeHandler.Invoke();
            }

        }

        void ApplyGameState_Fighting()
        {
            var gameEntity = battleFacades.GameEntity;
            var fsm = gameEntity.FSMComponent;
            var gameState = fsm.BattleState;

            if (gameState != BattleState.Fighting)
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
                // - Loop
                stateMod.maintainFrame--;
                if (stateMod.maintainFrame % 300 == 0)
                {
                    airDropHandler?.Invoke();
                }
            }
            else
            {
                // - Stage
                gameEntity.AddStage(BattleStage.GameOver);

                // - State 
                fsm.EnterGameState_BattleSettlement(150);
                stateAndStageChangeHandler.Invoke();
            }
        }

        void ApplyGameState_BattleSettlement()
        {
            var gameEntity = battleFacades.GameEntity;
            var fsm = gameEntity.FSMComponent;
            var gameState = fsm.BattleState;

            if (gameState != BattleState.Settlement)
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
                gameEntity.RemoveStage(BattleStage.GameOver);

                // - State 
                fsm.EnterGameState_BattleSpawningField(BattleStage.Level1);
                stateAndStageChangeHandler.Invoke();
            }
        }

    }

}