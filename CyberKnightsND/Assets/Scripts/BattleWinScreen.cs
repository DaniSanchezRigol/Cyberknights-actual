using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleWinScreen : MonoBehaviour
{
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.X))

            BattleExit.Instance.ExitBattle(BattleManager.Instance.getEndOfBattleEvent());
    }
}
