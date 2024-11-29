using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SymptomsRandom : MonoBehaviour
{
    [SerializeField] List<GameObject> SymptomsPrefabRandom;

    private GameObject spawnedSymptom;

    private void Start()
    {
        SpawnRandomSymptom();
    }

    public void SpawnRandomSymptom()
    {
        if (spawnedSymptom == null)
        {
            int randomIndex = Random.Range(0, SymptomsPrefabRandom.Count);
            spawnedSymptom = Instantiate(SymptomsPrefabRandom[randomIndex], transform.position, Quaternion.identity);
        }
    }

    public void DestroySpawnedSymptom()
    {
        if (spawnedSymptom != null)
        {
            Destroy(spawnedSymptom);
            spawnedSymptom = null;
        }
    }
}
