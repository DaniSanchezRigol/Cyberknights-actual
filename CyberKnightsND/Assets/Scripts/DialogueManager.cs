using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using System;

public class DialogueManager : MonoBehaviour
{
	private static DialogueManager _instance;

	public static DialogueManager Instance
	{
		get
		{
			if (!_instance)
			{
				_instance = GameObject.FindObjectOfType<DialogueManager>();
			}
			return _instance;
		}
	}
	public TextMeshProUGUI nameText;
	public TextMeshProUGUI dialogueText;
	private Queue<Sentence> sentences;
	public Animator animator;
	public Image Helah_sprite;
	public Image Hades_sprite;
	public Image Alhan_sprite;
	public GameObject Flechita;
	public GameObject Canvas;
	public UnityEvent startConversation;
	public Animator fadeAnimator;
	private Image previousActiveSprite;
	private string speakSound = "";
	private bool isTyping = false;
	private Sentence currentSentence;
	private UnityEvent dialogueEndEvent;
	private bool canSkipText;
	private bool audioPlayed;
	 
	// Start is called before the first frame update
	void Start()
    {
		sentences = new Queue<Sentence>();
    }
    private void Update()
    {
        if(Input.GetKey(KeyCode.F))
        {
			DisplayNextSentence();
        }
    }
    private IEnumerator WaitForSeconds(float seconds, Action onTimeComplete)
	{
		yield return new WaitForSeconds(seconds);
		onTimeComplete();
	}
	public void StartDialogue(Dialogue dialogue, UnityEvent dialogueEndEvent) 
	{
		Canvas.SetActive(true);
		animator.SetBool("IsOpen", true);
		this.dialogueEndEvent = dialogueEndEvent;
		sentences.Clear();
		nameText.text = "";
		dialogueText.text = "";
		fadeAnimator.Play("FadeOut");
		StartCoroutine(WaitForSeconds(0.5f, () =>
		{
			foreach (Sentence sentence in dialogue.Sentences)
			{
				sentences.Enqueue(sentence);
			}
			DisplayNextSentence();
		}
			));
		
	}
	public void ManageSpritesSound(string name, Sentence sentence)
    {
		if(previousActiveSprite != null)
			previousActiveSprite.enabled = false;

		switch (name)
		{
			case "Hades":
				Hades_sprite.sprite = sentence.GetSprite();
				Hades_sprite.enabled = true;
				previousActiveSprite = Hades_sprite;
				speakSound = "Hades";
				break;
			case "Helah":
				Helah_sprite.sprite = sentence.GetSprite();
				Helah_sprite.enabled = true;
				previousActiveSprite = Helah_sprite;
				speakSound = "Helah";
				break;
			case "Alhan":
				Alhan_sprite.sprite = sentence.GetSprite();
				Alhan_sprite.enabled = true;
				previousActiveSprite = Alhan_sprite;
				speakSound = "Alhan";
				break;
			case "Asalvajado":
				Alhan_sprite.sprite = sentence.GetSprite();
				Alhan_sprite.enabled = true;
				previousActiveSprite = Alhan_sprite;
				speakSound = "Alhan";
				break;
			case "???":
				speakSound = "Generic";
				break;
			case "":
				speakSound = "Narrador";
				break;
			default:
				speakSound = "Generic";
				break;
		}
	}
	public void DisplayNextSentence()
    {
		if(isTyping)
        {
			StopAllCoroutines();
			isTyping = false;
			Flechita.SetActive(true);
			dialogueText.text = currentSentence.GetSentence();
			return;
		}
		if(sentences.Count == 0)
        {
			StopAllCoroutines();
			EndDialogue();
			return;
        }
		currentSentence = sentences.Dequeue();

		string name = currentSentence.GetCharacterName().ToString();
		if (name == "Desconocido")
		{
			name = "???";
		}
		if(name == "Narrador")
        {
			name = "";
        }
		nameText.text = name;
		ManageSpritesSound(name, currentSentence);

		StopAllCoroutines();
		StartCoroutine(TypeSentence(currentSentence.GetSentence()));
		StartCoroutine(CharacterSpeaks());
		if (audioPlayed == true)
		{
			SoundManager.Instance.StopSoundFX();
		}
		if(currentSentence.GetAudioClip() != null)
        {
			SoundManager.Instance.PlaySoundFX(currentSentence.GetAudioClip(), 0.5f);
			audioPlayed = true;
			return;
        }
		audioPlayed = false;
    }
	IEnumerator TypeSentence (string sentence)
    {
		isTyping = true;
		Flechita.SetActive(false);
		dialogueText.text = "";
		foreach(char letter in sentence.ToCharArray())
        {
			dialogueText.text += letter;
			canSkipText = true;
			yield return new WaitForSeconds(0.02f);
        }
		isTyping = false;
		Flechita.SetActive(true);

	}
	IEnumerator CharacterSpeaks()
    {
		while(isTyping == true)
        {
			//if(speakSound == "Hades")
   //         {
			//	int number = Random.Range(0, 2);
			//	if(number == 0)
			//		SoundManager.Instance.PlaySoundFX("Hades1", 1f);
			//	else
			//		SoundManager.Instance.PlaySoundFX("Hades2", 1f);
			//	yield return new WaitForSeconds(0.15f);
			//}
   //         else
           // {
				SoundManager.Instance.PlaySoundFX(speakSound, 1f);
				yield return new WaitForSeconds(0.09f);
			//}
		}
    }
	public bool GetCanSkipText()
    {
		return canSkipText;
    }
	void EndDialogue()
    {
		canSkipText = false;
		if(previousActiveSprite != null)
			previousActiveSprite.enabled = false;
		animator.SetBool("IsOpen", false);
		Debug.Log("End of dialogue");
		fadeAnimator.Play("FadeIn");
		StartCoroutine(WaitForSeconds(1f, () =>
		{
			Canvas.SetActive(false);
			dialogueEndEvent.Invoke();
			
		}));
	}
}
