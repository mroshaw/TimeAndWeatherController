using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace DaftAppleGames.Common.Weather
{
    /// <summary>
    /// Base scriptable object for weather settings in the Time and Weather manager
    /// </summary>
    public class WeatherPresetSettingsBase : ScriptableObject
    {
        // Public serializable properties
        [Tooltip("Plain text name of the weather preset. This must be unique amongst other presets selected for a given TimeAndWeatherManager.")]
        [BoxGroup("General Settings")] public string presetName;
        [Tooltip("A prefab asset containing ParticleSystem components for the weather effect. This will be instantiated and parented to the Main Camera.")]
        [BoxGroup("Particle Effects")] public GameObject cameraParticlePrefab;
        [Tooltip("The duration in seconds that the particle emission rate will interpolate when activated and deactivated in a transition.")]
        [BoxGroup("Particle Effects")] public float particleFadeDuration = 5.0f;
        [Tooltip("A prefab asset containing an AudioSource component for the weather sound effects. This will be instantiated and parented to the Main Camera.")]
        [BoxGroup("Audio Effects")] public GameObject audioEffectsPrefab;
        [Tooltip("The duration in seconds that the AudioSource volume will interpolate when activated and deactivated in a transition.")]
        [BoxGroup("Audio Effects")] public float audioFadeDuration = 5.0f;
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