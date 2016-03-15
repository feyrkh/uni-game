using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using SimpleJSON;

namespace AssemblyCSharp
{
	[System.Serializable]
	public class DialogEntry {
		public string portrait;
		public string text;

		public DialogEntry(string portrait, string text) {
			this.portrait = portrait;
			this.text = text;
		}
	}

	public enum DialogTriggerConditionOperator {
		LT, LTE, GT, GTE, EQ, NE
	}

	public class DialogTriggerCondition {
		public string variable;
		public DialogTriggerConditionOperator op;
		public int value;

		public bool IsTriggered(string speaker, Dictionary<string,int> context) {
			int variableVal = 0;
			string variable = this.variable;
			if (variable == "hearts") {
				variable += ":" + speaker;
			}
			if (context.ContainsKey (variable)) {
				variableVal = context [variable];
			}
			switch (op) {
			case DialogTriggerConditionOperator.LT:
				return variableVal < value;
			case DialogTriggerConditionOperator.LTE:
				return variableVal <= value;
			case DialogTriggerConditionOperator.GT:
				return variableVal > value;
			case DialogTriggerConditionOperator.GTE:
				return variableVal >= value;
			case DialogTriggerConditionOperator.EQ:
				return variableVal == value;
			case DialogTriggerConditionOperator.NE:
				return variableVal != value;
			}
			throw new Exception ("Unexpected dialog trigger condition operator: "+this.variable+" " + op+" "+this.value);
		}

		public static DialogTriggerCondition Parse (string value)
		{
			DialogTriggerCondition retval = new DialogTriggerCondition ();
			string[] pieces;
			int varValLen = 0;
			pieces = value.Split (new string[] {"<=",">=","!=", "<",">","=" }, StringSplitOptions.RemoveEmptyEntries);
			retval.variable = pieces [0];
			varValLen += pieces [0].Length;
			if (pieces.Length > 1) {
				retval.value = int.Parse (pieces [1]);
				varValLen += pieces [1].Length;
			} else {
				retval.value = 0;
			}
			string opName = value.Substring (pieces [0].Length, value.Length - varValLen);
			DialogTriggerConditionOperator op = DialogTriggerConditionOperator.EQ;
			switch (opName) {
			case ">=":
				op = DialogTriggerConditionOperator.GTE;
				break;
			case "<=":
				op = DialogTriggerConditionOperator.LTE;
				break;
			case "=":
				op = DialogTriggerConditionOperator.EQ;
				break;
			case "<":
				op = DialogTriggerConditionOperator.LT;
				break;
			case ">":
				op = DialogTriggerConditionOperator.GT;
				break;
			case "!=":
				op = DialogTriggerConditionOperator.NE;
				break;
			default:
				throw new Exception ("Unexpected operator in dialog trigger condition for " + value + ": " + opName);
			}
			retval.op = op;
			return retval;
		}
	}

	public class DialogTrigger {
		public DialogTriggerCondition[] triggers;
		public string file;
		public int priority = 100;
		public int energy = 1;
		public int cooldownHours = 12;


		public bool IsTriggered(string speaker, Dictionary<string,int> context) {
			foreach (var condition in triggers) {
				if (!condition.IsTriggered (speaker, context))
					return false;
			}
			return true;
		}
	}

	public class DialogTriggerManager {
		private Dictionary<String, List<DialogTrigger>> triggersByCharacterName = new Dictionary<string, List<DialogTrigger>>();

		public void AddTrigger(String characterName, DialogTrigger trigger) {
			if (!triggersByCharacterName.ContainsKey (characterName)) {
				triggersByCharacterName.Add (characterName, new List<DialogTrigger> ());
			}
			List<DialogTrigger> list = triggersByCharacterName [characterName];
			list.Add (trigger);
		}

		public void LoadTriggers(String filename) {
			string jsonStr = File.ReadAllText (filename);
			JSONNode fileJson = JSON.Parse(jsonStr);

			DialogTrigger trigger = new DialogTrigger ();
			foreach(KeyValuePair<string, JSONNode> entry in fileJson.AsObject) {
				string speakerName = entry.Key;
				JSONArray jsonList = entry.Value.AsArray;
				foreach(JSONNode json in jsonList.AsArray) {
					JSONNode triggersNode = json ["trigger"];
					if (triggersNode != null) {
						DialogTriggerCondition[] triggers = new DialogTriggerCondition[triggersNode.Count];
						int i = 0;
						foreach (JSONNode t in triggersNode.AsArray) {
							triggers [i] = DialogTriggerCondition.Parse(t.Value);
							i++;
						}
						trigger.triggers = triggers;

					}	
					trigger.file = json ["file"].Value;
					if (json ["pri"] != null) {
						trigger.priority = json ["pri"].AsInt;
					}
					if (json ["energy"] != null) {
						trigger.energy = json ["energy"].AsInt;
					}
					if (json ["cooldown"] != null) {
						trigger.cooldownHours = json ["cooldown"].AsInt;
					}
					AddTrigger (speakerName, trigger);
				}
			}
		}

		public void LoadAllTriggers(String path) {
			string[] files = Directory.GetFiles (Application.dataPath+"/"+path);
			foreach (var file in files) {
				try {
					LoadTriggers (file);
				} catch(Exception e) {
					Debug.LogError ("Exception while reading " + file + ": " + e.ToString ());
				}
			}
		}

		public List<DialogTrigger> GetTriggers(String speaker, Dictionary<string, int> context) {
			List<DialogTrigger> triggers = triggersByCharacterName [speaker];
			List<DialogTrigger> retval = new List<DialogTrigger> ();
			foreach (var dialogTrigger in triggers) {
				if (dialogTrigger.IsTriggered (speaker, context))
					retval.Add (dialogTrigger);
			}
			return retval;
		}
	}


	public class DialogSystem : MonoBehaviour
	{
		DialogEntry currentDialog;
		public bool inDialog = false;
		public bool nextKeyPressed = true;
		public KeyCode nextKey;

		DialogTriggerManager mgr;

		void Awake() {
			this.mgr = new DialogTriggerManager ();
			mgr.LoadAllTriggers ("JSON/ConversationTriggers");
		}

		IEnumerator WaitForKeyDown(KeyCode keyCode) {
			Debug.Log("Waiting for key press: " + keyCode);
			nextKey = keyCode;
			nextKeyPressed = false;
			while(!nextKeyPressed)
				yield return null;
			Debug.Log ("Key pressed!");
		}

		void Update() {
			if (inDialog) {
				if (Input.GetKeyDown (nextKey))
					nextKeyPressed = true;
			}
		}

		public void StartDialog(params DialogEntry[] dialogs) {
			if (!inDialog) {
				StartCoroutine (_StartDialog (dialogs));
	
			}
		}

		private IEnumerator _StartDialog(params DialogEntry[] dialogs) {
			Debug.Log ("Starting dialog");
			inDialog = true;
			var prevTimeScale = Time.timeScale;
			Time.timeScale = 0;
			foreach (var dialogEntry in dialogs) {
				currentDialog = null;
				currentDialog = dialogEntry;
				yield return StartCoroutine (WaitForKeyDown (KeyCode.Space));
			}
			currentDialog = null;
			inDialog = false;
			Time.timeScale = prevTimeScale;
			yield return null;
		}


		public void OnGUI() {
			if (currentDialog != null && inDialog) {
				GUI.Label (new Rect (100, 80, Screen.width - 200, 30), currentDialog.portrait);
				GUI.Label (new Rect (100, 100, Screen.width - 200, 400), currentDialog.text);
			}
		}
	}
}

