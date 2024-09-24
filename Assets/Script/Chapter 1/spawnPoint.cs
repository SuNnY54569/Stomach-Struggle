using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnPoint : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject[] SymptomsPrefab;
    [Header("Settings")]
    [SerializeField] float secondSpawn = 1.5f;
    [SerializeField] float minTras;
    [SerializeField] float maxTras;
    [SerializeField] float speeds;

    private void Start()
    {
        StartCoroutine(SymptomsSpawn());
    }

    IEnumerator SymptomsSpawn()
    {
        while (true)
        {
            float wanted = Random.Range(minTras, maxTras);
            Vector3 position = new Vector3(transform.position.x, wanted);

            GameObject spawnedSymptom = Instantiate(SymptomsPrefab[Random.Range(0, SymptomsPrefab.Length)], position, Quaternion.identity);
            StartCoroutine(MoveObjectLeft(spawnedSymptom));

            yield return new WaitForSeconds(secondSpawn);
        }
    }

    IEnumerator MoveObjectLeft(GameObject obj)
    {
        while (obj != null)
        {
            obj.transform.Translate(Vector3.left * Time.deltaTime * speeds);
            yield return null;
        }
    }
}
