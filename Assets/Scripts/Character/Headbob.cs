using System;
using UnityEngine;

public class Headbob : MonoBehaviour
{
    /// <summary>
    /// Plays footstep takes IsSprinting and IsCrouching
    /// </summary>
    public event Action<bool, bool> OnFootstep;
    private bool hasTriggeredFootstep = false;
    
    [Header("Position Headbob")]
    [SerializeField] private Vector2 walkBobFrequency = new Vector2(0f, 14f); 
    [SerializeField] private Vector2 walkBobAmplitude = new Vector2(0f, 0.06f); 
    [Space]
    [SerializeField] private Vector2 sprintBobFrequency = new Vector2(15f, 20f); 
    [SerializeField] private Vector2 sprintBobAmplitude = new Vector2(0.1f, 0.07f); 
    
    private Vector3 originalPosition;
    private float positionTimerY; 
    private float positionTimerX; 
    
    [Header("Rotation Headbob")]
    [SerializeField] private float sprintRollFrequency = 11f;
    [SerializeField] private float sprintRollAmplitude = 0.2f;

    private Quaternion originalRotation;
    private float rotationTimerX;
    
    //Smoothing
    [Header("Smoothing")]
    [SerializeField] private float frequencyLerpSpeed = 10f;
    [SerializeField] private float amplitudeLerpSpeed = 10f;
    
    private float currentBobFrequencyY;
    private float currentBobFrequencyX;
    private float currentBobAmplitudeY;
    private float currentBobAmplitudeX;
    
    public void Initialize()
    {
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;
        
        positionTimerY = 0f;
        positionTimerX = 0f;
        
        rotationTimerX = 0f;

        currentBobFrequencyY = walkBobFrequency.y;
        currentBobFrequencyX = walkBobFrequency.x;
        currentBobAmplitudeY = walkBobAmplitude.y;
        currentBobAmplitudeX = walkBobAmplitude.x;
    }

    public void UpdateHeadbob(float deltaTime, bool bIsSprinting, bool bIsCrouching, bool bIsGrounded, bool bIsMoving)
    {
        if (!bIsGrounded || !bIsMoving || bIsCrouching)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition, 2f * deltaTime);
            transform.localRotation = originalRotation;
            hasTriggeredFootstep = false;
            currentBobFrequencyY = walkBobFrequency.y;
            currentBobFrequencyX = walkBobFrequency.x;
            currentBobAmplitudeY = walkBobAmplitude.y;
            currentBobAmplitudeX = walkBobAmplitude.x;
            return;
        }

        float targetFrequencyY = bIsSprinting ? sprintBobFrequency.y : walkBobFrequency.y;
        float targetFrequencyX = bIsSprinting ? sprintBobFrequency.x : walkBobFrequency.x;
        float targetAmplitudeY = bIsSprinting ? sprintBobAmplitude.y : walkBobAmplitude.y;
        float targetAmplitudeX = bIsSprinting ? sprintBobAmplitude.x : walkBobAmplitude.x;

        currentBobFrequencyY = Mathf.Lerp(currentBobFrequencyY, targetFrequencyY, deltaTime * frequencyLerpSpeed);
        currentBobFrequencyX = Mathf.Lerp(currentBobFrequencyX, targetFrequencyX, deltaTime * frequencyLerpSpeed);
        currentBobAmplitudeY = Mathf.Lerp(currentBobAmplitudeY, targetAmplitudeY, deltaTime * amplitudeLerpSpeed);
        currentBobAmplitudeX = Mathf.Lerp(currentBobAmplitudeX, targetAmplitudeX, deltaTime * amplitudeLerpSpeed);
        
        positionTimerY += deltaTime * currentBobFrequencyY;
        positionTimerX += deltaTime * currentBobFrequencyX;
        
        transform.localPosition = new Vector3(
            originalPosition.x + Mathf.Sin(positionTimerX) * currentBobAmplitudeX, 
            originalPosition.y + Mathf.Sin(positionTimerY) * currentBobAmplitudeY, 
            transform.localPosition.z);
        
        if (Mathf.Sin(positionTimerY) < -0.99f && OnFootstep != null && !hasTriggeredFootstep) // Sin wave reaches the lowest point
        {
            OnFootstep.Invoke(bIsSprinting, bIsCrouching);
            hasTriggeredFootstep = true;
        }
        else if (Mathf.Sin(positionTimerY) > 0f)
        {
            hasTriggeredFootstep = false;
        }
        
        //Rotation headbob
        rotationTimerX += deltaTime * (bIsSprinting ? sprintRollFrequency : 0);
        float rollOffset = Mathf.Sin(rotationTimerX) * (bIsSprinting ? sprintRollAmplitude : 0f);
        transform.localRotation = originalRotation * Quaternion.Euler(0, 0, rollOffset);
    }
}
