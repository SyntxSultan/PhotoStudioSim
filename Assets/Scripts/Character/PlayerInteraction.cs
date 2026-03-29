using System;
using UnityEngine;

/// <summary>
/// TEŞHİS ÖNCELIK SIRASI (aynı objede ikisi varsa):
///   1) Elde item yoksa + IPickable varsa → item al
///   2) Elde item varsa + IInteractable varsa → interact et
///   3) Elde item yoksa + sadece IInteractable varsa → interact et
/// </summary>
public class PlayerInteraction : MonoBehaviour
{
    public static PlayerInteraction Instance { get; private set; }

    [Header("Raycast Ayarları")]
    [SerializeField] private float interactionRange = 4f;
    [SerializeField] private LayerMask interactableLayer;

    private bool shouldCheckInteraction = true;
    private Camera mainCam;
    
    //---------Public API-----------
    
    public IPickable DetectedPickable { get; private set; }
    public IInteractable DetectedInteractable { get; private set; }
    public bool HasDetection => DetectedPickable != null || DetectedInteractable != null;
    public event Action<bool> OnShouldCheckInteractionStateChanged;
    public event Action<IPickable, IInteractable> OnDetectionChanged;
    
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
        mainCam = Camera.main;
        if (!mainCam)
            Debug.LogError("[PlayerInteraction] Ana kamera bulunamadı!");
    }

    private void Update()
    {
        if (!shouldCheckInteraction) return;
        PerformRaycast();
        HandleInteractInput();
    }

    public void DisableInteraction()
    {
        shouldCheckInteraction = false;
        DetectedPickable = null;
        DetectedInteractable = null;
        OnShouldCheckInteractionStateChanged?.Invoke(shouldCheckInteraction);
    }

    public void EnableInteraction()
    {
        shouldCheckInteraction = true;
        OnShouldCheckInteractionStateChanged?.Invoke(shouldCheckInteraction);
    }

    private void PerformRaycast()
    {
        var prevPickable = DetectedPickable;
        var prevInteractable = DetectedInteractable;
    
        DetectedPickable = null;
        DetectedInteractable = null;

        if (mainCam)
        {
            Ray ray = mainCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

            if (Physics.Raycast(ray, out RaycastHit hit, interactionRange, interactableLayer))
            {
                Collider hitCol = hit.collider;

                if (!PlayerItemHolder.Instance.IsHoldingItem)
                {
                    hitCol.TryGetComponent(out IPickable detectedPick);
                    DetectedPickable = detectedPick;
                }

                DetectedInteractable = hitCol.GetComponent<IInteractable>();
            }
        }

        if (prevPickable != DetectedPickable || prevInteractable != DetectedInteractable)
            OnDetectionChanged?.Invoke(DetectedPickable, DetectedInteractable);
    }

    private void HandleInteractInput()
    {
        if (!InputManager.Instance.InteractKeyPressed()) return;

        if (DetectedPickable != null)
        {
            PlayerItemHolder.Instance.TryPickup(DetectedPickable);
            return;
        }

        if (DetectedInteractable != null)
        {
            DetectedInteractable.Interact();
        }
    }
}