using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using ErikW.Dialogue;
using TMPro;

public class DialogueManager : MonoBehaviour {

	[Header ("Debug")]
	public bool debug;
	public string debugDialogue;
	public int debugLine = 1;

	[Header ("CSV & UI Settings")]
	public TextMeshProUGUI textBox;
	public TextMeshProUGUI nameTextBox;
	public TextMeshProUGUI secondaryTextBox;
	public TextMeshProUGUI indicator;
	public Image dialogueBackground;
	public ParticleSystem textParticleSystem;
	ParticleSystem.MainModule textParticleSystemMain;
	public SpriteRenderer glowCircle;

	[SerializeField]
	public List<TextMeshProUGUI> optionTextBoxes = new List<TextMeshProUGUI> ();
	public TextAsset dialogueCSV;
	public Dictionary<string, Dialogue> dialogues = new Dictionary<string, Dialogue> ();
	public bool isReading = false;
	private bool triggerGoto = false;

	[Header ("Popup Settings")]
	public TextMeshProUGUI popupTitleTextbox;
	public TextMeshProUGUI popupDescriptionTextBox;
	public Image popupBackground;
	public TextMeshProUGUI popupIndicator;
	public bool isDisplayingPopup = false;

	[Header ("Dialogue Defaults")]
	public float defaultLineSpeed = 0.035f;
	public int defaultLineType = 1;
	public float pauseTime = 0.3f;
	public float defaultFadeDuration = 2f;
	public TMP_FontAsset defaultFont;
	public int defaultFontSize = 26;
	public Color defaultColor;
	public Color textColor;
	public Color stylingColor;

	[Header ("Default Sounds")]
	public AudioClip defaultSpeakerVoice;
	public SoundLibrary defaultSoundLibrary;

	[Header ("Speakers")]
	public DialogueSpeaker Story;
	public DialogueSpeaker Info;
	public DialogueSpeaker Alice;
	public DialogueSpeaker Youkai;
	public DialogueSpeaker Professor;
	public DialogueSpeaker NPC;
	public DialogueSpeaker Echo;
	public DialogueSpeaker Tohi;

	public Dictionary<string, DialogueSpeaker> dialogueSpeakers = new Dictionary<string, DialogueSpeaker> ();
	private AudioSource audioSource;

	void Awake () {
		dialogueSpeakers.Add (Story.name, Story);
		dialogueSpeakers.Add (Info.name, Info);
		dialogueSpeakers.Add (Alice.name, Alice);
		dialogueSpeakers.Add (Youkai.name, Youkai);
		dialogueSpeakers.Add (Professor.name, Professor);
		dialogueSpeakers.Add (NPC.name, NPC);
		dialogueSpeakers.Add (Echo.name, Echo);
		dialogueSpeakers.Add (Tohi.name, Tohi);

		audioSource = GetComponent<AudioSource> ();
		//DontDestroyOnLoad (this.gameObject);
		textParticleSystemMain = textParticleSystem.main;
	}

	void Start () {
		print (dialogueCSV.text);
		CreateDialogueDictionary ();
		if (debug) {
			ReadDialogue (debugDialogue, debugLine);
		}
	}

	public void CreateDialogueDictionary () {
		List<string> lines = dialogueCSV.text.Split ('\n').ToList<string> ();
		Dictionary<int, List<string>> all = new Dictionary<int, List<string>> ();

		for (int i = 0; i < lines.Count; i++) {
			all[i] = lines[i].Split (";"[0]).ToList<string> ();
		}

		for (int i = 0; i < all.Count; i++) {

			if (all[i][0].ToUpper () == "ID") {
				List<DialogueLine> dls = new List<DialogueLine> ();
				Dialogue d = new Dialogue (dls, all[i][1].ToUpper ());
				i++;
				string CSVName = "";
				while (all[i][0].ToUpper () != "END") {

					if (all[i][0] != "") {
						CSVName = all[i][0];
					}

					if (CSVName.ToUpper () != "TRIGGER") {
						if (dialogueSpeakers.ContainsKey (CSVName)) {
							DialogueLine dl1 = new DialogueLine (dialogueSpeakers [CSVName], all [i] [1], (all [i] [2] != "" ? int.Parse (all [i] [2]) : defaultLineType), (all [i] [3] != "" ? float.Parse (all [i] [3]) : defaultLineSpeed));
							d.lines.Add (dl1);
						} else {
							DialogueSpeaker newSpeaker2 = new DialogueSpeaker (CSVName, defaultColor, defaultSpeakerVoice, defaultFont);
							DialogueLine dl2 = new DialogueLine (newSpeaker2, all [i] [1], (all [i] [2] != "" ? int.Parse (all [i] [2]) : defaultLineType), (all [i] [3] != "" ? float.Parse (all [i] [3]) : defaultLineSpeed));
							d.lines.Add (dl2);
						}
					} else {
						List<string> options = new List<string> ();
						for (int j = 4; j <= 6; j++) {
							if (all [i] [j] != "")
								options.Add (all [i] [j]);
						}

						DialogueSpeaker newSpeaker3 = new DialogueSpeaker (CSVName, defaultColor, defaultSpeakerVoice, defaultFont);
						if (all [i] [1].ToUpper () != "CHOICE" && all [i] [1].ToUpper () != "SAVECHOICE") {
							DialogueLine dl3 = new DialogueLine (newSpeaker3, all [i] [1], true, options);
							d.lines.Add (dl3);
						} else {

							List<string> choices = new List<string> ();
							for (int j = 4; j <= 6; j++) {
								if (all [i + 1] [j] != "") {
									choices.Add (all [i+1] [j]);
								}
							}
							DialogueLine dl4 = new DialogueLine (newSpeaker3, all [i] [1], true, options, choices);
							d.lines.Add (dl4);
						}
					}
					i++;
				}
				dialogues.Add (d.ID, d);
			}
		}
	}

	public void Popup (string title, string description) {
		StartCoroutine (DisplayPopup (title, description));
	}

	public IEnumerator DisplayPopup (string title, string description) {
		isDisplayingPopup = true;
		popupTitleTextbox.text = title;
		popupDescriptionTextBox.text = description;

		StartCoroutine (SetImageAlpha (0.26f, popupBackground));
		StartCoroutine (SetIndicatorAlpha (1f, popupTitleTextbox));
		yield return new WaitForSeconds (0.5f);

		StartCoroutine (SetIndicatorAlpha (1f, popupDescriptionTextBox));
		yield return new WaitForSeconds (1f);

		StartCoroutine (SetIndicatorAlpha (1f, popupIndicator));
		while (!Input.GetKeyDown(KeyCode.Space)) {
			yield return null;
		}

		StartCoroutine (SetIndicatorAlpha (0f, popupIndicator));
		StartCoroutine (SetIndicatorAlpha (0f, popupDescriptionTextBox));
		StartCoroutine (SetImageAlpha (0f, popupBackground));
		StartCoroutine (SetIndicatorAlpha (0f, popupTitleTextbox));
		isDisplayingPopup = false;
	}

	public void ReadDialogue (string dialogueName) {
		ReadDialogue (dialogueName, 1);
	}

	public void ReadDialogue (string dialogueName, int startLineIndex) {
		if (dialogues.ContainsKey (dialogueName.ToUpper ())) {
			ReadDialogue (dialogues[dialogueName.ToUpper ()], startLineIndex);
		} else {
			Debug.LogError ("Dialogue Error : Dialogue '" + dialogueName.ToUpper () + "' could not be found");
		}
	}

	public void ReadDialogue (Dialogue dialogue) {
		ReadDialogue (dialogue, 1);
	}

	public void ReadDialogue (Dialogue dialogue, int startLineIndex) {
		StartCoroutine (ReadLines (dialogue.lines, startLineIndex));
	}
		
	public IEnumerator ReadLines (List<DialogueLine> lines, int startLineIndex) {

		StartCoroutine (SetIndicatorAlpha (1f, nameTextBox));
		dialogueBackground.color = lines.First ().speaker.nameColor;
		StartCoroutine (SetImageAlpha (1f, dialogueBackground));
		yield return new WaitForSeconds (0.3f);
		StartCoroutine (SetIndicatorAlpha (1f, textBox));
		StartCoroutine (SetIndicatorAlpha (1f, secondaryTextBox));

		for (int i = startLineIndex-1; i < lines.Count; i++) {
			DialogueLine line = lines [i];
			if (line.speaker.name.ToUpper () != "TRIGGER") {
				StartCoroutine (ReadLine (lines[i].speaker.name, line.speaker.nameColor, line.text, line.lineType, line.speed, line.speaker.voice, line.speaker.font, line.speaker.fontSize));

				if (i + 1 < lines.Count) {
					if (lines [i + 1].speaker.name.ToUpper () == "TRIGGER" && (lines [i + 1].text.ToUpper () == "CHOICE" || lines [i + 1].text.ToUpper () == "SAVECHOICE" || lines [i + 1].text.ToUpper () == "SOUND")) {
						while (isReading || isDisplayingPopup) {
							yield return null;
						}
					} else {
						indicator.text = "" + lines[i].speaker.name [0];
						indicator.color = new Color (line.speaker.nameColor.r, line.speaker.nameColor.g, line.speaker.nameColor.b, 0f);
						if (i > 0 && lines [i - 1].speaker.name [0] != null) {
							if (lines [i - 1].speaker.name [0] != lines [i].speaker.name [0]) {
								StartCoroutine (SetIndicatorAlpha (1f));
							}
						} else {
							StartCoroutine (SetIndicatorAlpha (1f));
						}

						while (isReading || isDisplayingPopup) {
							yield return null;
						}
						while (!Input.GetKeyDown(KeyCode.Space)) {
							yield return null;
						}
						if (lines [i + 1].speaker.name [0] != null) {
							if (lines[i + 1].speaker.name [0] != lines[i].speaker.name [0]) {
								StartCoroutine (SetIndicatorAlpha (0f));
							}
						}
					}
				} else {
					while (isReading || isDisplayingPopup) {
						yield return null;
					}
					while (!Input.anyKeyDown) {
						yield return null;
					}

					Color endColor = new Color (0f,0f,0f,0f);

					for (float t = 0; t < 1f; t+= Time.deltaTime) {
						textBox.color = Color.Lerp (textBox.color, endColor, t/defaultFadeDuration);
						yield return new WaitForEndOfFrame ();
					}
				}

			} else {
				switch (line.text.ToUpper ()) {
				case "PLAYSOUND":
				case "SOUND":
					if (!SoundLibraryContains (line.options [0], lines[i+1].speaker.soundLibrary)) {
						Debug.LogError ("Audio Error : Audio '" + line.options [0].ToUpper () + "' could not be found");
					}
					break;
				case "PLAYSOUNDTYPE":
					if (!PlayRandomSoundOfType ((SoundType)System.Enum.Parse(typeof(SoundType), line.options [0]), lines[i+1].speaker.soundLibrary)) {
						Debug.LogError ("Audio Error : Audio of type '" + line.options [0].ToUpper () + "' could not be found");
					}
					break;
				case "GOTO":
					if (dialogues.ContainsKey (line.options [0].ToUpper ())) {
						triggerGoto = true;
						ReadDialogue (dialogues [line.options [0].ToUpper ()]);
					}  else {
						Debug.LogError ("Dialogue Error : Dialogue '" + line.options [0].ToUpper () + "' could not be found");
					}
					break;
				case "SAVECHOICE":
				case "CHOICE":
					int k = 0;
					int n = 0;

					if (!SoundLibraryContains ("Question01", defaultSoundLibrary)) {
						Debug.LogError ("Audio Error : Audio '" + line.options [0].ToUpper () + "' could not be found");
					}

					List<TextMeshProUGUI> relevantOptionTextBoxes = new List<TextMeshProUGUI> ();
					Color optionBoxColor = optionTextBoxes [0].GetComponentInChildren<Image> ().color;
					switch (line.options.Count) {
					default:
					case 1:
						relevantOptionTextBoxes.Add (optionTextBoxes [2]);
						break;
					case 2:
						relevantOptionTextBoxes.Add (optionTextBoxes [1]);
						relevantOptionTextBoxes.Add (optionTextBoxes [2]);
						break;
					case 3:
						relevantOptionTextBoxes = optionTextBoxes;
						break;
					}

					foreach (TextMeshProUGUI t in optionTextBoxes) {
						Image img = t.GetComponentInChildren<Image> ();
						img.color = new Color (optionBoxColor.r, optionBoxColor.g, optionBoxColor.b, 0F);
					}

					foreach (string choice in line.options) {
						TMP_Text choiceBox = relevantOptionTextBoxes [k];
						StartCoroutine (SetOptionBoxAlpha (0.26f, relevantOptionTextBoxes [k]));
						choiceBox.color = new Color (choiceBox.color.r, choiceBox.color.g, choiceBox.color.b, 0f);
						choiceBox.text = choice;
						k++;
					}

					yield return new WaitForEndOfFrame ();


					while (!Input.GetKeyDown (KeyCode.Space)) {
						int previousN = n;

						if (Input.GetKeyUp (KeyCode.DownArrow) || Input.GetKeyUp (KeyCode.S)) {
							n += 1;
						}

						if (Input.GetKeyUp (KeyCode.UpArrow) || Input.GetKeyUp (KeyCode.W)) {
							n -= 1;
						}

						if (n != previousN) {
							if (n >= line.options.Count) {
								n = 0;
							}
							if (n < 0) {
								n = line.options.Count - 1;
							}
						}

						k = 0;
						foreach (string choice in line.options) {
							TMP_Text choiceBox = relevantOptionTextBoxes [k];
							if (k == n) {
								choiceBox.color = new Color (choiceBox.color.r, choiceBox.color.g, choiceBox.color.b, Mathf.Lerp (choiceBox.color.a, 1f, Time.deltaTime * 8f));
								//choiceBox.fontSize = Mathf.Lerp (choiceBox.fontSize, 26f, Time.deltaTime * 15f);
							} else {
								choiceBox.color = new Color (choiceBox.color.r, choiceBox.color.g, choiceBox.color.b, Mathf.Lerp (choiceBox.color.a, 0.3f, Time.deltaTime * 8f));
								//choiceBox.fontSize = Mathf.Lerp (choiceBox.fontSize, 22f, Time.deltaTime * 15f);
							}
							k++;
						}

						yield return null;
					}

					foreach (TextMeshProUGUI ob in relevantOptionTextBoxes) {
						if (ob != null)
							ob.text = "";
						
						StartCoroutine (SetOptionBoxAlpha (0f, ob));
					}

					if (dialogues.ContainsKey (line.choices [n].ToUpper ())) {
						triggerGoto = true;
						ReadDialogue (dialogues [line.choices [n].ToUpper ()]);
					} else {
						Debug.LogError ("Dialogue Error : Dialogue '" + line.choices [n].ToUpper () + "' could not be found");
					}

					break;
				case "LEARN":
					if (!SoundLibraryContains ("Info01", defaultSoundLibrary)) {
						Debug.LogError ("Audio Error : Audio '" + line.options [0].ToUpper () + "' could not be found");
					}
	
					switch (line.options[0].ToUpper ()) {
					default:
					case "NAME":
						DialogueSpeaker newSpeaker = dialogueSpeakers [line.options [1]];
						if (newSpeaker != null) {
							dialogueSpeakers [line.options [1]].hasEncountered = true;
							string tmpTitle = "<color=" + ToRGBHex (newSpeaker.nameColor) + ">" + newSpeaker.name + "</color>";
							Popup (tmpTitle, "You've just got to know " + newSpeaker.name + "!");
						} else {
							Debug.LogError ("No name to be learned : " + line.options[0]);
						}
						break;
					case "STRING":

						break;
					case "BOOL":

						break;
					case "NUMBER":

						break;
					}
					break;
				default:
					break;
				}
			}
		}

		if (!triggerGoto) {
			StartCoroutine (SetIndicatorAlpha (0f, nameTextBox));
			StartCoroutine (SetImageAlpha (0f, dialogueBackground));
			StartCoroutine (SetIndicatorAlpha (0f, textBox));
			StartCoroutine (SetIndicatorAlpha (0f, secondaryTextBox));
			StartCoroutine (SetIndicatorAlpha (0f));
		} else {
			triggerGoto = false;
		}
	}

	public IEnumerator SetIndicatorAlpha (float alpha) {
		for (float t = 0; t < 1f; t+= Time.deltaTime) {
			indicator.color = Color.Lerp (indicator.color, new Color (indicator.color.r, indicator.color.g, indicator.color.b, alpha), t/0.2f);
			yield return new WaitForSeconds (0f);
		}
	}

	public IEnumerator SetIndicatorAlpha (float alpha, TextMeshProUGUI _indicator) {
		for (float t = 0; t < 1f; t+= Time.deltaTime) {
			_indicator.color = Color.Lerp (_indicator.color, new Color (_indicator.color.r, _indicator.color.g, _indicator.color.b, alpha), t/0.2f);
			yield return new WaitForSeconds (0f);
		}
	}

	public IEnumerator SetImageAlpha (float alpha, Image img) {
		for (float t = 0; t < 1f; t+= Time.deltaTime) {
			img.color = Color.Lerp (img.color, new Color (img.color.r, img.color.g, img.color.b, alpha), t/0.5f);
			yield return new WaitForSeconds (0f);
		}
	}

	public IEnumerator SetOptionBoxAlpha (float alpha, TextMeshProUGUI optionBox) {
		for (float t = 0; t < 1f; t+= Time.deltaTime) {
			Image img = optionBox.GetComponentInChildren<Image> ();
			img.color = Color.Lerp (img.color, new Color (img.color.r, img.color.g, img.color.b, alpha), t/0.5f);
			yield return new WaitForSeconds (0f);
		}
	}

	public IEnumerator RemoveText (TextMeshProUGUI tb) {
		for (float t = 0; t < 1f; t+= Time.deltaTime) {
			tb.color = Color.Lerp (tb.color, new Color (tb.color.r, tb.color.g, tb.color.b, 0f), t/defaultFadeDuration);
			yield return new WaitForSeconds (0f);
		}
		tb.text = "";
	}

	public IEnumerator AddText (TextMeshProUGUI tb, string text) {
		tb.text = text;
		for (float t = 0; t < 1f; t+= Time.deltaTime) {
			tb.color = Color.Lerp (new Color (tb.color.r, tb.color.g, tb.color.b, 0f), new Color (tb.color.r, tb.color.g, tb.color.b, 1f), t/(defaultFadeDuration/2f));
			yield return new WaitForSeconds (0f);
		}
	}

	public IEnumerator ReadLine (string name, Color nameColor, string text, int type, float speed, AudioClip voice, TMP_FontAsset font, int fontSize) {
		isReading = true;

		string tmpText = textBox.text;
		secondaryTextBox.text = tmpText;
		secondaryTextBox.color = textBox.color;
		StartCoroutine (FadeGlow(name, nameColor));

		textBox.font = (font == null ? defaultFont : font);
		textBox.fontSize = (fontSize == 0 ? defaultFontSize : fontSize);

		StartCoroutine (RemoveText (secondaryTextBox));
		dialogueBackground.color = nameColor;
		textBox.text = "";

		yield return new WaitForSeconds (0.15f);

		if (voice != null) {
			audioSource.clip = voice;
			audioSource.Play ();
		}

		Color endColor = textColor;

		textBox.color = new Color (0f,0f,0f,0f);
		float size = textBox.fontSize;
		string stylingHex = ToRGBHex (stylingColor);

		if ((dialogueSpeakers.ContainsKey (name) && dialogueSpeakers [name].hasEncountered) || !dialogueSpeakers.ContainsKey (name)) {
			nameTextBox.text = /*"<color=" + stylingHex + ">«</color>*/  "<color=" + ToRGBHex (nameColor) + ">" + name + "</color> " /*"<color=" + stylingHex + ">»</color>"*/;
		}
		else {
			nameTextBox.text = /*"<color=" + stylingHex + ">«</color> */ "<color=" + ToRGBHex (nameColor) + ">Stranger</color>" /* <color=" + stylingHex + ">»</color>"*/;
		}

		if (name == "Info" || name == "Echo" || type == 5) {
			textBox.fontStyle = FontStyles.Italic;
			secondaryTextBox.fontStyle = FontStyles.Italic;
			text = "* " + text + " *";
			textBox.text = text;
		} else  {
			textBox.fontStyle = FontStyles.Normal;
			secondaryTextBox.fontStyle = FontStyles.Normal;
			textBox.text = text;
		}
			
		textBox.color = nameColor;
		textParticleSystemMain.startColor = nameColor;

		bool pause = false;
	
		//StartCoroutine (AddText (textBox, text));

		float timeToRead = text.Length / 30f;
		float t = 0f;
		int i = 0;

		while (t < timeToRead) {
			t += Time.deltaTime;
			int tmpi = Mathf.RoundToInt((t / timeToRead) * text.Length);
			i = tmpi < text.Length ? tmpi : text.Length;
			textBox.text = text.Substring (0, i) + "<color=#0000>" + text.Substring (i) + "</color>";
			yield return new WaitForEndOfFrame ();

			if (pause) {
				yield return new WaitForSeconds (0.4f);
				pause = false;
			}
				
			char c = text [(i < text.Length ? i : text.Length - 1)];
			if ((c == ',' || c == '?' || c == '!') && i < text.Length-1) {
				pause = true;
			}
			while (isDisplayingPopup) {
				yield return null;
			}
		}
			
		textBox.text = text;

		isReading = false;
	}
		

	public string ToRGBHex(Color c) {
		return string.Format("#{0:X2}{1:X2}{2:X2}", ToByte(c.r), ToByte(c.g), ToByte(c.b));
	}

	private byte ToByte(float f) {
		f = Mathf.Clamp01(f);
		return (byte)(f * 255);
	}

	public IEnumerator FadeGlow (string name, Color nameColor) {
		// glowCircle.material = (dialogueSpeakers [name].glowMaterial != null ? dialogueSpeakers [name].glowMaterial : glowCircle.material);
		for (float t = 0f; t <= 1f; t+= Time.deltaTime) {
			glowCircle.material.Lerp (glowCircle.material, dialogueSpeakers [name].glowMaterial, t / 0.5f);
			yield return new WaitForSeconds (Time.deltaTime);
		}
	}

	public bool PlayRandomSoundOfType (SoundType soundType, SoundLibrary soundLibrary) {
		DialogueSound sound;

		List<DialogueSound> tmpSounds = new List<DialogueSound> ();

		foreach (DialogueSound ds in soundLibrary.sounds) {
			if (ds.soundType == soundType) {
				tmpSounds.Add (ds);
			}
		}

		if (tmpSounds.Count > 0) {
			sound = tmpSounds [Random.Range (0, tmpSounds.Count - 1)];

			audioSource.clip = sound.clip;
			audioSource.Play ();
			return true;
		} else {
			Debug.LogError ("Could not load DialogueSound of type " + soundType.ToString () + " from " + soundLibrary);
			return false;
		}
	}

	public bool SoundLibraryContains (string _name) {
		return SoundLibraryContains (_name, null, defaultSoundLibrary);
	}

	public bool SoundLibraryContains (string _name, SoundLibrary soundLibrary) {
		return SoundLibraryContains (_name, null, soundLibrary);
	}

	public bool SoundLibraryContains (AudioClip _clip) {
		return SoundLibraryContains ("", _clip, defaultSoundLibrary);
	}

	public bool SoundLibraryContains (string _name, AudioClip _clip, SoundLibrary soundLibrary) {
		if (soundLibrary != null) {
			foreach (DialogueSound ds in soundLibrary.sounds) {
				if ((_name != "" && ds.name.ToUpper() == _name.ToUpper()) || (_clip != null && ds.clip == _clip)) {
					if (ds.clip == null) {
						return false;
					}
					audioSource.clip = ds.clip;
					audioSource.Play ();
					return true;
				}
			}
		}

		if (soundLibrary != defaultSoundLibrary) {
			foreach (DialogueSound ds in defaultSoundLibrary.sounds) {
				if ((_name != "" && ds.name.ToUpper() == _name.ToUpper()) || (_clip != null && ds.clip == _clip)) {
					if (ds.clip == null) {
						return false;
					}
					audioSource.clip = ds.clip;
					audioSource.Play ();
					return true;
				}
			}
		}

		Debug.LogError ("Could not load DialogueSound from any soundlibrary with the name " + _name);
		return false;
	}
}