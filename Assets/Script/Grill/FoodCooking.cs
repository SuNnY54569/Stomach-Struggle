using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodCooking : MonoBehaviour
{
    [SerializeField] private float cookingTime; // Time it takes to cook the food
    [SerializeField] private float overcookedTime; // Time after which the food is overcooked

    [SerializeField] private float cookingTimer; // Timer for cooking state
    public bool isCooking; // Track cooking state
    
    void Update()
    {
        if (isCooking)
        {
            cookingTimer += Time.deltaTime;

            // Check for overcooked condition
            if (cookingTimer >= overcookedTime)
            {
                Debug.Log(gameObject.name + " is overcooked!");
                gameObject.tag = "Overcooked";
                StopCooking();
            }
            else if (cookingTimer >= cookingTime && cookingTimer < overcookedTime)
            {
                gameObject.tag = "Cooked";
            }
            else
            {
                gameObject.tag = "Raw";
            }
        }
    }
    
    public void StartCooking()
    {
        isCooking = true; // Mark food as being cooked
        Debug.Log("Started cooking " + gameObject.name);
    }
    
    public void StopCooking()
    {
        isCooking = false; // Stop cooking
        Debug.Log("Stopped cooking " + gameObject.name);
    }
    
    public void PlaceOnPlate()
    {
        StopCooking(); // Ensure cooking stops when placed on plate

        // Determine cooking result based on the timer
        if (cookingTimer < cookingTime)
        {
            // Undercooked
            Debug.Log(gameObject.name + " is undercooked!");
            // Apply health penalty or fail state
            Health.Instance.DecreaseHealth(1); // Example: lose health if undercooked
        }
        else if (cookingTimer >= cookingTime && cookingTimer < overcookedTime)
        {
            // Properly cooked
            Debug.Log(gameObject.name + " is perfectly cooked!");
            // Reward the player (add score or success state)
            ScoreMeatShop.Instance.ScoreUp(1); // Example: increase score if cooked properly
        }
        else
        {
            // Overcooked
            Debug.Log(gameObject.name + " is overcooked!");
            // Apply health penalty
            Health.Instance.DecreaseHealth(1); // Example: lose health if overcooked
        }
    }
}
