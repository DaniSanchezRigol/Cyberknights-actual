using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System;
public class BattleExit : MonoBehaviour
{
    private static BattleExit _instance;
    public Animator fadeAnimator;
    public static BattleExit Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = GameObject.FindObjectOfType<BattleExit>();
            }
            return _instance;
        }
    }
    public void ExitBattle(UnityEvent exitBattleEvent)
    {
        fadeAnimator.Play("FadeIn");
        StartCoroutine(WaitForSeconds(1f, () =>
        {
            SceneManager.LoadScene("MainScene");
            if (SceneManager.GetActiveScene().name != "MainScene")
            {
                StartCoroutine("waitForSceneLoad", exitBattleEvent);
            }
        }));
        

    }
    IEnumerator waitForSceneLoad(UnityEvent exitBattleEvent)
    {
        while (SceneManager.GetActiveScene().name != "MainScene")
        {
            yield return null;
        }

        if (SceneManager.GetActiveScene().name == "MainScene")
        {
            exitBattleEvent.Invoke();
        }
    }
    private IEnumerator WaitForSeconds(float seconds, Action onTimeComplete)
    {
        yield return new WaitForSeconds(seconds);
        onTimeComplete();
    }
}
