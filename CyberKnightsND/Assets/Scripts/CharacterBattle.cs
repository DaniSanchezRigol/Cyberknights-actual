using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CharacterBattle : Character
{
    private CharacterBase characterBase;
    private State state;
    private Vector3 slideTargetPosition;
    private Action onSlideComplete;
    private Quaternion initialRotation;
    private GameObject selectionCircleGameObject;
    private GameObject selectionEnemyGameObject;
    [SerializeField] private BattleManager.LanePosition lanePosition;
    private bool isPlayerTeam;
    private bool bleeding = false;
    private int bleedingTurns = 0;
    private int bleedDamage = 0;
    private CharacterBattle AttackingCharacter;
    private bool finishedDeadAnim = false;
    [SerializeField] private bool Attacked = false;

    [SerializeField] private int currentHealth;
    [SerializeField] private int MaxHealth = 100;
    [SerializeField] private int currentEnergy;
    [SerializeField] private int MaxEnergy = 100;
    public HealthBar Healthbar;
    public EnergyBar EnergyBar;
    public GameObject BleedingIcon;
    public TextMeshProUGUI bleedingTurnsText;
    public GameObject selectFeedback;

    public enum DamageType
    {
        Effective,
        Neutral,
        Uneffective,
    }
    public enum State
    {
        Idle,
        Run,
        Roll,
        Blocking,
        Busy,
    }
 
    private void Awake()
    {
        characterBase = GetComponent<CharacterBase>();
        initialRotation = transform.rotation;
        selectionCircleGameObject = transform.Find("ActiveCircle").gameObject;
        HideSelectionTarget();
        HideActiveCircle();
        state = State.Idle;
    }
    private void Start()
    {
        currentHealth = MaxHealth;
        Healthbar.SetMaxHealth(MaxHealth);
        currentEnergy = MaxEnergy;
        EnergyBar.SetMaxEnergy(MaxEnergy);
    }

    private void Update()
    {
        switch(state) {
            case State.Idle:
                break;
            case State.Busy:
                break;
            case State.Run:
                float slideSpeed = 6f;
                transform.position += (slideTargetPosition - GetPosition()) * slideSpeed * Time.deltaTime;
                float reachedDistance = 0.05f;
                if (Vector3.Distance(GetPosition(), slideTargetPosition) < reachedDistance) {
                    //Llega a la posición del objetivo
                    transform.position = slideTargetPosition;
                    onSlideComplete();
                }
                break;

            case State.Roll:
                slideSpeed = 6f;
                transform.position += (slideTargetPosition - GetPosition()) * slideSpeed * Time.deltaTime;
                reachedDistance = 0.05f;
                if (Vector3.Distance(GetPosition(), slideTargetPosition) < reachedDistance)
                {
                    //Llega a la posición del objetivo
                    transform.position = slideTargetPosition;
                    onSlideComplete();
                }
                break;
        }
    }
    public bool IsBleeding()
    {
        return bleeding;
    }
    public void StartBleeding(int turns, int damage)
    {
        SoundManager.Instance.PlaySoundFX("FX_BLOOD", 1f);
        bleeding = true;
        bleedingTurns = turns;
        bleedingTurnsText.SetText(turns.ToString());
        bleedDamage = damage;
        BleedingIcon.SetActive(true);
    }
    public Vector3 GetPosition()
    {
        return transform.position;
    }
    public void NonTargetedDamage(string damageType)
    {
        if(damageType == "Bleed")
        {
            currentHealth -= bleedDamage;
            SoundManager.Instance.PlaySoundFX("FX_BLOOD", 1f);
            Healthbar.SetHealth(currentHealth);
            Color color = new Color32(255, 0, 0, 255);
            DamagePopup.CreateDamagePopupSimple(transform.position + new Vector3(0f, 2f, 0f), bleedDamage, 30, color);
            OtherTextPopups.CreateEffectPopup(transform.position + new Vector3(0f, 0.5f, -0.7f), "Bleeding");
            if (IsDead())
            {
                characterBase.PlayAnimDeath();
            }
            else
            {
                characterBase.PlayAnimDamaged();
                if (state == State.Blocking)
                {
                    characterBase.PlayAnimBlocking();
                }
            }
            bleedingTurns--;
            bleedingTurnsText.SetText(bleedingTurns.ToString());
            if (bleedingTurns <= 0)
            {
                bleeding = false;
                BleedingIcon.SetActive(false);
            }
        }
    }
    public void RestoreEnergy(int amount)
    {
        if(amount == 0)
        {
            amount = 1;
        }
        if (currentEnergy + amount > MaxEnergy)
        {
            if (MaxEnergy != currentEnergy)
            {
                OtherTextPopups.CreateNumberPopup(transform.position, "EnergyRecover", MaxEnergy - currentEnergy);
                currentEnergy = MaxEnergy;
                EnergyBar.SetEnergy(currentEnergy);
            }
        }
        else
        {
            currentEnergy += amount;
            OtherTextPopups.CreateNumberPopup(transform.position, "EnergyRecover", amount);
            EnergyBar.SetEnergy(currentEnergy);
        }
    }
    public void Heal(int healAmount)
    {
        print(healAmount);
        if (healAmount == 0)
        {
            healAmount = 1;
        }

        if (currentHealth + healAmount > MaxHealth)
        {
            if (MaxHealth != currentHealth)
            {
                OtherTextPopups.CreateNumberPopup(transform.position, "Heal", MaxHealth - currentHealth);
                currentHealth = MaxHealth;
                Healthbar.SetHealth(currentHealth);
            }
        }
        else
        {
            currentHealth += healAmount;
            OtherTextPopups.CreateNumberPopup(transform.position, "Heal", healAmount);
            Healthbar.SetHealth(currentHealth);
        }
        print(healAmount);
    }

    private int Damage(int damageAmount, CharacterBattle attackerCharacter)
    {
        float scaleMultiplier = 1f;
        int fontSize = 35;
        Color textColor = new Color32(255, 37, 0, 255);
        DamageType attackEffect = DamageType.Neutral;
        bool isCriticalHit = UnityEngine.Random.Range(0, 100) < attackerCharacter.GetCritChance();

        switch(attackerCharacter.GetTyping())
        {
            case Typing.Techno:
                textColor = new Color32(64, 224, 208, 255);
                switch (this.GetTyping())
                {
                    case Typing.Techno:
                        break;
                    case Typing.Wild:
                        attackEffect = DamageType.Effective;
                        break;
                    case Typing.Moon:
                        break;
                    case Typing.Magic:
                        attackEffect = DamageType.Uneffective;
                        break;
                }
                break;
            case Typing.Wild:
                textColor = new Color32(125, 55, 28, 255);
                switch (this.GetTyping())
                {
                    case Typing.Techno:
                        attackEffect = DamageType.Uneffective;
                        break;
                    case Typing.Wild:
                        break;
                    case Typing.Moon:
                        attackEffect = DamageType.Effective;
                        break;
                    case Typing.Magic:
                        break;
                }
                break;
            case Typing.Moon:
                textColor = new Color32(255, 246, 88, 255);
                switch (this.GetTyping())
                {
                    case Typing.Techno:
                        break;
                    case Typing.Wild:
                        attackEffect = DamageType.Uneffective;
                        break;
                    case Typing.Moon:
                        break;
                    case Typing.Magic:
                        attackEffect = DamageType.Effective;
                        break;
                }
                break;
            case Typing.Magic:
                textColor = new Color32(155, 76, 255, 255);
                switch (this.GetTyping())
                {
                    case Typing.Techno:
                        attackEffect = DamageType.Effective;
                        break;
                    case Typing.Wild:
                        break;
                    case Typing.Moon:
                        attackEffect = DamageType.Uneffective;
                        break;
                    case Typing.Magic:
                        break;
                }
                break;
        }

        switch(attackEffect)
        {
            case DamageType.Effective:
                //scaleMultiplier = 0.9f;
                fontSize = 40;
                damageAmount = Convert.ToInt32(damageAmount * 1.5f);
                SoundManager.Instance.PlaySoundFX("FX_EFFECTIVE", 1f);
                break;
            case DamageType.Neutral:
                //scaleMultiplier = 0.7f;
                SoundManager.Instance.PlaySoundFX("FX_NEUTRAL", 1f);
                break;
            case DamageType.Uneffective:
                //scaleMultiplier = 0.5f;
                fontSize = 30;
                damageAmount = Convert.ToInt32(damageAmount * 0.5f);
                SoundManager.Instance.PlaySoundFX("FX_UNEFFECTIVE", 1f);
                break;
        }

        if(isCriticalHit)
        {
            damageAmount = Convert.ToInt32(damageAmount * (attackerCharacter.GetCritDamage()/100));
            SoundManager.Instance.PlaySoundFX("FX_CRITICALHIT", 1f);
            fontSize = 40;
            //scaleMultiplier = 1.1f;
            //textColor = new Color32(255, 0, 0, 255);
        }

        if(state == State.Blocking)
        {
            if (!isCriticalHit)
            {
                damageAmount = Convert.ToInt32(damageAmount * 0.5f);
                SoundManager.Instance.PlaySoundFX("FX_DEFENDBLOCK", 1f);
                //scaleMultiplier = 0.5f;
                fontSize = 25;
            }
            state = State.Idle;
        }

        //AstralReflectionI(damageAmount, attackerCharacter);
        //if(isCriticalHit && attackEffect == DamageType.Effective)
        //{
        //    MoonShell();
        //}
        currentHealth -= damageAmount;
        Healthbar.SetHealth(currentHealth);

        if(IsPlayerTeam())
        {
            TextPopup.CreateEffectivenessPopup(true, transform.position + new Vector3(0f, 0.3f, -0.5f), attackEffect, isCriticalHit);
        }
        else
        {
            TextPopup.CreateEffectivenessPopup(false, transform.position + new Vector3(0f, 0.3f, -0.5f), attackEffect, isCriticalHit);
        }
       
        DamagePopup.CreateDamagePopup(transform.position + new Vector3 (0f, 2f, 0f), damageAmount,fontSize, textColor, scaleMultiplier, isCriticalHit);
        CinemachineShake.Instance.ShakeCamera(1f, .10f);
        if (IsDead())
        {
            characterBase.PlayAnimDeath();
            StartCoroutine(CyberTransfusion(attackerCharacter));
        }
        else
        {
           characterBase.PlayAnimDamaged();
        }
        return damageAmount;
    }

    private void AstralReflection(int damage, CharacterBattle attackerCharacter)
    {
        if (GetPasive1() == Character.Pasive1.AstralReflection)
        {
            attackerCharacter.NonTargetedDamage("Bleed");
        }
    }

    private int MoonShell(int damage)
    {
        if (GetPasive2() == Character.Pasive2.MoonShell)
        {
            damage = (damage * 10) / 100;
        }

        return damage;
    }

    private IEnumerator CyberTransfusion(CharacterBattle character)
    {
        yield return new WaitUntil(()=> finishedDeadAnim == true);
        if (character.GetPasive1() == Character.Pasive1.CyberTransfusion)
        {
            character.RestoreEnergy(Convert.ToInt32((MaxEnergy * 15) / 100));
        }
        yield return null;
    }

    private void VitalAbsortion(int damageAmount)
    {
        if (GetPasive2() == Character.Pasive2.VitalAbsortion)
        {
            Heal(Convert.ToInt32((damageAmount * 50) / 100));
        }
    }
    public bool IsDead()
    {
        if(currentHealth <= 0)
        {
            return true;
        }
        return false;
    }

    public void Setup(BattleManager.LanePosition lanePosition, bool isPlayerTeam)
    {
        this.lanePosition = lanePosition;
        this.isPlayerTeam = isPlayerTeam;
        if(this.GetName() == "Helah")
        {

        }
        if(this.GetName() == "Alhan")
        {

        }
    }
    public void BasicAttack(CharacterBattle targetCharacterBattle, Action onAttackComplete)
    {
        Vector3 slideTargetPosition = targetCharacterBattle.GetPosition() + (GetPosition() - targetCharacterBattle.GetPosition()).normalized * 1f;
        int damage = 0;
        if (GetCategory() == Category.Ranged)
        {
            state = State.Busy;
            transform.LookAt(slideTargetPosition);
            SoundManager.Instance.PlaySoundFX("FX_ARCHERBASICATTACK", 1f);
            characterBase.PlayAnimBow(() => {
                targetCharacterBattle.Damage(GetAttack(), this);
            }, () => {
                    state = State.Idle;
                    transform.rotation = initialRotation;
                    onAttackComplete();
                });
        }
        else
        {
            Vector3 startingPosition = GetPosition();
            transform.LookAt(slideTargetPosition);
            //Moverse hacia el objetivo
            RunToPosition(slideTargetPosition, () => {
                //Llega al objetivo y ataca
                state = State.Busy;
                Vector3 attackDir = (targetCharacterBattle.GetPosition() - GetPosition()).normalized;
                characterBase.PlayAnimAttack(() => {
                    SoundManager.Instance.PlaySoundFX("FX_MCBASICATTACK", 1f);
                    damage = targetCharacterBattle.Damage(GetAttack(), this);
                }, () => {
                    RollToPosition(startingPosition, () => {
                        //Movimiento completado, de vuelta a idle
                        VitalAbsortion(damage);
                        state = State.Idle;
                        transform.rotation = initialRotation;
                        onAttackComplete();
                    });

                });
            });
        }
    }
    public void Defend ()
    {
        if (state != State.Blocking)
        {
            characterBase.PlayAnimBlocking();
            state = State.Blocking;
        }
        SoundManager.Instance.PlaySoundFX("FX_DEFENDCAST", 1f);
        increaseEnergy(10);
    }
    public void Electrocorte(List<CharacterBattle> targetCharacterBattle, Action onAttackComplete)
    {
        Vector3 slideTargetPosition;
        if (targetCharacterBattle.Count == 2)
        {
            Vector3 positionBetweenCharacters = ((targetCharacterBattle[0].GetPosition() + targetCharacterBattle[1].GetPosition()) / 2);
            slideTargetPosition = positionBetweenCharacters + (GetPosition() - targetCharacterBattle[0].GetPosition()).normalized * 2f;
        }
        else
        {
            slideTargetPosition = targetCharacterBattle[0].GetPosition() + (GetPosition() - targetCharacterBattle[0].GetPosition()).normalized * 2f;
        }

        decreaseEnergy(MoveManager.Instance.getMoveCost("Electrocorte"));
            Vector3 startingPosition = GetPosition();
        transform.LookAt(slideTargetPosition);
        //Moverse hacia el objetivo
        RunToPosition(slideTargetPosition, () => {
            //Llega al objetivo y ataca
            state = State.Busy;
            characterBase.PlayAnim("Electrocorte", () => {
                int damage = (MoveManager.Instance.getMoveDamage("Electrocorte") * GetAttack()) / 100;
                foreach (CharacterBattle characterBattle in targetCharacterBattle)
                {
                    characterBattle.Damage(damage, this);
                }
                SoundManager.Instance.PlaySoundFX("FX_MCSPECIALATTACK1", 1f);
                SoundManager.Instance.PlaySoundFX("FX_MCSPECIALATTACK1_2", 1f);

            }, () => {
                RollToPosition(startingPosition, () => {
                    //Movimiento completado, de vuelta a idle
                    state = State.Idle;
                    transform.rotation = initialRotation;
                    onAttackComplete();
                });

            });
        });
    }
    public void HemoFlecha(CharacterBattle targetCharacterBattle, Action onAttackComplete)
    {
        Vector3 slideTargetPosition = targetCharacterBattle.GetPosition() + (GetPosition() - targetCharacterBattle.GetPosition()).normalized * 1f;
        state = State.Busy;
        transform.LookAt(slideTargetPosition);
        decreaseEnergy(MoveManager.Instance.getMoveCost("HemoFlecha"));
        SoundManager.Instance.PlaySoundFX("FX_ARCHERBASICATTACK", 1f);
        
        characterBase.PlayAnimBow(() => {
            int damage = (MoveManager.Instance.getMoveDamage("HemoFlecha") * GetAttack()) / 100;
            targetCharacterBattle.Damage(damage, this);
            targetCharacterBattle.StartBleeding(2, Convert.ToInt32(damage/5));
            OtherTextPopups.CreateEffectPopup(targetCharacterBattle.transform.position + new Vector3(0f, 0.5f, -0.7f), "Bleeding");
        }, () => {
            state = State.Idle;
            transform.rotation = initialRotation;
            onAttackComplete();
        });
    }
  
public void DagaFulgurante(CharacterBattle targetCharacterBattle, Action onAttackComplete)
    {
        Vector3 slideTargetPosition = targetCharacterBattle.GetPosition() + (GetPosition() - targetCharacterBattle.GetPosition()).normalized * 1f;
        Vector3 startingPosition = GetPosition();
        int damage = 0;
        transform.LookAt(slideTargetPosition);
        decreaseEnergy(MoveManager.Instance.getMoveCost("DagaFulgurante"));
        //Moverse hacia el objetivo
        RunToPosition(slideTargetPosition, () => {
            //Llega al objetivo y ataca
            state = State.Busy;
            Vector3 attackDir = (targetCharacterBattle.GetPosition() - GetPosition()).normalized;
            characterBase.PlayAnim2hits("DagaFulgurante", () => {
                damage = targetCharacterBattle.Damage((MoveManager.Instance.getMoveDamage("DagaFulgurante") * GetAttack()) / 100, this);
                SoundManager.Instance.PlaySoundFX("FX_MCSPECIALATTACK2", 1f);
            },()=> {
                damage = targetCharacterBattle.Damage((MoveManager.Instance.getMoveDamage("DagaFulgurante") * GetAttack()) / 100, this);
                SoundManager.Instance.PlaySoundFX("FX_MCSPECIALATTACK2", 1f);
            }, () => {
                RollToPosition(startingPosition, () => {
                    //Movimiento completado, de vuelta a idle
                    VitalAbsortion(damage);
                    state = State.Idle;
                    transform.rotation = initialRotation;
                    onAttackComplete();
                });

            });
        });
    }
    public void BolasSalvajes(CharacterBattle targetCharacterBattle, Action onAttackComplete)
    {
        Vector3 slideTargetPosition = targetCharacterBattle.GetPosition() + (GetPosition() - targetCharacterBattle.GetPosition()).normalized * 1f;
        state = State.Busy;
        transform.LookAt(slideTargetPosition);
        decreaseEnergy(MoveManager.Instance.getMoveCost("BolasSalvajes"));
        SoundManager.Instance.PlaySoundFX("FX_ARCHERBASICATTACK", 1f);

        characterBase.PlayAnimBow(() => {
            int damage = (MoveManager.Instance.getMoveDamage("BolasSalvajes") * GetAttack()) / 100;
            targetCharacterBattle.Damage(damage, this);
            decreaseSpeed(targetCharacterBattle, 80);
            OtherTextPopups.CreateEffectPopup(targetCharacterBattle.transform.position + new Vector3(0f, 0.5f, -0.7f), "Ralentizado");
        }, () => {
            state = State.Idle;
            transform.rotation = initialRotation;
            onAttackComplete();
        });
    }
    private void decreaseSpeed(CharacterBattle character, int percentage)
    {
        if((character.GetSpeed() * percentage) / 100 < 1)
        {
            character.SetSpeed(character.GetSpeed() - 1);
        }
        else
        {
            character.SetSpeed(Convert.ToInt32(character.GetSpeed() - ((character.GetSpeed() * percentage) / 100)));
        }
    }
    private void decreaseEnergy(int number)
    {
        currentEnergy -= number;
        EnergyBar.SetEnergy(currentEnergy);
        if(currentEnergy < 0)
        {
            currentEnergy = 0;
        }
    }
    private void increaseEnergy(int number)
    {
        currentEnergy += number;
        EnergyBar.SetEnergy(currentEnergy);
        if (currentEnergy > MaxEnergy)
        {
            currentEnergy = 100;
        }
    }
    private void RunToPosition(Vector3 slideTargetPosition, Action onSlideComplete)
    {
        this.slideTargetPosition = slideTargetPosition;
        this.onSlideComplete = onSlideComplete;
        state = State.Run;
        characterBase.PlayAnimRun();
    } 
    private void RollToPosition(Vector3 slideTargetPosition, Action onSlideComplete)
    {
        this.slideTargetPosition = slideTargetPosition;
        this.onSlideComplete = onSlideComplete;
        SoundManager.Instance.PlaySoundFX("FX_ROLLBACK", 1f);
        state = State.Roll;
        characterBase.PlayAnimRollAway();
    }
    public void HideSelectionTarget()
    {
        selectFeedback.SetActive(false);
    }
    public void ShowSelectionTarget()
    {
        selectFeedback.SetActive(true);
    }
    public void HideActiveCircle()
    {
        selectionCircleGameObject.SetActive(false);
    }
    public void ShowActiveCircle()
    {
        selectionCircleGameObject.SetActive(true);
    }
    public BattleManager.LanePosition GetLanePosition()
    {
        return lanePosition;
    }
    
    public void ShowMoves()
    {

    }
    public bool IsPlayerTeam()
    {
        return isPlayerTeam;
    }

    public void PlayStepSound()
    {
        SoundManager.Instance.PlaySoundFX("FX_STEPMETALIC", 1f);
    }
    public IEnumerator DeleteCharacter()
    {
        finishedDeadAnim = true;
        yield return new WaitForSeconds(0.2f);
        this.gameObject.SetActive(false);
    }
    public State GetState()
    {
        return state;
    }
    public void SetState(State state)
    {
        if (this.state == State.Blocking && state == State.Idle)
        {
            characterBase.PlayAnimIdle();
        }
        this.state = state;
    }
    public int GetEnergy()
    {
        return currentEnergy;
    }

    public void SetHasAttacked(bool boolean)
    {
        Attacked = boolean;
    }

    public bool HasAttacked()
    {
        return Attacked;
    }
}
