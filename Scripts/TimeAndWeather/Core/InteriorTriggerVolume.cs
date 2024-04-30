#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#else
using DaftAppleGames.Core.MetaAttributes;
using DaftAppleGames.Core.DrawerAttributes;
#endif
using UnityEngine;
using UnityEngine.Audio;

namespace DaftAppleGames.TimeAndWeather.Core
{
    public class InteriorTriggerVolume : TriggerVolumeBase
    {
        [Tooltip("Select the AudioMixerSnapshot with appropriate effects applied for an indoor environment.")]
        [BoxGroup("Snapshots")]public AudioMixerSnapshot indoorSnapshot;
        [Tooltip("Select the AudioMixerSnapshot with appropriate effects applied for an outdoor environment.")]
        [BoxGroup("Snapshots")] public AudioMixerSnapshot outdoorSnapshot;
        [Tooltip("The time in seconds that the transition from one snapshot to another will take.")]
        [BoxGroup("Settings")] public float transitionTime = 0.1f;

        /// <summary>
        /// When collider enters the volume, we want to:
        /// Dampen the ambient sound (weather etc).
        /// Prevent particle system particles penetrating the volume
        /// </summary>
        /// <param name="other"></param>
        protected override void OnTriggerVolumeEnter(Collider other)
        {
            indoorSnapshot.TransitionTo(transitionTime);
        }

        /// <summary>
        /// When collider exists the volume, reverse the effects.
        /// </summary>
        /// <param name="other"></param>
        protected override void OnTriggerVolumeExit(Collider other)
        {
            outdoorSnapshot.TransitionTo(transitionTime);
        }
    }
}