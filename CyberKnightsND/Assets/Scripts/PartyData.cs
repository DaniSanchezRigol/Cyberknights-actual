using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyData : MonoBehaviour
{
	private static PartyData _instance;

	public static PartyData Instance
	{
		get
		{
			if (!_instance)
			{
				_instance = GameObject.FindObjectOfType<PartyData>();
			}
			return _instance;
		}
	}
	[SerializeField] private Transform[] AllySetup;
	CharacterBattle Helah;
    CharacterBattle Alhan;
    private void Start()
    {
    }

    public void GetCharacterData()
    {
    }
	public Transform[] GetAllySetup()
    {
		return AllySetup;
    }
}

