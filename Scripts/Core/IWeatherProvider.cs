using UnityEngine;

namespace DaftAppleGames.Common.Weather
{
    public interface IWeatherProvider
    {
        public WeatherPresetSettingsBase GetWeatherPresets();
        public void StartWeather(WeatherPresetSettingsBase weatherPreset);
    }
}