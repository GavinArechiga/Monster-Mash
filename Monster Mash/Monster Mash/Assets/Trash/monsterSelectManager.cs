using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class monsterSelectManager : MonoBehaviour
{
    //public EventSystem eventHandler;
    public MultiplayerEventSystem[] eventHandler;
    private stageSelectManager stageSelect;
    private int selectedStagetageNumber = 0;
    public GameObject[] stageSelectionButtons;

    [SerializeField]
    public PlayerInputManager playerInputManager;

    public GameObject monsterSelectPanel;
    public GameObject stageSelectPanel;
    public Animation camera_MonsterToStageTransition;
    public GameObject portal;


    public monsterAttackSystem[] tempMonsterLibrary;
    public monsterAttackSystem player1SelectedMonster;

    private void Awake()
    {
        stageSelect = FindObjectOfType<stageSelectManager>();
        //tempMonsterLibrary = FindObjectsOfType<monsterAttackSystem>();
        establishMonsterConnections();
    }

    public void activateStageSelect()
    {
        //wait a few seconds to open the stage select
        playerInputManager.DisableJoining();
        StartCoroutine(portalDelay());
        //turn off monster select
        monsterSelectPanel.SetActive(false);
        //turn on portal
        portal.SetActive(true);
        //activate animation on camera
        camera_MonsterToStageTransition.Play();
    }

    private IEnumerator portalDelay()
    {
        yield return new WaitForSeconds(1);
        stageSelectPanel.SetActive(true);
        selectNewButton(stageSelectPanel);
    }

    public void cycleStageSelectionRight()
    {
        if (selectedStagetageNumber < stageSelectionButtons.Length - 1)
        {
            stageSelectionButtons[selectedStagetageNumber].SetActive(false);
            selectedStagetageNumber++;
            stageSelectionButtons[selectedStagetageNumber].SetActive(true);
            //selectNewButton(stageSelectionButtons[selectedStagetageNumber]);
        }
        else
        {
            stageSelectionButtons[selectedStagetageNumber].SetActive(false);
            selectedStagetageNumber = 0;
            stageSelectionButtons[selectedStagetageNumber].SetActive(true);
            //selectNewButton(stageSelectionButtons[selectedStagetageNumber]);
        }
    }

    public void cycleStageSelectionLeft()
    {
        if (selectedStagetageNumber > 0)
        {
            stageSelectionButtons[selectedStagetageNumber].SetActive(false);
            selectedStagetageNumber--;
            stageSelectionButtons[selectedStagetageNumber].SetActive(true);
            //selectNewButton(stageSelectionButtons[selectedStagetageNumber]);
        }
        else
        {
            stageSelectionButtons[selectedStagetageNumber].SetActive(false);
            selectedStagetageNumber = stageSelectionButtons.Length - 1;
            stageSelectionButtons[selectedStagetageNumber].SetActive(true);
            //selectNewButton(stageSelectionButtons[selectedStagetageNumber]);
        }
    }

    public void buttonDampening(Button menuButton)
    {
        StartCoroutine(buttonDampenTiming(menuButton));
    }

    public IEnumerator buttonDampenTiming(Button menuButton) //to stop "on click" from going off like 3 times
    {
        menuButton.interactable = false;
        yield return new WaitForSeconds(0.2f);
        menuButton.interactable = true;

        for (int i = 0; i < eventHandler.Length; i++)
        {
            eventHandler[i].enabled = false;
            Button firstButton = menuButton;
            eventHandler[i].firstSelectedGameObject = firstButton.gameObject;
            eventHandler[i].enabled = true;
            firstButton.Select();
        }
    }

    public void confirmStage()
    {

    }

    private void selectNewButton(GameObject menuPanel)
    {
        for (int i = 0; i < eventHandler.Length; i++)
        {
            eventHandler[i].enabled = false;
            Button firstButton = menuPanel.GetComponent<menuPanel>().firstButtonInPanel;
            eventHandler[i].firstSelectedGameObject = firstButton.gameObject;
            eventHandler[i].enabled = true;
            firstButton.Select();
        }
    }

    public void establishMonsterConnections()
    {
        for (int i = 0; i < tempMonsterLibrary.Length; i++)
        {
            tempMonsterLibrary[i].turnOnLimbConnectors();
        }
    }

    public void turnOnSelectedMonster(int monsterInLibrary)
    {
        //extremely temp code
        if (monsterInLibrary == 0)
        {
            monsterAttackSystem selectedMonster = tempMonsterLibrary[monsterInLibrary];
            playerController player1 = GameObject.Find("Player 1").GetComponent<playerController>();
            selectedMonster.transform.parent = player1.transform;
            player1.myMonster = selectedMonster;
            player1.myMonster.myPlayer = player1;
            player1.myMonster.turnOffLimbConnectors();
            player1.myMonster.connectCurrentLimbs();
            player1.myMonster.awakenTheBeast();
        }
        else if (monsterInLibrary == 1)
        {
            monsterAttackSystem selectedMonster = tempMonsterLibrary[monsterInLibrary];
            playerController player2 = GameObject.Find("Player 2").GetComponent<playerController>();
            selectedMonster.transform.parent = player2.transform;
            player2.myMonster = selectedMonster;
            player2.myMonster.myPlayer = player2;
            player2.myMonster.turnOffLimbConnectors();
            player2.myMonster.connectCurrentLimbs();
            player2.myMonster.awakenTheBeast();
        }
    }
}
