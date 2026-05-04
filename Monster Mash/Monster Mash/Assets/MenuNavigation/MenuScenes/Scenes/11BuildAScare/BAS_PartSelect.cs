using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BAS_PartSelect : MonoBehaviour
{
    [SerializeField] private GameObject[] partTypes = new GameObject[] { };
    private BAS_Cursor cursor;
    private PartPool partPool;

    private string selectedPart;

    [SerializeField] private GameObject torsoParent;

    // Start is called before the first frame update
    void Start()
    {
        cursor = FindObjectOfType<BAS_Cursor>();
        partPool = FindObjectOfType<PartPool>();
    }

    public void SelectType(GameObject type)
    {
        for (int i = 0; i < partTypes.Length; i++)
        {
            if (partTypes[i] == type)
            {
                partTypes[i].SetActive(true);
            }
            else
            {
                partTypes[i].SetActive(false);
            }
        }
    }

    public void SelectTorso(int x)
    {
        selectedPart = partPool.GetMonsterParts()[0][x];

        var monsterPartLoad = Resources.Load<GameObject>(selectedPart);

        if (!monsterPartLoad)
        {
            Debug.Log("error monster part dont exist");
        }
        else
        {
            Destroy(torsoParent.transform.GetChild(0).gameObject);
            GameObject monsterPart = Instantiate(monsterPartLoad, torsoParent.transform);
            monsterPart.transform.localEulerAngles = new Vector3();
            monsterPart.transform.localScale = Vector3.one;
            monsterPart.transform.localPosition = new Vector3();
        }
    }

    public void SelectArm(int x)
    {
        selectedPart = partPool.GetMonsterParts()[1][x];
        cursor.SetSelectedPart(selectedPart);
    }

    public void SelectLeg(int x)
    {
        selectedPart = partPool.GetMonsterParts()[2][x];
        cursor.SetSelectedPart(selectedPart);
    }

    public void SelectHead(int x)
    {
        selectedPart = partPool.GetMonsterParts()[3][x];
        cursor.SetSelectedPart(selectedPart);
    }

    public void SelectEye(int x)
    {
        selectedPart = partPool.GetMonsterParts()[4][x];
        cursor.SetSelectedPart(selectedPart);
    }

    public void SelectMouth(int x)
    {
        selectedPart = partPool.GetMonsterParts()[5][x];
        cursor.SetSelectedPart(selectedPart);
    }

    public void SelectTail(int x)
    {
        selectedPart = partPool.GetMonsterParts()[6][x];
        cursor.SetSelectedPart(selectedPart);
    }

    public void SelectWing(int x)
    {
        selectedPart = partPool.GetMonsterParts()[7][x];
        cursor.SetSelectedPart(selectedPart);
    }

    public void SelectHorn(int x)
    {
        selectedPart = partPool.GetMonsterParts()[8][x];
        cursor.SetSelectedPart(selectedPart);
    }

    public void SelectDecor(int x)
    {
        selectedPart = partPool.GetMonsterParts()[9][x];
        cursor.SetSelectedPart(selectedPart);
    }
}
