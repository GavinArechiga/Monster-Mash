using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class mainMenuManager : MonoBehaviour
{
    public EventSystem eventHandler;
    private stageSelectManager stageSelect;
    public GameObject mainMenuPanel;
    public GameObject versusPanel;
    public GameObject newStoryPanel;
    public GameObject overwriteWarning;
    public GameObject classicModeOptionsPanel;
    public GameObject kaijuModeOptionsPanel;
    public GameObject arcadeModeOptionsPanel;
    public GameObject naturalSelectionModeOptionsPanel;
    public GameObject extrasPanel;
    public GameObject optionsPanel;
    public GameObject trainingPanel;


    private void Awake()
    {
        activateMainMenuPanel();
        stageSelect = FindObjectOfType<stageSelectManager>();
    }

    public void activateMainMenuPanel()
    {
        //close all other panels
        mainMenuPanel.SetActive(true);
        versusPanel.SetActive(false);
        newStoryPanel.SetActive(false);
        overwriteWarning.SetActive(false);
        classicModeOptionsPanel.SetActive(false);
        kaijuModeOptionsPanel.SetActive(false);
        arcadeModeOptionsPanel.SetActive(false);
        naturalSelectionModeOptionsPanel.SetActive(false);
        extrasPanel.SetActive(false);
        optionsPanel.SetActive(false);
        trainingPanel.SetActive(false);
        selectNewButton(mainMenuPanel);
    }

    #region Versus
    public void activateVersusPanel()
    {
        versusPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
        selectNewButton(versusPanel);
    }

    public void playVersusMatch()
    {
        //make changes based on parameters adjusted before match
        stageSelect.SelectStage(2);
    }
    #endregion

    #region New and Continue Story
    public void activateNewStoryPanel()
    {
        //if a save load is already detected, throw up warning about overwriting save
        //otherwise...
        newStoryPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
        selectNewButton(newStoryPanel);
    }

    public void throwOverwriteWarning()
    {
        overwriteWarning.SetActive(true);
        selectNewButton(overwriteWarning);
    }

    public void newClassicStory()
    {
        //make changes based on parameters adjusted before story
    }

    public void newKaijuStory()
    {
        //make changes based on parameters adjusted before story
    }

    public void newArcadeStory()
    {
        //make changes based on parameters adjusted before story
    }

    public void newNaturalSelectionStory()
    {
        //make changes based on parameters adjusted before story
    }

    public void continueStory()
    {
        //make changes based on parameters adjusted before story
    }
    #endregion

    #region Build-A-Scare
    public void buildAScare()
    {

    }
    #endregion

    #region Extras
    public void activateExtras()
    {
        extrasPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
        selectNewButton(extrasPanel);
    }

    public void beastiary()
    {

    }

    public void activateTrainingPanel()
    {
        trainingPanel.SetActive(true);
        extrasPanel.SetActive(false);
        selectNewButton(trainingPanel);
    }

    public void training()
    {
        //make changes based on parameters adjusted before training
    }

    public void gauntlet()
    {

    }
    #endregion

    #region Options

    public void activateOptions()
    {
        optionsPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
        selectNewButton(optionsPanel);
    }

    #endregion

    #region Quit

    public void quitGame()
    {

    }

    #endregion

    private void selectNewButton(GameObject menuPanel)
    {
        eventHandler.enabled = false;
        Button firstButton = menuPanel.GetComponent<menuPanel>().firstButtonInPanel;
        eventHandler.firstSelectedGameObject = firstButton.gameObject;
        eventHandler.enabled = true;
        firstButton.Select();
    }
}
