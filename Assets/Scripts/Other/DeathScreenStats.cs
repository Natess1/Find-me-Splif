using TMPro;
using UnityEngine;

/// <summary>
/// Экран смерти: выводит итоги забега — время выживания, достигнутый уровень, рекорды.
/// Повесь на объект сцены DeathScene и назначь TMP_Text
/// (если поле пустое — попробует найти объект "DeathText").
/// </summary>
public class DeathScreenStats : MonoBehaviour
{
    [SerializeField] private TMP_Text statsText;
    [SerializeField] private bool showRecords = true;

    private void Start()
    {
        if (statsText == null)
        {
            var found = GameObject.Find("DeathText");
            if (found != null) statsText = found.GetComponent<TMP_Text>();
        }

        if (statsText == null)
        {
            Debug.LogWarning("DeathScreenStats: не назначен TMP_Text для итогов");
            return;
        }

        statsText.text = BuildStatsText();
    }

    private string BuildStatsText()
    {
        float lastTime = PlayerPrefs.GetFloat("last_run_time", 0f);
        int lastLevel = PlayerPrefs.GetInt("last_run_level", 1);

        string text = $"Продержался: {LevelSystem.FormatTime(lastTime)}\nДостигнут уровень: {lastLevel}";

        if (showRecords)
        {
            float bestTime = PlayerPrefs.GetFloat("best_time", 0f);
            int bestLevel = PlayerPrefs.GetInt("best_level", 0);

            text += $"\n\nРекорд: {LevelSystem.FormatTime(bestTime)} / Ур. {bestLevel}";
        }

        return text;
    }
}
