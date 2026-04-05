using UnityEngine;
using UnityEngine.Localization;

public class ItemPaperPrinter : MonoBehaviour, IInteractable, INetworkDevice
{
    [SerializeField] private NetworkDeviceSO networkData;
    [SerializeField] private LocalizedString interactHint;
    public LocalizedString InteractHint => interactHint;

    private bool isPowered = false;

    public void Interact()
    {
        isPowered = !isPowered;
        if (isPowered)
        {
            Router.Instance.Connect(networkData);
        }
        else
        {
            Router.Instance.Disconnect(networkData);
        }
    }
    
    public NetworkDeviceSO GetNetworkDeviceData() => networkData;
}
