using System;
using System.Collections.Generic;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#else
using DaftAppleGames.Core.MetaAttributes;
#endif
using UnityEngine;

namespace DaftAppleGames.TimeAndWeather.Core.Weather
{
    /// <summary>
    /// Base scriptable object for weather settings in the Time and Weather manager
    /// </summary>
    public class WeatherPresetSettingsBase : TimeAndWeatherPresetSettingsBase
    {
        // Public serializable properties
        [Tooltip("A prefab asset containing ParticleSystem components for the weather effect. This will be instantiated and parented to the Main Camera.")]
        [BoxGroup("Particle Effects")] public GameObject cameraParticlePrefab;
        [Tooltip("The duration in seconds that the particle emission rate will interpolate when activated and deactivated in a transition.")]
        [BoxGroup("Particle Effects")] public float particleFadeDuration = 5.0f;
        [Tooltip("A prefab asset containing an AudioSource component for the weather sound effects. This will be instantiated and parented to the Main Camera.")]
        [BoxGroup("Audio Effects")] public GameObject audioEffectsPrefab;
        [Tooltip("The duration in seconds that the AudioSource volume will interpolate when activated and deactivated in a transition.")]
        [BoxGroup("Audio Effects")] public float audioFadeDuration = 5.0f;

        [Tooltip("Relative wetness of this preset, with 0 being dry / min wetness to 1 being max wetness.")]
        [BoxGroup("Sync Settings")] [Range(0, 1)] public float wetness = 1.0f;
        [Tooltip("Time in seconds that wetness will move to the target setting during sync.")]
        [BoxGroup("Sync Settings")] public float wetnessSyncDuration;
        [Tooltip("Relative overlay / snow of this preset, with 0 being no overlay to 1 being max.")]
        [BoxGroup("Sync Settings")] [Range(0, 1)] public float overlay = 1.0f;
        [Tooltip("Time in seconds that overlay / snow will move to the target setting during sync.")]
        [BoxGroup("Sync Settings")] public float overlaySyncDuration;
        [Tooltip("Relative strength of the wind of this preset, with 0 being no wind to 1 being max wind.")]
        [BoxGroup("Sync Settings")] [Range(0, 1)] public float wind;
        [Tooltip("Time in seconds that wind will move to the target setting during sync.")]
        [BoxGroup("Sync Settings")] public float windSyncDuration;
        [Tooltip("A list of weather presets that will be considered for random transitions, when the random transition function is enabled.")]
        [BoxGroup("Transitions")] public List<WeatherTransition> canTransitionToWeatherPresets;

        /// <summary>
        /// Provides pairs of Weather settings and transition durations, which can then be added
        /// to the list of permitted Weather transitions in the main weather preset.
        /// This allows customisation of what weather presets each can transition to, and allows control
        /// over how long the duration lasts for each.
        /// </summary>
        [Serializable]
        public class WeatherTransition
        {
            [Tooltip("The weather preset that can be transitioned to from this one.")]
            public WeatherPresetSettingsBase CanTransitionToWeatherPreset;
            [Tooltip("The duration in seconds that the transition to this preset will last")]
            public float InterpolationDuration;
            [Tooltip("The relative liklihood of this preset occuring, in relation to other permitted transition presets")]
            [Range(0, 1)] public float ChanceOfOccuring;

            /// <summary>
            /// Simple class constructor
            /// </summary>
            /// <param name="canTransitionToWeatherPreset"></param>
            /// <param name="interpolationDuration"></param>
            /// <param name="chanceOfOccuring"></param>
            public WeatherTransition(WeatherPresetSettingsBase canTransitionToWeatherPreset, float interpolationDuration, float chanceOfOccuring)
            {
                CanTransitionToWeatherPreset = canTransitionToWeatherPreset;
                InterpolationDuration = interpolationDuration;
                ChanceOfOccuring = chanceOfOccuring;
            }
        }
    }
}