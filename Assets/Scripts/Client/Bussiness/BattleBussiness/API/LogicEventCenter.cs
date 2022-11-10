using UnityEngine;
using System;

namespace Game.Client.Bussiness.BattleBussiness.API
{

    public class LogicEventCenter
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

        Action<int, Vector3, Transform> bulletHitFieldAction;
        public void Regist_BulletHitFieldAction(Action<int, Vector3, Transform> action) => bulletHitFieldAction += action;
        public void Invoke_BulletHitFieldAction(int bulleID, Vector3 hitPos, Transform hitTF) => bulletHitFieldAction?.Invoke(bulleID, hitPos, hitTF);

        public LogicEventCenter() { }

    }

}