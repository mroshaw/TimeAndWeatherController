using System.Collections.Generic;
using Codice.CM.Common.Merge;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#else
using DaftAppleGames.Core.MetaAttributes;
using DaftAppleGames.Core.DrawerAttributes;
#endif
using DaftAppleGames.TimeAndWeather.Core.TimeOfDay;
using DaftAppleGames.TimeAndWeather.Core.Weather;

using UnityEngine;
using UnityEngine.Events;

namespace DaftAppleGames.TimeAndWeather.Core
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

        /// <summary>
        /// Hour public property
        /// </summary>
#if ODIN_INSPECTOR
        [ShowInInspector] [PropertyRange(0, 23)] [PropertyOrder(-1)] [BoxGroup("Time Of Day")]
#else
        [ShowNativeProperty]
#endif
        public int Hour {
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
#if ODIN_INSPECTOR
        [ShowInInspector] [PropertyRange(0, 59)] [PropertyOrder(-1)] [BoxGroup("Time Of Day")]
#else
        [ShowNativeProperty]
#endif
        public int Minute {
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
#if ODIN_INSPECTOR
        [ShowInInspector] [PropertyRange(0, 59)] [PropertyOrder(-1)] [BoxGroup("Time Of Day")]
#else
        [ShowNativeProperty]
#endif
        public int Second {
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
        [BoxGroup("Time Of Day")] [Range(0, 9999)] public float rateOfTimeProgression = 1.0f;
        [Tooltip("How frequently the simulated time is evaluated. A value of 1 is every frame, a value of 2 is every second frame, up to 60 which is once every 60 frames.")]
        [BoxGroup("Time Of Day")] [Range(1, 60)] public float timeEvaluationFrequency = 1.0f;

        [Tooltip("Automatically transition from current weather to permitted weather presets over time.")]
        [BoxGroup("Weather")] public bool automaticWeatherTransitions;
        [Tooltip("The minimum time in seconds between automatic weather transitions.")]
        [BoxGroup("Weather")] public float minTimeBetweenTransitions;
        [Tooltip("The maximum time in seconds between automatic weather transitions.")]
        [BoxGroup("Weather")] public float maxTimeBetweenTransitions;

        [Tooltip("How frequently automatic weather transition is evaluated. A value of 1 is every frame, a value of 2 is every second frame, up to 60 which is once every 60 frames.")]
        [BoxGroup("Weather")] [Range(1, 60)] public float weatherEvaluationFrequency = 1.0f;

        [Tooltip("All time of day presets that can be applied or transitioned to by this particular manager instance.")]
        [BoxGroup("Presets")] public List<TimeOfDayPresetSettingsBase> timeOfDayPresets;
        [Tooltip("Initial time of day preset that will be applied when the component first starts.")]
        [BoxGroup("Presets")] public TimeOfDayPresetSettingsBase defaultTimeOfDayPreset;

        [Tooltip("All weather presets that can be applied or transitioned to by this particular manager instance.")]
        [BoxGroup("Presets")] public List<WeatherPresetSettingsBase> weatherPresets;
        [Tooltip("Initial weather preset that will be applied when the component first starts.")]
        [BoxGroup("Presets")] public WeatherPresetSettingsBase defaultWeatherPreset;

        [FoldoutGroup("Weather Events")] public UnityEvent<WeatherPresetSettingsBase> onWeatherPresetAppliedEvent;
        [FoldoutGroup("Time Events")] public UnityEvent<TimeAndWeatherPresetSettingsBase> onTimePresetAppliedEvent;
        [FoldoutGroup("Time Events")] public UnityEvent<int> onHourPassedEvent;
        [FoldoutGroup("Time Events")] public UnityEvent<int> onMinutePassedEvent;
        [FoldoutGroup("Time Events")] public UnityEvent onDayTimeStartedEvent;
        [FoldoutGroup("Time Events")] public UnityEvent onNightTimeStartedEvent;

        [BoxGroup("Providers")] public WeatherProviderBase weatherProvider;
        [BoxGroup("Providers")] public TimeProviderBase timeProvider;

        private float _framesSinceLastWeatherEvaluation;
        private float _framesSinceLastTimeEvaluation;
        private float _timeAtLastEvaluation;
        private float _timeOfNextWeatherTransition;

        /// <summary>
        /// Get references to the provider components and subscribe to events.
        /// </summary>
        private void OnEnable()
        {
            // Try and grab a Time Provider and Weather Provider from the current gameobject, if we don't have one
            // explicitly defined.
            if (!timeProvider)
            {
                TimeProviderBase gameObjectTimeProvider = GetComponent<TimeProviderBase>();
                if (gameObjectTimeProvider)
                {
                    timeProvider = gameObjectTimeProvider;
                }
            }

            if (!weatherProvider)
            {
                WeatherProviderBase gameObjectWeatherProvider = GetComponent<WeatherProviderBase>();
                if (gameObjectWeatherProvider)
                {
                    weatherProvider = gameObjectWeatherProvider;
                }
            }

            // Register with the TimeAndWeatherManager
            if (timeProvider)
            {
                timeProvider.TimeAndWeatherManager = this;
            }

            // Register with the TimeAndWeatherManager
            if (weatherProvider)
            {
                weatherProvider.TimeAndWeatherManager = this;
            }

            if (Application.isPlaying)
            {
                // Subscribe to events
                weatherProvider.onWeatherPresetAppliedEvent.AddListener(OnWeatherPresetAppliedProxy);
                timeProvider.onTimePresetAppliedEvent.AddListener(OnTimePresetAppliedProxy);
                timeProvider.onHourPassedEvent.AddListener(OnHourPassedProxy);
                timeProvider.onMinutePassedEvent.AddListener(OnMinutePassedProxy);
            }
        }

        /// <summary>
        /// Unsubscribe from events when disabled.
        /// </summary>
        private void OnDisable()
        {
            if (Application.isPlaying)
            {
                weatherProvider.onWeatherPresetAppliedEvent.RemoveListener(OnWeatherPresetAppliedProxy);
                timeProvider.onTimePresetAppliedEvent.RemoveListener(OnTimePresetAppliedProxy);
                timeProvider.onHourPassedEvent.RemoveListener(OnHourPassedProxy);
                timeProvider.onMinutePassedEvent.RemoveListener(OnMinutePassedProxy);
            }
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

            SetNextWeatherTransitionTime();
        }

        /// <summary>
        /// Proxy to call weather applied event handler in Weather Provider.
        /// </summary>
        /// <param name="weatherSettings"></param>
        private void OnWeatherPresetAppliedProxy(WeatherPresetSettingsBase weatherSettings)
        {
            onWeatherPresetAppliedEvent.Invoke(weatherSettings);
        }

        /// <summary>
        /// Proxy to call time applied event handler in Time Provider.
        /// </summary>
        /// <param name="timeSettings"></param>
        private void OnTimePresetAppliedProxy(TimeOfDayPresetSettingsBase timeSettings)
        {
            onTimePresetAppliedEvent.Invoke(timeSettings);
        }

        /// <summary>
        /// Proxy to call hour passed event handler
        /// </summary>
        /// <param name="hour"></param>
        private void OnHourPassedProxy(int hour)
        {
            onHourPassedEvent.Invoke(hour);
        }

        /// <summary>
        /// Proxy to call hour passed event handler
        /// </summary>
        /// <param name="hour"></param>
        private void OnMinutePassedProxy(int hour)
        {
            onMinutePassedEvent.Invoke(hour);
        }

        /// <summary>
        /// Proxy to call daytime started handler
        /// </summary>
        private void OnDayTimeStartedProxy()
        {
            onDayTimeStartedEvent.Invoke();
        }

        /// <summary>
        /// Proxy to call night time started event handler
        /// </summary>
        private void OnNightTimeStartedProxy()
        {
            onNightTimeStartedEvent.Invoke();
        }

        /// <summary>
        /// Calculates when the next weather transition will be
        /// </summary>
        private void SetNextWeatherTransitionTime()
        {
            System.Random newRandom = new System.Random();
            _timeOfNextWeatherTransition =
                Time.time + (float)newRandom.NextDouble() * (minTimeBetweenTransitions - minTimeBetweenTransitions) + minTimeBetweenTransitions;
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
        }

        /// <summary>
        /// Evaluates the automatic weather transition
        /// </summary>
        private void EvaluateWeather()
        {
            // Check if we're ready to evaluate
            if (_framesSinceLastWeatherEvaluation < weatherEvaluationFrequency)
            {
                _framesSinceLastWeatherEvaluation++;
                return;
            }

            // Reset evaluation timer
            _framesSinceLastWeatherEvaluation = 0;

            // If the calculated next weather time is past, we can make a transition
            if (Time.time < _timeOfNextWeatherTransition)
            {
                return;
            }

            SetNextWeatherTransitionTime();
            ChangeWeather();
        }

        /// <summary>
        /// Trigger a change in the weather
        /// </summary>
        private void ChangeWeather()
        {
            Debug.Log("Time for a change of weather...");
            weatherProvider.StartRandomWeather();
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
            presetNames.Sort();
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
            presetNames.Sort();
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