using System.Collections;
using UnityEngine;

public class ComputerCase : MonoBehaviour, IInteractable
{
    public string InteractHint => "Bilgisayarı Aç/Kapat";

    public void Interact()
    {
        ComputerState.Instance.TogglePower();
    }
}