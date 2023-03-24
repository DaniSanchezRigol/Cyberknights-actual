using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ExplicacionManager : MonoBehaviour
{
    public void ShowExplicacion(GameObject explicacion)
    {
        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            explicacion.SetActive(true);
        }
    }
}
