using UnityEngine;
using Game.Generic;
using Game.Client.Bussiness.BattleBussiness.Shot;
using System;

namespace Game.Client.Bussiness.BattleBussiness
{

    public class RoleStateComponent
    {

        RoleState roleState;
        public RoleState RoleState => roleState;

        // Model

        public RoleStateComponent()
        {
            roleState = RoleState.Normal;
        }

        public void EnterNormal()
        {
            roleState = RoleState.Normal;
        }

        public void EnterBeHit()
        {
            roleState = RoleState.BeHit;
        }

    }

}