using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace DaftAppleGames.Common.Weather
{
    /// <summary>
    /// Provides core Weather / Sky / Cloud functionality for use across all providers.
    /// </summary>
    public abstract class WeatherProviderBase : TimeAndWeatherProviderBase
    {
        [PropertyOrder(1)] [BoxGroup("Events")] public UnityEvent<WeatherPresetSettingsBase> onWeatherPresetAppliedEvent;

        // Provides a lookup dictionary of instantiated prefabs by preset name
        private Dictionary<string, ParticleVfx> _particleVfxInstances;
        private Dictionary<string, AudioSfx> _audioSfxInstances;

        private ParticleVfx _currentParticleVfx;
        private AudioSfx _currentAudioSfx;
        private string _currentWeather = "";
        private Camera _mainCamera;
        private bool _isInTransition;

        /// <summary>
        /// Allows components to query the transition state
        /// </summary>
        public bool IsIntransition => _isInTransition;

        /// <summary>
        /// To save time later, iterate over all possible prefabs associated to possible weather presets and
        /// instantiate them and parent to the Main Camera. These can then be activated, deactivated and lerps by
        /// accessiing them by preset name from the two Dictionaries.
        /// </summary>
        private void Awake()
        {
            _mainCamera = Camera.main;
            InstantiateFxPrefabs();
            _isInTransition = false;
        }

        /// <summary>
        /// Pre-instantiate particle and audio FX prefabs on the camera, so they can be enabled and disabled
        /// </summary>
        private void InstantiateFxPrefabs()
        {
            _particleVfxInstances = new();
            _audioSfxInstances = new();

            foreach (WeatherPresetSettingsBase settings in TimeAndWeatherManager.weatherPresets)
            {
                if (settings.cameraParticlePrefab)
                {
                    Vector3 relativePosition = settings.cameraParticlePrefab.transform.localPosition;
                    Quaternion relativeRotation = settings.cameraParticlePrefab.transform.localRotation;
                    GameObject newParticleFx = Instantiate(settings.cameraParticlePrefab, _mainCamera.gameObject.transform, true);
                    newParticleFx.SetActive(false);
                    newParticleFx.name = $"{settings.presetName} VFX";
                    newParticleFx.transform.localPosition = relativePosition;
                    newParticleFx.transform.localRotation = relativeRotation;
                    _particleVfxInstances.Add(settings.presetName, new ParticleVfx(newParticleFx));
                }

                if (settings.audioEffectsPrefab)
                {
                    GameObject newAudioFx = Instantiate(settings.audioEffectsPrefab, _mainCamera.gameObject.transform, true);
                    newAudioFx.name = $"{settings.presetName} SFX";
                    newAudioFx.transform.localPosition = Vector3.zero;
                    newAudioFx.transform.localRotation = Quaternion.identity;
                    newAudioFx.GetComponent<AudioSource>().Stop();
                    _audioSfxInstances.Add(settings.presetName, new AudioSfx(newAudioFx));
                }
            }
        }

        /// <summary>
        /// Start weather with given weather presets. Check first if we're already in a transition. If not,
        /// then lerp VFX and SFX, if they are provided, and then call the provider abstract method so the
        /// provider can do whatever it needs to do.
        /// </summary>
        /// <param name="weatherPreset"></param>
        /// <param name="isImmediate"></param>
        public void ApplyWeatherPreset(WeatherPresetSettingsBase weatherPreset, bool isImmediate = false)
        {
            if (_currentWeather == weatherPreset.presetName)
            {
                return;
            }

            // Don't allow another preset until the last one has been applied
            if (_isInTransition)
            {
                Debug.Log("Already in a Weather Transition - aborting");
                return;
            }

            _isInTransition = true;

            _currentWeather = weatherPreset.presetName;

            EnableWeatherVfx(weatherPreset);
            PlayWeatherSfx(weatherPreset);
            ApplyWeatherPresetProvider(weatherPreset, ProviderTransitionComplete, isImmediate);
            onWeatherPresetAppliedEvent.Invoke(weatherPreset);
        }

        /// <summary>
        /// Call back action for provider to tell us the transition is done.
        /// </summary>
        private void ProviderTransitionComplete()
        {
            _isInTransition = false;
        }

        /// <summary>
        /// Weather provider implementation of StartWeather
        /// </summary>
        /// <param name="weatherPreset"></param>
        /// <param name="transitionCompleteCallback"></param>
        /// <param name="isImmediate"></param>
        protected abstract void ApplyWeatherPresetProvider(WeatherPresetSettingsBase weatherPreset, Action transitionCompleteCallback, bool isImmediate);

        /// <summary>
        /// Plays associated weather sounds effects by lerps to and from target volume. This uses the internal AudioSfx
        /// class to store initial volume values for each prefab, allowing us to lerp to and from without losing the 'baseline'
        /// volume state of each prefab
        /// </summary>
        /// <param name="weatherPreset"></param>
        private void PlayWeatherSfx(WeatherPresetSettingsBase weatherPreset)
        {
            // Stop current Audio
            if (_currentAudioSfx != null)
            {
                FadeAudioSource(_currentAudioSfx, weatherPreset.audioFadeDuration, false);
            }
            // Play weather SFX AudioSource
            if (weatherPreset.audioEffectsPrefab)
            {
                FadeAudioSource(_audioSfxInstances[weatherPreset.presetName], weatherPreset.audioFadeDuration, true);
                _currentAudioSfx = _audioSfxInstances[weatherPreset.presetName];
            }
        }

        /// <summary>
        /// Enables weather specific particle FX by lerping the emission rate over time. This uses the internal ParticleVfx
        /// class to store 'baseline' emission rates, so we don't lose them when lerping to and from 0.
        /// </summary>
        /// <param name="weatherPreset"></param>
        private void EnableWeatherVfx(WeatherPresetSettingsBase weatherPreset)
        {
            // Stop current VFX
            if (_currentParticleVfx != null)
            {
                FadeParticleSystem(_currentParticleVfx, weatherPreset.audioFadeDuration, false);
            }
            // Enable camera particle system
            if (weatherPreset.cameraParticlePrefab)
            {
                FadeParticleSystem(_particleVfxInstances[weatherPreset.presetName], weatherPreset.audioFadeDuration, true);
                _currentParticleVfx = _particleVfxInstances[weatherPreset.presetName];
            }
        }

        /// <summary>
        /// Synchronous wrapper for the asynch cooroutine responsible for fading the AudioSource in and out
        /// </summary>
        /// <param name="audioSfx"></param>
        /// <param name="fadeDuration"></param>
        /// <param name="fadeIn"></param>
        private void FadeAudioSource(AudioSfx audioSfx, float fadeDuration, bool fadeIn)
        {
            StartCoroutine(FadeAudioSourceAsync(audioSfx, fadeDuration, fadeIn));
        }

        /// <summary>
        /// Fades the AudioSource over time by lerping between the 'baseline' volume, captured in an AudioSfx instance,
        /// and 0.
        /// </summary>
        /// <param name="audioSfx"></param>
        /// <param name="fadeDuration"></param>
        /// <param name="fadeIn"></param>
        /// <returns></returns>
        private IEnumerator FadeAudioSourceAsync(AudioSfx audioSfx, float fadeDuration, bool fadeIn)
        {
            float startValue = fadeIn ? 0 : audioSfx.AudioVolume;
            float endValue = fadeIn ? audioSfx.AudioVolume : 0;

            if (fadeIn)
            {
                audioSfx.AudioSource.volume = startValue;
                audioSfx.AudioSource.Play();
            }

            float time = 0;

            while (time < fadeDuration)
            {
                audioSfx.AudioSource.volume = Mathf.Lerp(startValue, endValue, time / fadeDuration);
                time += Time.deltaTime;
                yield return null;
            }

            audioSfx.AudioSource.volume = endValue;

            if (!fadeIn)
            {
                audioSfx.AudioSource.Stop();
                _currentAudioSfx = null;
            }
        }

        /// <summary>
        /// Synchronous wrapper for the asynchronous cooroutine for lerping ParticleSystem emission rate over time
        /// </summary>
        /// <param name="particleVfx"></param>
        /// <param name="fadeDuration"></param>
        /// <param name="fadeIn"></param>
        private void FadeParticleSystem(ParticleVfx particleVfx, float fadeDuration, bool fadeIn)
        {
            StartCoroutine(FadeParticleSystemAsync(particleVfx, fadeDuration, fadeIn));
        }

        /// <summary>
        /// Asynchronous lerping of particle system emission rates over time.
        /// </summary>
        /// <param name="particleVfx"></param>
        /// <param name="fadeDuration"></param>
        /// <param name="fadeIn"></param>
        /// <returns></returns>
        private IEnumerator FadeParticleSystemAsync(ParticleVfx particleVfx, float fadeDuration, bool fadeIn)
        {
            float startValue = fadeIn ? 0 : particleVfx.EmissionRate;
            float endValue = fadeIn ? particleVfx.EmissionRate : 0;

            ParticleSystem.EmissionModule emissionModule = particleVfx.ParticleSystem.emission;

            if (fadeIn)
            {
                particleVfx.ParticleInstanceGameObject.SetActive(true);
                emissionModule.rateOverTime = startValue;
            }

            float time = 0;

            while (time < fadeDuration)
            {
                emissionModule.rateOverTime= Mathf.Lerp(startValue, endValue, time / fadeDuration);
                time += Time.deltaTime;
                yield return null;
            }

            emissionModule.rateOverTime = endValue;

            if (!fadeIn)
            {
                particleVfx.ParticleInstanceGameObject.SetActive(false);
                emissionModule.rateOverTime = particleVfx.EmissionRate;
                _currentParticleVfx = null;
            }
        }

        /// <summary>
        /// Internal class that allows us to record 'baseline' states of all instantiated audio SFX prefabs.
        /// This allows us to access the instantiated Game Object instance, and to know at all times what the
        /// default or 'baseline' AudioSource volume was at the point of instantiation. This allows us to modify the
        /// volume, and always be able to return it to it's baseline value.
        /// </summary>
        private class AudioSfx
        {
            public GameObject AudioInstanceGameObject;
            public AudioSource AudioSource;
            public float AudioVolume;

            public AudioSfx(GameObject audioInstanceGameObject)
            {
                AudioInstanceGameObject = audioInstanceGameObject;
                AudioSource = audioInstanceGameObject.GetComponent<AudioSource>();
                AudioVolume = audioInstanceGameObject.GetComponent<AudioSource>().volume;
            }
        }

        /// <summary>
        /// Internal class that allows us to record 'baseline' states of all instantiated particle VFX prefabs.
        /// This allows us to access the instantiated Game Object instance, and to know at all times what the
        /// default or 'baseline' ParticleSystem emissionRate was at the point of instantiation. This allows us to modify the
        /// emissionRate, and always be able to return it to it's baseline value.
        /// </summary>
        private class ParticleVfx
        {
            public GameObject ParticleInstanceGameObject;
            public ParticleSystem ParticleSystem;
            public float EmissionRate;
            public ParticleVfx(GameObject particleInstanceGameObject)
            {
                ParticleInstanceGameObject = particleInstanceGameObject;
                ParticleSystem = particleInstanceGameObject.GetComponent<ParticleSystem>();
                EmissionRate = particleInstanceGameObject.GetComponent<ParticleSystem>().emission.rateOverTime.constant;

            }
        }
    }
}