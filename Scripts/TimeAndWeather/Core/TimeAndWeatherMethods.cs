using UnityEngine;

namespace DaftAppleGames.TimeAndWeather.Core
{
    /// <summary>
    /// Wrapper class to expose TimeAndWeatherManager singleton methods via a component instance.
    /// This can be added to a scene in a multiscene setup, and used by UnityEvents to call various
    /// methods of the manager.
    /// </summary>
    public class TimeAndWeatherMethods : MonoBehaviour
    {
        /// <summary>
        /// Look up the weather preset based on the name given and apply it
        /// via the TimeAndWeatherManager
        /// </summary>
        /// <param name="weatherPresetName"></param>
        public void ApplyWeatherPreset(string weatherPresetName)
        {
            if (!IsValidTimeAndWeatherManager())
            {
                return;
            }
            TimeAndWeatherManager.Instance.ApplyWeatherPreset(weatherPresetName);
        }

        /// <summary>
        /// Look up the time preset based on the name given and apply it
        /// via the TimeAndWeatherManager
        /// </summary>
        /// <param name="timePresetName"></param>
        public void ApplyTimePreset(string timePresetName)
        {
            if (!IsValidTimeAndWeatherManager())
            {
                return;
            }
            TimeAndWeatherManager.Instance.ApplyTimePreset(timePresetName);
        }

        /// <summary>
        /// Call the TimeAndWeatherManager method to lerp towards the given time stamp
        /// </summary>
        /// <param name="hour"></param>
        /// <param name="minute"></param>
        /// <param name="second"></param>
        public void GoToTime(int hour, int minute, int second)
        {
            if (!IsValidTimeAndWeatherManager())
            {
                return;
            }
        }

        /// <summary>
        /// Call the TimeAndWeatherManager method to lerp towards the given hour
        /// </summary>
        public void GoToHour(int hour)
        {
            if (!IsValidTimeAndWeatherManager())
            {
                return;
            }
        }

        /// <summary>
        /// Returns true if a TimeAndWeatherManager can be found in the currently open scenes
        /// </summary>
        /// <returns></returns>
        private bool IsValidTimeAndWeatherManager()
        {
            if (!TimeAndWeatherManager.Instance)
            {
                Debug.LogError($"TimeAndWeatherMethods on {gameObject} is expecting a TimeAndWeatherManager instance in open scenes!");
                return false;
            }

            return true;
        }
    }
}