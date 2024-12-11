using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class spawnPointdown : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject[] itemPrefab;
    [SerializeField] private Timer timer;
    [Header("Settings")]
    [SerializeField] float secondSpawn = 1.5f;
    [SerializeField] float minTras;
    [SerializeField] float maxTras;
    [SerializeField] float speeds;

    private void Start()
    {
        //GameManager.Instance.SetScoreTextActive(false);
        StartCoroutine(itemSpawn());
    }

    IEnumerator itemSpawn()
    {
        while (!timer.isGameOver)
        {
            
            float wanted = Random.Range(minTras, maxTras);
            var position = new Vector3(wanted,transform.position.y,0);

            GameObject spawnedItem = Instantiate(itemPrefab[Random.Range(0, itemPrefab.Length)], position, Quaternion.identity);
            StartCoroutine(MoveObjectDown(spawnedItem));

            yield return new WaitForSeconds(secondSpawn);
            Destroy(spawnedItem,5f);
        }
    }

    IEnumerator MoveObjectDown(GameObject obj)
    {
        while (obj != null && !timer.isGameOver)
        {
            obj.transform.Translate(Vector3.down * Time.deltaTime * speeds);
            yield return null;
        }
    }
}
