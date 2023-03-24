using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManageMainMoves : MonoBehaviour
{
    public Button Attack;
    public Button Defend;
    public Button Abilities;
    public GameObject desplegable;

    public void ActivateDeactivateButtons(bool activate)
    {
        Attack.enabled = activate;
        Defend.enabled = activate;
        Abilities.enabled = activate;
    }

    public void RestartInterface()
    {
        desplegable.SetActive(false);
        Attack.enabled = true;
        Defend.enabled = true;
        Abilities.enabled = true;
    }
}
