using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

[Serializable]
public class DefaultEmote
{
    [HideInInspector] public EmoteManager.EmoteSlot EmoteSlot;
    public EmoteManager.Emote EmoteName;
    public InputActionReference emoteInputAction;
}

[Serializable]
public class EmoteManager
{
    public enum EmoteSlot
    {
        Emote1 = 0,
        Emote2 = 1,
        Emote3 = 2,
        Emote4 = 3,
    }

    public enum Emote
    {
        Fierce,
        Gas,
        Mocking,
        Dance,
        Jack,
        Boo,
        Hula,
        Vomit,
        Sleep,
        Explosive,
        Sneezing,
        Random,
    }

    [Tooltip("Only Assign 4 emotes")] public DefaultEmote[] defaultEmotes = new DefaultEmote[4];
    private Dictionary<Emote, Action> emoteNameToActionRef;
    private Action<InputAction.CallbackContext>[] emoteHandlers = new Action<InputAction.CallbackContext>[4];
    private monsterAttackSystem attackSystem;
    private bool isInitilised;

    public void Initilize(monsterAttackSystem attackSystem, NewPlayerController player)
    {
        if (isInitilised) { return; }

        this.attackSystem = attackSystem;

        emoteNameToActionRef = new Dictionary<Emote, Action>
        {
            { Emote.Fierce, () => fierceEmote(attackSystem) },
            { Emote.Gas, () => gasEmote(attackSystem) },
            { Emote.Mocking, () => mockingEmote(attackSystem) },
            { Emote.Dance, () => danceEmote(attackSystem) },
            { Emote.Jack, () => jackEmote(attackSystem) },
            { Emote.Boo, () => booEmote(attackSystem) },
            { Emote.Hula, () => hulaEmote(attackSystem) },
            { Emote.Vomit, () => vomitEmote(attackSystem) },
            { Emote.Sleep, () => sleepEmote(attackSystem) },
            { Emote.Explosive, () => explosiveEmote(attackSystem) },
            { Emote.Sneezing, () => sneezingEmote(attackSystem) },
            { Emote.Random, PlayRandomEmote },
        };

        for (int i = 0; i < defaultEmotes.Length; i++)
        {
            defaultEmotes[i].EmoteSlot = (EmoteSlot)i;
            SwapEmote(defaultEmotes[i].EmoteSlot, defaultEmotes[i].EmoteName);
        }

        isInitilised = true;
    }

    /// <summary>
    /// Swaps the passed in emote slot's current emote 
    /// </summary>
    /// <param name="emoteSlot">The slot you want to swap</param>
    /// <param name="emoteName">The Enum name of the emote you want to set the slot to</param>
    public void SwapEmote(EmoteSlot emoteSlot, Emote emoteName)
    {
        int slotIndex = (int)emoteSlot;
        DefaultEmote defaultEmoteForSlot = Array.Find(defaultEmotes, e => e.EmoteSlot == emoteSlot);

        if (defaultEmoteForSlot == null ||
            defaultEmoteForSlot.emoteInputAction == null ||
            defaultEmoteForSlot.emoteInputAction.action == null)
            return;

        InputActionAsset playerActions = attackSystem.myPlayer.inputHandler.playerInput.actions;

        InputAction inputAction = playerActions.FindAction(defaultEmoteForSlot.emoteInputAction.name);
        if (!inputAction.enabled)
            inputAction.Enable();

        Action emoteAction = emoteNameToActionRef[emoteName];

        // Unsubscribe previous function if there is one
        if (emoteHandlers[slotIndex] != null)
            inputAction.performed -= emoteHandlers[slotIndex];

        // Store and subscribe the new handler
        emoteHandlers[slotIndex] = ctx => emoteAction();
        inputAction.performed += emoteHandlers[slotIndex];

        // Update the emote name for this slot
        defaultEmoteForSlot.EmoteName = emoteName;
    }

    /// <summary>
    /// Plays a randomly selected emote
    /// </summary>
    public void PlayRandomEmote()
    {
        if (attackSystem.emoteActive) { return; }

        var emoteFunctions = emoteNameToActionRef
            .Where(entry => entry.Key != Emote.Random)
            .Select(entry => entry.Value)
            .ToArray();

        int randIndex = Random.Range(0, emoteFunctions.Length);
        emoteFunctions[randIndex].Invoke();
    }

    // Animation Emotes

    public void fierceEmote(monsterAttackSystem attackSystem)
    {
        if (attackSystem.damageLocked)
        {
            return;
        }

        if (attackSystem.focusedAttackActive == false && attackSystem.isGrounded && attackSystem.emoteActive == false
            && attackSystem.isRunning == false && attackSystem.isWalking == false && attackSystem.isCrouching == false)
        {
            attackSystem.emoteActive = true;
            attackSystem.calm = false;
            attackSystem.myAnimator.SetBool("Idle Bounce Allowed", false);

            for (int i = 0; i < attackSystem.allMonsterParts.Length; i++)
            {
                attackSystem.allMonsterParts[i].fierceEmote();
            }

            attackSystem.forceStopCrouch();
        }
    }

    public void gasEmote(monsterAttackSystem attackSystem)
    {
        if (attackSystem.damageLocked)
        {
            return;
        }

        if (attackSystem.focusedAttackActive == false && attackSystem.isGrounded && attackSystem.emoteActive == false
            && attackSystem.isRunning == false && attackSystem.isWalking == false && attackSystem.isCrouching == false)
        {
            attackSystem.emoteActive = true;
            attackSystem.calm = false;
            attackSystem.myAnimator.SetBool("Idle Bounce Allowed", false);
            attackSystem.gasVisual.Stop();
            attackSystem.gasVisual.Play();

            for (int i = 0; i < attackSystem.allMonsterParts.Length; i++)
            {
                attackSystem.allMonsterParts[i].gasEmote();
            }

            attackSystem.forceStopCrouch();
        }
    }

    public void mockingEmote(monsterAttackSystem attackSystem)
    {
        if (attackSystem.damageLocked)
        {
            return;
        }

        if (attackSystem.focusedAttackActive == false && attackSystem.isGrounded && attackSystem.emoteActive == false
            && attackSystem.isRunning == false && attackSystem.isWalking == false && attackSystem.isCrouching == false)
        {
            attackSystem.emoteActive = true;
            attackSystem.calm = false;
            attackSystem.myAnimator.SetBool("Idle Bounce Allowed", false);

            for (int i = 0; i < attackSystem.allMonsterParts.Length; i++)
            {
                attackSystem.allMonsterParts[i].mockingEmote();
            }

            attackSystem.forceStopCrouch();
        }
    }

    public void danceEmote(monsterAttackSystem attackSystem)
    {
        if (attackSystem.damageLocked)
        {
            return;
        }

        if (attackSystem.focusedAttackActive == false && attackSystem.isGrounded && attackSystem.emoteActive == false
            && attackSystem.isRunning == false && attackSystem.isWalking == false && attackSystem.isCrouching == false)
        {
            attackSystem.emoteActive = true;
            attackSystem.calm = false;
            attackSystem.myAnimator.SetBool("Idle Bounce Allowed", false);
            attackSystem.musicVisual.Stop();
            attackSystem.musicVisual.Play();

            for (int i = 0; i < attackSystem.allMonsterParts.Length; i++)
            {
                attackSystem.allMonsterParts[i].danceEmote();
            }

            attackSystem.forceStopCrouch();
        }
    }

    public void jackEmote(monsterAttackSystem attackSystem)
    {
        if (attackSystem.myAnimator == null) { return; }

        if (attackSystem.damageLocked)
        {
            return;
        }

        if (attackSystem.focusedAttackActive == false && attackSystem.isGrounded && attackSystem.emoteActive == false && attackSystem.isRunning == false && attackSystem.isWalking == false && attackSystem.isCrouching == false)
        {
            attackSystem.emoteActive = true;
            attackSystem.calm = false;
            attackSystem.myAnimator.SetBool("Idle Bounce Allowed", false);

            for (int i = 0; i < attackSystem.allMonsterParts.Length; i++)
            {
                attackSystem.allMonsterParts[i].jackEmote();
            }

            attackSystem.forceStopCrouch();
        }
    }

    public void thinkingEmote(monsterAttackSystem attackSystem)
    {
        if (attackSystem.damageLocked)
        {
            return;
        }

        if (attackSystem.focusedAttackActive == false && attackSystem.isGrounded && attackSystem.emoteActive == false
            && attackSystem.isRunning == false && attackSystem.isWalking == false && attackSystem.isCrouching == false)
        {
            attackSystem.emoteActive = true;
            attackSystem.calm = false;
            attackSystem.myAnimator.SetBool("Idle Bounce Allowed", false);

            for (int i = 0; i < attackSystem.allMonsterParts.Length; i++)
            {
                attackSystem.allMonsterParts[i].thinkingEmote();
            }

            attackSystem.forceStopCrouch();
        }
    }

    public void booEmote(monsterAttackSystem attackSystem)
    {
        if (attackSystem.damageLocked)
        {
            return;
        }

        if (attackSystem.focusedAttackActive == false && attackSystem.isGrounded && attackSystem.emoteActive == false
            && attackSystem.isRunning == false && attackSystem.isWalking == false && attackSystem.isCrouching == false)
        {
            attackSystem.emoteActive = true;
            attackSystem.calm = false;
            attackSystem.spookyVisual.Stop();
            attackSystem.spookyVisual.Play();
            attackSystem.myAnimator.SetBool("Idle Bounce Allowed", false);

            for (int i = 0; i < attackSystem.allMonsterParts.Length; i++)
            {
                attackSystem.allMonsterParts[i].booEmote();
            }

            attackSystem.forceStopCrouch();
        }
    }

    public void excerciseEmote(monsterAttackSystem attackSystem)
    {
        if (attackSystem.damageLocked)
        {
            return;
        }

        if (attackSystem.focusedAttackActive == false && attackSystem.isGrounded && attackSystem.emoteActive == false && attackSystem.isRunning == false && attackSystem.isWalking == false && attackSystem.isCrouching == false)
        {
            attackSystem.emoteActive = true;
            attackSystem.calm = false;
            attackSystem.myAnimator.SetBool("Idle Bounce Allowed", false);

            for (int i = 0; i < attackSystem.allMonsterParts.Length; i++)
            {
                attackSystem.allMonsterParts[i].excerciseEmote();
            }

            attackSystem.forceStopCrouch();
        }
    }

    public void hulaEmote(monsterAttackSystem attackSystem)
    {
        if (attackSystem.damageLocked)
        {
            return;
        }

        if (attackSystem.focusedAttackActive == false && attackSystem.isGrounded && attackSystem.emoteActive == false
            && attackSystem.isRunning == false && attackSystem.isWalking == false && attackSystem.isCrouching == false)
        {
            attackSystem.emoteActive = true;
            attackSystem.calm = false;
            attackSystem.hulaHoopVisual.Stop();
            attackSystem.hulaHoopVisual.Play();
            attackSystem.myAnimator.SetBool("Idle Bounce Allowed", false);

            for (int i = 0; i < attackSystem.allMonsterParts.Length; i++)
            {
                attackSystem.allMonsterParts[i].hulaEmote();
            }

            attackSystem.forceStopCrouch();
        }
    }

    public void vomitEmote(monsterAttackSystem attackSystem)
    {
        if (attackSystem.damageLocked)
        {
            return;
        }

        if (attackSystem.focusedAttackActive == false && attackSystem.isGrounded && attackSystem.emoteActive == false && attackSystem.isRunning == false && attackSystem.isWalking == false && attackSystem.isCrouching == false)
        {
            attackSystem.emoteActive = true;
            attackSystem.calm = false;
            attackSystem.vomitVisual.Stop();
            attackSystem.vomitVisual.Play();
            attackSystem.myAnimator.SetBool("Idle Bounce Allowed", false);

            for (int i = 0; i < attackSystem.allMonsterParts.Length; i++)
            {
                attackSystem.allMonsterParts[i].vomitEmote();
            }

            attackSystem.forceStopCrouch();
        }
    }

    public void brianEmote(monsterAttackSystem attackSystem)
    {
        if (attackSystem.damageLocked)
        {
            return;
        }

        if (attackSystem.focusedAttackActive == false && attackSystem.isGrounded && attackSystem.emoteActive == false
            && attackSystem.isRunning == false && attackSystem.isWalking == false && attackSystem.isCrouching == false)
        {
            attackSystem.emoteActive = true;
            attackSystem.calm = false;
            attackSystem.myAnimator.SetBool("Idle Bounce Allowed", false);

            for (int i = 0; i < attackSystem.allMonsterParts.Length; i++)
            {
                attackSystem.allMonsterParts[i].brianEmote();
            }

            attackSystem.forceStopCrouch();
        }
    }

    public void sleepEmote(monsterAttackSystem attackSystem)
    {
        if (attackSystem.damageLocked)
        {
            return;
        }

        if (attackSystem.focusedAttackActive == false && attackSystem.isGrounded && attackSystem.emoteActive == false
            && attackSystem.isRunning == false && attackSystem.isWalking == false && attackSystem.isCrouching == false)
        {
            attackSystem.emoteActive = true;
            attackSystem.calm = false;

            if (attackSystem.facingRight)
            {
                //sleepVisual_Right.Stop();
                //sleepVisual_Right.Play();
                attackSystem.sleepVisual.Stop();
                attackSystem.sleepVisual.Play();
            }
            else
            {
                attackSystem.sleepVisual.Stop();
                attackSystem.sleepVisual.Play();
            }

            attackSystem.myAnimator.SetBool("Idle Bounce Allowed", false);

            for (int i = 0; i < attackSystem.allMonsterParts.Length; i++)
            {
                attackSystem.allMonsterParts[i].sleepEmote();
            }

            attackSystem.forceStopCrouch();
        }
    }

    public void explosiveEmote(monsterAttackSystem attackSystem)
    {
        if (attackSystem.damageLocked)
        {
            return;
        }

        if (attackSystem.focusedAttackActive == false && attackSystem.isGrounded && attackSystem.emoteActive == false
            && attackSystem.isRunning == false && attackSystem.isWalking == false && attackSystem.isCrouching == false)
        {
            attackSystem.emoteActive = true;
            attackSystem.calm = false;
            attackSystem.fieryVisual.Stop();
            attackSystem.fieryVisual.Play();
            attackSystem.myAnimator.SetBool("Idle Bounce Allowed", false);

            for (int i = 0; i < attackSystem.allMonsterParts.Length; i++)
            {
                attackSystem.allMonsterParts[i].explosiveEmote();
            }

            attackSystem.forceStopCrouch();
        }
    }

    public void laughingEmote(monsterAttackSystem attackSystem)
    {
        if (attackSystem.damageLocked)
        {
            return;
        }

        if (attackSystem.focusedAttackActive == false && attackSystem.isGrounded && attackSystem.emoteActive == false && attackSystem.isRunning == false && attackSystem.isWalking == false && attackSystem.isCrouching == false)
        {
            attackSystem.emoteActive = true;
            attackSystem.calm = false;
            attackSystem.myAnimator.SetBool("Idle Bounce Allowed", false);

            for (int i = 0; i < attackSystem.allMonsterParts.Length; i++)
            {
                attackSystem.allMonsterParts[i].laughingEmote();
            }

            attackSystem.forceStopCrouch();
        }
    }

    public void sneezingEmote(monsterAttackSystem attackSystem)
    {
        if (attackSystem.damageLocked)
        {
            return;
        }

        if (attackSystem.focusedAttackActive == false && attackSystem.isGrounded && attackSystem.emoteActive == false && attackSystem.isRunning == false && attackSystem.isWalking == false && attackSystem.isCrouching == false)
        {
            attackSystem.emoteActive = true;
            attackSystem.calm = false;
            attackSystem.sneezeVisual.Stop();
            attackSystem.sneezeVisual.Play();
            attackSystem.myAnimator.SetBool("Idle Bounce Allowed", false);

            for (int i = 0; i < attackSystem.allMonsterParts.Length; i++)
            {
                attackSystem.allMonsterParts[i].sneezingEmote();
            }

            attackSystem.forceStopCrouch();
        }
    }

}
