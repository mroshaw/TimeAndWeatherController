using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace DaftAppleGames.Common.Weather
{
    /// <summary>
    /// Provides core Time functionality for use across all potential providers.
    /// </summary>
    public abstract class TimeProviderBase : TimeAndWeatherProviderBase
    {
        [PropertyOrder(1)] [BoxGroup("Events")] public int dayStartTime = 6;
        [PropertyOrder(1)] [BoxGroup("Events")] public int nightStartTime = 20;
        [PropertyOrder(1)] [BoxGroup("Events")] public UnityEvent <int>onHourPassedEvent;
        [PropertyOrder(1)] [BoxGroup("Events")] public UnityEvent <int>onMinutePassedEvent;
        [PropertyOrder(1)] [BoxGroup("Events")] public UnityEvent onTimeTransitionCompleteEvent;
        [PropertyOrder(1)] [BoxGroup("Events")] public UnityEvent onDayStartedEvent;
        [PropertyOrder(1)] [BoxGroup("Events")] public UnityEvent onNightStartedEvent;

        private int _currHour;
        private int _currMinute;
        private int _currSecond;
        private bool _isInTransition;

        /// <summary>
        /// Allows components to query the transition state
        /// </summary>
        public bool IsIntransition => _isInTransition;

        /// <summary>
        /// Reset transition state
        /// </summary>
        private void Awake()
        {
            _isInTransition = false;
        }

        /// <summary>
        /// Move to the given hour
        /// </summary>
        /// <param name="timeSettings"></param>
        public void GotoTime(TimeOfDayPresetSettingsBase timeSettings)
        {
            if (_isInTransition)
            {
                Debug.Log("Already in a Time Transition - aborting");
                return;
            }
            StartCoroutine(GotoTimeAsync(timeSettings));
        }

        /// <summary>
        /// Sets time to the given hour over time
        /// </summary>
        /// <param name="timeSettings"></param>
        /// <returns></returns>
        private IEnumerator GotoTimeAsync(TimeOfDayPresetSettingsBase timeSettings)
        {
            _isInTransition = true;
            float timeElapsed = 0;
            int startHour = _currHour;
            int startMinutes = _currMinute;

            int targetHour = timeSettings.hour;
            int targetMinute = timeSettings.minute;
            float transitionDuration = timeSettings.timeInterpolationDuration;

            int initialMinutes = (startHour * 60) + startMinutes;
            int targetMinutes;
            if (targetHour <= startHour)
            {
                targetMinutes = (targetHour+24) * 60 + targetMinute;
            }
            else
            {
                targetMinutes = targetHour * 60 + targetMinute;
            }

            while (timeElapsed < transitionDuration)
            {
                int currentMinutes = (int)Mathf.Lerp(initialMinutes, targetMinutes, timeElapsed / transitionDuration);
                int  newHours = currentMinutes / 60;
                int newMinutes = currentMinutes - (newHours * 60);

                // Cater for going past midnight
                if (newHours > 23)
                {
                    newHours -= 24;
                }

                // Update the time of day
                Hour = newHours;
                Minute = newMinutes;

                timeElapsed += Time.deltaTime;
                yield return null;
            }

            // Set the final time of day
            Hour = targetHour;
            Minute = targetMinute;

            // Call any vendor implementation
            GotoTimeProvider(timeSettings, ProviderTransitionComplete);
        }

        /// <summary>
        /// Abstract method for provider to implement any functionality specific to moving to the given time preset
        /// </summary>
        public abstract void GotoTimeProvider(TimeOfDayPresetSettingsBase timeSettings, Action transitionCompleteCallback);

        /// <summary>
        /// Abstract DateTime property
        /// </summary>
        /// <returns></returns>
        public virtual DateTime DateTime
        {
            get => DateTimeProvider;
            set => DateTimeProvider = value;
        }

        /// <summary>
        /// Virtal hour property
        /// </summary>
        public virtual int Hour {
            get
            {
                _currHour = HourProvider;
                return _currHour;
            }
            set
            {
                if (value != _currHour)
                {
                    onHourPassedEvent.Invoke(value);

                    if (value == dayStartTime)
                    {
                        onDayStartedEvent.Invoke();
                    }

                    if (value == nightStartTime)
                    {
                        onNightStartedEvent.Invoke();
                    }
                }

                _currHour = value;
                HourProvider = _currHour;
            }
        }

        /// <summary>
        /// Virtual minute property
        /// </summary>
        public virtual int Minute {
            get
            {
                _currMinute = MinuteProvider;
                return _currMinute;
            }
            set
            {
                if (value != _currMinute)
                {
                    onMinutePassedEvent.Invoke(value);
                }
                _currMinute = value;
                MinuteProvider = _currMinute;
            }
        }

        /// <summary>
        /// Virtual second property
        /// </summary>
        public virtual int Second {
            get
            {
                _currSecond = SecondProvider;
                return _currSecond;
            }
            set
            {
                _currSecond = value;
                SecondProvider = _currSecond;
            }
        }

        protected abstract int HourProvider { get; set; }

        protected abstract int MinuteProvider { get; set; }

        protected abstract int SecondProvider { get; set; }

        protected abstract DateTime DateTimeProvider { get; set; }

        /// <summary>
        /// Applies the given time of day preset. If isImmediate is true, interpolation is skipped and
        /// the time is set immediately.
        /// </summary>
        /// <param name="timePreset"></param>
        /// <param name="isImmediate"></param>
        public void ApplyTimePreset(TimeOfDayPresetSettingsBase timePreset, bool isImmediate = false)
        {
            if (_isInTransition)
            {
                Debug.Log("Already in a Time Transition - aborting");
                return;
            }

            ApplyTimePresetProvider(timePreset, ProviderTransitionComplete);
            if (timePreset.timeInterpolationDuration == 0 || isImmediate)
            {
                Hour = timePreset.hour;
                Minute = timePreset.minute;
                _isInTransition = false;
            }
            else
            {
                GotoTime(timePreset);
            }
        }

        /// <summary>
        /// Call back action for provider to tell us the transition is done.
        /// </summary>
        private void ProviderTransitionComplete()
        {
            _isInTransition = false;
            onTimeTransitionCompleteEvent.Invoke();
        }

        /// <summary>
        /// Abstract provider implementation of ApplyTimePreset
        /// </summary>
        /// <param name="timePreset"></param>
        /// <param name="transitionCompleteCallback"></param>
        protected abstract void ApplyTimePresetProvider(TimeOfDayPresetSettingsBase timePreset, Action transitionCompleteCallback);
    }
}