using System;
using UnityEngine;

[Serializable]
struct FootstepStruct
{
    public FootstepList footsteps;
    public SurfaceType surfaceType;
}

[Serializable]
struct FootstepList
{
    public AudioClip[] walkFootsteps;
    public AudioClip[] runFootsteps;
    public AudioClip[] landFootsteps;
}

public class Footstep : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioSource audioSourceLand;
    [SerializeField] private FootstepStruct[] footstepList;
    
    private SurfaceType currentSurface;
    private int lastIndex = -1;
    
    public void Initialize(Headbob headbob, PlayerCharacter playerCharacter)
    {
        headbob.OnFootstep += Headbob_OnFootstep;
        playerCharacter.OnLand += PlayerCharacter_OnLand;
    }

    public void UpdateFootstep(float deltaTime, Vector3 raycastOrigin)
    {
        if (Physics.Raycast(raycastOrigin, Vector3.down, out RaycastHit hit, 10f))
        {
            SurfaceIdentifier surface = hit.collider.GetComponent<SurfaceIdentifier>();
            if (surface != null)
            {
                currentSurface = surface.GetSurfaceType();
            }
            else
            {
                currentSurface = SurfaceType.Rock;
            }
        }
    }
    
    private void Headbob_OnFootstep(bool isSprinting, bool isCrouching)
    {
        foreach (var footstepStruct in footstepList)
        {
            if (footstepStruct.surfaceType == currentSurface)
            {
                int randomIndex;
                
                if (isSprinting)
                {
                    do
                    {
                        randomIndex = UnityEngine.Random.Range(0, footstepStruct.footsteps.runFootsteps.Length);
                    } while (randomIndex == lastIndex);
                    lastIndex = randomIndex;
                    audioSource.clip = footstepStruct.footsteps.runFootsteps[randomIndex];
                }
                else
                {
                    do
                    {
                        randomIndex = UnityEngine.Random.Range(0, footstepStruct.footsteps.walkFootsteps.Length);
                    } while (randomIndex == lastIndex);
                    lastIndex = randomIndex;
                    audioSource.clip = footstepStruct.footsteps.walkFootsteps[randomIndex];
                }
                audioSource.Play();
                return;
            }
        }
    }
    
    private void PlayerCharacter_OnLand()
    {
        foreach (var footstepStruct in footstepList)
        {
            if (footstepStruct.surfaceType == currentSurface)
            {
                int randomIndex = UnityEngine.Random.Range(0, footstepStruct.footsteps.landFootsteps.Length);
                
                audioSourceLand.clip = footstepStruct.footsteps.landFootsteps[randomIndex];
                
                audioSourceLand.Play();
                return;
            }
        }
    }
}
