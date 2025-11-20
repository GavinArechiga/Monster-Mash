using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class groundedLimbArea : MonoBehaviour
{
    [SerializeField] private NewPlayerController myPlayer;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Solid") || collision.CompareTag("Semi Solid"))
        {
            myPlayer.SetGroundedState(true);
            myPlayer.ResetAttackColliders();
            myPlayer.ResetLegAnimations();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Solid") || collision.CompareTag("Semi Solid"))
        {
            myPlayer.SetGroundedState(false);
        }
    }
}