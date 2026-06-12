using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;

[Serializable]
public class ItemInteraction
{
    [SerializeField] private InputActionReference _actionReference;
    [SerializeField] private LocalizedString _hint;

    public InputActionReference ActionReference => _actionReference;
    public LocalizedString Hint => _hint;

    // Input System event'lerine bağlanacak delegateler
    public Action<InputAction.CallbackContext> OnStarted;
    public Action<InputAction.CallbackContext> OnPerformed;
    public Action<InputAction.CallbackContext> OnCanceled;

    public ItemInteraction(InputActionReference actionReference, LocalizedString hint)
    {
        _actionReference = actionReference;
        _hint = hint;
    }
}

public interface IComplexUsable
{
    /// <summary>
    /// Eşyanın şu anki moduna/durumuna göre yapabileceği tüm etkileşimlerin listesini döner.
    /// </summary>
    List<ItemInteraction> GetInteractions();
}