using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class stageSelectManager : MonoBehaviour
{
    //This can be changed to a string if needed
    public int stageIndex;
    private bool busyLoading;

    public void ButtonSelected(MultiplayerCursor cursor)
    {

        if (CharacterSelectManager.Instance.storedPlayerInformation.Count > 0)
        {
            CharacterSelectManager.Instance.storedPlayerInformation.Clear();
        }

        foreach (MultiplayerJoinManager.PlayerInformation info in cursor.joinManager.playerInfo)
        {

            CharacterSelectManager.Instance.storedPlayerInformation.Add(info);
        }

        SelectStage(stageIndex);
    }

    public void SelectStage(int stageIndex)
    {
        if (busyLoading == false)
        {
            busyLoading = true;
            SceneManager.LoadSceneAsync(stageIndex);
        }
    }
}
