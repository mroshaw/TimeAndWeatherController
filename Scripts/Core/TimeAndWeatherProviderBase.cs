using UnityEngine;

namespace DaftAppleGames.Common.Weather
{
    public abstract class TimeAndWeatherProviderBase : MonoBehaviour
    {
        private TimeAndWeatherManager _timeAndWeatherManager;
        protected TimeAndWeatherManager TimeAndWeatherManager => GetComponent<TimeAndWeatherManager>();
    }
}