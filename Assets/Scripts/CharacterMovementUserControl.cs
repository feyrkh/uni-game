using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(CharacterMovement))]
public class CharacterMovementUserControl : MonoBehaviour {
	CharacterMovement movement;

	// Use this for initialization
	void Start () {
		this.movement = GetComponent<CharacterMovement> ();
	}

	void Update() {
		if (!this.movement.paused) {
			movement.StartMoving(CrossPlatformInputManager.GetAxis ("Horizontal"));
		}
	}
}
