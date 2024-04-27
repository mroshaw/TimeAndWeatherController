using System;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#else
using DaftAppleGames.Core.MetaAttributes;
#endif
using Expanse;

using UnityEngine;

namespace DaftAppleGames.TimeAndWeather.Core.TimeOfDay
{
    public class ExpanseTimeProvider : TimeProviderBase
    {
        [Tooltip("The Expanse DateTimeController component. Be sure to use the FullSkies (Interpolable) prefab from Expanse.")]
        [BoxGroup("Expanse Settings")] public DateTimeController expanseDateTimeController;

        /// <summary>
        /// Return the Expanse time, if the component is connected
        /// </summary>
        protected override int HourProvider
        {
            get => (expanseDateTimeController==null? 0: expanseDateTimeController.m_timeLocal.hour);
            set { if(expanseDateTimeController!=null) expanseDateTimeController.m_timeLocal.hour = value; }
        }

        /// <summary>
        /// Return the Expanse minute, if the component is connected
        /// </summary>
        protected override int MinuteProvider
        {
            get => (expanseDateTimeController==null? 0: expanseDateTimeController.m_timeLocal.minute);
            set { if(expanseDateTimeController!=null) expanseDateTimeController.m_timeLocal.minute = value; }
        }

        /// <summary>
        /// Return the Expanse minute, if the component is connected
        /// </summary>
        protected override int SecondProvider
        {
            get => (expanseDateTimeController==null? 0: expanseDateTimeController.m_timeLocal.second);
            set { if(expanseDateTimeController!=null) expanseDateTimeController.m_timeLocal.second = value; }
        }

        /// <summary>
        /// Return the Expanse date and time as a DateTime
        /// </summary>
        protected override DateTime DateTimeProvider
        {
            get => (expanseDateTimeController==null? DateTime.Now: expanseDateTimeController.GetDateTimeLocal());
            set { if(expanseDateTimeController!=null) expanseDateTimeController.m_timeLocal.SetFromDateTime(value); }
        }

        /// <summary>
        /// Sets the selected Expanse time preset
        /// </summary>
        /// <param name="timePreset"></param>
        /// <param name="transitionCompleteCallback"></param>
        protected override void ApplyTimePresetProvider(TimeOfDayPresetSettingsBase timePreset, Action transitionCompleteCallback)
        {
            // Handle the unlikely event that we get passed a different provider settings class
            if (timePreset is ExpanseTimeOfDayPresetSettings expanseTimePreset)
            {
                // If there are weather / cloud settings associated, apply them
                if (expanseTimePreset.expanseWeatherSettings)
                {
                    TimeAndWeatherManager.ApplyWeatherPreset(expanseTimePreset.expanseWeatherSettings);
                }
            }

            // Invoke the callback to say we're done.
            // Needs looking into, as ApplyWeatherPreset is asynchronous, so this will fire before
            // the weather transition part is complete.
            transitionCompleteCallback.Invoke();
        }

        /// <summary>
        /// Abstract implementation of GotoTime. Nothing to do here for example, so simply invoke the callback.
        /// </summary>
        /// <param name="timeSettings"></param>
        /// <param name="transitionCompleteCallback"></param>
        /// <exception cref="NotImplementedException"></exception>
        public override void GotoTimeProvider(TimeOfDayPresetSettingsBase timeSettings, Action transitionCompleteCallback)
        {
            transitionCompleteCallback.Invoke();
        }
    }
}