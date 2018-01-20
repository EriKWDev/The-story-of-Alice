using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueActivator : MonoBehaviour {

	[SerializeField]
	public List<string> dialogues = new List<string> ();
	private DialogueManager dm;

	public KeyCode activationKey = KeyCode.E;
	public float activationDiameter = 4f;
	public bool loopLastDialogue = false;
	public int currentDialogueIndex = 0;


	void Start () {
		dm = GameObject.Find ("SceneManager").GetComponent<DialogueManager> ();
	}

	void Update () {
		if (Input.GetKeyUp (activationKey) && !dm.isReading) {

			if (currentDialogueIndex >= dialogues.Count && loopLastDialogue)
				currentDialogueIndex = dialogues.Count - 1;
			
			if (currentDialogueIndex <= dialogues.Count - 1) 
				dm.ReadDialogue (dialogues [currentDialogueIndex]);

			currentDialogueIndex++;
		}
	}
}
