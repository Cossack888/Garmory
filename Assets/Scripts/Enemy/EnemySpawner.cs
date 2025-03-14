using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject player;
    [SerializeField] private int maxEnemies = 4;
    [SerializeField] private float spawnRadius = 10f;
    private float percentageLuck;
    private Coroutine spawnCoroutine;
    Health playerHealth;
    [SerializeField] private float minDistanceFromPlayer = 10f;

    private void Start()
    {
        playerHealth = player.GetComponent<Health>();
        playerHealth.OnDeath += StopSpawning;
    }

    private void OnDisable()
    {
        if (playerHealth != null)
            playerHealth.OnDeath -= StopSpawning;
    }

    public void StartSpawning()
    {
        if (spawnCoroutine == null)
        {
            spawnCoroutine = StartCoroutine(SpawnEnemiesCoroutine());
        }
    }

    public void StopSpawning()
    {
        StopAllCoroutines();
        spawnCoroutine = null;
    }
    private IEnumerator SpawnEnemiesCoroutine()
    {
        while (!playerHealth.IsDead())
        {
            int enemyNumber = 0;
            foreach (EnemyAI enemy in FindObjectsByType<EnemyAI>(FindObjectsSortMode.None))
            {
                if (!enemy.IsDead())
                {
                    enemyNumber++;
                }
            }
            if (enemyNumber < maxEnemies)
            {
                SpawnWave();
            }
            yield return new WaitForSeconds(10f);
        }
    }


    public void SpawnWave()
    {
        float playerLuck = player.GetComponent<Statistics>().Luck;
        percentageLuck = playerLuck / 100;

        int enemyCount = 0;

        for (int i = 0; i < maxEnemies; i++)
        {
            float randomValue = Random.value;

            if (randomValue > percentageLuck)
            {
                Vector3 spawnPoint;
                if (GetRandomNavMeshPoint(out spawnPoint))
                {
                    GameObject enemy = Instantiate(enemyPrefab, spawnPoint, Quaternion.identity);
                    Health health = enemy.GetComponent<Health>();
                    health.InitialiseWeapons();
                    health.InitialiseHealthPoints(enemy.GetComponent<Statistics>().HealthPoints + 1);
                    enemyCount++;
                }
            }
        }
    }

    private bool GetRandomNavMeshPoint(out Vector3 point)
    {
        for (int attempt = 0; attempt < 10; attempt++)
        {
            Vector3 randomPoint = player.transform.position + Random.insideUnitSphere * spawnRadius;
            if (Vector3.Distance(randomPoint, player.transform.position) < minDistanceFromPlayer)
            {
                continue;
            }
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, spawnRadius, NavMesh.AllAreas))
            {
                point = hit.position;
                return true;
            }
        }
        point = Vector3.zero;
        return false;
    }
}
