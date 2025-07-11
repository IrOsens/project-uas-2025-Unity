using UnityEngine;
using System.Collections;
public class BoxSpawner : MonoBehaviour
{
    public GameObject[] boxPrefabs;
    public BoxCollider spawnVolume;
    public float lifetimePerBox = 3.0f;
    public float intervalAntarSpawn = 5.0f;

    void Start()
    {
        if (boxPrefabs == null || boxPrefabs.Length == 0)
        {
            Debug.LogError("BoxSpawner: Tidak ada prefab box yang di-assign!");
            enabled = false;
            return;
        }

        if (spawnVolume == null)
        {
            Debug.LogError("BoxSpawner: Spawn Volume (BoxCollider) belum di-assign!");
            enabled = false;
            return;
        }

        StartCoroutine(SpawnBoxLoop());
    }

    IEnumerator SpawnBoxLoop()
    {
        while (true)
        {
            int randomIndex = Random.Range(0, boxPrefabs.Length);
            GameObject prefabToSpawn = boxPrefabs[randomIndex];

            Bounds bounds = spawnVolume.bounds;
            float randomX = Random.Range(bounds.min.x, bounds.max.x);
            float randomY = Random.Range(bounds.min.y, bounds.max.y);
            float randomZ = Random.Range(bounds.min.z, bounds.max.z);
            Vector3 spawnPosition = new Vector3(randomX, randomY, randomZ);

            GameObject spawnedBox = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
            Debug.Log($"BoxSpawner: Memunculkan {spawnedBox.name} di {spawnPosition}");

            Destroy(spawnedBox, lifetimePerBox);

            yield return new WaitForSeconds(intervalAntarSpawn);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (spawnVolume != null)
        {
            Gizmos.color = new Color(0, 1, 0, 0.3f);
            Gizmos.DrawCube(spawnVolume.bounds.center, spawnVolume.bounds.size);
        }
    }
}