using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveSetChoice : MonoBehaviour
{
    public Button AttackButton;
    public Button DefendButton;
    public Button SpecialMove1;
    public Button SpecialMove2;


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            AttackButton.onClick.Invoke();
        }

        if(Input.GetKeyDown(KeyCode.S))
        {
            DefendButton.onClick.Invoke();
        }

        if(Input.GetKeyDown(KeyCode.D))
        {
            SpecialMove1.onClick.Invoke();
        }

        if(Input.GetKeyDown(KeyCode.F))
        {
            SpecialMove2.onClick.Invoke();
        }
    }
}
