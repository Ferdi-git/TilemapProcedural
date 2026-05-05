using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    Interactable currentInteractable;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var inter = collision.GetComponent<Interactable>();
        if(inter != null)
            currentInteractable = inter;
    }

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.E))
        {
            if(currentInteractable != null)
                currentInteractable.Interact();
        }
    }
}
