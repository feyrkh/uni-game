using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Interactor : MonoBehaviour {
	bool interacting = false;

	public bool Interacting {
		get {
			return interacting;
		}
		set {
			interacting = value;
		}
	}

	Hashtable nearbyColliders = new Hashtable();


	// Use this for initialization
	void Awake () {
		nearbyColliders[-1] = new List<GameObject> ();
	}

	public List<GameObject>  GetNearbyColliders(int layer) {
		List<GameObject>  list = (List<GameObject>)nearbyColliders [layer];
		if(list != null) {
			list.RemoveAll (item => item == null);
			return list;
		} else {
			return new List<GameObject>();
		}
	}

	 void OnTriggerEnter2D(Collider2D other) {
		GameObject go = other.gameObject;
		List<GameObject> list = (List<GameObject>)nearbyColliders [-1];
		if (!list.Contains (go)) {
			list.Add (go);
		}
	}

	 void OnTriggerExit2D(Collider2D other) {
		GameObject go = other.gameObject;
		List<GameObject> list = (List<GameObject>)nearbyColliders [-1];
		if(list != null)
			list.Remove (go);
	}
	
	public void InteractNearby () {
		if (!interacting) {
			List<GameObject> hits = GetNearbyColliders (-1);
			InteractWith closest = null;
			float closestDist = float.PositiveInfinity;
			foreach (GameObject obj in hits) {
				InteractWith action = obj.GetComponent<InteractWith> ();
				if (action == null) {
					continue;
				}
				var curDist = Vector3.Distance (transform.position, obj.transform.position);
				if (curDist < closestDist) {
					closestDist = curDist;
					closest = action;
				}
			}
			if (closest != null) {
				closest.OnInteraction (this);
			}
		}
	}
}
