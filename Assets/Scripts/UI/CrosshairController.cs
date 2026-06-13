using UnityEngine;
using UnityEngine.UI;

public class CrosshairController : MonoBehaviour
{
    [Header("Crosshair Image")]
    [SerializeField] private Image crosshairImage;
    [SerializeField] private Sprite defaultCrosshair;
    [SerializeField] private Sprite handCrosshair;
    [SerializeField] private Sprite interactCrosshair;

    [Header("Crosshair Scale Animation")]
    [SerializeField] private float normalSize = 32f;
    [SerializeField] private float highlightSize = 42f;
    [SerializeField, Range(0f, 0.99f)]private float sizeSpeed = 10f;

    private bool isCrosshairEnabled = true;
    private PlayerInteraction interaction;
    private RectTransform crosshairRect;

    private void Awake()
    {
        if (crosshairImage != null)
            crosshairRect = crosshairImage.rectTransform;
    }

    private void Start()
    {
        interaction = PlayerInteraction.Instance;
        interaction.OnShouldCheckInteractionStateChanged += PlayerInteract_CheckInteractChanged;
        interaction.OnDetectionChanged += PlayerInteract_HandleDetectionChanged;
    }

    private void PlayerInteract_CheckInteractChanged(bool checkInteraction)
    {
        isCrosshairEnabled = checkInteraction;
        crosshairImage.gameObject.SetActive(checkInteraction);
    }
    
    private void PlayerInteract_HandleDetectionChanged(IPickable pickable, IInteractable interactable)
    {
        if (pickable != null)
            crosshairImage.sprite = handCrosshair;
        else if (interactable != null)
            crosshairImage.sprite = interactCrosshair;
        else
            crosshairImage.sprite = defaultCrosshair;
    }

    private void Update()
    {
        if (!isCrosshairEnabled) return;
        UpdateCrosshairSize();
    }

    private void UpdateCrosshairSize()
    {
        float targetSize = interaction.HasDetection ? highlightSize : normalSize;
        Vector2 currentSize = crosshairRect.sizeDelta;
        float current = currentSize.x;

        const float tolerance = 0.001f; // Adjustable tolerance

        if (Mathf.Abs(current - targetSize) <= tolerance)
        {
            if (Mathf.Abs(current - targetSize) > 0f)
            {
                currentSize.x = currentSize.y = targetSize;
                crosshairRect.sizeDelta = currentSize;
            }
            return;
        }

        float t = 1f - Mathf.Pow(1f - Mathf.Clamp01(sizeSpeed), Time.deltaTime * 60f);
        float newSize = Mathf.Lerp(current, targetSize, t);

        currentSize.x = currentSize.y = newSize;
        crosshairRect.sizeDelta = currentSize;
    }

    private void OnDestroy()
    {
        interaction.OnDetectionChanged -= PlayerInteract_HandleDetectionChanged;
        interaction.OnShouldCheckInteractionStateChanged -= PlayerInteract_CheckInteractChanged;
    }
}