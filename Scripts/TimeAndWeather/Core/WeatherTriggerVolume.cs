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
        [Tooltip("The weather preset that will be applied when a collider enters the volume.")]
        [BoxGroup("Weather Presets")] public WeatherPresetSettingsBase onEnterWeatherSettings;
        [Tooltip("The weather preset that will be applied when a collider exits the volume.")]
        [BoxGroup("Weather Presets")] public WeatherPresetSettingsBase onExitWeatherSettings;
        /// <summary>
        /// Applies the weather preset when a collider enters the trigger volume
        /// </summary>
        /// <param name="other"></param>
        protected override void OnTriggerVolumeEnter(Collider other)
        {
            if (onEnterWeatherSettings)
            {
                TimeAndWeatherManager.Instance.ApplyWeatherPreset(onEnterWeatherSettings);
            }
        }

        /// <summary>
        /// No action taken, at the moment, when the collider exits the volume
        /// </summary>
        /// <param name="other"></param>
        protected override void OnTriggerVolumeExit(Collider other)
        {
            if (onExitWeatherSettings)
            {
                TimeAndWeatherManager.Instance.ApplyWeatherPreset(onExitWeatherSettings);
            }
        }
    }
}