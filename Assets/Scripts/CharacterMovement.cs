using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public enum Facing {
	Left=-1, Right=1
}

public class CharacterMovement : MonoBehaviour {
	[SerializeField] public float walkSpeed = 250f;
	[SerializeField] public float moving = 0;
	[SerializeField] public bool running = false;
	[Tooltip("If the user is holding over less than this amount, don't move")]
	[SerializeField] public float moveThreshold = 1f;
	[SerializeField] public Facing _facing = Facing.Right;

	SpriteRenderer sprite;
	Transform position;

	public bool paused {
		get;
		set;
	}

	public Facing facing {
		get {
			return _facing;
		}
		set {
			_facing = value;
			if (_facing == Facing.Left)
				sprite.flipX = true;
			else
				sprite.flipX = false;
		}
	}

	// Use this for initialization
	void Start () {
		this.sprite = GetComponent<SpriteRenderer> ();
		this.position = GetComponent<Transform> ();
	}
		
	void Update() {
		if (GameState.instance.dialogSystem.inDialog) {
			return;
		}
		if (!this.paused) {
			if (moving < -0.001) {
				facing = Facing.Left;
			}
			if (moving > 0.001) {
				facing = Facing.Right;
			}
			if (Mathf.Abs (moving) >= moveThreshold) {
				position.Translate (new Vector3 (moving * walkSpeed * Time.deltaTime, 0, 0));
			}
		}
	}

	public void StartMoving (float dx)
	{
		this.moving = dx;
	}
}
