using System;
using System.Collections.Generic;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#else
using DaftAppleGames.Core.MetaAttributes;
#endif
using UnityEngine;

namespace DaftAppleGames.TimeAndWeather.Core
{
    /// <summary>
    /// Base scriptable object for all settings in the Time and Weather manager
    /// </summary>
    public class TimeAndWeatherPresetSettingsBase : ScriptableObject
    {
        [Tooltip("Plain text name of the weather preset. This must be unique amongst other presets selected for a given TimeAndWeatherManager.")]
        [BoxGroup("General Settings")] public string presetName;
    }
}