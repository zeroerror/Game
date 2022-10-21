using UnityEngine;

namespace Game.Client.Bussiness.BattleBussiness
{

    public class RoleInputComponent
    {

        Vector3 moveDir;
        public Vector3 MoveDir => moveDir;
        public void SetMoveDir(Vector3 moveDir) => this.moveDir = moveDir;

        Vector3 rollDir;
        public Vector3 RollDir => rollDir;
        public void SetRollDir(Vector3 rollDir) => this.rollDir = rollDir;

        Vector3 faceDir;
        public Vector3 FaceDir => faceDir;
        public void SetFaceDir(Vector3 faceDir) => this.faceDir = faceDir;

        Vector3 fireDir;
        public Vector3 FireDir => fireDir;
        public void SetFireDir(Vector3 fireDir) => this.fireDir = fireDir;

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