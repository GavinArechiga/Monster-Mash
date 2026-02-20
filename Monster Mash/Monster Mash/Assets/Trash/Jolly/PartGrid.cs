using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PartGrid : MonoBehaviour
{
    public PartSelector partSelector;

    public CanvasGroup canvasGroup;
    public RectTransform rect;
    public Animator anim;
    public ConveyorCopy conveyorCopy;
    public GridLayoutGroup grid;

    [HideInInspector] public List<GameObject> parts;

    int pageSize;
    [HideInInspector] public int pageCount;
    [HideInInspector] public int currentPage = 1;


    private void Awake()
    {
        pageSize = partSelector.itemsPerPage;

        parts = new List<GameObject>();
        foreach (Transform part in grid.transform.GetComponentInChildren<Transform>())
        {
            parts.Add(part.gameObject);
            part.GetComponent<PartButton>().grid = this;
        }
    }

    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        PageConsciousResize();
    }

    public void FinishAnim()
    {
        partSelector.FinishSwitchTabAnim();
    }

    public void Interactable(bool enable)
    {
        canvasGroup.interactable = enable;
        if (enable)
        {
            // upon enabling, select first item in category
            grid.transform.GetChild(0).GetComponent<Button>().Select();
        }
    }

    private void OnEnable()
    {
        JostlePartIcons();
    }
    void JostlePartIcons()
    {
        foreach(GameObject part in parts)
        {
            part.GetComponent<PartButton>().PlayJostleAnim();
        }
    }
 
   
    // adjust Rect height according to grid contents
    void Resize()
    {
        // calculate and set new height by getting the
        // difference between first and last child y positions
        float first = parts[0].GetComponent<RectTransform>().position.y;
        float last = parts[parts.Count - 1].GetComponent<RectTransform>().position.y;
        float difference = Mathf.Abs(last - first) + grid.cellSize.y;
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, difference + 60f);
    }

    void PageConsciousResize()
    {
        float pagesFilled = parts.Count / pageSize;

        if (pagesFilled % pageSize == 0) 
        {
            pageCount = (int)pagesFilled;
        }
        else
        {
            pageCount = Mathf.FloorToInt(pagesFilled) + 1;
        }
        float newHeight = Screen.height * pageCount;
        
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, newHeight + grid.padding.top + grid.padding.bottom);

        // Debug.Log(pageCount + " pages; " + newHeight + " tall");
    }


    // called from part icon animation.
    // passes the selected item's index number
    // to the PartSelector script, to check if
    // it needs to scroll to a different page.

    // i don't have this function working
    // correctly yet, it is OK to replace this
    // method (and the Scroll(bool) method in
    // PartSelector.cs) if someone else
    // implements working page scrolling
    // functionality! -Jolly
    public void CheckScrollPage(GameObject item)
    {
        /**
        int index = parts.IndexOf(item) + 1;

        if (index > currentPage * pageSize)
        {
            currentPage++;
            partSelector.Scroll(false);
        }
        else if (index < currentPage - 1 * pageSize )
        {
            currentPage--;
            partSelector.Scroll(true);
            Debug.Log("scroll up");
        }
        **/
    }

}
