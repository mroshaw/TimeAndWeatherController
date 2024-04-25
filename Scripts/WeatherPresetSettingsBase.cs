using Expanse;
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
        [BoxGroup("General Settings")] public string presetName;
        [BoxGroup("Particle Effects")] public GameObject cameraParticlePrefab;
        [BoxGroup("Particle Effects")] public float particleFadeDuration = 5.0f;
        [BoxGroup("Audio Effects")] public GameObject audioEffectsPrefab;
        [BoxGroup("Audio Effects")] public float audioFadeDuration = 5.0f;
    }
}