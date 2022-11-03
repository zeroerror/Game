using System;

namespace Game.Client.Bussiness.BattleBussiness.API
{

    public class LogicTriggerAPI
    {

        public Action<DamageRecordArgs> damageRecordAction;
        public void Invoke_DamageRecordAction(DamageRecordArgs args) => damageRecordAction?.Invoke(args);

        public LogicTriggerAPI() { }

    }

}