using UnityEngine;

namespace DaftAppleGames.TimeAndWeather.Core
{
    public abstract class TimeAndWeatherProviderBase : MonoBehaviour
    {
        public TimeAndWeatherManager TimeAndWeatherManager { get; set; }
    }
}