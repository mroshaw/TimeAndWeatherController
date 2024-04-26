using Expanse;
using UnityEngine;
using Sirenix.OdinInspector;

namespace DaftAppleGames.Common.Weather
{
    /// <summary>
    /// Expanse specific scriptable object definition for the Time and Weather Manager
    /// </summary>
    [CreateAssetMenu(fileName = "ExpanseWeatherPresetSettings", menuName = "Daft Apple Games/Weather/Expanse Weather preset settings", order = 1)]
    public class ExpanseWeatherPresetSettings : WeatherPresetSettingsBase
    {
        [BoxGroup("Sky Settings")] public UniversalCloudLayer expanseCloudLayer;
        [BoxGroup("Sky Settings")] public float skyInterpolateDuration = 5.0f;
        [BoxGroup("Sky Settings")] public float fogInterpolateDuration = 5.0f;
        [BoxGroup("Sky Settings")] public float fogVisibilityDistance;
        [BoxGroup("Sky Settings")] public float fogRadius;
        [BoxGroup("Sky Settings")] public float fogThickness;
    }
}