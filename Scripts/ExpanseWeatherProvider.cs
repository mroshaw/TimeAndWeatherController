using System;
using System.Collections;
using Expanse;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DaftAppleGames.Common.Weather
{
    public enum FogSetting { VisibiltyDistance, Radius, Thickness }
    public class ExpanseWeatherProvider : WeatherProviderBase
    {
        [BoxGroup("Settings")] public CloudLayerInterpolator expanseCloudLayerInterpolator;
        [BoxGroup("Settings")] public CreativeFog expanseCreativeFog;
        [BoxGroup("Settings")] public ExpanseWeatherPresetSettings defaultPreset;

        /// <summary>
        /// Expanse implementation of Start Weather
        /// </summary>
        /// <param name="weatherPreset"></param>
        /// <param name="transitionCompleteCallback"></param>
        /// <param name="isImmediate"></param>
        public override void ApplyWeatherPresetProvider(WeatherPresetSettingsBase weatherPreset, Action transitionCompleteCallback, bool isImmediate)
        {
            if (!(weatherPreset is ExpanseWeatherPresetSettings expanseWeatherPreset))
            {
                Debug.Log("Provided Weather Preset is not an Expanse preset! Aborting!");
                transitionCompleteCallback.Invoke();
                return;
            }

            // Set the cloud interpolator transition
            expanseCloudLayerInterpolator.m_transitionTime = isImmediate? 0.1f : expanseWeatherPreset.skyInterpolateDuration;

            // Check if this is first weather, if so load default to give something to extrapolate from
            if (expanseCloudLayerInterpolator.m_currentPreset == null)
            {
                expanseCloudLayerInterpolator.LoadPreset(defaultPreset.expanseCloudLayer);
            }

            // Interpolate to the target settings
            StartCoroutine(ApplyWeatherPresetAsync(expanseWeatherPreset, transitionCompleteCallback));

            // Apply fog settings
            ApplyFogSetting(FogSetting.VisibiltyDistance, expanseWeatherPreset);
            ApplyFogSetting(FogSetting.Radius, expanseWeatherPreset);
            ApplyFogSetting(FogSetting.Thickness, expanseWeatherPreset);
        }

        /// <summary>
        /// Waits until any current interpolation is complete, then triggers a transition to the new preset
        /// </summary>
        /// <param name="expansePreset"></param>
        /// <param name="transitionCompleteCallback"></param>
        /// <returns></returns>
        private IEnumerator ApplyWeatherPresetAsync(ExpanseWeatherPresetSettings expansePreset, Action transitionCompleteCallback)
        {
            yield return new WaitForSeconds(0.5f);
            while (expanseCloudLayerInterpolator.IsInterpolating())
            {
                yield return null;
            }
            expanseCloudLayerInterpolator.LoadPreset(expansePreset.expanseCloudLayer);
            yield return new WaitForSeconds(0.5f);
            while (expanseCloudLayerInterpolator.IsInterpolating())
            {
                yield return null;
            }
            Debug.Log("Weather transition complete. Invoking callback...");
            transitionCompleteCallback.Invoke();
        }

        /// <summary>
        /// Applies one of the 3 Fog settings over time
        /// </summary>
        /// <param name="fogSetting"></param>
        /// <param name="expansePreset"></param>
        public void ApplyFogSetting(FogSetting fogSetting, ExpanseWeatherPresetSettings expansePreset)
        {
            StartCoroutine(ApplyFogSettingAsync(fogSetting, expansePreset));
        }

        private IEnumerator ApplyFogSettingAsync(FogSetting fogSetting, ExpanseWeatherPresetSettings expansePreset)
        {
            float time = 0;

            float startValue = 0;
            float endValue = 0;
            switch (fogSetting)
            {
                case FogSetting.Radius:
                    startValue = expanseCreativeFog.m_radius;
                    endValue = expansePreset.fogRadius;
                    break;
                case FogSetting.Thickness:
                    startValue = expanseCreativeFog.m_thickness;
                    endValue = expansePreset.fogThickness;
                    break;
                case FogSetting.VisibiltyDistance:
                    startValue = expanseCreativeFog.m_visibilityDistance;
                    endValue = expansePreset.fogVisibilityDistance;
                    break;
            }

            while (time < expansePreset.fogInterpolateDuration)
            {
                switch (fogSetting)
                {
                    case FogSetting.Radius:
                        expanseCreativeFog.m_radius = Mathf.Lerp(startValue, endValue, time / expansePreset.fogInterpolateDuration);
                        break;
                    case FogSetting.Thickness:
                        expanseCreativeFog.m_thickness = Mathf.Lerp(startValue, endValue, time / expansePreset.fogInterpolateDuration);
                        break;
                    case FogSetting.VisibiltyDistance:
                        expanseCreativeFog.m_visibilityDistance = Mathf.Lerp(startValue, endValue, time / expansePreset.fogInterpolateDuration);
                        break;
                }

                time += Time.deltaTime;
                yield return null;
            }
        }
    }
}