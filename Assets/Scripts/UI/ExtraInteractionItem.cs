using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class ExtraInteractionItem : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private LocalizeStringEvent localizedTextEvent;

    public void Setup(Sprite actionIcon, LocalizedString hint)
    {
        if (actionIcon != null)
        {
            iconImage.sprite = actionIcon;
            iconImage.gameObject.SetActive(true);
        }
        else
        {
            iconImage.gameObject.SetActive(false);
        }

        if (localizedTextEvent != null && hint != null)
        {
            localizedTextEvent.StringReference = hint;
            localizedTextEvent.RefreshString();
        }
    }
}