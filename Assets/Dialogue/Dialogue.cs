using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

namespace ErikW.Dialogue {

	public class Dialogue {
		public List<DialogueLine> lines;
		public string ID = "undefined";

		public Dialogue (List<DialogueLine> _lines, string _ID) {
			lines = _lines;
			ID = _ID;
		}

		public Dialogue (List<DialogueLine> _lines) {
			lines = _lines;
		}
	}

	public class DialogueLine {

		public DialogueSpeaker speaker;
		public string text;
		public float speed;
		public int lineType;
		public bool isTrigger = false;
		public List<string> options = new List<string> ();
		public List<string> choices = new List<string> ();

		public DialogueLine (DialogueSpeaker _speaker, string _text, int _lineType, float _speed) {
			if (_speaker.name.ToUpper () == "TRIGGER") {
				isTrigger = true;
			}
			speaker = _speaker;
			text = _text;
			speed = _speed;
			lineType = _lineType;
		}

		public DialogueLine (DialogueSpeaker _speaker, string _text, List<string> _options) {
			if (_speaker.name.ToUpper () == "TRIGGER") {
				isTrigger = true;
			}

			options = _options;
			speaker = _speaker;
			text = _text;
		}

		public DialogueLine (DialogueSpeaker _speaker, string _text, List<string> _options, List<string> _choices) {
			if (_speaker.name.ToUpper () == "TRIGGER") {
				isTrigger = true;
			}

			options = _options;
			choices = _choices;
			speaker = _speaker;
			text = _text;
		}

		public DialogueLine (DialogueSpeaker _speaker, string _text, bool _isTrigger, List<string> _options) {
			speaker = _speaker;
			text = _text;
			isTrigger = _isTrigger;
			options = _options;
		}

		public DialogueLine (DialogueSpeaker _speaker, string _text, bool _isTrigger, List<string> _options, List<string> _choices) {
			speaker = _speaker;
			text = _text;
			isTrigger = _isTrigger;
			options = _options;
			choices = _choices;
		}
	}

	[System.Serializable]
	public class DialogueSound {
		public string name;
		public AudioClip clip;
		public SoundType soundType;

		public DialogueSound (string _name, AudioClip _clip) {
			name = _name;
			clip = _clip;
			soundType = SoundType.Undefined;
		}

		public DialogueSound (string _name, AudioClip _clip, SoundType _soundType) {
			name = _name;
			clip = _clip;
			soundType = _soundType;
		}

	}

	[System.Serializable]
	public enum SoundType {
		Misc,
		Happy,
		Sad,
		Disappointed,
		Frightened,
		Greeting,
		Farewell,
		Song,
		Whistle,
		Cough,
		Idle,
		Question,
		Thinking,
		Pain,
		Confused,
		Laugh,
		Evil,
		Uncertain,
		Surprised,
		Undefined
	}

	[System.Serializable]
	[CreateAssetMenu(fileName = "SoundLibrary", menuName = "Dialogue/Sound Library", order = 1)]
	public class SoundLibrary : ScriptableObject {
		public List<DialogueSound> sounds;
	}

	[System.Serializable]
	public class DialogueSpeaker {
		public string name;
		public Color nameColor;
		public AudioClip voice;
		public SoundLibrary soundLibrary;
		public TMP_FontAsset font;
		public int fontSize;
		public bool hasEncountered = false;
		public Texture2D symbol;
		public Material glowMaterial;

		public DialogueSpeaker (string _name, Color _nameColor, AudioClip _voice, TMP_FontAsset _font) {
			name = _name;
			nameColor = _nameColor;
			voice = _voice;
			font = _font;
		}

		public DialogueSpeaker (string _name, Color _nameColor, AudioClip _voice, TMP_FontAsset _font, Texture2D _symbol) {
			name = _name;
			nameColor = _nameColor;
			voice = _voice;
			font = _font;
			symbol = _symbol;
		}

		public DialogueSpeaker (string _name, Color _nameColor, AudioClip _voice, TMP_FontAsset _font, SoundLibrary _sounds) {
			name = _name;
			nameColor = _nameColor;
			voice = _voice;
			font = _font;
			soundLibrary = _sounds;
		}

		public DialogueSpeaker (string _name, Color _nameColor, AudioClip _voice, TMP_FontAsset _font, SoundLibrary _sounds, Material _glowMaterial) {
			name = _name;
			nameColor = _nameColor;
			voice = _voice;
			font = _font;
			soundLibrary = _sounds;
			glowMaterial = _glowMaterial;
		}
	}
}