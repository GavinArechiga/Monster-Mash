using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hubInteractionTrigger : MonoBehaviour
{
    public monstroHubManager hubManager;
    public Animator interactionAnimator;
    public bool requiresButtonPrompt = false;

    public enum hubInteractions
    {
        none,
        portal,
        buildAScare,
        chopShop,
        loreBook,
        conceptPaintings
    }

    public hubInteractions interactionTrigger = new hubInteractions();

    public void resetAimationTriggers()
    {
        if (interactionAnimator != null)
        {
            interactionAnimator.ForceResetTriggers();
        }
    }

    public void enterInteractionArea()
    {
        resetAimationTriggers();

        if (interactionAnimator != null)
        {
            interactionAnimator.SetTrigger("enter");
        }
    }

    public void exitInteractionArea()
    {
        //resetAimationTriggers();

        if (interactionAnimator != null)
        {
            interactionAnimator.SetTrigger("exit");
        }
    }

    public void buttonInteraction()
    {
        if (interactionTrigger.ToString() == "portal")
        {

        }

    }
}
