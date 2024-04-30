#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#else
using DaftAppleGames.Core.MetaAttributes;
using DaftAppleGames.Core.DrawerAttributes;
#endif
using DaftAppleGames.TimeAndWeather.Core.Weather;
using UnityEngine;

namespace DaftAppleGames.TimeAndWeather.Core
{
    public class WeatherTriggerVolume : TriggerVolumeBase
    {
        [Tooltip("The weather preset that will be applied when a collider triggers the volume.")]
        [BoxGroup("Weather Preset")] public WeatherPresetSettingsBase weatherSettings;
        /// <summary>
        /// Applies the weather preset when a collider enters the trigger volume
        /// </summary>
        /// <param name="other"></param>
        protected override void OnTriggerVolumeEnter(Collider other)
        {
            TimeAndWeatherManager.Instance.ApplyWeatherPreset(weatherSettings);
        }

        /// <summary>
        /// No action taken, at the moment, when the collider exits the volume
        /// </summary>
        /// <param name="other"></param>
        protected override void OnTriggerVolumeExit(Collider other)
        {
        }
    }
}