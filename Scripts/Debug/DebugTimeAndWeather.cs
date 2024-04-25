using System;
using System.Collections;
using System.Collections.Generic;
using DaftAppleGames.Common.Weather;
using UnityEngine;

namespace DaftAppleGames.Common.Debugger
{
    public class DebugTimeAndWeather : DebugBase
    {
        /// <summary>
        /// Get the current time of day preset names
        /// </summary>
        /// <returns></returns>
        public List<string> GetTimePresetNames()
        {
            return TimeAndWeatherManager.Instance.GetTimePresetNames();
        }

        /// <summary>
        /// Get the current time of day preset names
        /// </summary>
        /// <returns></returns>
        public List<string> GetWeatherPresetNames()
        {
            return TimeAndWeatherManager.Instance.GetWeatherPresetNames();
        }

        /// <summary>
        /// Set the time of day
        /// </summary>
        /// <param name="presetName"></param>
        public void SetTimeOfDay(string presetName)
        {
            TimeAndWeatherManager.Instance.ApplyTimePreset(presetName);
        }

        /// <summary>
        /// Set the weather preset
        /// </summary>
        /// <param name="presetName"></param>
        public void SetWeather(string presetName)
        {
            TimeAndWeatherManager.Instance.ApplyWeatherPreset(presetName);
        }

        /// <summary>
        /// Call the async function to check when weather transition is complete
        /// </summary>
        /// <param name="transitionCompleteCallback"></param>
        public void WaitForWeatherTransitionToComplete(Action transitionCompleteCallback)
        {
            StartCoroutine(WaitForWeatherTransitionToCompleteAsync(transitionCompleteCallback));
        }

        /// <summary>
        /// Call the async function to check when time transition is complete
        /// </summary>
        /// <param name="transitionCompleteCallback"></param>
        public void WaitForTimeTransitionToComplete(Action transitionCompleteCallback)
        {
            StartCoroutine(WaitForTimeTransitionToCompleteAsync(transitionCompleteCallback));
        }

        /// <summary>
        /// Waits for the weather transition to complete, then calls the callback method
        /// </summary>
        /// <param name="transitionCompleteCallback"></param>
        /// <returns></returns>
        public IEnumerator WaitForWeatherTransitionToCompleteAsync(Action transitionCompleteCallback)
        {
            yield return new WaitForSeconds(1);
            while (TimeAndWeatherManager.Instance.weatherProvider.IsIntransition)
            {
                yield return null;
            }
            transitionCompleteCallback.Invoke();
        }

        /// <summary>
        /// Waits for the time transition to complete, then calls the callback method
        /// </summary>
        /// <param name="transitionCompleteCallback"></param>
        /// <returns></returns>
        public IEnumerator WaitForTimeTransitionToCompleteAsync(Action transitionCompleteCallback)
        {
            yield return new WaitForSeconds(1);
            while (TimeAndWeatherManager.Instance.timeProvider.IsIntransition)
            {
                yield return null;
            }
            transitionCompleteCallback.Invoke();
        }
    }
}