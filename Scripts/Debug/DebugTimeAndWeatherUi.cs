using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;
using System.Collections.Generic;

namespace DaftAppleGames.Common.Debugger
{
    public class DebugTimeAndWeatherUi : DebugBaseUi
    {
        [BoxGroup("UI Settings")] public TMP_Dropdown timePresetDropdown;
        [BoxGroup("UI Settings")] public TMP_Dropdown weatherPresetDropdown;
        [BoxGroup("Settings")] public string debugTimeAndWeatherObjectName;

        private DebugTimeAndWeather _debugTimeAndWeather;

        /// <summary>
        /// Set up the component
        /// </summary>
        public override void Start()
        {
            base.Start();

            if (string.IsNullOrEmpty(debugTimeAndWeatherObjectName))
            {
                Debug.LogError($"DebugTimeAndWeatherUi: Please set the debugTimeAndWeatherObjectName property on {gameObject.name}!");
            }
            else
            {
                _debugTimeAndWeather = (DebugTimeAndWeather)base.FindDebugObject<DebugTimeAndWeather>(debugTimeAndWeatherObjectName);
            }
            PopulateTimePresets();
            PopulateWeatherPresets();
        }

        /// <summary>
        /// Populate the Time Preset dropdown
        /// </summary>
        private void PopulateTimePresets()
        {
            timePresetDropdown.ClearOptions();
            List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

            foreach (string timePresetNames in _debugTimeAndWeather.GetTimePresetNames())
            {
                TMP_Dropdown.OptionData newOption = new TMP_Dropdown.OptionData
                {
                    text = timePresetNames
                };
                options.Add(newOption);
            }
            timePresetDropdown.options = options;
        }

        /// <summary>
        /// Populate the Weather Preset dropdown
        /// </summary>
        private void PopulateWeatherPresets()
        {
            weatherPresetDropdown.ClearOptions();
            List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

            foreach (string timePresetNames in _debugTimeAndWeather.GetWeatherPresetNames())
            {
                TMP_Dropdown.OptionData newOption = new TMP_Dropdown.OptionData
                {
                    text = timePresetNames
                };
                options.Add(newOption);
            }
            weatherPresetDropdown.options = options;
        }

        /// <summary>
        /// Handler for the Set Time button
        /// </summary>
        public void SetTimeProxy()
        {
            string currentTargetName = timePresetDropdown.options[timePresetDropdown.value].text;
            ShowLog("Time transition started...");
            _debugTimeAndWeather.SetTimeOfDay(currentTargetName);
            _debugTimeAndWeather.WaitForTimeTransitionToComplete(TimeTransitionComplete);
        }

        /// <summary>
        /// Handler for the Set Weather button
        /// </summary>
        public void SetWeatherProxy()
        {
            string currentTargetName = weatherPresetDropdown.options[weatherPresetDropdown.value].text;
            ShowLog("Weather transition started...");
            _debugTimeAndWeather.SetWeather(currentTargetName);
            _debugTimeAndWeather.WaitForWeatherTransitionToComplete(WeatherTransitionComplete);
        }

        /// <summary>
        /// Weather transition callback
        /// </summary>
        private void WeatherTransitionComplete()
        {
            ShowLog("Weather transition complete!");
        }

        /// <summary>
        /// Time transition callback
        /// </summary>
        private void TimeTransitionComplete()
        {
            ShowLog("Time transition complete!");
        }
    }
}