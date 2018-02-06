using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour {

	private ParticleSystem[] _effects;

	void Start () {
		_effects = GetComponentsInChildren<ParticleSystem> ();
	}

	public void Play(){
		for (int i = 0; i < _effects.Length; i++) {
			_effects [i].Play ();
		}
	}

	public void Stop(bool stopImmediately = false){
		for (int i = 0; i < _effects.Length; i++) {
			if (stopImmediately) {
				_effects [i].Clear ();
			}
			_effects [i].Stop ();
		}
	}
}
