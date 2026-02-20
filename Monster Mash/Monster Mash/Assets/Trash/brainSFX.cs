using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class brainSFX : MonoBehaviour
{
    public int brainNumber;
    public AudioSource voiceOutput;
    public AudioSource universalOutput;
    public int volumeModifier = 1;

    [Header("Universal Audio Libraries")]
    public AudioClip universalJumpSound;
    public AudioClip universalLandSound;

    [Header("Brain Audio Libraries")]
    //public AudioClip[] jump1Library;
    //public AudioClip[] jump2Library;
    //public AudioClip[] jump3Library;
    public AudioClip[] doubleJump1Library;
    public AudioClip[] doubleJump2Library;
    public AudioClip[] doubleJump3Library;
    //public AudioClip[] land1Library;
    //public AudioClip[] land2Library;
    //public AudioClip[] land3Library;
    public AudioClip[] neutralAttack1Library;
    public AudioClip[] nuetralAttack2Library;
    public AudioClip[] neutralAttack3Library;
    public AudioClip[] heavyAttack1Library;
    public AudioClip[] heavyAttack2Library;
    public AudioClip[] heavyAttack3Library;
    public AudioClip[] neutralDamage1Library;
    public AudioClip[] neutralDamage2Library;
    public AudioClip[] heavyDamage1Library;
    public AudioClip[] heavyDamage2Library;
    public AudioClip[] agreeLibrary;
    public AudioClip[] disagreeLibrary;
    public AudioClip[] successLibrary;
    public AudioClip[] failLibrary;

    //Chosen Brain Sounds
    //private AudioClip jump1Sound;
    //private AudioClip jump2Sound;
    //private AudioClip jump3Sound;
    private AudioClip doubleJump1Sound;
    private AudioClip doubleJump2Sound;
    private AudioClip doubleJump3Sound;
    //private AudioClip land1Sound;
    //private AudioClip land2Sound;
    //private AudioClip land3Sound;
    private AudioClip neutralAttack1Sound;
    private AudioClip neutralAttack2Sound;
    private AudioClip neutralAttack3Sound;
    private AudioClip heavyAttack1Sound;
    private AudioClip heavyAttack2Sound;
    private AudioClip heavyAttack3Sound;
    private AudioClip neutralDamage1Sound;
    private AudioClip neutralDamage2Sound;
    private AudioClip heavyDamage1Sound;
    private AudioClip heavyDamage2Sound;
    private AudioClip agreeSound;
    private AudioClip disagreeSound;
    private AudioClip successSound;
    private AudioClip failSound;

    public void updateBrainSounds()
    {
        //jump1Sound = jump1Library[brainNumber];
        //jump2Sound = jump2Library[brainNumber];
        //jump3Sound = jump3Library[brainNumber];
        doubleJump1Sound = doubleJump1Library[brainNumber];
        doubleJump2Sound = doubleJump2Library[brainNumber];
        doubleJump3Sound = doubleJump3Library[brainNumber];
        //land1Sound = land1Library[brainNumber];
        //land2Sound = land2Library[brainNumber];
        //land3Sound = land3Library[brainNumber];
        neutralAttack1Sound = neutralAttack1Library[brainNumber];
        neutralAttack2Sound = nuetralAttack2Library[brainNumber];
        neutralAttack3Sound = neutralAttack3Library[brainNumber];
        heavyAttack1Sound = heavyAttack1Library[brainNumber];
        heavyAttack2Sound = heavyAttack2Library[brainNumber];
        heavyAttack3Sound = heavyAttack3Library[brainNumber];
        neutralDamage1Sound = neutralDamage1Library[brainNumber];
        neutralDamage2Sound = neutralDamage2Library[brainNumber];
        heavyDamage1Sound = heavyDamage1Library[brainNumber];
        heavyDamage2Sound = heavyDamage2Library[brainNumber];
        agreeSound = agreeLibrary[brainNumber];
        disagreeSound = disagreeLibrary[brainNumber];
        successSound = successLibrary[brainNumber];
        failSound = failLibrary[brainNumber];
    }

    #region Audio Clips
    /*
    public void playJump1Sound()
    {
        voiceOutput.Stop();
        voiceOutput.clip = jump1Sound;
        voiceOutput.Play();
    }

    public void playJump2Sound()
    {
        voiceOutput.Stop();
        voiceOutput.clip = jump2Sound;
        voiceOutput.Play();
    }

    public void playJump3Sound()
    {
        voiceOutput.Stop();
        voiceOutput.clip = jump3Sound;
        voiceOutput.Play();
    }
    */

    public void playJumpSound()
    {
        universalOutput.Stop();
        universalOutput.volume = 0.25f * volumeModifier;
        universalOutput.clip = universalJumpSound;
        universalOutput.Play();
    }

    public void playLandSound()
    {
        universalOutput.Stop();
        universalOutput.volume = 0.05f * volumeModifier;
        universalOutput.clip = universalLandSound;
        universalOutput.Play();
    }

    public void playDoubleJump1Sound()
    {
        voiceOutput.Stop();
        voiceOutput.volume = 0.25f * volumeModifier;
        voiceOutput.clip = doubleJump1Sound;
        voiceOutput.Play();
        playJumpSound();
    }

    public void playDoubleJump2Sound()
    {
        voiceOutput.Stop();
        voiceOutput.volume = 0.25f * volumeModifier;
        voiceOutput.clip = doubleJump2Sound;
        voiceOutput.Play();
        playJumpSound();
    }
    public void playDoubleJump3Sound()
    {
        voiceOutput.Stop();
        voiceOutput.volume = 0.25f * volumeModifier;
        voiceOutput.clip = doubleJump3Sound;
        voiceOutput.Play();
        playJumpSound();
    }

    /*
    public void playLand1Sound()
    {
        voiceOutput.Stop();
        voiceOutput.clip = land1Sound;
        voiceOutput.Play();
    }

    public void playLand2Sound()
    {
        voiceOutput.Stop();
        voiceOutput.clip = land2Sound;
        voiceOutput.Play();
    }

    public void playLand3Sound()
    {
        voiceOutput.Stop();
        voiceOutput.clip = land3Sound;
        voiceOutput.Play();
    }
    */

    public void playNeutralAttack1Sound()
    {
        voiceOutput.Stop();
        voiceOutput.clip = neutralAttack1Sound;
        voiceOutput.Play();
    }

    public void playNeutralAttack2Sound()
    {
        voiceOutput.Stop();
        voiceOutput.clip = neutralAttack2Sound;
        voiceOutput.Play();
    }

    public void playNeutralAttack3Sound()
    {
        voiceOutput.Stop();
        voiceOutput.clip = neutralAttack3Sound;
        voiceOutput.Play();
    }

    public void playHeavyAttack1Sound()
    {
        voiceOutput.Stop();
        voiceOutput.clip = heavyAttack1Sound;
        voiceOutput.Play();
    }

    public void playHeavyAttack2Sound()
    {
        voiceOutput.Stop();
        voiceOutput.clip = heavyAttack2Sound;
        voiceOutput.Play();
    }

    public void playHeavyAttack3Sound()
    {
        voiceOutput.Stop();
        voiceOutput.clip = heavyAttack3Sound;
        voiceOutput.Play();
    }

    public void playNeutralDamage1Sound()
    {
        voiceOutput.Stop();
        voiceOutput.clip = neutralDamage1Sound;
        voiceOutput.Play();
    }

    public void playNeutralDamage2Sound()
    {
        voiceOutput.Stop();
        voiceOutput.clip = neutralDamage2Sound;
        voiceOutput.Play();
    }

    public void playHeavyDamage1Sound()
    {
        voiceOutput.Stop();
        voiceOutput.clip = heavyDamage1Sound;
        voiceOutput.Play();
    }

    public void playHeavyDamage2Sound()
    {
        voiceOutput.Stop();
        voiceOutput.clip = heavyDamage2Sound;
        voiceOutput.Play();
    }

    public void playAgreeSound()
    {
        voiceOutput.Stop();
        voiceOutput.clip = agreeSound;
        voiceOutput.Play();
    }

    public void playDisagreeSound()
    {
        voiceOutput.Stop();
        voiceOutput.clip = disagreeSound;
        voiceOutput.Play();
    }

    public void playSuccessSound()
    {
        voiceOutput.Stop();
        voiceOutput.clip = successSound;
        voiceOutput.Play();
    }

    public void playFailSound()
    {
        voiceOutput.Stop();
        voiceOutput.clip = failSound;
        voiceOutput.Play();
    }
    #endregion
}
