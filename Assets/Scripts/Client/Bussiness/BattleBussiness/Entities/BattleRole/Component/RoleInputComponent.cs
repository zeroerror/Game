using UnityEngine;

namespace Game.Client.Bussiness.BattleBussiness
{

    public class RoleInputComponent
    {

        Vector3 moveDir;
        public Vector3 MoveDir => moveDir;
        public void SetMoveDir(Vector3 moveDir) => this.moveDir = moveDir;

        Vector3 rotEuler;
        public Vector3 RotEuler => rotEuler;
        public void SetRotEuler(Vector3 rotDir) => this.rotEuler = rotDir;

        Vector3 rollDir;
        public Vector3 RollDir => rollDir;
        public void SetRollDir(Vector3 rollDir) => this.rollDir = rollDir;

        public bool pressReload;
        public bool PressReload => pressReload;
        public void SetPressReload() => pressReload = true;

        public RoleInputComponent()
        {
        }

        public void Reset()
        {
            moveDir = Vector3.zero;
            rollDir = Vector3.zero;

            pressReload = false;
        }

    }

}