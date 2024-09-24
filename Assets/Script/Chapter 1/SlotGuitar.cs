using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotGuitar : MonoBehaviour
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
                ScoreGuitar.scoreValue += 1;
            }
            else
            {
                DraggableGuitar draggable = collidingObject.GetComponent<DraggableGuitar>();
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
