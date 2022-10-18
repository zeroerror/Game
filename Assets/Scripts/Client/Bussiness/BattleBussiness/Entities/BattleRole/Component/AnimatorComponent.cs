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

        public void PlayIdle_Rifle()
        {
            currentStateName = "Idle_Rifle";
            animator.CrossFade(currentStateName, 0.1f);
        }

        public void PlayRun()
        {
            currentStateName = "Run";
            animator.CrossFade(currentStateName, 0.1f);
        }

        public void PlayRun_Rifle()
        {
            currentStateName = "Run_Rifle";
            animator.CrossFade(currentStateName, 0.1f);
        }

        public void PlayReload()
        {
            currentStateName = "Reload";
            animator.CrossFade(currentStateName, 0.1f);
        }

        public void PlayReload_Run()
        {
            currentStateName = "Reload_Run";
            animator.CrossFade(currentStateName, 0.1f);
        }

        public void PlayRoll()
        {
            currentStateName = "Roll";
            animator.Play(currentStateName);
        }

        public void PlayBeHit()
        {
            currentStateName = "BeHit";
            animator.Play(currentStateName);
        }

        public void PlayBeHit_Rifle()
        {
            currentStateName = "BeHit_Rifle";
            animator.Play(currentStateName);
        }

        public void PlayShoot_Rifle_Run()
        {
            currentStateName = "Shoot_Rifle_Run";
            animator.Play(currentStateName);
        }

        public void PlayShoot_Rifle()
        {
            currentStateName = "Shoot_Rifle";
            animator.Play(currentStateName);
        }

        public void PlayDead()
        {
            currentStateName = "Dead";
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