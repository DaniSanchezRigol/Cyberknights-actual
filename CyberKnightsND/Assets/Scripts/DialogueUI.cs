using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class DialogueUI : MonoBehaviour{
    public UnityEvent nextSentence;
    private void Update()
    {
        if(DialogueManager.Instance.GetCanSkipText() == true)
            if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.X))
            {
                nextSentence.Invoke();
            }

    }
}
