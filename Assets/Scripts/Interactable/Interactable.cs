using UnityEngine;

public class Interactable : MonoBehaviour, IInteractable
{
    [SerializeField] private string interactHint;
    
    public string InteractHint => interactHint;
}