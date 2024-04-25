using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace DaftAppleGames.Common.Weather
{
    public abstract class TimeProviderBase : TimeAndWeatherProviderBase
    {
        [PropertyOrder(1)] [BoxGroup("Events")] public int dayStartTime = 6;
        [PropertyOrder(1)] [BoxGroup("Events")] public int nightStartTime = 20;
        [PropertyOrder(1)] [BoxGroup("Events")] public UnityEvent <int>onHourPassedEvent;
        [PropertyOrder(1)] [BoxGroup("Events")] public UnityEvent <int>onMinutePassedEvent;
        [PropertyOrder(1)] [BoxGroup("Events")] public UnityEvent onGotoHourStartEvent;
        [PropertyOrder(1)] [BoxGroup("Events")] public UnityEvent onGotoHourEndEventEvent;
        [PropertyOrder(1)] [BoxGroup("Events")] public UnityEvent<TimeOfDayPresetSettingsBase> onTimePresetAppliedEvent;
        [PropertyOrder(1)] [BoxGroup("Events")] public UnityEvent onDayStartedEvent;
        [PropertyOrder(1)] [BoxGroup("Events")] public UnityEvent onNightStartedEvent;

        private int _currHour;
        private int _currMinute;
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
        /// <param name="transitionCompleteCallback"></param>
        public void GotoTime(TimeOfDayPresetSettingsBase timeSettings, Action transitionCompleteCallback)
        {
            if (_isInTransition)
            {
                Debug.Log("Already in a Time Transition - aborting");
                transitionCompleteCallback.Invoke();
                return;
            }
            StartCoroutine(GotoTimeAsync(timeSettings, transitionCompleteCallback));
        }

        /// <summary>
        /// Sets time to the given hour over time
        /// </summary>
        /// <param name="timeSettings"></param>
        /// <param name="transitionCompleteCallback"></param>
        /// <returns></returns>
        private IEnumerator GotoTimeAsync(TimeOfDayPresetSettingsBase timeSettings, Action transitionCompleteCallback)
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
                int currentMinutes = (int)Mathf.Lerp(initialMinutes, (float)targetMinutes, timeElapsed / transitionDuration);
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

            // Invoke the callback, to say we're done
            _isInTransition = false;
            transitionCompleteCallback.Invoke();
        }

        /// <summary>
        /// Abstract hour property
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
        /// Abstract minute property
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

        public abstract int HourProvider { get; set; }

        public abstract int MinuteProvider { get; set; }

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

            ApplyTimePresetProvider(timePreset);
            if (timePreset.timeInterpolationDuration == 0 || isImmediate)
            {
                Hour = timePreset.hour;
                Minute = timePreset.minute;
                _isInTransition = false;
            }
            else
            {
                GotoTime(timePreset, ProviderTransitionComplete);
            }
            onTimePresetAppliedEvent.Invoke(timePreset);
        }

        /// <summary>
        /// Call back action for provider to tell us the transition is done.
        /// </summary>
        public void ProviderTransitionComplete()
        {
            _isInTransition = false;
        }


        /// <summary>
        /// Abstract provider implementation of ApplyTimePreset
        /// </summary>
        /// <param name="timePreset"></param>
        public abstract void ApplyTimePresetProvider(TimeOfDayPresetSettingsBase timePreset);
    }
}