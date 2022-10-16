using UnityEngine;

namespace Game.Client.Bussiness.BattleBussiness
{

    public class AnimatorComponent
    {

        Animator animator;

        string currentStateName;

        public AnimatorComponent(Animator animator)
        {
            this.animator = animator;
        }

        public void PlayAnimClip(string stateName)
        {
            animator.Play(stateName);
        }

        public void PlayIdle()
        {
            currentStateName = "Idle";
            animator.CrossFade(currentStateName, 0.1f);
        }

        public void PlayIdleWithGun()
        {
            currentStateName = "Idle_With_Gun";
            animator.CrossFade(currentStateName, 0.1f);
        }

        public void PlayRunning()
        {
            currentStateName = "Running";
            animator.CrossFade(currentStateName, 0.1f);
        }

        public void PlayRunnigWithGun()
        {
            currentStateName = "Running_With_Gun";
            animator.CrossFade(currentStateName, 0.1f);
        }

        public void PlayReloading()
        {
            currentStateName = "Reloading";
            animator.CrossFade(currentStateName, 0.1f);
        }

        public void PlayShooting()
        {
            currentStateName = "Shooting";
            animator.Play(currentStateName);
        }

        public void PlayDead()
        {
            currentStateName = "Dead";
            animator.Play(currentStateName);
        }

        public void PlayRolling()
        {
            currentStateName = "Rolling";
            animator.Play(currentStateName);
        }

        public bool IsInState(string stateName)
        {
            return currentStateName == stateName;
        }

        public int GetCurrentClipCurrentFrameIndex()
        {
            var clip = GetCurrentClip();
            var frameRate = clip.frameRate;
            int totalFrame = (int)(clip.length * frameRate);

            var playedTime = GetCurrentClipAlreadyPlayedTime();
            int playedFrame = (int)(playedTime / clip.length) * totalFrame;

            Debug.Log($"frameRate {frameRate} totalFrame {totalFrame} playedFrame:{playedFrame}");
            return playedFrame;
        }

        public AnimationClip GetCurrentClip()
        {
            return animator.GetCurrentAnimatorClipInfo(0)[0].clip;
        }

        public float GetCurrentClipAlreadyPlayedTime()
        {
            return animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        }

    }

}