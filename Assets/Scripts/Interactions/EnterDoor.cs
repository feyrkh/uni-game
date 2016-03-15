using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class EnterDoor : InteractWith {
	public string sceneName;
	public string spawnPointName;
	Interactor interactor;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
	}

	public override void OnInteraction (Interactor interactor)
	{
		if (interactor.Interacting)
			return;
		GameState.instance.EnterDoorway (sceneName, spawnPointName);
	}
}
