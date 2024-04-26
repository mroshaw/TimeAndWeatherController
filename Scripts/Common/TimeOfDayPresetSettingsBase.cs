using UnityEngine;
using Sirenix.OdinInspector;

namespace DaftAppleGames.Common.Weather
{
    /// <summary>
    /// Base time of day settings scriptable object
    /// </summary>
    public class TimeOfDayPresetSettingsBase : ScriptableObject
    {
        // Public serializable properties
        [BoxGroup("General Settings")] public string presetName;
        [BoxGroup("Time Settings")] [PropertyRange(0, 23)] public int hour;
        [BoxGroup("Time Settings")] [PropertyRange(0, 59)] public int minute;
        [BoxGroup("Time Settings")] public float timeInterpolationDuration = 5.0f;
    }
}