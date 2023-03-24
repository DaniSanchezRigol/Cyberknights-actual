using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using TMPro;
public class BattleManager : MonoBehaviour
{
    [SerializeField] private Transform pfHelah;
    [SerializeField] private Transform pfAlhan;
    [SerializeField] private Transform pfTank;
    [SerializeField] private Transform pfMage;
    [SerializeField] private Transform pfEnemyMelee;
    [SerializeField] private Transform pfEnemyRanged;
    public GameObject ActionPopup;
    public TextMeshProUGUI TextoAction;
    //[SerializeField] private Transform[] AllyCharacters;
    //[SerializeField] private Transform[] EnemyCharacters;

    private static BattleManager _instance;

    public static BattleManager Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = GameObject.FindObjectOfType<BattleManager>();
            }
            return _instance;
        }
    }
    private CharacterBattle activeCharacterBattle;
    private CharacterBattle selectedTargetCharacterBattle;
    private CharacterBattle lastAttackedAlly;
    [SerializeField] private List<CharacterBattle> characterBattleList;
    [SerializeField] List<CharacterBattle> turnOrderList;
    List<CharacterBattle> selectedCharactersList;
    private State state;
    [SerializeField] private int turn = 0;
    private GameObject ActiveMoves;
    private float turnWaitingTime = 0.2f;
    private float newTurnWaitingTime = 1f;
    private Character.Move attackType;
    private int attackIndex;
    private bool movesAgain = false;
    private Transform[] EnemyCharacters;
    private UnityEvent endBattleEvent;
    private int timeszpressed;
    
    public GameObject MainCharacterMoves;
    public GameObject ArcherMoves;
    public GameObject TankMoves;
    public GameObject MageMoves;
    public GameObject WinScreen;
    private enum State
    {
        WaitingForPlayer,
        BasicAttack,
        Defend,
        SpecialMove,
        Abilities,
        Busy,
    }
    private void Awake()
    {
        characterBattleList = new List<CharacterBattle>();
        turnOrderList = new List<CharacterBattle>();
        selectedCharactersList = new List<CharacterBattle>();
        ActionPopup.SetActive(false);
    }
    //public void SetEnemies(Transform[] enemyCharacters)
    //{
    //    EnemyCharacters = new Transform[enemyCharacters.Length];
    //    for (int i = 0; i < EnemyCharacters.Length; i++)
    //    {
    //        EnemyCharacters[i] = enemyCharacters[i];
    //        print("hola" + i);
    //    }
    //}
    private void Start()
    {
        Animator animator = GameObject.Find("FadeInOut").GetComponent<Animator>();
        animator.Play("FadeOut");
    }
    public void StartBattle(Transform[] enemyCharacters, UnityEvent endBattleEvent)
    {
        Transform[] AllyCharacters = new Transform[PartyData.Instance.GetAllySetup().Length];
        for (int i = 0; i < PartyData.Instance.GetAllySetup().Length; i++)
        {
            AllyCharacters[i] = PartyData.Instance.GetAllySetup()[i];
        }
        for(int i = 0; i < AllyCharacters.Length; i++)
        {
            SpawnCharacter(true, GetSetupNextLanePosition(i), AllyCharacters[i]);
        }
        for (int i = 0; i < enemyCharacters.Length; i++)
        {
            SpawnCharacter(false, GetSetupNextLanePosition(i), enemyCharacters[i]);
        }
        //SpawnCharacter(true, LanePosition.FrontUp, pfHelah);
        //SpawnCharacter(true, LanePosition.FrontDown, pfTank);
        //SpawnCharacter(true, LanePosition.FrontDown, pfAlhan);
        //SpawnCharacter(true, LanePosition.BackUp, pfMage);
        //SpawnCharacter(false, LanePosition.FrontUp, pfEnemyMelee);
        //SpawnCharacter(false, LanePosition.FrontDown, pfEnemyMelee);
        //SpawnCharacter(false, LanePosition.BackUp, pfEnemyRanged);
        //SpawnCharacter(false, LanePosition.BackDown, pfEnemyRanged);
        this.endBattleEvent = endBattleEvent;
        turnOrderList.Sort(SetBattleOrder);
        AutoSetTargetCharacterBattle();
        Invoke("CheckEndAndSelectCharacter", turnWaitingTime);
    }
    public enum LanePosition
    {
        FrontUp,
        FrontDown,
        BackUp,
        BackDown,
    }

    public enum ArrowMove
    {
        Up,
        Right,
        Left,
        Down,
    }
    public static LanePosition GetSetupNextLanePosition(int i)
    {
        switch(i)
        {
            case 0:
                return LanePosition.FrontUp;
            case 1:
                return LanePosition.FrontDown;
            case 2:
                return LanePosition.BackUp;
            case 3:
                return LanePosition.BackDown;
            default:
                return LanePosition.FrontUp;
        }
    }
    public static Vector3 GetPosition(LanePosition lanePosition, bool isPlayerTeam)
    {
        float TeamPosition = isPlayerTeam ? -2f : 2f;
        float backTeamUbication = isPlayerTeam ? -2f : 2f;

        switch (lanePosition)
        {
            default:
            case LanePosition.FrontUp: return new Vector3(2, 0, TeamPosition);
            case LanePosition.FrontDown: return new Vector3(4, 0, TeamPosition);
            case LanePosition.BackUp: return new Vector3(2, 0, TeamPosition + backTeamUbication);
            case LanePosition.BackDown: return new Vector3(4, 0, TeamPosition + backTeamUbication);
        }

    }
    public static LanePosition GetNextLanePosition(LanePosition lanePosition, ArrowMove move, CharacterBattle.Category category)
    {
        switch(move)
        {
            default:
            case ArrowMove.Up:
                switch (lanePosition)
                {
                    default:
                    case LanePosition.FrontUp: return LanePosition.FrontUp;
                    case LanePosition.FrontDown:  return LanePosition.FrontUp; 
                    case LanePosition.BackUp: return LanePosition.BackUp;
                    case LanePosition.BackDown: return LanePosition.BackUp;
                }
            case ArrowMove.Right:
                switch (lanePosition)
                {
                    default:
                    case LanePosition.FrontUp: if (category != Character.Category.Melee) {  return LanePosition.BackUp; } return LanePosition.FrontUp;
                    case LanePosition.BackDown: return LanePosition.BackDown;
                    case LanePosition.BackUp: return LanePosition.BackUp;
                    case LanePosition.FrontDown: if (category != Character.Category.Melee) {  return LanePosition.BackDown; } return LanePosition.FrontDown;

                }
            case ArrowMove.Left:
                switch (lanePosition)
                {
                    default:
                    case LanePosition.FrontUp: return LanePosition.FrontUp;
                    case LanePosition.FrontDown: return LanePosition.FrontDown;
                    case LanePosition.BackUp: ; return LanePosition.FrontUp;
                    case LanePosition.BackDown:; return LanePosition.FrontDown;
                }
            case ArrowMove.Down:
                switch (lanePosition)
                {
                    default:
                    case LanePosition.FrontUp: ; return LanePosition.FrontDown;
                    case LanePosition.FrontDown: return LanePosition.FrontDown;
                    case LanePosition.BackDown: return LanePosition.BackDown;
                    case LanePosition.BackUp: ; return LanePosition.BackDown;
                }
        }
     
    }
    private void SpawnCharacter(bool isPlayerTeam, LanePosition lanePosition, Transform type)
    {
        Vector3 position;
        Quaternion rotation;
        
        if (isPlayerTeam)
        {
            position = GetPosition(lanePosition, isPlayerTeam);
            rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else
        {
            position = GetPosition(lanePosition, isPlayerTeam);
            rotation = Quaternion.Euler(0f, 180f, 0f);
        }
        Transform characterTransform = Instantiate(type, position, rotation);
        CharacterBattle characterBattle = characterTransform.GetComponent<CharacterBattle>();
        characterBattleList.Add(characterBattle);
        turnOrderList.Add(characterBattle);
        characterBattle.Setup(lanePosition, isPlayerTeam);
    }
    private void SetActiveMoves()
    {
        switch(activeCharacterBattle.GetCharacterType())
        {
            case Character.CharacterType.MainCharacter:
                ActiveMoves = MainCharacterMoves;
                break;
            case Character.CharacterType.Archer:
                ActiveMoves = ArcherMoves;
                break;
            case Character.CharacterType.Tank:
                ActiveMoves = TankMoves;
                break;
            case Character.CharacterType.Mage:
                ActiveMoves = MageMoves;
                break;
        }
        ActiveMoves.SetActive(true);
    }
    private void MakeMove()
    {
        BasicAttackAction();
        DefendAction();
        SpecialMoveAction();
    }

    private void BasicAttackAction()
    {
        if (state == State.BasicAttack)
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                timeszpressed++;
            }
            selectedTargetCharacterBattle.ShowSelectionTarget();
            MoveEnemySelect();
            if (timeszpressed >= 2)
            {
                TextoAction.text = "Atacar";
                activeCharacterBattle.BasicAttack(selectedTargetCharacterBattle, () =>
                {
                    Invoke("CheckEndAndSelectCharacter", turnWaitingTime);
                });
            }
        }
    }
    private void DefendAction()
    {
        if (state == State.Defend)
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                timeszpressed++;
            }
            activeCharacterBattle.ShowSelectionTarget();

            if (timeszpressed >= 2)
            {
                TextoAction.text = "Defender";
                activeCharacterBattle.Defend();
                Invoke("CheckEndAndSelectCharacter", turnWaitingTime);
            }
        }
    }
    private void SpecialMoveAction()
    {
        if (state == State.SpecialMove)
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                timeszpressed++;
            }
            switch (attackType)
            {
                default: break;
                case Character.Move.Eletrocorte:
                        ShowSelectionMultiple(getFrontLine(false));
                        if (timeszpressed >= 2)
                        {
                            TextoAction.text = "Electrocorte";
                            activeCharacterBattle.Electrocorte(getFrontLine(false), () =>
                                {
                                    Invoke("CheckEndAndSelectCharacter", turnWaitingTime);
                                });
                        }
                    break;

                case Character.Move.BolasSalvajes:
                    selectedTargetCharacterBattle.ShowSelectionTarget();
                    MoveEnemySelect();
                    if (timeszpressed >= 2)
                    {
                        TextoAction.text = "Bolas Salvajes";
                        activeCharacterBattle.BolasSalvajes(selectedTargetCharacterBattle, () =>
                        {
                            turnOrderList.Sort(SetBattleOrder);
                            Invoke("CheckEndAndSelectCharacter", turnWaitingTime);
                        });
                    }
                    break;

                case Character.Move.DagaFulgurante:
                        selectedTargetCharacterBattle.ShowSelectionTarget();
                        MoveEnemySelect();
                            if (timeszpressed >= 2)
                            {
                                TextoAction.text = "Daga Fulgurante";
                                activeCharacterBattle.DagaFulgurante(selectedTargetCharacterBattle, () =>
                                {
                                    Invoke("CheckEndAndSelectCharacter", turnWaitingTime);
                                });
                        }
                    break;

                case Character.Move.HemoFlecha:
                    selectedTargetCharacterBattle.ShowSelectionTarget();
                    MoveEnemySelect();
                    if (timeszpressed >= 2)
                    {
                        TextoAction.text = "Hemoflecha";
                        activeCharacterBattle.HemoFlecha(selectedTargetCharacterBattle, () =>
                        {
                            Invoke("CheckEndAndSelectCharacter", turnWaitingTime);
                        });
                    }
                    break;

            }
        }
    }
    public void CloseAbilities()
    {
        state = State.WaitingForPlayer;
        SoundManager.Instance.PlaySoundFX("FX_BATTLEACTION", 1f);
    }
    private void CancelOrConfirm()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            timeszpressed--;
            if (state == State.SpecialMove)
                state = State.Abilities;
        else
            state = State.WaitingForPlayer;
            ActiveMoves.SetActive(true);
            SoundManager.Instance.PlaySoundFX("FX_BATTLEACTION", 1f);
            HideSelectionMultiple(characterBattleList);
        }
        if (timeszpressed >= 2)
        {
            timeszpressed = 0;
            state = State.Busy;
            SoundManager.Instance.PlaySoundFX("FX_BATTLEACTION", 1f);
            ActiveMoves.SetActive(false);
            ActionPopup.SetActive(true);
            HideSelectionMultiple(characterBattleList);
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
            Time.timeScale = 3;
        if (Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        MakeMove();

        if (state == State.BasicAttack || state == State.Defend || state == State.SpecialMove)
        {
            CancelOrConfirm();
        }
        print(turn);
    }
    private IEnumerator WaitForSeconds(float seconds, Action onTimeComplete)
    {
        yield return new WaitForSeconds(seconds);
        onTimeComplete();
    }
    private void MoveEnemySelect()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            SetNextSelectedCharacterBattle(selectedTargetCharacterBattle.GetLanePosition(), ArrowMove.Up);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            SetNextSelectedCharacterBattle(selectedTargetCharacterBattle.GetLanePosition(), ArrowMove.Right);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            SetNextSelectedCharacterBattle(selectedTargetCharacterBattle.GetLanePosition(), ArrowMove.Left);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            SetNextSelectedCharacterBattle(selectedTargetCharacterBattle.GetLanePosition(), ArrowMove.Down);
        }
    }
    private void HideSelectionMultiple(List<CharacterBattle> list)
    {
        foreach (CharacterBattle characterBattle in list)
        {
            characterBattle.HideSelectionTarget();
        }
    }
    private void ShowSelectionMultiple(List<CharacterBattle> list)
    {
        foreach (CharacterBattle characterBattle in list)
        {
            characterBattle.ShowSelectionTarget();
        }
    }

    private void SetSelectedCharacterList(List<CharacterBattle> list)
    {
        foreach (CharacterBattle characterBattle in list)
        {
            selectedCharactersList.Add(characterBattle);
        }
    }
    public void BasicAttack()
    {
        if(state == State.WaitingForPlayer)
        {
            state = State.BasicAttack;
            ActiveMoves.SetActive(false);
            SoundManager.Instance.PlaySoundFX("FX_BATTLEACTION", 1f);
        }
    }
    public void Defend()
    {
        if(state == State.WaitingForPlayer)
        {
            state = State.Defend;
            ActiveMoves.SetActive(false);
            SoundManager.Instance.PlaySoundFX("FX_BATTLEACTION", 1f);
        }
    }

    private bool EnoughEnergy(int numberIndex)
    {
        switch(numberIndex)
        {
            default: break;
            case 1:
                attackType = activeCharacterBattle.GetAttackType1();
                break;
            case 2:
                attackType = activeCharacterBattle.GetAttackType2();
                break;
            case 3:
                attackType = activeCharacterBattle.GetAttackType3();
                break;
            case 4:
                attackType = activeCharacterBattle.GetAttackType4();
                break;
        }
        switch(attackType)
        {
            default: break;
            case Character.Move.Eletrocorte:
                if (activeCharacterBattle.GetEnergy() >= MoveManager.Instance.getMoveCost("Electrocorte"))
                {
                    return true;
                }
                break;
            case Character.Move.DisparoDoble:
                if (activeCharacterBattle.GetEnergy() >= MoveManager.Instance.getMoveCost("DisparoDoble"))
                {
                    return true;
                }
                break;
            case Character.Move.BolasSalvajes:
                if (activeCharacterBattle.GetEnergy() >= MoveManager.Instance.getMoveCost("BolasSalvajes"))
                {
                    return true;
                }
                break;

            case Character.Move.DagaFulgurante:
                if (activeCharacterBattle.GetEnergy() >= MoveManager.Instance.getMoveCost("GolpeFulgurante"))
                {
                    return true;
                }
                break;
            case Character.Move.HemoFlecha:
                if (activeCharacterBattle.GetEnergy() >= MoveManager.Instance.getMoveCost("HemoFlecha"))
                {
                    return true;
                }
                break;
        }
        return false;
    }
    public void SpecialMove(int number)
    {
        if (state == State.Abilities && EnoughEnergy(number))
        {
            state = State.SpecialMove;
            ActiveMoves.SetActive(false);
            attackIndex = number;
            SoundManager.Instance.PlaySoundFX("FX_BATTLEACTION", 1f);
        }
    }
    public void Abilities()
    {
        if(state == State.WaitingForPlayer)
        {
            state = State.Abilities;
        }
    }
    private void SelectNextCharacter(LanePosition nextLanePosition)
    {
        foreach (CharacterBattle characterBattle in characterBattleList)
        {
            if (characterBattle.IsPlayerTeam() == false && characterBattle.GetLanePosition() == nextLanePosition)
            {
                selectedTargetCharacterBattle.HideSelectionTarget();
                selectedTargetCharacterBattle = characterBattle;
                selectedTargetCharacterBattle.ShowSelectionTarget();
            }
        }
    }
    private void SetNextSelectedCharacterBattle(LanePosition lanePosition, ArrowMove move)
    {
        LanePosition nextLanePosition = GetNextLanePosition(selectedTargetCharacterBattle.GetLanePosition(), move, activeCharacterBattle.GetCategory());
        bool found = false;
        
        foreach (CharacterBattle characterBattle in characterBattleList)
        {
            if (characterBattle.IsPlayerTeam() == false && characterBattle.GetLanePosition() == nextLanePosition)
            {
                if(characterBattle != selectedTargetCharacterBattle)
                {
                    SoundManager.Instance.PlaySoundFX("FX_SELECTENEMY", 1f);
                }
                selectedTargetCharacterBattle.HideSelectionTarget();
                selectedTargetCharacterBattle = characterBattle;
                selectedTargetCharacterBattle.ShowSelectionTarget();
                found = true;
            }
        }

        if (!found && activeCharacterBattle.GetCategory() == Character.Category.Ranged)
        {
            if (nextLanePosition == LanePosition.FrontUp)
            {
                if (move == ArrowMove.Up)
                {
                    nextLanePosition = LanePosition.BackUp;
                }
                if (move == ArrowMove.Left)
                {
                    nextLanePosition = LanePosition.FrontDown;
                }
                SoundManager.Instance.PlaySoundFX("FX_SELECTENEMY", 1f);
                SelectNextCharacter(nextLanePosition);
                return;
            }

            if (nextLanePosition == LanePosition.BackDown)
            {
                if (move == ArrowMove.Down)
                {
                    nextLanePosition = LanePosition.FrontDown;
                }
                if (move == ArrowMove.Right)
                {
                    nextLanePosition = LanePosition.BackUp;
                }
                SoundManager.Instance.PlaySoundFX("FX_SELECTENEMY", 1f);
                SelectNextCharacter(nextLanePosition);
                return;
            }
            if (nextLanePosition == LanePosition.BackUp)
            {
                if (move == ArrowMove.Up)
                {
                    nextLanePosition = LanePosition.FrontUp;
                }
                if (move == ArrowMove.Right)
                {
                    nextLanePosition = LanePosition.BackDown;
                }
                SoundManager.Instance.PlaySoundFX("FX_SELECTENEMY", 1f);
                SelectNextCharacter(nextLanePosition);
                return;
            }

            if (nextLanePosition == LanePosition.FrontDown)
            {
                if (move == ArrowMove.Down)
                {
                    nextLanePosition = LanePosition.BackDown;
                }
                if (move == ArrowMove.Left)
                {
                    nextLanePosition = LanePosition.FrontUp;
                }
                SoundManager.Instance.PlaySoundFX("FX_SELECTENEMY", 1f);
                SelectNextCharacter(nextLanePosition);
                return;
            }
        }
    }
    private int SetBattleOrder(CharacterBattle a, CharacterBattle b)
    {
        if(a == null || b == null)
        {
            return 0;
        }

        if(a.GetSpeed() > b.GetSpeed())
        {
            return -1;
        }
        if (a.GetSpeed() < b.GetSpeed())
        {
            return 1;
        }
        return 0;
    }
    private void SetActiveCharacterBattle(CharacterBattle characterBattle)
    {
        if (activeCharacterBattle != null)
        {
            activeCharacterBattle.HideActiveCircle();
        }
        activeCharacterBattle = characterBattle;
        activeCharacterBattle.ShowActiveCircle();
    }

    private void AutoSetTargetCharacterBattle()
    {
        foreach (CharacterBattle characterBattle in characterBattleList)
        {
            if(characterBattle.IsPlayerTeam()==false && characterBattle.GetLanePosition() == LanePosition.FrontUp)
            {
                selectedTargetCharacterBattle = characterBattle;
                return;
            }

            if(characterBattle.IsPlayerTeam()==false && characterBattle.GetLanePosition() == LanePosition.FrontDown)
            {
                selectedTargetCharacterBattle = characterBattle;
                return;
            }

            if(characterBattle.IsPlayerTeam()==false && characterBattle.GetLanePosition() == LanePosition.BackUp)
            {
                selectedTargetCharacterBattle = characterBattle;
                return;
            }

            if(characterBattle.IsPlayerTeam()==false && characterBattle.GetLanePosition() == LanePosition.BackDown)
            {
                selectedTargetCharacterBattle = characterBattle;
                return;
            }

        }
    }
    private void ApplyStatus()
    {
        foreach (CharacterBattle characterBattle in characterBattleList)
        {
            characterBattle.SetHasAttacked(false);
            if (characterBattle.IsBleeding())
           {
                characterBattle.NonTargetedDamage("Bleed");
           }
        }
    }
    private void CheckEndAndSelectCharacter()
    {
        CheckKilledCharacters();
        if (movesAgain)
        {
            movesAgain = false;
            turn--;
            turnOrderList[turn].SetHasAttacked(false);
            OtherTextPopups.CreateEffectPopup(activeCharacterBattle.GetPosition(), "BloodFrenzy");
        }
        else
        {
            if (turn >= turnOrderList.Count)
            {
                turn = 0;
                ApplyStatus();
                CheckKilledCharacters();
                Invoke("ChooseNextActiveCharacter", newTurnWaitingTime);
                return;
            }
        }
        Invoke("ChooseNextActiveCharacter", turnWaitingTime);
    }
   
    private void ChooseNextActiveCharacter()
    {
        ActionPopup.SetActive(false);
        selectedTargetCharacterBattle.HideSelectionTarget();
        if (AllEnemiesDead())
        {
            print("You win");
            WinScreen.SetActive(true);
            return;
        }

        if (AllPlayerTeamDead())
        {
            print("You loose");
            return;
        }
        while (turnOrderList[turn] == null || turnOrderList[turn].HasAttacked() == true)
        {
            turn++;
            if (turn >= turnOrderList.Count)
            {
                turn = 0;
            }
        }

        if (turnOrderList[turn].IsPlayerTeam() == false)
        {
            SetActiveCharacterBattle(turnOrderList[turn]);
            activeCharacterBattle.SetState(CharacterBattle.State.Idle);
            state = State.Busy;
            ActionPopup.SetActive(true);
            TextoAction.text = "Atacar";
            turnOrderList[turn].BasicAttack(GetTargetedAllyCharacter(turnOrderList[turn]), () =>
            {
                Invoke("CheckEndAndSelectCharacter", turnWaitingTime);
            });
        }
        else
        {
            SetActiveCharacterBattle(turnOrderList[turn]);
            activeCharacterBattle.SetState(CharacterBattle.State.Idle);
            var eventSystem = EventSystem.current;
            SetActiveMoves();
            ActiveMoves.GetComponent<ManageMainMoves>().RestartInterface();
            eventSystem.SetSelectedGameObject(ActiveMoves.transform.GetChild(0).gameObject, new BaseEventData(eventSystem));
            state = State.WaitingForPlayer;
            AutoSetTargetCharacterBattle();
        }
        turnOrderList[turn].SetHasAttacked(true);
        turn++;
    }
    private List<CharacterBattle> getAllCharacters(bool isPlayerTeam)
    {
        List <CharacterBattle> allCharacters = new List<CharacterBattle>();
        foreach (CharacterBattle characterBattle in characterBattleList)
        {
            if(characterBattle.IsPlayerTeam() == isPlayerTeam)
            {
                allCharacters.Add(characterBattle);
            }
        }
        return allCharacters;
    } 
    private List<CharacterBattle> getFrontLine(bool isPlayerTeam)
    {
        bool frontLineAlive = false;
        List<CharacterBattle> frontLineCharacters = new List<CharacterBattle>();
        foreach (CharacterBattle characterBattle in characterBattleList)
        {
            if (characterBattle.IsPlayerTeam() == isPlayerTeam && (characterBattle.GetLanePosition() == LanePosition.FrontUp
                 || characterBattle.GetLanePosition() == LanePosition.FrontDown))
            {
                frontLineCharacters.Add(characterBattle);
                frontLineAlive = true;
            }
        }
        if(!frontLineAlive)
        {
            return getBackLine(isPlayerTeam);
        }
        return frontLineCharacters;
    }
    private List<CharacterBattle> getBackLine(bool isPlayerTeam)
    {
        bool frontLineAlive = false;
        List<CharacterBattle> backLineCharacters = new List<CharacterBattle>();
        foreach (CharacterBattle characterBattle in characterBattleList)
        {
            if (characterBattle.IsPlayerTeam() == isPlayerTeam && (characterBattle.GetLanePosition() == LanePosition.BackDown
                || characterBattle.GetLanePosition() == LanePosition.BackUp))
            {
                backLineCharacters.Add(characterBattle);
                frontLineAlive = true;
            }
        }
        if (!frontLineAlive)
        {
            return getFrontLine(isPlayerTeam);
        }
        return backLineCharacters;
    }

    private bool IsAllyFrontLineDead()
    {
        foreach (CharacterBattle characterBattle in characterBattleList)
        {
            if (characterBattle.IsPlayerTeam() && (characterBattle.GetLanePosition() == LanePosition.FrontUp
                || characterBattle.GetLanePosition() == LanePosition.FrontDown))
            {
                return false;
            }
        }
        return true;
    }
    private CharacterBattle GetTargetedAllyCharacter(CharacterBattle attackerCharacter)
    {

        List<int> indexDead = new List<int>();
        int indexCount = 0;
        foreach (CharacterBattle characterBattle in characterBattleList)
        {
            if (characterBattle.IsPlayerTeam() && attackerCharacter.GetCategory() == Character.Category.Ranged)
            {
                indexDead.Add(indexCount);
            }
            if (characterBattle.IsPlayerTeam() && attackerCharacter.GetCategory() == Character.Category.Melee
                && (characterBattle.GetLanePosition() == LanePosition.FrontUp
                || characterBattle.GetLanePosition() == LanePosition.FrontDown))
            {
                indexDead.Add(indexCount);
            }
            if (IsAllyFrontLineDead() && characterBattle.IsPlayerTeam())
            {
                indexDead.Add(indexCount);
            }

            indexCount++;
        }
        int randomNumber = UnityEngine.Random.Range(0, indexDead.Count);
        return characterBattleList[indexDead[randomNumber]];

    }
    private void CharacterDie()
    {
        print("muere");
    //    int index = 0;
    //    foreach (CharacterBattle characterBattle in characterBattleList)
    //    {
    //        if (characterBattle.IsDead())
    //        {
    //            turnOrderList[index] = null;
    //        }
    //        index++;
    //    }
    }
    private void BloodFrenzyPasive()
    {
        if (activeCharacterBattle.GetPasive2() == Character.Pasive2.BloodFrenzy)
        {
            if(UnityEngine.Random.Range(0, 2) == 1)
            {
                movesAgain = true;
            }
        }
    }
    private void CheckKilledCharacters()
    {
        List<CharacterBattle> indexDead = new List<CharacterBattle>();
        List<int> indexNumber = new List<int>();
        int indexCount = 0;

        foreach (CharacterBattle characterBattle in turnOrderList)
        {
            if (characterBattle != null && characterBattle.IsDead())
            {
                indexDead.Add(characterBattle);
                indexNumber.Add(indexCount);
                BloodFrenzyPasive();
            }
            indexCount++;
        }

        foreach (CharacterBattle characterBattle in indexDead)
        {
            characterBattleList.Remove(characterBattle);
        } 
        foreach (int number in indexNumber)
        {
            turnOrderList[number] = null;
        }
            AutoSetTargetCharacterBattle();
    }
    private bool AllEnemiesDead()
    {
        foreach (CharacterBattle characterBattle in characterBattleList)
        {
            if(!characterBattle.IsPlayerTeam())
            {
                return false;
            }
        }
        return true;
    }
    private bool AllPlayerTeamDead()
    {
        foreach (CharacterBattle characterBattle in characterBattleList)
        {
            if (characterBattle.IsPlayerTeam())
            {
                return false;
            }
        }
        return true;
    }
    public UnityEvent getEndOfBattleEvent()
    {
        return endBattleEvent;
    }

}
