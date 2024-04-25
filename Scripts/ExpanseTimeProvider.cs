using Expanse;
using Sirenix.OdinInspector;

namespace DaftAppleGames.Common.Weather
{
    public class ExpanseTimeProvider : TimeProviderBase
    {
        [BoxGroup("Settings")] public DateTimeController expanseDateTimeController;

        /// <summary>
        /// Return the Expanse time, if the component is connected
        /// </summary>
        public override int HourProvider
        {
            get => (expanseDateTimeController==null? 0: expanseDateTimeController.m_timeLocal.hour);
            set { if(expanseDateTimeController!=null) expanseDateTimeController.m_timeLocal.hour = value; }
        }

        /// <summary>
        /// Return the Expanse minute, if the component is connected
        /// </summary>
        public override int MinuteProvider
        {
            get => (expanseDateTimeController==null? 0: expanseDateTimeController.m_timeLocal.minute);
            set { if(expanseDateTimeController!=null) expanseDateTimeController.m_timeLocal.minute = value; }
        }

        /// <summary>
        /// Sets the selected Expanse time preset
        /// </summary>
        /// <param name="timePreset"></param>
        public override void ApplyTimePresetProvider(TimeOfDayPresetSettingsBase timePreset)
        {
            if (!(timePreset is ExpanseTimeOfDayPresetSettings expanseTimePreset))
            {
                return;
            }

            // If there are weather / cloud settings associated, apply them
            if (expanseTimePreset.expanseWeatherSettings)
            {
                TimeAndWeatherManager.ApplyWeatherPreset(expanseTimePreset.expanseWeatherSettings);
            }
        }
    }
}