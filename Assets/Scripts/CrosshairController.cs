using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization;

/// <summary>
/// Oyunun ekran ortası crosshair'ini ve hint yazılarını yönetir.
/// </summary>
public class CrosshairController : MonoBehaviour
{
    [Header("Crosshair Görseli")]
    [SerializeField] private Image crosshairImage;
    [SerializeField] private Sprite defaultCrosshair;
    [SerializeField] private Sprite handCrosshair;
    [SerializeField] private Sprite interactCrosshair;

    [Header("Crosshair Boyut Animasyonu")]
    [SerializeField] private float normalSize = 32f;
    [SerializeField] private float highlightSize = 42f;
    [SerializeField] private float sizeSpeed = 10f;

    [Header("Hint Metinleri")]
    [SerializeField] private TextMeshProUGUI interactionHintText;
    [SerializeField] private TextMeshProUGUI useHintText;
    [SerializeField] private TextMeshProUGUI dropHintText;
    [SerializeField] private TextMeshProUGUI heldItemNameText;

    private bool isCrosshairEnabled = true;

    private void Start()
    {
        SetText(interactionHintText, false);
        SetText(useHintText, false);
        SetText(dropHintText, false);
        SetText(heldItemNameText, false);
    }

    private void Update()
    {
        UpdateCrosshairSprite();
        UpdateCrosshairSize();
        UpdateHints();
    }

    private void UpdateCrosshairSprite()
    {
        if (!crosshairImage) return;

        var interaction = PlayerInteraction.Instance;

        if (!interaction.IsInteractionEnabled)
        {
            crosshairImage.gameObject.SetActive(false);
            isCrosshairEnabled = false;
        }
        else if (interaction.IsInteractionEnabled && !isCrosshairEnabled)
        {
            crosshairImage.gameObject.SetActive(true);
            isCrosshairEnabled=true;
        }


        if (interaction.DetectedPickable != null)
            crosshairImage.sprite = handCrosshair;
        else if (interaction.DetectedInteractable != null)
            crosshairImage.sprite = interactCrosshair;
        else
            crosshairImage.sprite = defaultCrosshair;
    }

    private void UpdateCrosshairSize()
    {
        if (!crosshairImage) return;

        float targetSize = PlayerInteraction.Instance.HasDetection ? highlightSize : normalSize;
        float currentSize = crosshairImage.rectTransform.sizeDelta.x;
        float newSize = Mathf.Lerp(currentSize, targetSize, Time.deltaTime * sizeSpeed);

        crosshairImage.rectTransform.sizeDelta = new Vector2(newSize, newSize);
    }

    private void UpdateHints()
    {
        var interaction = PlayerInteraction.Instance;
        var holder = PlayerItemHolder.Instance;

        if (interaction.DetectedPickable != null)
        {
            SetText(interactionHintText, true, $"<b>E: </b> {interaction.DetectedPickable.PickupHint}");
        }
        else if (interaction.DetectedInteractable != null)
        {
            SetText(interactionHintText, true, $"<b>E: </b> {interaction.DetectedInteractable.InteractHint}");
        }
        else
        {
            SetText(interactionHintText, false);
        }

        if (holder.IsHoldingItem)
        {
            SetText(heldItemNameText, true, holder.GetHeldItemName());

            string useHint = holder.GetUseHint();
            if (!string.IsNullOrEmpty(useHint))
                SetText(useHintText, true, useHint);
            else
                SetText(useHintText, false);

            LocalizedString localizedDropHint = new LocalizedString("HUD", "hud.drop");
            SetText(dropHintText, true, localizedDropHint.GetLocalizedString());
        }
        else
        {
            SetText(heldItemNameText, false);
            SetText(useHintText, false);
            SetText(dropHintText, false);
        }
    }

    private void SetText(TextMeshProUGUI tmp, bool active, string text="")
    {
        if (!tmp) return;
        if (active && !string.IsNullOrEmpty(text)) tmp.text = text;
        tmp.gameObject.SetActive(active);
    }
}