using System;
using UnityEngine;

/// <summary>
/// Bilgisayar oturumunun merkezi state deposu.
/// </summary>
public class ComputerState : MonoBehaviour
{
    public static ComputerState Instance { get; private set; }

    public bool IsComputerPowered {  get; private set; }
    public bool IsPlayerSit { get; private set; }

    public event Action OnPowerOn;
    public event Action OnPowerOut;
    public event Action OnPlayerSit;
    public event Action OnPlayerOut;

    private void Awake()
    {
        if (Instance != null && Instance != this) 
        { 
            Destroy(gameObject); 
            return; 
        }
        Instance = this;
    }

    private void Start()
    {
        PowerOff();
    }

    public void TogglePower()
    {
        if (IsComputerPowered)
        {
            PowerOff();
        }
        else PowerOn();
    }

    public void PowerOn()
    {
        IsComputerPowered = true;
        OnPowerOn?.Invoke();
    }

    public void PowerOff()
    {
        IsComputerPowered = false;
        OnPowerOut?.Invoke();
    }

    public void PlayerSit()
    {
        if (IsPlayerSit) return;
        IsPlayerSit = true;
        OnPlayerSit?.Invoke();
        PlayerInteraction.Instance.DisableInteraction();
    }

    public void PlayerStand()
    {
        if (!IsPlayerSit) return;
        IsPlayerSit = false;
        OnPlayerOut?.Invoke();
        PlayerInteraction.Instance.EnableInteraction();
    }
}