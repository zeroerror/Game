using UnityEngine;

namespace Game.Client.Bussiness.BattleBussiness
{

    public class RoleInputComponent
    {

        public Vector3 moveDir;
        public Vector3 rollDir;

        public RoleInputComponent()
        {
        }

        public void Reset()
        {
            moveDir = Vector3.zero;
            rollDir = Vector3.zero;
        }

    }

}