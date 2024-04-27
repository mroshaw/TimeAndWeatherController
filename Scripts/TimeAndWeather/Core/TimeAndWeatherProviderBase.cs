using UnityEngine;

namespace DaftAppleGames.TimeAndWeather.Core
{
    public abstract class TimeAndWeatherProviderBase : MonoBehaviour
    {
        private TimeAndWeatherManager _timeAndWeatherManager;
        protected TimeAndWeatherManager TimeAndWeatherManager => GetComponent<TimeAndWeatherManager>();
    }
}