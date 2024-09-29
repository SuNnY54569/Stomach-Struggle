using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodPickUp : MonoBehaviour
{
    public delegate void PickedUpAction();
    public event PickedUpAction OnPickedUp;

    // Call this method when the food is picked up by the player
    public void PickUp()
    {
        // Notify that the food was picked up
        OnPickedUp?.Invoke();
        
        // Optionally destroy or deactivate the food
        Destroy(gameObject);
    }
}
