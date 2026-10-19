using System;
using UnityEngine;

/// <summary>
/// Система уровней и таймер забега.
/// Опыт капает за убийства (EnemySO.xpReward), уровень живёт один забег
/// и сбрасывается при загрузке игровой сцены.
/// Навсегда сохраняются только рекорды (best_time / best_level).
/// Урон меча растёт с уровнем: Sword читает CurrentLevel.
/// </summary>
public class LevelSystem : MonoBehaviour
{
    public static LevelSystem Instance { get; private set; }

    public static event Action<int> OnLevelUp;

    [Header("Настройки опыта")]
    [SerializeField] private int startLevel = 1;
    [SerializeField] private int xpPerLevelBase = 10;   // уровень N требует N * xpPerLevelBase XP
    [SerializeField] private int swordBonusPerLevel = 1; // +урон меча за каждый уровень

    public int CurrentLevel { get; private set; }
    public int CurrentXP { get; private set; }
    public int SwordBonusPerLevel => swordBonusPerLevel;

    /// <summary>Сколько XP нужно, чтобы подняться с текущего уровня.</summary>
    public int XPToNextLevel => CurrentLevel * xpPerLevelBase;

    /// <summary>Прогресс до следующего уровня (0..1).</summary>
    public float XPProgress => Mathf.Clamp01((float)CurrentXP / XPToNextLevel);

    public float RunStartTime { get; private set; }

    /// <summary>Сколько секунд длится текущий забег.</summary>
    public float ElapsedTime => Time.time - RunStartTime;

    private void Awake()
    {
        // LevelSystem живёт только в игровой сцене: новая сцена = новый забег
        Instance = this;
        CurrentLevel = startLevel;
        CurrentXP = 0;
        RunStartTime = Time.time;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void AddXP(int amount)
    {
        if (amount <= 0) return;

        CurrentXP += amount;

        // Может быть несколько повышений сразу
        while (CurrentXP >= XPToNextLevel)
        {
            CurrentXP -= XPToNextLevel;
            CurrentLevel++;
            OnLevelUp?.Invoke(CurrentLevel);
        }
    }

    /// <summary>
    /// Вызывается при смерти: сохраняет результаты забега и обновляет рекорды.
    /// </summary>
    public void SaveRunResults()
    {
        float time = ElapsedTime;
        int level = CurrentLevel;

        PlayerPrefs.SetFloat("last_run_time", time);
        PlayerPrefs.SetInt("last_run_level", level);

        if (time > PlayerPrefs.GetFloat("best_time", 0f))
        {
            PlayerPrefs.SetFloat("best_time", time);
        }

        if (level > PlayerPrefs.GetInt("best_level", 0))
        {
            PlayerPrefs.SetInt("best_level", level);
        }
    }

    /// <summary>Форматирует секунды в "мм:сс".</summary>
    public static string FormatTime(float seconds)
    {
        int total = Mathf.FloorToInt(Mathf.Max(0f, seconds));
        return $"{total / 60:00}:{total % 60:00}";
    }
}
