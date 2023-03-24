using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class DamagePopup : MonoBehaviour
{
    private const float DISAPPEAR_TIMER_MAX = 1f;
    private TextMeshPro textMesh;
    private SpriteRenderer sprite;
    private float dissapearTimer = 1f;
    private Color textColor;
    private Vector3 moveVector;
    private float alphaDecrease = 1f;

    private void Awake()
    {
        textMesh = transform.GetComponent<TextMeshPro>();
        sprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
    }
    //Create a DamagePopup
    public static DamagePopup CreateDamagePopup(Vector3 position, int damageAmount, int fontSize, Color textColor, float scaleMultiplier, bool isCriticalHit)
    {
        Transform damagePopupTransform = Instantiate(Assets.i.pfDamagePopup, position, Quaternion.identity);
        DamagePopup damagePopup = damagePopupTransform.GetComponent<DamagePopup>();
        damagePopup.SetupDamagePopup(damageAmount, fontSize, textColor,scaleMultiplier, isCriticalHit);
        return damagePopup;
    }
    public static DamagePopup CreateDamagePopupSimple(Vector3 position, int damageAmount, int fontSize, Color textColor)
    {
        Transform damagePopupTransform = Instantiate(Assets.i.pfDamagePopup, position, Quaternion.identity);
        DamagePopup damagePopup = damagePopupTransform.GetComponent<DamagePopup>();
        damagePopup.SetupDamagePopupSimple(damageAmount, fontSize, textColor);
        return damagePopup;
    }
    public void SetupDamagePopupSimple(int damageAmount, int fontSize, Color textColor)
    {
        textMesh.SetText(damageAmount.ToString());
        textMesh.fontSize = fontSize;
        textMesh.color = textColor;
        moveVector = new Vector3(0f, 0.5f) * 7f;
    }
    public void SetupDamagePopup(int damageAmount, int fontSize, Color textColor, float scaleMultiplier, bool isCriticalHit)
    {
        this.transform.localScale = this.transform.localScale * scaleMultiplier;
        textMesh.SetText(damageAmount.ToString());
        textMesh.fontSize = fontSize;
        if(isCriticalHit)
        {
            sprite.gameObject.SetActive(true);
        }
        textMesh.color = textColor;
        moveVector = new Vector3(0f, 0.5f) * 7f;
    }

    private void Update()
    {       

        if(dissapearTimer > DISAPPEAR_TIMER_MAX * 0.6f)
        {
            //First half of the popup lifetime
            float increaseScaleAmount = 0.05f;
            transform.localScale += Vector3.one * increaseScaleAmount * Time.deltaTime;
            transform.position += moveVector * Time.deltaTime;
            moveVector -= moveVector * 8f * Time.deltaTime;
        }

        if (dissapearTimer < DISAPPEAR_TIMER_MAX * 0.4f)
        {
            //Second half of the popup lifetime
            float decreaseScaleAmount = 0.2f;
            transform.localScale -= Vector3.one * decreaseScaleAmount * Time.deltaTime;
            textMesh.color = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, alphaDecrease);
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, alphaDecrease);
            alphaDecrease -= alphaDecrease * 8f * Time.deltaTime;
        }
        dissapearTimer -= Time.deltaTime;
        if(transform.localScale.x < 0)
        {
            this.gameObject.SetActive(false);
        }
    }
}
