using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using AssemblyCSharp;

[RequireComponent(typeof(Interactor))]
public class InteractorUserControl : MonoBehaviour {
	Interactor interactor;

	// Use this for initialization
	void Start () {
		interactor = GetComponent<Interactor> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (GameState.instance.dialogSystem.inDialog) {
			return;
		}
		if (Input.GetKeyDown(KeyCode.UpArrow)) {
			interactor.InteractNearby ();
		}
	}
}
