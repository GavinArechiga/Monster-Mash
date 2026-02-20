using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerAudioManager : MonoBehaviour
{
    public AudioSource brainSoundMachine;
    //heavy attacking brain library
    //idle brain library
    //wind up brain library 
    //heavy damage brain library
    //double jump brain library
    //part removal brain library
    //destruction brain library

    public AudioSource locomotionSoundMachine;
    public AudioClip jumpSound;
    public AudioClip doubleJumpSound;
    //running sound
    //walking sound
    //jump sound
    //double jump sound
    //land sound
    //glide sound
    //player bounce sound

    public AudioSource attackSoundMachine;
    //collect all neutral attack sounds from monster parts
    //collect all heavy attack sounds from monster parts
    //some universal wind up sound

    public AudioSource damageSoundMachine;
    //library of neutral damage sounds
    public AudioClip[] neutralDamageSounds;
    private int neutralDamageSoundCounter = 0;
    public AudioClip[] heavyDamageSounds;
    private int heavyDamageSoundCounter = 0;
    //library of heavy damage sounds
    //part removal sound
    //destruction sound

    public AudioSource statusEffectSoundMachine;
    //burned sound effect
    //electrified sound effect
    //poisioned sound effect
    //stinky sound effect
    //cursed sound effect
    //confused sound effect
    //slimed sound effect
    //frozen sound effect
    //squashed sound effect
    //slowed sound effect
    //grabbed sound effect

    public AudioSource miscSoundMachine;

    public void playNeutralDamageSound()
    {
        if (neutralDamageSoundCounter < neutralDamageSounds.Length - 1)
        {
            neutralDamageSoundCounter++;
        }
        else
        {
            neutralDamageSoundCounter = 0;
        }

        damageSoundMachine.Stop();
        damageSoundMachine.clip = neutralDamageSounds[neutralDamageSoundCounter];
        damageSoundMachine.Play();
    }

    public void playHeavyDamageSound()
    {
        if (heavyDamageSoundCounter < heavyDamageSounds.Length - 1)
        {
            heavyDamageSoundCounter++;
        }
        else
        {
            heavyDamageSoundCounter = 0;
        }

        damageSoundMachine.Stop();
        damageSoundMachine.clip = heavyDamageSounds[heavyDamageSoundCounter];
        damageSoundMachine.Play();
    }

    public void playJumpSound()
    {
        locomotionSoundMachine.Stop();
        locomotionSoundMachine.clip = jumpSound;
        locomotionSoundMachine.Play();
    }

    public void playDoubleJumpSound()
    {
        locomotionSoundMachine.Stop();
        locomotionSoundMachine.clip = doubleJumpSound;
        locomotionSoundMachine.Play();
    }
}
