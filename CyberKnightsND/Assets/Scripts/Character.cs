using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] private string Name;
    [SerializeField] private CharacterType type;
    [SerializeField] private Category category;
    [SerializeField] private Typing typing;
    [SerializeField] private int speed;
    [SerializeField] private int MaxSpeed;
    [SerializeField] private int attack;
    [SerializeField] private int maxAttack;
    [SerializeField] private int specialMax;
    [SerializeField] private float CritChance;
    [SerializeField] private float CritDamage;
    [SerializeField] private Move Special1;
    [SerializeField] private Move Special2;
    [SerializeField] private Move Special3;
    [SerializeField] private Move Special4;
    [SerializeField] private Pasive1 pasive1;
    [SerializeField] private Pasive2 pasive2;

    public string GetName()
    {
        return Name;
    }
    public Pasive1 GetPasive1()
    {
        return pasive1;
    }
    public Pasive2 GetPasive2()
    {
        return pasive2;
    }
    public Typing GetTyping()
    {
        return typing;
    }
    public float GetCritChance()
    {
        return CritChance;
    }
    public float GetCritDamage()
    {
        return CritDamage;
    }
    public void SetCritChance(float number)
    {
        CritChance = number;
    }
    public void SetCritDamage(float number)
    {
        CritDamage = number;
    }
    public Move GetAttackType1()
    {
        return Special1;
    }
    public Move GetAttackType2()
    {
        return Special2;
    }
    public Move GetAttackType3()
    {
        return Special3;
    }
    public Move GetAttackType4()
    {
        return Special4;
    }
    public void SetSpeed(int speed)
    {
        this.speed = speed;
    }
    public int GetSpeed()
    {
        return speed;
    }
    public int GetMaxSpeed()
    {
        return MaxSpeed;
    }
    public int GetAttack()
    {
        return attack;
    }
    public int GetMaxAttack()
    {
        return maxAttack;
    }
    public CharacterType GetCharacterType()
    {
        return type;
    }
    public Category GetCategory()
    {
        return category;
    }
    public enum CharacterType
    {
        MainCharacter,
        Archer,
        Tank,
        Mage,
        Enemy,
    }
    
    public enum Pasive1
    {
        CyberTransfusion,
        
        PiercingArrows,

        AstralReflection,
        
        ZeonicCorruption,
       
        None,
    }
    public enum Pasive2
    {
        VitalAbsortion,

        BloodFrenzy,

        MoonShell,

        MysticalDespair,

        None,
    }
    public enum Typing
    {
        Techno,
        Wild,
        Moon,
        Magic,
    }
    public enum Category
    {
        Melee,
        Ranged,
    }

    public enum Move
    {
        BasicAttack,
        Eletrocorte,
        DagaFulgurante,
        MC_Special3,
        MC_Special4,
        DisparoDoble,
        HemoFlecha,
        BolasSalvajes,
        Bow_Special4,
        Tank_Special1,
        Tank_Special2,
        Tank_Special3,
        Tank_Special4,
        Mage_Special1,
        Mage_Special2,
        Mage_Special3,
        Mage_Special4,
        None,
    }
}
