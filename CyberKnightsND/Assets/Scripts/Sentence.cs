using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Sentence 
{
    [SerializeField] [TextArea(3, 10)] private string sentence;
    [SerializeField] private Character characterName;
    [SerializeField] private Sprite characterSprite;
    [SerializeField] private string audioClip;
    public enum Character
    {
        Helah,
        Hades,
        Desconocido,
        Alhan,
        Asalvajado,
        Narrador,
    }
    public string GetAudioClip()
    {
        return audioClip;
    }
    public Character GetCharacterName()
    {
        return characterName;
    }

    public string GetSentence()
    {
        return sentence;
    }

    public Sprite GetSprite()
    {
        return characterSprite;
    }

}
