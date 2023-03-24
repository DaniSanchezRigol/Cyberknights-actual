using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class BattleInfo : MonoBehaviour
{
    [SerializeField] private Transform[] enemyCharacters;
    [SerializeField] private UnityEvent endBattleEvent;
    public void BattleOn()
    {

        SceneManager.LoadScene("BattleScene");
        if (SceneManager.GetActiveScene().name != "BattleScene")
        {
            StartCoroutine("waitForSceneLoad");
        }
        
    }
    IEnumerator waitForSceneLoad()
    {
        while (SceneManager.GetActiveScene().name != "BattleScene")
        {
            yield return null;
        }

        if (SceneManager.GetActiveScene().name == "BattleScene")
        {
            BattleManager.Instance.StartBattle(enemyCharacters, endBattleEvent);
        }
    }
}
