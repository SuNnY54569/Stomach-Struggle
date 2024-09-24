using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
    [SerializeField] private float timeToDestroy;
    private Collider2D collidingObject;
    private float collisionTime;


    private void OnTriggerStay2D(Collider2D other)
    {
        if (gameObject.CompareTag(other.gameObject.tag))
        {
            collidingObject = other;
            collisionTime = Time.time;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other == collidingObject)
        {
            collidingObject = null;
            collisionTime = 0f;
        }
    }

    private void Update()
    {
        if (collidingObject != null && Time.time - collisionTime >= timeToDestroy)
        {
            bool isMatch = CheckMatch(collidingObject.gameObject);

            if (isMatch)
            {
                Destroy(collidingObject.gameObject);
                Score.scoreValue += 1;

                SymptomsRandom symptomsRandom = FindObjectOfType<SymptomsRandom>();
                if (symptomsRandom != null)
                {
                    symptomsRandom.DestroySpawnedSymptom();
                    symptomsRandom.SpawnRandomSymptom();
                }
            }
            else
            {
                Draggable draggable = collidingObject.GetComponent<Draggable>();
                if (draggable != null)
                {
                    draggable.transform.position = draggable.lastPosition;
                }
            }

            collidingObject = null;
            collisionTime = 0f;
        }
    }

    private bool CheckMatch(GameObject collidedObject)
    {
        return collidedObject.CompareTag(gameObject.tag);
    }
}
