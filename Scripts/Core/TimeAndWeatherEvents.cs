using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Events;

namespace DaftAppleGames.Common.Weather
{
    [ExecuteInEditMode]
    public class TimeAndWeatherEvents : MonoBehaviour
    {
        [BoxGroup("Events")] public UnityEvent onDayStartedEvent;
        [BoxGroup("Events")] public UnityEvent onNightStartedEvent;
        [BoxGroup("Events")] public UnityEvent <int>onHourPassedEvent;
        [BoxGroup("Events")] public UnityEvent <int>onMinutePassedEvent;

        /// <summary>
        /// Register events with the TimeAndWeather Manager
        /// </summary>
        private void Start()
        {
            TimeAndWeatherManager.Instance.timeProvider.onDayStartedEvent.AddListener(OnDayStartedEventProxy);
            TimeAndWeatherManager.Instance.timeProvider.onNightStartedEvent.AddListener(OnNightStartedEventProxy);
            TimeAndWeatherManager.Instance.timeProvider.onHourPassedEvent.AddListener(OnHourPassedEventProxy);
            TimeAndWeatherManager.Instance.timeProvider.onMinutePassedEvent.AddListener(OnMinutePassedEventProxy);
        }

        /// <summary>
        /// Proxy event handler
        /// </summary>
        private void OnDayStartedEventProxy()
        {
            onDayStartedEvent.Invoke();
        }

        /// <summary>
        /// Proxy event handler
        /// </summary>
        private void OnNightStartedEventProxy()
        {
            onNightStartedEvent.Invoke();
        }

        /// <summary>
        /// Proxy event handler
        /// </summary>
        private void OnHourPassedEventProxy(int hour)
        {
            onHourPassedEvent.Invoke(hour);
        }

        /// <summary>
        /// Proxy event handler
        /// </summary>
        private void OnMinutePassedEventProxy(int minute)
        {
            onMinutePassedEvent.Invoke(minute);
        }
    }
}