using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UpgradeSystem;

public class EnemySpawnManager : MonoBehaviour
{
    [Header("Spawn Configuration")]
    public float waveInterval = 10f; // Time between waves (10 seconds as per document)
    public float spawnRadius = 20f; // Distance from player to spawn enemies
    public LayerMask groundLayer = 1; // What counts as ground for spawning
    
    [Header("Enemy Types")]
    public List<GameObject> enemies = new List<GameObject>();
    
    [Header("Wave Configuration")]
    public int spawnEnemyCount = 4;
    public float enemySpawnInterval = 0.5f;
    
    [Header("Debug")]
    public bool showSpawnGizmos = true;
    public bool logSpawnInfo = true;
    
    private Transform playerTransform;
    private int currentWave = 0;
    private float nextWaveTime;
    private bool gameStarted = false;
    private List<GameObject> activeEnemies = new List<GameObject>();
    private System.Random random;
    
    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (playerTransform == null)
        {
            Debug.LogError("EnemySpawnSystem: No player found with 'Player' tag!");
            return;
        }
        
        random = new System.Random();
        // Validate enemy types
        ValidateEnemyTypes();
        
        // Start the first wave after a short delay
        nextWaveTime = Time.time + 2f;
        gameStarted = true;
    }
    
    private void Update()
    {
        if (!gameStarted || UpgradeManager.Instance?.Paused == true || GameManager.Instance.Paused) return;
        
        // Check if it's time for the next wave
        if (Time.time >= nextWaveTime)
        {
            StartNextWave();
        }
        
        // Clean up destroyed enemies from the active list
        activeEnemies.RemoveAll(enemy => enemy == null);
    }
    
    private void StartNextWave()
    {
        currentWave++;
        
        int enemyCount = spawnEnemyCount;
        float spawnInterval = enemySpawnInterval;
        
        if (logSpawnInfo)
        {
            Debug.Log($"Starting Wave {currentWave} with {enemyCount} enemies");
        }
        
        // Start spawning enemies for this wave
        StartCoroutine(SpawnWaveEnemies(enemyCount, spawnInterval));
        
        // Schedule next wave
        nextWaveTime = Time.time + waveInterval;
    }
    
    private IEnumerator SpawnWaveEnemies(int enemyCount, float spawnInterval)
    {
        for (int i = 0; i < enemyCount; i++)
        {
            SpawnRandomEnemy();
            
            if (i < enemyCount - 1) // Don't wait after the last enemy
            {
                yield return new WaitForSeconds(spawnInterval);
            }
        }
    }
    
    private void SpawnRandomEnemy()
    {
        if (enemies.Count == 0)
        {
            Debug.LogWarning($"No available enemies to spawn");
            return;
        }
        
        var selectedEnemy = enemies[random.Next(enemies.Count)];
        Vector3 spawnPosition = GetRandomSpawnPosition();
        
        if (spawnPosition != Vector3.zero)
        {
            GameObject enemy = Instantiate(selectedEnemy, spawnPosition, Quaternion.identity);
            
            activeEnemies.Add(enemy);
            
            if (logSpawnInfo)
            {
                Debug.Log($"Spawned {selectedEnemy.name} at {spawnPosition} for wave {currentWave}");
            }
        }
    }
    
    private Vector3 GetRandomSpawnPosition()
    {
        int attempts = 0;
        const int maxAttempts = 20;
        
        while (attempts < maxAttempts)
        {
            // Generate random position around player
            Vector2 randomCircle = Random.insideUnitCircle.normalized * spawnRadius;
            Vector3 spawnPosition = new Vector3(randomCircle.x, 0 , randomCircle.y);
            
            // Check if position is valid (on ground)
            if (Physics.Raycast(spawnPosition + Vector3.up * 4f, Vector3.down, out RaycastHit hit, 5f, groundLayer))
            {
                return hit.point + Vector3.up * 0.1f; // Slightly above ground
            }
            
            attempts++;
        }
        
        Debug.LogWarning("Could not find valid spawn position after " + maxAttempts + " attempts");
        return Vector3.zero;
    }
    
    private void ValidateEnemyTypes()
    {
        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            if (enemies[i] == null)
            {
                Debug.LogWarning($"Enemy type '{enemies[i].name}' has no prefab assigned. Removing from list.");
                enemies.RemoveAt(i);
            }
        }
        
        if (enemies.Count == 0)
        {
            Debug.LogError("EnemySpawnSystem: No valid enemy types configured!");
        }
    }
    
    public void StopSpawning()
    {
        gameStarted = false;
    }
    
    public void ResumeSpawning()
    {
        gameStarted = true;
        nextWaveTime = Time.time + waveInterval;
    }
    
    public int GetCurrentWave()
    {
        return currentWave;
    }
    
    public int GetActiveEnemyCount()
    {
        activeEnemies.RemoveAll(enemy => enemy == null);
        return activeEnemies.Count;
    }
    
    public void KillAllEnemies()
    {
        foreach (GameObject enemy in activeEnemies)
        {
            if (enemy != null)
            {
                EnemyAi enemyAi = enemy.GetComponent<EnemyAi>();
                if (enemyAi != null)
                {
                    enemyAi.GetHit(1000); // Kill with high damage
                }
            }
        }
        activeEnemies.Clear();
    }
    
    private void OnDrawGizmos()
    {
        if (!showSpawnGizmos || playerTransform == null) return;
        
        // Draw active enemy positions
        Gizmos.color = Color.yellow;
        foreach (GameObject enemy in activeEnemies)
        {
            if (enemy != null)
            {
                Gizmos.DrawWireSphere(enemy.transform.position, 0.5f);
            }
        }
    }
}
