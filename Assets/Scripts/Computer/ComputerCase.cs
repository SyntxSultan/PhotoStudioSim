using UnityEngine;
using UnityEngine.Localization;

namespace SyntaxSultan.ComputerSystem
{
    public class ComputerCase : MonoBehaviour, IInteractable
    {
        [SerializeField] private Computer computer;
        [SerializeField] private LocalizedString interactHint;
        [SerializeField] private LocalizedString caseName;
        
        public LocalizedString InteractHint => interactHint;
        public LocalizedString InteractName => caseName;
        public bool CanInteract => computer != null;

        public void Interact()
        {
            computer.TogglePower();
        }
    }
}