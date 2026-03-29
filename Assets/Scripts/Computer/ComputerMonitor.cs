using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;

public class ComputerMonitor : MonoBehaviour, IInteractable
{
    [SerializeField] private CinemachineCamera seatCamera;

    public string InteractHint => "Bilgisayara Otur";

    
    private void Start()
    {
        ComputerState.Instance.OnPlayerOut += Stand;
    }
    
    private void OnDestroy()
    {
        if (ComputerState.Instance != null)
            ComputerState.Instance.OnPlayerOut -= Stand;
    }

    private void Update()
    {
        if (ComputerState.Instance.IsPlayerSit && Keyboard.current.tabKey.wasPressedThisFrame)
            ComputerState.Instance.PlayerStand();
    }
    

    // ── IInteractable ────────────────────────────────────────────

    public void Interact()
    {
        if (ComputerState.Instance.IsPlayerSit) return;
        Sit();
    }

    private void Sit()
    {
        seatCamera.Priority = 2;

        ComputerState.Instance.PlayerSit();
    }

    private void Stand()
    {
        seatCamera.Priority = -1;
    }
}