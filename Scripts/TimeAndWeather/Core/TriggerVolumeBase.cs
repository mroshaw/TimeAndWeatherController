#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#else
using DaftAppleGames.Core.MetaAttributes;
using DaftAppleGames.Core.DrawerAttributes;
#endif
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace DaftAppleGames.TimeAndWeather.Core
{
    public abstract class TriggerVolumeBase : MonoBehaviour
    {
        [Tooltip("The trigger volume events will fire only if the collider that hits it has one of these tags. Leave empty to ignore this check.")]
        [BoxGroup("Trigger Settings")] public string[] triggerTags;
        [Tooltip("The trigger volume events will fire only if the collider that hits it has one of these layers. Leave empty to ignore this check.")]
        [BoxGroup("Trigger Settings")] public LayerMask triggerLayers;

        [FoldoutGroup("Events")] public UnityEvent<Collider> onTriggerVolumeEnterEvent;
        [FoldoutGroup("Events")] public UnityEvent<Collider> onTriggerVolumeExitEvent;

        /// <summary>
        /// When a collider enters the trigger volume, check if it has any of the specified Tags / Layers.
        /// If so, invoke the abstract method and event.
        /// </summary>
        /// <param name="other"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnTriggerEnter(Collider other)
        {
            string otherTag = other.tag;
            LayerMask otherLayer = other.gameObject.layer;

            // If the other collider matches any of the trigger tags or trigger layer mask, invoke the event
            if ((triggerTags.Length == 0 || triggerTags.Any(otherTag.Contains)) &&
                (triggerLayers == 0 || (triggerLayers & (1 << otherLayer)) != 0))
            {
                OnTriggerVolumeEnter(other);
                onTriggerVolumeEnterEvent.Invoke(other);
            }
        }

        /// <summary>
        /// When a collider exists the trigger volume, do the same checks as above.
        /// </summary>
        /// <param name="other"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnTriggerExit(Collider other)
        {
            string otherTag = other.tag;
            LayerMask otherLayer = other.gameObject.layer;

            // If the other collider matches any of the trigger tags or trigger layer mask, invoke the event
            if ((triggerTags.Length == 0 || triggerTags.Any(otherTag.Contains)) &&
                (triggerLayers == 0 || (triggerLayers & (1 << otherLayer)) != 0))
            {
                OnTriggerVolumeExit(other);
                onTriggerVolumeExitEvent.Invoke(other);
            }
        }

        /// <summary>
        /// Abstract method that's triggered when a valid collider enters the volume
        /// </summary>
        /// <param name="other"></param>
        protected abstract void OnTriggerVolumeEnter(Collider other);

        /// <summary>
        /// Abstract method that's triggered when a valid collider exist the volume
        /// </summary>
        /// <param name="other"></param>
        protected abstract void OnTriggerVolumeExit(Collider other);
    }
}