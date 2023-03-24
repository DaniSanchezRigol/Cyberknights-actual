using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OtherTextPopups : MonoBehaviour
{
    private const float DISAPPEAR_TIMER_MAX = 1.5f;
    private float dissapearTimer = 1.5f;
    private Vector3 moveVector;
    private Vector3 moveVector2;
    private TextMeshPro textMesh;
    private float alphaDecrease = 1f;
    private void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
        moveVector = new Vector3(0f, -0.5f) * 1f;
    }
    public static void CreateEffectPopup(Vector3 position, string type)
    {
        switch(type)
        {
            case "Bleeding":
                Instantiate(Assets.i.pfBleedingPopup, position + new Vector3(0.7f, 1f, 0), Quaternion.identity);
                break;
            case "BloodFrenzy":
                Instantiate(Assets.i.pfBloodFrenzyPopup, position + new Vector3(0f, 1f, 0), Quaternion.identity);
                break;
            case "Ralentizado":
                Instantiate(Assets.i.pfRalentizadoPopup, position + new Vector3(0f, 1f, 0), Quaternion.identity);
                break;
        }  
    }
    public static void CreateNumberPopup(Vector3 position, string type, int amount)
    {
        switch (type)
        {
            case "EnergyRecover":
                Transform energyPopup = Instantiate(Assets.i.pfDamagePopup, position + new Vector3(0f, 2.3f, 0.2f), Quaternion.identity);
                TextMeshPro TMProcomponent = energyPopup.GetComponent<TextMeshPro>();
                TMProcomponent.SetText(amount.ToString());
                TMProcomponent.color = new Color32(0, 100, 255, 255);
                break;
            case "Heal":
                Transform energyPopup1 = Instantiate(Assets.i.pfDamagePopup, position + new Vector3(0f, 2.3f, -0.2f), Quaternion.identity);
                TextMeshPro TMProcomponent1 = energyPopup1.GetComponent<TextMeshPro>();
                TMProcomponent1.SetText(amount.ToString());
                TMProcomponent1.color = new Color32(83, 212, 55, 255);
                break;
        }
    }

    private void Update()
    {
        if(dissapearTimer > DISAPPEAR_TIMER_MAX * 0.85f)
        {
             float increaseScaleAmount = 0.2f;
            transform.localScale += Vector3.one * increaseScaleAmount * Time.deltaTime;
        }
        if (dissapearTimer < DISAPPEAR_TIMER_MAX * 0.60f)
        {
            //First half of the popup lifetime
            float decreaseScaleAmount = 0.1f;
            transform.localScale -= Vector3.one * decreaseScaleAmount * Time.deltaTime;
            //transform.position += moveVector * Time.deltaTime;
            //moveVector -= moveVector * 6f * Time.deltaTime;
        }

        if (dissapearTimer < DISAPPEAR_TIMER_MAX * 0.45f)
        {
            //Second half of the popup lifetime
            transform.position += moveVector * Time.deltaTime;
            textMesh.color = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, alphaDecrease);
            alphaDecrease -= alphaDecrease * 8f * Time.deltaTime;
        }
        dissapearTimer -= Time.deltaTime;
        if (alphaDecrease < 0)
        {
            this.gameObject.SetActive(false);
        }
    }
}
