using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawn : MonoBehaviour
{
    [SerializeField] private GameObject objectPrefab;
    public void spawnObject()
    {
            Instantiate(objectPrefab, transform.position, Quaternion.identity);
    }
}
