using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class monstroMiscVisuals : MonoBehaviour
{
    public Transform groundCheck;
    public GameObject playerRingPrefab;
    private GameObject playerRing;
    private bool playerRingActive = true;

    //effects
    public ParticleSystem burningEffect;
    public ParticleSystem electricEffect;
    public ParticleSystem lostLimbEffect;
    public ParticleSystem destroyedMonsterEffect;
    public GameObject destroyedBrainBall;

    void Update()
    {
        if (playerRing != null)
        {
            RaycastHit detectedGroundPoint;

            if (Physics.Raycast(groundCheck.position, Vector3.down, out detectedGroundPoint, 100))
            {
                Vector3 normalizedPlayerPoint = new Vector3(detectedGroundPoint.point.x, detectedGroundPoint.point.y + 0.2f, detectedGroundPoint.point.z);

                playerRing.transform.position = normalizedPlayerPoint;
            }
        }
    }

    public void generatePlayerRing()
    {
        playerRing = Instantiate(playerRingPrefab);
        hidePlayerRing();
    }

    public void hidePlayerRing()
    {
        playerRing.SetActive(false);
    }

    public void showPlayerRing()
    {
        playerRing.SetActive(true);
    }

    public void playFireEffect()
    {
        burningEffect.Play();
    }

    public void stopFireEffect()
    {
        burningEffect.Stop();
    }

    public void playElectricEffect()
    {
        electricEffect.Play();
    }

    public void stopElectricEffect()
    {
        electricEffect.Stop();
    }

    public void playLostLimbEffect()
    {
        lostLimbEffect.Play();
    }

    public void playDestroyedMonsterEffect()
    {
        destroyedMonsterEffect.Play();
        StartCoroutine(brainBallVisualDelay());
    }

    IEnumerator brainBallVisualDelay()
    {
        yield return new WaitForSecondsRealtime(1.5f);
        destroyedBrainBall.SetActive(true);
    }
}
