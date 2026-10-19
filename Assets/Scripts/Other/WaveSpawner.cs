using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Волны врагов для режима выживания.
/// Каждую волну спавнит группу врагов вокруг игрока (на валидной точке NavMesh),
/// количество врагов зависит от уровня игрока (LevelSystem), интервал между волнами
/// сокращается, предел живых врагов тоже растёт с уровнем.
/// Спавненные враги на несколько секунд принудительно преследуют игрока (ForceChase),
/// потом живут по обычной логике EnemyAI.
/// Повесь на любой объект игровой сцены, назначь префабы врагов.
/// </summary>
public class WaveSpawner : MonoBehaviour
{
    [Header("Враги (префабы, напр. Sceleton и Goblin)")]
    [SerializeField] private GameObject[] enemyPrefabs;

    [Header("Волны")]
    [SerializeField] private float firstWaveDelay = 5f;   // пауза перед первой волной
    [SerializeField] private float waveInterval = 20f;    // базовый интервал между волнами
    [SerializeField] private float intervalMultiplier = 0.95f; // каждая волна чуть быстрее
    [SerializeField] private float minInterval = 8f;      // нижний предел интервала
    [SerializeField] private int baseEnemyCount = 3;      // врагов на 1-м уровне игрока
    [SerializeField] private int enemiesPerLevel = 2;     // +врагов за каждый уровень игрока
    [SerializeField] private int maxEnemiesAlive = 25;    // предел живых врагов на 1-м уровне
    [SerializeField] private int extraAlivePerLevel = 2;  // +к пределу живых за каждый уровень игрока

    [Header("Спавн вокруг игрока")]
    [SerializeField] private float minSpawnRadius = 8f;
    [SerializeField] private float maxSpawnRadius = 14f;
    [SerializeField] private float navMeshSampleDistance = 3f;
    [SerializeField] private int sampleAttempts = 12;     // попыток найти точку на NavMesh

    [Header("Поведение после спавна")]
    [SerializeField] private float forceChaseDuration = 6f; // сколько секунд враги идут к игроку

    private float timer;
    private int waveNumber;
    private readonly List<GameObject> alive = new List<GameObject>();

    private void Start()
    {
        timer = firstWaveDelay;
    }

    private void Update()
    {
        alive.RemoveAll(enemy => enemy == null);

        if (Player.Instance == null || !Player.Instance.IsAlive()) return;

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            SpawnWave();
            float interval = waveInterval * Mathf.Pow(intervalMultiplier, waveNumber);
            timer = Mathf.Max(minInterval, interval);
        }
    }

    private void SpawnWave()
    {
        waveNumber++;

        // Размер волны зависит от уровня игрока, а не от номера волны
        int count = baseEnemyCount + enemiesPerLevel * (PlayerLevel - 1);
        count = Mathf.Min(count, MaxAliveEnemies - alive.Count);

        for (int i = 0; i < count; i++)
        {
            SpawnOne();
        }
    }

    private int PlayerLevel => LevelSystem.Instance != null ? LevelSystem.Instance.CurrentLevel : 1;

    // Предел живых врагов растёт вместе с уровнем игрока
    private int MaxAliveEnemies => maxEnemiesAlive + extraAlivePerLevel * (PlayerLevel - 1);

    private void SpawnOne()
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0) return;
        if (Player.Instance == null) return;

        Vector2 playerPos = Player.Instance.transform.position;

        for (int attempt = 0; attempt < sampleAttempts; attempt++)
        {
            Vector2 direction = Random.insideUnitCircle.normalized;
            Vector2 point = playerPos + direction * Random.Range(minSpawnRadius, maxSpawnRadius);

            if (NavMesh.SamplePosition(point, out NavMeshHit hit, navMeshSampleDistance, NavMesh.AllAreas))
            {
                GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
                GameObject enemy = Instantiate(prefab, hit.position, Quaternion.identity);
                alive.Add(enemy);

                // Враг сразу идёт к игроку, а не бродит
                var ai = enemy.GetComponent<EnemyAI>();
                if (ai != null)
                {
                    ai.ForceChase(forceChaseDuration);
                }

                return;
            }
        }
    }

    public int WaveNumber => waveNumber;
    public int AliveCount => alive.Count;
}
