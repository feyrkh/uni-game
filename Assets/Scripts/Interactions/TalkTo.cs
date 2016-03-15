using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class TalkTo : InteractWith {
	string text;
	bool showText;
	Interactor interactor;

	public string characterName;
	public DialogEntry[] dialogs;

	// Use this for initialization
	void Start () {
	
	}

	public override void OnInteraction (Interactor interactor)
	{
		if (interactor.Interacting)
			return;
		// Pause the game
		Debug.Log("Starting to talk");
		GameState.instance.dialogSystem.StartDialog (dialogs);
	}

	public void OnInteractionFinish() {
		Debug.Log("Finished talking");
	}
}
