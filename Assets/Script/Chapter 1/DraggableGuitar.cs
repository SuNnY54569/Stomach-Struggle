using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DraggableGuitar : MonoBehaviour
{
    public bool IsDragging;

    public Vector3 lastPosition;

    private Collider2D _collider;

    private DragControllerGuitar _dragController;

    private float _movementTime = 15f;

    private System.Nullable<Vector3> _movmentDestination;

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
        _dragController = FindObjectOfType<DragControllerGuitar>();
    }

    private void FixedUpdate()
    {
        if (_movmentDestination.HasValue)
        {
            if (IsDragging)
            {
                _movmentDestination = null;
                return;
            }
            if (transform.position == _movmentDestination)
            {
                gameObject.layer = Layer.Default;
                _movmentDestination = null;
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, _movmentDestination.Value, _movementTime * Time.fixedDeltaTime);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Draggable collidedDraggable = other.GetComponent<Draggable>();

        if (collidedDraggable != null && _dragController.LastDragged.gameObject == gameObject)
        {
            ColliderDistance2D colliderDistance2D = other.Distance(_collider);
            Vector3 diff = new Vector3(colliderDistance2D.normal.x, colliderDistance2D.normal.y) * colliderDistance2D.distance;
            transform.position -= diff;
        }

        if (gameObject.CompareTag(other.tag))
        {
            _movmentDestination = other.transform.position;
        }
        else
        {
            Health playerHealth = FindObjectOfType<Health>();
            if (playerHealth != null)
            {
                playerHealth.DecreaseHealth(1);
            }

            Destroy(this.gameObject);
        }
    }
}
