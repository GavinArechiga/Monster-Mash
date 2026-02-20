using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PartSelector : MonoBehaviour
{
    [Tooltip("replace with controller B later")] public KeyCode backKey;

    public int defaultActiveCategory;
    public int itemsPerPage;
    public int rowsPerPage;
    float pageSize;
    Coroutine scrollToPage;

    public ScrollRect scrollRect;
    public Transform conveyor;
    public ConveyorCopy conveyorCopy;
    int activeCategory;
    int previousActiveCategory;
    public CanvasGroup tabsDock;
    public Toggle[] tabButtons;
    public PartGrid[] partsGrids;

    private void Awake()
    {
        activeCategory = defaultActiveCategory;
        tabButtons[defaultActiveCategory].isOn = true;
    }

    private void Start()
    {
        tabButtons[defaultActiveCategory].GetComponent<Animator>().SetTrigger("Pressed");

        conveyor.SetParent(partsGrids[activeCategory].transform);
        conveyor.SetAsFirstSibling();
    }

    private void Update()
    {
        // if button is pressed & player is currently on part grid
        if (

            Input.GetKeyDown(KeyCode.Escape) // change this to B button later, or add "or controller B button"

            && partsGrids[activeCategory].canvasGroup.interactable)
        {
            partsGrids[activeCategory].Interactable(false);
            tabsDock.interactable = true;

            // enables special animation behavior for current activated button
            tabButtons[activeCategory].GetComponent<Animator>().SetLayerWeight(1, 1);
            tabButtons[activeCategory].Select();
        }
    }


    public void EnableCategory(int categoryNumber)
    {
        if (tabButtons[categoryNumber].isOn)
        {
            tabsDock.interactable = false;
            // disables special animation behavior for current activated button
            tabButtons[activeCategory].GetComponent<Animator>().SetLayerWeight(1, 0);
            tabButtons[categoryNumber].GetComponent<Animator>().SetTrigger("Pressed");

            if (categoryNumber != activeCategory)
            {
                previousActiveCategory = activeCategory;
                activeCategory = categoryNumber;
                
                StartSwitchTabAnim();
            }
            else
            {
                partsGrids[activeCategory].Interactable(true);
            }

        }
    }

    // parents the reserve copy of the conveyor to the selected category,
    // places both off-screen, and plays the animation that makes it
    // re-enter the screen.
    void StartSwitchTabAnim()
    {
        conveyorCopy.AssistSwitchIn(partsGrids[activeCategory].transform);
        partsGrids[activeCategory].gameObject.SetActive(true);
        partsGrids[activeCategory].anim.SetTrigger("Switch");
    }

    // called from PartGrid script via AnimationEvent.
    // re-parents main conveyor, dismisses the duplicate conveyor,
    // re-enables player's ability to interact with the screen,
    // disables previous category.
    public void FinishSwitchTabAnim()
    {
        conveyor.SetParent(partsGrids[activeCategory].transform);
        conveyor.SetAsFirstSibling();
        conveyorCopy.FinishAnimAssist();
        partsGrids[activeCategory].Interactable(true);
        partsGrids[previousActiveCategory].gameObject.SetActive(false);
    }




    /**
    public void Scroll(bool up)
    {
        PartGrid currentCategory = partsGrids[activeCategory];
        RectTransform rect = currentCategory.rect;
        currentCategory.grid.enabled = false;

        
        // set pageSize if it hasn't been set yet
        if (pageSize == 0)
        {
            pageSize = currentCategory.grid.cellSize.y * (rowsPerPage - 1);
            pageSize += currentCategory.grid.cellSize.y * 0.5f;
        }

        float newY = pageSize;

        if ( (!up && currentCategory.currentPage > 2)
            //|| up && currentCategory.currentPage == 1
            )
        {
            newY += currentCategory.grid.cellSize.y * 0.5f;
        }
        if (up)
        {
            newY *= -1f;
        }

        rect.position = new Vector2(rect.position.x, rect.position.y + newY);
        

        currentCategory.grid.enabled = true;
    }
    **/

}
