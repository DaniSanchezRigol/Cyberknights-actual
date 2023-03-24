using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;
    public UnityEvent dialogueEndEvent;
    public string musicName;
    public string soundEffectsName;
    public GameObject backgroundImage;
    public void TriggerDialogue()
    {
        if (backgroundImage != null)
            backgroundImage.SetActive(true);
        if (musicName != null)
            SoundManager.Instance.PlayAmbienceSoundBackground(musicName, true, 1f);
        if (soundEffectsName != null)
            SoundManager.Instance.PlaySoundBackground(soundEffectsName, true, 0.3f);
        DialogueManager.Instance.StartDialogue(dialogue, dialogueEndEvent);
    }
}
