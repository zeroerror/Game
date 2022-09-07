using UnityEngine;

namespace Game.Client.Bussiness.WorldBussiness
{

    public class AnimatorComponent
    {

        Animator animator;

        public AnimatorComponent(Animator animator)
        {
            this.animator = animator;
        }

        public void PlayAnimClip(string animClipName)
        {
            animator.Play(animClipName);
        }

        public void PlayIdle()
        {
            animator.Play("Idle");
        }

        public void PlayRun()
        {
            animator.Play("Run");
        }

        public void PlayJump()
        {
            animator.CrossFade("Jump", 0.1f);
        }

        public void PlayHooking()
        {
            animator.Play("Hooking");
        }

    }

}