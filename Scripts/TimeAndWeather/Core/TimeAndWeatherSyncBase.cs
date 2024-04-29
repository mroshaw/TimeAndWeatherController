#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#else
using DaftAppleGames.Core.MetaAttributes;
#endif
using System;
using System.Collections;
using UnityEngine;

namespace DaftAppleGames.TimeAndWeather.Core
{
    /// <summary>
    /// Base class for functionality related to sync'ing aspects of time and weather with 3rd party tools
    /// such as The Vegetation Engine and Microsplat
    /// </summary>
    public class TimeAndWeatherSyncBase : MonoBehaviour
    {
        /// <summary>
        /// Lerps between given values, calling a delegate each frame. The delegate implements the specific setting change
        /// for a particular sync provider.
        /// </summary>
        /// <param name="syncStartValue"></param>
        /// <param name="syncEndValue"></param>
        /// <param name="syncDuration"></param>
        /// <param name="syncValueDelegate"></param>
        public void Synchronise(float syncStartValue, float syncEndValue, float syncDuration,  Action<float> syncValueDelegate)
        {
            StartCoroutine(SynchroniseAsync(syncStartValue, syncEndValue,syncDuration, syncValueDelegate));
        }

        /// <summary>
        /// Lerp between min and max values, calling the delegate each frame.
        /// </summary>
        /// <param name="syncStartValue"></param>
        /// <param name="syncEndValue"></param>
        /// <param name="syncDuration"></param>
        /// <param name="syncValueDelegate"></param>
        /// <returns></returns>
        private IEnumerator SynchroniseAsync(float syncStartValue, float syncEndValue, float syncDuration,  Action<float> syncValueDelegate)
        {
            float timeElapsed = 0;

            while (timeElapsed < syncDuration)
            {
                syncValueDelegate.Invoke(Mathf.Lerp(syncStartValue, syncEndValue, timeElapsed / syncDuration));
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            // Set the final value
            syncValueDelegate.Invoke(syncEndValue);
        }
    }
}