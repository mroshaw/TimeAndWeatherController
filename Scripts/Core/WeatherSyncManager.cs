#if THE_VEGETATION_ENGINE
using System.Collections;
using TheVegetationEngine;
using UnityEngine;

namespace DaftAppleGames.Common.Weather
{
    [ExecuteInEditMode]
    public class WeatherSyncManager : MonoBehaviour
    {
        [Header("Main Configuration")]
        public bool manageSnow = true;
        public bool manageWetness = true;
        public bool manualWeatherControl = false;

        [Header("Weather Providers")]
        public BaseWeatherRainProvider rainProvider;
        public BaseWeatherSnowProvider snowProvider;

        [Header("Manual Weather Snow Settings")]
        public float minSnow = 0.0f;
        public float maxSnow = 1.0f;
        private float _currentSnowLevel = 0.0f;
        public float snowTransitionDuration = 5.0f;

        [Header("Manual Weather Rain Settings")]
        public float minWet = 0.0f;
        public float maxWet = 1.0f;
        private float _currentWetLevel = 0.0f;
        public float wetTransitionDuration = 5.0f;

        [Header("Overrides")]
        public bool overrideWeather = false;
        [Range(0.0f, 1.0f)]
        public float snowLevelOverride = 0.0f;
        [Range(0.0f, 1.0f)]
        public float wetLevelOverride = 0.0f;

        /// <summary>
        /// Set Snow and Wet levels to default
        /// </summary>
        public void Start()
        {
            SetSnowLevel();
            SetWetLevel();
        }

        public void Update()
        {
            SetSnowLevel();
            SetWetLevel();
        }

        /// <summary>
        /// Get the current snow level from provider
        /// </summary>
        /// <returns></returns>
        private float GetProviderSnowLevel()
        {
            return snowProvider.GetSnowLevel();
        }

        /// <summary>
        /// Get the current rain level from provider
        /// </summary>
        /// <returns></returns>
        private float GetProviderRainLevel()
        {
            return rainProvider.GetRainLevel();
        }

        /// <summary>
        /// Sets the Snow level on the shaders
        /// </summary>
        /// <param name="snowLevel"></param>
        public void SetSnowLevel()
        {
            // If not managing snow, or now providers (manual or systems) specified, do nothing
            if(!manageSnow || (!overrideWeather && !manualWeatherControl && !snowProvider))
            {
                return;
            }

            float snowLevel = 0.0f;

            if (overrideWeather)
            {
                snowLevel = snowLevelOverride;
            }
            else if(manualWeatherControl)
            {
                snowLevel = _currentSnowLevel;
            }
            else if(snowProvider)
            {
                snowLevel = GetProviderSnowLevel();
            }

            // Set the appropriate values on shaders and whatever else
            TVEManager.Instance.globalControl.globalOverlay = snowLevel;
            Shader.SetGlobalFloat("_Global_SnowLevel", snowLevel);
        }

        /// <summary>
        /// Set the Wetness shared values
        /// </summary>
        /// <param name="wetLevel"></param>
        public void SetWetLevel()
        {
            // If not managing wet, or now providers (manual or systems) specified, do nothing
            if (!manageWetness ||(!overrideWeather && !manualWeatherControl && !rainProvider))
            {
                return;
            }

            float wetLevel = 0.0f;

            if(overrideWeather)
            {
                wetLevel = wetLevelOverride;
            }
            else if (manualWeatherControl)
            {
                wetLevel = _currentWetLevel;
            }
            else if (rainProvider)
            {
                wetLevel = GetProviderRainLevel();
            }

            // Set the appropriate values on shaders and whatever else
            TVEManager.Instance.globalControl.globalWetness = wetLevel;
            Shader.SetGlobalVector("_Global_WetnessParams", new Vector2(minWet, wetLevel));
            Shader.SetGlobalFloat("_Global_PuddleParams", wetLevel);
        }

        /// <summary>
        /// Start snow transition for manual control
        /// </summary>
        public void StartSnow()
        {
            SetSnowOverTime(minSnow, maxSnow, snowTransitionDuration);
        }

        /// <summary>
        /// End snow transition for manual control
        /// </summary>
        public void StopSnow()
        {
            SetSnowOverTime(maxSnow, minSnow, snowTransitionDuration);
        }

        /// <summary>
        /// Sync wrapper to set Snow over time
        /// </summary>
        /// <param name="startSnow"></param>
        /// <param name="endSnow"></param>
        /// <param name="duration"></param>
        private void SetSnowOverTime(float startSnow, float endSnow, float duration)
        {
            StartCoroutine(SetSnowOverTimeAsync(startSnow, endSnow, duration));
        }

        /// <summary>
        /// Sets the Snow level over time
        /// </summary>
        /// <param name="startSnow"></param>
        /// <param name="endSnow"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        private IEnumerator SetSnowOverTimeAsync(float startSnow, float endSnow, float duration)
        {
            float time = 0;
            float _currentSnowLevel = startSnow;
            while (time < duration)
            {
                _currentSnowLevel = Mathf.Lerp(startSnow, endSnow, time / duration);
                time += Time.deltaTime;
                yield return null;
            }
        }

        /// <summary>
        /// Start rain transition
        /// </summary>
        public void StartRain()
        {
            SetWetOverTime(minWet, maxWet, snowTransitionDuration);
        }

        /// <summary>
        /// End rain transition
        /// </summary>
        public void StopRain()
        {
            SetWetOverTime(maxWet, minWet, wetTransitionDuration);
        }

        /// <summary>
        /// Sync wrapper to set Wet over time
        /// </summary>
        /// <param name="startWet"></param>
        /// <param name="endWet"></param>
        /// <param name="duration"></param>
        private void SetWetOverTime(float startWet, float endWet, float duration)
        {
            StartCoroutine(SetWetOverTimeAsync(startWet, endWet, duration));
        }


        /// <summary>
        /// Sets the Wet level over time
        /// </summary>
        /// <param name="startWet"></param>
        /// <param name="endWet"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        private IEnumerator SetWetOverTimeAsync(float startWet, float endWet, float duration)
        {
            float time = 0;
            _currentWetLevel = startWet;
            while (time < duration)
            {
                _currentWetLevel = Mathf.Lerp(startWet, endWet, time / duration);
                time += Time.deltaTime;
                yield return null;
            }
        }
    }
}
#endif