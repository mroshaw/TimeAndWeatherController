using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace DaftAppleGames.Common.Weather
{
    public enum WeatherSeason { Spring, Summer, Autumn, Winter }

    /// <summary>
    /// Main controller class for the Time and Weather Manager. Implements a singleton, so it can be invoked in
    /// multi-scene scenarios.
    /// </summary>
    [ExecuteInEditMode]
    public class TimeAndWeatherManager : MonoBehaviour
    {
        // Singleton static instance
        private static TimeAndWeatherManager instance;
        public static TimeAndWeatherManager Instance => instance;

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

        /// <summary>
        /// Second public property
        /// </summary>
        [ShowInInspector] [BoxGroup("Time Of Day")] [PropertyRange(0, 59)] [PropertyOrder(-1)] public int Second {
            get =>
                !timeProvider ?0 :timeProvider.Second;
            set
            {
                if(timeProvider) timeProvider.Second = value;
            }
        }

        [Tooltip("Automatically progress time when in run mode.")]
        [BoxGroup("Time Of Day")] public bool simulateTimeProgression;
        [Tooltip("The rate at which simulated time progresses. A value of 1 is real time, a value of 2 is twice as fast, and so on.")]
        [BoxGroup("Time Of Day")] [Range(0, 1000)] public float rateOfTimeProgression = 1.0f;
        [Tooltip("How frequently the simulated time is evaluated. A value of 1 is every frame, a value of 2 is every second frame, up to 60 which is once every 60 frames.")]
        [BoxGroup("Time Of Day")] [Range(1, 60)] public float timeEvaluationFrequency = 1.0f;
        [Tooltip("All time of day presets that can be applied or transitioned to by this particular manager instance.")]
        [BoxGroup("Time Of Day")] public List<TimeOfDayPresetSettingsBase> timeOfDayPresets;
        [Tooltip("Initial time of day preset that will be applied when the component first starts.")]
        [BoxGroup("Time Of Day")] public TimeOfDayPresetSettingsBase defaultTimeOfDayPreset;

        [Tooltip("Automatically transition from current weather to permitted weather presets over time.")]
        [BoxGroup("Weather")] public bool automaticWeatherTransitions;
        [Tooltip("The minimum time in seconds between automatic weather transitions.")]
        [BoxGroup("Weather")] public float minTimeBetweenTransitions;
        [Tooltip("The maximum time in seconds between automatic weather transitions.")]
        [BoxGroup("Weather")] public float maxTimeBetweenTransitions;

        [Tooltip("How frequently automatic weather transition is evaluated. A value of 1 is every frame, a value of 2 is every second frame, up to 60 which is once every 60 frames.")]
        [BoxGroup("Weather")] [Range(1, 60)] public float weatherEvaluationFrequency = 1.0f;

        [Tooltip("All weather presets that can be applied or transitioned to by this particular manager instance.")]
        [BoxGroup("Weather")] public List<WeatherPresetSettingsBase> weatherPresets;
        [Tooltip("Initial weather preset that will be applied when the component first starts.")]
        [BoxGroup("Weather")] public WeatherPresetSettingsBase defaultWeatherPreset;

        private float _timeSinceLastWeatherChange;
        private float _framesSinceLastWeatherEvaluation;
        private float _framesSinceLastTimeEvaluation;
        private float _timeAtLastEvaluation;

        /// <summary>
        /// Get references to the provider components.
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
            if (instance != null && instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                instance = this;
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

            // Set up the time progress and random weather
            _timeAtLastEvaluation = Time.time;
        }

        /// <summary>
        /// Run the time and weather simulation, if selected. Uses the settings to control how
        /// frequently each system is evaluated, to allow for some performance tuning.
        /// </summary>
        private void Update()
        {

            // We don't want any of this happening in the Unity editor
            if (!Application.isPlaying)
            {
                return;
            }

            if (simulateTimeProgression)
            {
                EvaluateTime();
            }

            if (automaticWeatherTransitions)
            {
                EvaluateWeather();
            }
        }

        /// <summary>
        /// Evaluates the time progression simulation.
        /// </summary>
        private void EvaluateTime()
        {
            // Check if we're ready to evaluate
            if (_framesSinceLastTimeEvaluation < timeEvaluationFrequency)
            {
                _framesSinceLastTimeEvaluation++;
                return;
            }

            timeProvider.DateTime = timeProvider.DateTime.AddSeconds((Time.time - _timeAtLastEvaluation) * rateOfTimeProgression);
            _timeAtLastEvaluation = Time.time;
            _framesSinceLastTimeEvaluation = 0;

            // Forces the Inspector to refresh
            #if UNITY_EDITOR
            UnityEditor.SceneView.RepaintAll();
            #endif
        }

        /// <summary>
        /// Evaluates the automatic weather transition
        /// </summary>
        private void EvaluateWeather()
        {
            // Check if we're ready to evaluate
            if (_framesSinceLastWeatherEvaluation < weatherEvaluationFrequency)
            {
                return;
            }
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
        private void ApplyTimePreset(TimeOfDayPresetSettingsBase timePreset)
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