﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoClear : MonoBehaviour {
	private InputField _input;

	private void OnEnable(){
		if (!_input) {
			_input = GetComponent<InputField> ();
		}

		_input.text = string.Empty;
	}
}
