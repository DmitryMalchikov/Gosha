using UnityEngine;

namespace Assets.Scripts.Gameplay
{
    public class RandomAnimation : StateMachineBehaviour
    {
        public byte TransitionsNumber;
        private int _previousNumber = 0;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo info, int layerIndex)
        {
            byte newNumber = (byte)Random.Range(1, TransitionsNumber);

            if (_previousNumber == newNumber)
            {
                newNumber = (byte)((newNumber + 1) % TransitionsNumber);
            }

            animator.SetInteger("AnimationIndex", newNumber);
            _previousNumber = newNumber;
        }
    }
}
