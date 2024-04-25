using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace DaftAppleGames.Common.Weather
{
    public enum WeatherSeason { Spring, Summer, Autumn, Winter }

    [ExecuteInEditMode]
    public class TimeAndWeatherManager : MonoBehaviour
    {
        // Singleton static instance
        private static TimeAndWeatherManager _instance;
        public static TimeAndWeatherManager Instance => _instance;

        [HideIf("weatherProvider")]
        [InfoBox("Please add a Weather Provider component!", InfoMessageType.Error, "IsWeatherProviderPresent")]
        [BoxGroup("Providers")] public WeatherProviderBase weatherProvider;
        private bool IsWeatherProviderPresent()
        {
            return weatherProvider == null;
        }

        [HideIf("timeProvider")]
        [InfoBox("Please add a Time Provider component!", InfoMessageType.Error, "IsTimeProviderPresent")]
        [BoxGroup("Providers")]  public TimeProviderBase timeProvider;
        private bool IsTimeProviderPresent()
        {
            return timeProvider == null;
        }

        /// <summary>
        /// Hour public property
        /// </summary>
        [ShowInInspector] [BoxGroup("Time Of Day")] [PropertyRange(0, 23)] [PropertyOrder(-1)] public int Hour {
            get =>
                !timeProvider ?0 :timeProvider.Hour;
            set
            {
                if(timeProvider) timeProvider.Hour = value;
            }
        }

        /// <summary>
        /// Minute public property
        /// </summary>
        [ShowInInspector] [BoxGroup("Time Of Day")] [PropertyRange(0, 59)] [PropertyOrder(-1)] public int Minute {
            get =>
                !timeProvider ?0 :timeProvider.Minute;
            set
            {
                if(timeProvider) timeProvider.Minute = value;
            }
        }
        [BoxGroup("Time Of Day")] public TimeOfDayPresetSettingsBase defaultTimeOfDayPreset;
        [BoxGroup("Time Of Day")] public List<TimeOfDayPresetSettingsBase> timeOfDayPresets;
        [BoxGroup("Weather")] public WeatherPresetSettingsBase defaultWeatherPreset;
        [BoxGroup("Weather")] public List<WeatherPresetSettingsBase> weatherPresets;

        /// <summary>
        /// Initialise the provider components
        /// </summary>
        private void OnEnable()
        {
            timeProvider = GetComponent<TimeProviderBase>();
            weatherProvider = GetComponent<WeatherProviderBase>();
        }

        /// <summary>
        /// Initialise the TimeAndWeatherManager Singleton instance
        /// </summary>
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
            }
        }

        /// <summary>
        /// Set up component and apply and defaults
        /// </summary>
        private void Start()
        {
            if (!timeProvider)
            {
                return;
            }

            // Set time defaults
            timeProvider.ApplyTimePreset(defaultTimeOfDayPreset, true);

            if (!weatherProvider)
            {
                return;
            }

            // Start the default weather
            weatherProvider.ApplyWeatherPreset(defaultWeatherPreset, true);
        }

        /// <summary>
        /// Apply a given preset based on name
        /// </summary>
        /// <param name="timePresetName"></param>
        public void ApplyTimePreset(string timePresetName)
        {
            ApplyTimePreset(GetTimePresetByName(timePresetName));
        }

        /// <summary>
        /// Apply a given time preset
        /// </summary>
        /// <param name="timePreset"></param>
        public void ApplyTimePreset(TimeOfDayPresetSettingsBase timePreset)
        {
            timeProvider.ApplyTimePreset(timePreset);
        }

        /// <summary>
        /// Apply a given weather preset by name
        /// </summary>
        /// <param name="weatherPresetName"></param>
        public void ApplyWeatherPreset(string weatherPresetName)
        {
            ApplyWeatherPreset(GetWeatherPresetByName(weatherPresetName));
        }

        /// <summary>
        /// Apply the given weather preset
        /// </summary>
        /// <param name="weatherPreset"></param>
        public void ApplyWeatherPreset(WeatherPresetSettingsBase weatherPreset)
        {
            weatherProvider.ApplyWeatherPreset(weatherPreset);
        }

        /// <summary>
        /// Return a list of names of registered time presets
        /// </summary>
        /// <returns></returns>
        public List<string> GetTimePresetNames()
        {
            List<string> presetNames = new List<string>();
            foreach (TimeOfDayPresetSettingsBase currSettings in timeOfDayPresets)
            {
                presetNames.Add(currSettings.presetName);
            }
            return presetNames;
        }

        /// <summary>
        /// Return a list of names of registered weather presets
        /// </summary>
        /// <returns></returns>
        public List<string> GetWeatherPresetNames()
        {
            List<string> presetNames = new List<string>();
            foreach (WeatherPresetSettingsBase currSettings in weatherPresets)
            {
                presetNames.Add(currSettings.presetName);
            }
            return presetNames;
        }

        /// <summary>
        /// Look up a weather preset by name
        /// </summary>
        /// <param name="weatherPresetName"></param>
        /// <returns></returns>
        private WeatherPresetSettingsBase GetWeatherPresetByName(string weatherPresetName)
        {
            foreach (WeatherPresetSettingsBase currSettings in weatherPresets)
            {
                if (currSettings.presetName == weatherPresetName)
                {
                    return currSettings;
                }
            }
            return null;
        }

        /// <summary>
        /// Look up a time of day preset by name
        /// </summary>
        /// <param name="timePresetName"></param>
        /// <returns></returns>
        private TimeOfDayPresetSettingsBase GetTimePresetByName(string timePresetName)
        {
            foreach (TimeOfDayPresetSettingsBase currSettings in timeOfDayPresets)
            {
                if (currSettings.presetName == timePresetName)
                {
                    return currSettings;
                }
            }
            return null;
        }
    }
}