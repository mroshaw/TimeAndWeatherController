using System;
using System.Collections;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#else
using DaftAppleGames.Core.MetaAttributes;
#endif
using Expanse;
using UnityEngine;

namespace DaftAppleGames.TimeAndWeather.Core.Weather
{
    /// <summary>
    /// This enum is used internally within the class to simplify the fog interpolation into a single method.
    /// </summary>
    internal enum FogSetting { VisibiltyDistance, Radius, Thickness }
    public class ExpanseWeatherProvider : WeatherProviderBase
    {
        [Tooltip("The Expanse CloudLayerInterpolator component. Be sure to use the FullSkies (Interpolable) prefab from Expanse.")]
        [BoxGroup("Expanse Settings")] public CloudLayerInterpolator expanseCloudLayerInterpolator;
        [Tooltip("The Expanse CreativeFog component from the Expanse FullSkies (Interpolable) prefab.")]
        [BoxGroup("Expanse Settings")] public CreativeFog expanseCreativeFog;
        [Tooltip("The WeatherPresetSettings for an 'primer' Cloud Layer that the interpolator will use to then interpolate to the very first chosen default preset.")]
        [BoxGroup("Expanse Settings")] public ExpanseWeatherPresetSettings interpolatorPrimerPreset;

        /// <summary>
        /// Applies Weather presets with Expanse specific functionalty. That is, using the CloudLayerInterpolator to transition from one
        /// CloudLayer to another. This method will also Lerp the VisibilityDistance, Radius and Thickness of the CreativeFog component.
        /// Once complete, the transitionCompleteCallback is invoked, to signal we're done.
        /// </summary>
        /// <param name="weatherPreset"></param>
        /// <param name="transitionCompleteCallback"></param>
        /// <param name="isImmediate"></param>
        protected override void ApplyWeatherPresetProvider(WeatherPresetSettingsBase weatherPreset, Action transitionCompleteCallback, bool isImmediate)
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
            if (expanseCloudLayerInterpolator.m_targetPreset == null)
            {
                expanseCloudLayerInterpolator.LoadPreset(interpolatorPrimerPreset.expanseCloudLayer);
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
            // yield return new WaitForSeconds(0.5f);
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
            transitionCompleteCallback.Invoke();
        }

        /// <summary>
        /// Applies one of the 3 Fog settings over time
        /// </summary>
        /// <param name="fogSetting"></param>
        /// <param name="expansePreset"></param>
        private void ApplyFogSetting(FogSetting fogSetting, ExpanseWeatherPresetSettings expansePreset)
        {
            StartCoroutine(ApplyFogSettingAsync(fogSetting, expansePreset));
        }

        /// <summary>
        /// Async method that Lerps the fog properties from and to targets that are determined by the 'initial'
        /// state of the fog, and the values in the given preset.
        /// </summary>
        /// <param name="fogSetting"></param>
        /// <param name="expansePreset"></param>
        /// <returns></returns>
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

                time += UnityEngine.Time.deltaTime;
                yield return null;
            }
        }
    }
}