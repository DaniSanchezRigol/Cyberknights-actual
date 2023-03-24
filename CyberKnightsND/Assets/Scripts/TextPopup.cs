using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextPopup : MonoBehaviour
{
    private const float DISAPPEAR_TIMER_MAX = 1.2f;
    private float dissapearTimer = 1.2f;
    private Vector3 moveVector;
    private Vector3 moveVector2;
    private TextMeshPro textMeshPro;
    private float alphaDecrease = 1f;
    private void Awake()
    {
        textMeshPro = GetComponent<TextMeshPro>();
        moveVector = new Vector3(0f, 0f, 1f) * 10f;
        moveVector2 = new Vector3(0f, 0f, 1f) * 7f;
    }
    public static void CreateEffectivenessPopup(bool isAlly, Vector3 position, CharacterBattle.DamageType effectiveness, bool isCriticalHit)
    {
        if (isCriticalHit)
        {
            Instantiate(Assets.i.pfCriticalPopup, position + new Vector3(0, 0.3f, 0), Quaternion.identity);
        }

        switch (effectiveness)
        {
            case CharacterBattle.DamageType.Effective:
                Instantiate(Assets.i.pfEffectivePopup, position, Quaternion.identity);
                break;
            case CharacterBattle.DamageType.Neutral:
                break;
            case CharacterBattle.DamageType.Uneffective:
                Instantiate(Assets.i.pfUneffectivePopup, position, Quaternion.identity);
                break;
        }
    }

    private void Update()
    {
        if (dissapearTimer > DISAPPEAR_TIMER_MAX * 0.6f)
        {
            //First half of the popup lifetime
            //float increaseScaleAmount = 0.1f;
            //transform.localScale += Vector3.one * increaseScaleAmount * Time.deltaTime;
            transform.position += moveVector * Time.deltaTime;
            moveVector -= moveVector * 10f * Time.deltaTime;
        }

        if (dissapearTimer < DISAPPEAR_TIMER_MAX * 0.5f)
        {
            //Second half of the popup lifetime
            //float decreaseScaleAmount = 0.1f;
            //transform.localScale -= Vector3.one * decreaseScaleAmount * Time.deltaTime;
            textMeshPro.color = new Color(textMeshPro.color.r, textMeshPro.color.g, textMeshPro.color.b, alphaDecrease);
            alphaDecrease -= alphaDecrease * 5f * Time.deltaTime;
        }
        dissapearTimer -= Time.deltaTime;
        if (alphaDecrease < 0)
        {
            this.gameObject.SetActive(false);
        }
    }
}
