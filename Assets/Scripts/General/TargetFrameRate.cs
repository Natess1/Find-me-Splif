using UnityEngine;

/// <summary>
/// Задаёт целевой FPS один раз при запуске приложения, во всех сценах.
/// Без явного Application.targetFrameRate игра на Android работает на дефолтах
/// платформы, из-за которых частота часто падает до 30 FPS.
///
/// Скрипт НЕ нужно вешать на объекты — [RuntimeInitializeOnLoadMethod] срабатывает
/// автоматически до показа сплэш-скрина. Достаточно, чтобы файл лежал в Assets.
/// </summary>
public static class TargetFrameRate
{
    /// <summary>Желаемая частота кадров. 60 — базовое значение; 120 — для экранов с высокой частотой.</summary>
    public const int Desired = 60;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    private static void Apply()
    {
        // На Android частотой кадров управляет targetFrameRate, а не vsync.
        // Выключаем vsync, чтобы он не смешивался с явным лимитом.
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = Desired;
    }
}
