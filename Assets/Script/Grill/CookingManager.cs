using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookingManager : MonoBehaviour
{
    public static CookingManager Instance;
    public float cookingTime = 5f; // Time to cook the food
    public float overcookTime = 10f; // Time after which food gets overcooked
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
    
    public void StartCooking(DragFood food)
    {
        StartCoroutine(CookFood(food));
    }
    
    private IEnumerator CookFood(DragFood food)
    {
        food.gameObject.tag = "Cooking"; // Change tag to indicate it's cooking
        yield return new WaitForSeconds(cookingTime);

        food.gameObject.tag = "Cooked"; // Change tag when food is cooked
        Debug.Log("Food is cooked!");

        yield return new WaitForSeconds(overcookTime - cookingTime);
        food.gameObject.tag = "Overcooked"; // Change tag to overcooked after a delay
        Debug.Log("Food is overcooked!");
    }
    
    public void PlaceOnPlate(DragFood food)
    {
        if (food.gameObject.CompareTag("Cooked"))
        {
            Debug.Log("Placed cooked food on the plate!");
            GameManager.Instance.IncreaseScore(1);
        }
        else if (food.gameObject.CompareTag("Raw"))
        {
            Debug.Log("Placed raw food on the plate. Lose health!");
            GameManager.Instance.DecreaseHealth(10);
        }
        else if (food.gameObject.CompareTag("Overcooked"))
        {
            Debug.Log("Placed overcooked food on the plate. Lose health!");
            GameManager.Instance.DecreaseHealth(10);
        }
    }
    
}
