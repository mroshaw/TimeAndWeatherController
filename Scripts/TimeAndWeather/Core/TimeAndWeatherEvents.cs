#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#else
using DaftAppleGames.Core.MetaAttributes;
#endif
using UnityEngine;
using UnityEngine.Events;

namespace DaftAppleGames.TimeAndWeather.Core
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
            TimeAndWeatherManager.Instance.onDayTimeStartedEvent.AddListener(OnDayStartedEventProxy);
            TimeAndWeatherManager.Instance.onNightTimeStartedEvent.AddListener(OnNightStartedEventProxy);
            TimeAndWeatherManager.Instance.onHourPassedEvent.AddListener(OnHourPassedEventProxy);
            TimeAndWeatherManager.Instance.onMinutePassedEvent.AddListener(OnMinutePassedEventProxy);
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