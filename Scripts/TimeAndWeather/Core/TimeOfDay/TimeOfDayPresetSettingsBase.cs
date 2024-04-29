#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#else
using DaftAppleGames.Core.MetaAttributes;
#endif
using UnityEngine;

namespace DaftAppleGames.TimeAndWeather.Core.TimeOfDay
{
    /// <summary>
    /// Base time of day settings scriptable object
    /// </summary>
    public class TimeOfDayPresetSettingsBase : TimeAndWeatherPresetSettingsBase
    {
        // Public serializable properties
        [BoxGroup("Time Settings")] [Range(0, 23)] public int hour;
        [BoxGroup("Time Settings")] [Range(0, 59)] public int minute;
        [BoxGroup("Time Settings")] public float timeInterpolationDuration = 5.0f;
    }
}