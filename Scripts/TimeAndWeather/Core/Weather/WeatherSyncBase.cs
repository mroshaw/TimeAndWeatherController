#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#else
using DaftAppleGames.Core.MetaAttributes;
#endif
using DaftAppleGames.TimeAndWeather.Core.Weather;
using UnityEngine;

namespace DaftAppleGames.TimeAndWeather.Core.Weather
{
    /// <summary>
    /// Base class for functionality related to sync'ing aspects of weather with 3rd party tools
    /// such as The Vegetation Engine and MicroSplat
    /// </summary>
    public abstract class WeatherSyncBase : TimeAndWeatherSyncBase
    {
        [Tooltip("Synchronisation will apply if weather transitions to any of the presets in this list")]
        [BoxGroup("Sync Properties")] public bool syncWind;
        [BoxGroup("Sync Properties")] public bool syncWetness;
        [BoxGroup("Sync Properties")] public bool syncOverlay;

        /// <summary>
        /// Attach this to the OnChanged event of the Time and Weather Manager. This will then check against
        /// the list of presets and, if there's a match, sync. If there's no match, then revert back.
        /// </summary>
        /// <param name="activeWeatherSettings"></param>
        public void SyncWeatherEventHandler(WeatherPresetSettingsBase activeWeatherSettings)
        {
            if (syncWind)
            {
                Synchronise( Wind, activeWeatherSettings.wind, activeWeatherSettings.windSyncDuration, SetWind);
            }

            if (syncWetness)
            {
                Synchronise(Wetness, activeWeatherSettings.wetness, activeWeatherSettings.wetnessSyncDuration, SetWetness);
            }

            if (syncOverlay)
            {
                Synchronise(Overlay, activeWeatherSettings.overlay,activeWeatherSettings.overlaySyncDuration, SetOverlay);
            }
        }

        /// <summary>
        /// Proxy delegate for setting provider wind value
        /// </summary>
        /// <param name="windValue"></param>
        private void SetWind(float windValue)
        {
            Wind = windValue;
        }

        /// <summary>
        /// Proxy delegate for setting provider wetness value
        /// </summary>
        /// <param name="wetnessValue"></param>
        private void SetWetness(float wetnessValue)
        {
            Wetness = wetnessValue;
        }

        /// <summary>
        /// Proxy delegate for setting provider overlay value
        /// </summary>
        /// <param name="overlayValue"></param>
        private void SetOverlay(float overlayValue)
        {
            Overlay = overlayValue;
        }

        /// <summary>
        /// Provider property for setting and getting wind strength
        /// </summary>
        protected abstract float Wind { get; set; }

        /// <summary>
        /// Provider property for setting and getting current wetness
        /// </summary>
        protected abstract float Wetness { get; set; }

        /// <summary>
        /// Provider property for setting and getting current overlay / snow
        /// </summary>
        protected abstract float Overlay { get; set; }
    }
}