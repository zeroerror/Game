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

        Vector3 shootDir;
        public Vector3 ShootDir => shootDir;
        public void SetShootDir(Vector3 dir) => this.shootDir = dir;

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