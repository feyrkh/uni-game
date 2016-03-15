using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityStandardAssets._2D;

namespace AssemblyCSharp
{
	public class GameState : MonoBehaviour
	{
		public static GameState instance = null;

		private string nextSpawnPoint = "newGameSpawnPoint";
		public CharacterMovementUserControl playerPrefab;
		public DialogSystem dialogSystem;

		void Awake() {
			if (instance == null) {
				instance = this;
				Setup ();
			} else if (instance != this) {
				Destroy (gameObject);
				instance.Awake ();
				return;
			}
			instance.OnLevelWasLoaded();
		}

		void Setup() {
			DontDestroyOnLoad (this);
			dialogSystem = GetComponent<DialogSystem> ();
		}

		void OnLevelWasLoaded() {
			SpawnPlayer(nextSpawnPoint);
		}

		public void EnterDoorway(string nextScene, string spawnPoint) {
			nextSpawnPoint = spawnPoint;
			SceneManager.LoadScene (nextScene);
		}

		void SpawnPlayer (string nextSpawnPoint)
		{
			CharacterMovementUserControl player = Instantiate<CharacterMovementUserControl>(playerPrefab);
			playerPrefab.transform.position = GameObject.Find (nextSpawnPoint).transform.position;
			Transform camera = player.transform.FindChild ("Main Camera");
			Camera2DFollow cameraFollow = camera.GetComponent<Camera2DFollow> ();
			Transform target =  player.transform.FindChild ("CameraTarget");
			cameraFollow.target = target;
		}
	}
}

