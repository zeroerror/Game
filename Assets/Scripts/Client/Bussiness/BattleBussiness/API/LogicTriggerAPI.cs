using System;

namespace Game.Client.Bussiness.BattleBussiness.API
{

    public class LogicTriggerAPI
    {

        Action<DamageRecordArgs> damageRecordAction;
        public void Regist_BattleDamageRecordAction(Action<DamageRecordArgs> action) => damageRecordAction += action;
        public void Invoke_BattleDamageRecordAction(DamageRecordArgs args) => damageRecordAction?.Invoke(args);

        Action battleStateAndStageChangeAction;
        public void Regist_BattleStateAndStageChangeHandler(Action action) => battleStateAndStageChangeAction += action;
        public void Invoke_BattleStateAndStageChangeHandler() => battleStateAndStageChangeAction?.Invoke();

        Action battleAirDropAction;
        public void Regist_BattleAirDropAction(Action action) => battleAirDropAction += action;
        public void Invoke_BattleAirDropAction()=> battleAirDropAction?.Invoke();

        public LogicTriggerAPI() { }

    }

}