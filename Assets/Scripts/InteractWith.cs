using UnityEngine;
using System.Collections;

public class InteractWith : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public virtual void OnInteraction (Interactor interactor)
	{
		throw new System.NotImplementedException ();
	}
}
