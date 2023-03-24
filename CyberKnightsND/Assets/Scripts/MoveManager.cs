using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveManager : MonoBehaviour
{
    [System.Serializable]
    public class Move
    {
        public string MoveName;
        public int damagePercent;
        public int cost;
    }
    [SerializeField] private Move[] availableMoves;
	public static MoveManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }

    public int getMoveDamage(string name)
    {
        for(int i = 0; i< availableMoves.Length; i++)
        {
            if(availableMoves[i].MoveName == name)
            {
                return availableMoves[i].damagePercent;
            }
        }
        return 0;
    }
    public int getMoveCost(string name)
    {
        for(int i = 0; i< availableMoves.Length; i++)
        {
            if(availableMoves[i].MoveName == name)
            {
                return availableMoves[i].cost;
            }
        }
        return 0;
    }
    
}
