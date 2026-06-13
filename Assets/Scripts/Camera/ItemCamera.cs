using System.Collections.Generic;
using System.IO;
using DG.Tweening;
using MoreMountains.Feedbacks;
using SyntaxSultan.InventoryModule;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;

public class ItemCamera : BasePickableItem, IComplexUsable, IStorable
{
    [Header("Lens")]
    [SerializeField] private Camera lensCamera;
    [Tooltip("Must be same RenderTexture with lensCamera.targetTexture.")]
    [SerializeField] private RenderTexture viewfinderTexture;
    [SerializeField] private CameraScreen cameraScreen;
    
    [Header("Zoom")]
    [SerializeField] private float zoomSpeed = 0.2f;
    [SerializeField] private Ease zoomEase = Ease.OutQuad;
    [SerializeField] private float maxFov = 100;
    [SerializeField] private float minFov = 20;  
    [SerializeField] private float normalFov = 50; 
    
    [SerializeField] private Vector3 focusOnCameraLocalPosition;

    [Header("Photo Settings")]
    [Tooltip("Saved photo resolution.")]
    [SerializeField] private Vector2Int photoResolution = new(256, 256);

    [Tooltip("Max camera storage")]
    [SerializeField] private int maxLocalPhotos = 10;

    [Header("Sound / Effect")]
    [SerializeField] private MMF_Player photoTakenEffects;
    
    private bool useFlash = true;
    private bool wasRightButtonPressed;
    private float currentFOV;
    private readonly List<Texture2D> localPhotos = new();
    private bool isHeld;
    
    [Header("Input References")]
    [SerializeField] private InputActionReference shootAction;
    [SerializeField] private InputActionReference focusAction;
    [SerializeField] private InputActionReference toggleFlashAction;
    [SerializeField] private InputActionReference uploadPhotosAction;

    [Header("Localization Hints")]
    [SerializeField] private LocalizedString shootHint;
    [SerializeField] private LocalizedString focusHint;
    [SerializeField] private LocalizedString toggleFlashHint;
    [SerializeField] private LocalizedString uploadPhotosHint;

    private List<ItemInteraction> interactions;

    protected override void Awake()
    {
        base.Awake();
        InitializeInteractions();
        currentFOV = lensCamera.fieldOfView;
    }
    
    private void InitializeInteractions()
    {
        interactions = new List<ItemInteraction>();

        // 1. Fotoğraf Çekme Etkileşimi
        var shootInteract = new ItemInteraction(shootAction, shootHint);
        shootInteract.OnPerformed += ctx => TakePhoto();
        interactions.Add(shootInteract);

        // 2. Ayar Değiştirme Etkileşimi
        var settingsInteract = new ItemInteraction(toggleFlashAction, toggleFlashHint);
        settingsInteract.OnPerformed += ctx => ToggleFlash();
        interactions.Add(settingsInteract);

        // 3. Fotoğraf Silme Etkileşimi
        var deleteInteract = new ItemInteraction(uploadPhotosAction, uploadPhotosHint);
        deleteInteract.OnPerformed += ctx => TransferToComputer();
        interactions.Add(deleteInteract);
        
        var focusInteract = new ItemInteraction(focusAction, focusHint);
        focusInteract.OnPerformed += ctx => HandleCameraFocus();
        interactions.Add(focusInteract);
    }

    private void Update()
    {
        if (!isHeld) return;
        
        HandleZoomInput();
    }

    private void HandleCameraFocus()
    {
        wasRightButtonPressed = !wasRightButtonPressed;
        if (wasRightButtonPressed)
        {
            PlayerInteraction.Instance.DisableInteraction();
            SetLocalPosition(focusOnCameraLocalPosition);
        }
        else
        {
            PlayerInteraction.Instance.EnableInteraction();
            SetLocalPosition(ItemData.holdPositionOffset);
        }
    } 

    private void HandleZoomInput()
    {
        float scroll = Mouse.current.scroll.ReadValue().y;

        if (scroll != 0f)
        {
            float targetFOV = Mathf.Clamp(
                currentFOV - scroll * zoomSpeed * (maxFov - minFov),
                minFov,
                maxFov
            );

            currentFOV = targetFOV;

            lensCamera.DOKill();
            lensCamera.DOFieldOfView(targetFOV, 0.25f).SetEase(zoomEase);
            
            cameraScreen.UpdateZoomDisplay(currentFOV, normalFov);
        }
    }
    
    public void TransferToComputer()
    {
        if (localPhotos.Count == 0) return;

        CameraStorage.Instance.Upload(localPhotos);
        //int count = localPhotos.Count;
        localPhotos.Clear();
    }
    
    protected override void OnPickedUp()
    {
        isHeld = true;
        lensCamera.enabled = true;
        cameraScreen.SetScreenActive(true);
        cameraScreen.UpdateZoomDisplay(currentFOV, normalFov);
    }

    protected override void OnDropped()
    {
        isHeld = false;
        wasRightButtonPressed = false; 
        lensCamera.enabled = false;
        cameraScreen.SetScreenActive(false);
    }

    private void ToggleFlash()
    {
        useFlash = !useFlash;
        cameraScreen.SetFlashImage(useFlash);
    }

    private void TakePhoto()
    {
        if (localPhotos.Count >= maxLocalPhotos) return;
        
        if (viewfinderTexture == null)
        {
            Debug.LogError("[GameCamera] viewfinderTexture atanmamış!");
            return;
        }

        Texture2D snap = new Texture2D(photoResolution.x, photoResolution.y, TextureFormat.RGB24, false);

        RenderTexture prev = RenderTexture.active;
        RenderTexture.active = viewfinderTexture;

        snap.ReadPixels(new Rect(0, 0, viewfinderTexture.width, viewfinderTexture.height), 0, 0);
        snap.Apply();
        
        RenderTexture.active = prev;
        
        SaveAsPNG(snap);

        localPhotos.Add(snap);

        photoTakenEffects.FeedbacksList[1].Active = useFlash;
        photoTakenEffects.PlayFeedbacks();
    }

    private void SaveAsPNG(Texture2D textureToSave)
    {
        byte[] bytes = textureToSave.EncodeToPNG();

        string folderPath = Path.Combine(Application.persistentDataPath, "SavedPhotos");
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        string filePath = Path.Combine(folderPath, $"Photo_{System.DateTime.Now:yyyyMMdd_HHmmss}.png");
        try
        {
            File.WriteAllBytes(filePath, bytes);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Save failed: {e.Message}");
        }
    }
    
    public bool CanStore => true;
    public Sprite Icon => ItemData.icon;
    public List<ItemInteraction> GetInteractions()
    {
        return interactions;
    }
}