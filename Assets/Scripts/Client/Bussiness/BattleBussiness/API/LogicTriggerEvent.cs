using UnityEngine;
using System;

namespace Game.Client.Bussiness.BattleBussiness.API
{

    public class LogicTriggerEvent
    {

        Action<DamageRecordArgs> damageRecordAction;
        public void Regist_BattleDamageRecordAction(Action<DamageRecordArgs> action) => damageRecordAction += action;
        public void Invoke_BattleDamageRecordAction(DamageRecordArgs args) => damageRecordAction?.Invoke(args);

        Action battleStateAndStageChangeAction;
        public void Regist_BattleStateAndStageChangeHandler(Action action) => battleStateAndStageChangeAction += action;
        public void Invoke_BattleStateAndStageChangeHandler() => battleStateAndStageChangeAction?.Invoke();

        Action battleAirDropAction;
        public void Regist_BattleAirDropAction(Action action) => battleAirDropAction += action;
        public void Invoke_BattleAirDropAction() => battleAirDropAction?.Invoke();

        Action<int, Transform> bulletHitAction;
        public void Regist_BulletHitFieldAction(Action<int, Transform> action) => bulletHitAction += action;
        public void Invoke_BulletHitFieldAction(int bulleID, Transform hitTF) => bulletHitAction?.Invoke(bulleID, hitTF);

        public LogicTriggerEvent() { }

    }

}