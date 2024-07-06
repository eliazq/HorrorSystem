using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour {

    private void Start()
    {
        InputManager.Instance.OnInteractionHolded += Instance_OnInteractionHolded;
        InputManager.Instance.OnInteractionClicked += Instance_OnInteractionClicked;
    }

    private void Instance_OnInteractionClicked(object sender, System.EventArgs e)
    {
        IInteractable interactable = GetInteractableObject();
        if (interactable != null && interactable.GetTransform().GetComponent<Weapon>() == null) {
            interactable.Interact(transform);
        }
    }

    private void Instance_OnInteractionHolded(object sender, System.EventArgs e)
    {
        IInteractable interactable = GetInteractableObject();
        if (interactable != null && interactable.GetTransform().GetComponent<Weapon>() != null) {
            interactable.Interact(transform);
        }
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.E)) {

        }
    }

    public IInteractable GetInteractableObject() {
        List<IInteractable> interactableList = new List<IInteractable>();
        float interactRange = 1.8f;
        Collider[] colliderArray = Physics.OverlapSphere(transform.position, interactRange);
        foreach (Collider collider in colliderArray) {
            if (collider.TryGetComponent(out IInteractable interactable)) {
                interactableList.Add(interactable);
            }
        }

        IInteractable closestInteractable = null;
        foreach (IInteractable interactable in interactableList) {
            if (closestInteractable == null) {
                closestInteractable = interactable;
            } else {
                if (Vector3.Distance(transform.position, interactable.GetTransform().position) < 
                    Vector3.Distance(transform.position, closestInteractable.GetTransform().position)) {
                    // Closer
                    closestInteractable = interactable;
                }
            }
        }

        return closestInteractable;
    }

}