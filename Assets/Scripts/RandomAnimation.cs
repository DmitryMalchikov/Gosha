using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAnimation : StateMachineBehaviour {
	public int TransitionsNumber;
	private int _previousNumber = 0;

	override public void OnStateEnter(Animator animator, AnimatorStateInfo info, int layerIndex){
		int newNumber = Random.Range (1, TransitionsNumber);

		if (_previousNumber != 0) {
			newNumber = 0;
		} else if (_previousNumber == newNumber) {
			newNumber = (newNumber + 1) % TransitionsNumber;
		}

		animator.SetInteger ("AnimationIndex", newNumber);
		_previousNumber = newNumber;
	}
}
