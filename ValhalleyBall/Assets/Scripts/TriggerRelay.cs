using UnityEngine;

public class TriggerRelay : MonoBehaviour
{
    public PlayerController parentController;

    private void OnTriggerStay(Collider other)
    {
        if (parentController != null)
        {
            parentController.HandleHitTrigger(other);
        }
    }
}