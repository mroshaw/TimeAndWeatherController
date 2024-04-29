#if __MICROSPLAT__
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#else
using DaftAppleGames.Core.MetaAttributes;
#endif
using System;
using UnityEngine;

namespace DaftAppleGames.TimeAndWeather.Core.Weather
{
    /// <summary>
    /// Provides core Weather / Sky / Cloud functionality for use across all providers.
    /// </summary>
    public class MicrosplatWeatherSync : WeatherSyncBase
    {
        private const string kWetnessParamsGlobal = "_Global_WetnessParams";
        private const string kPuddleParamsGlobal = "_Global_PuddleParams";
        private const string kStreamHeightMaxGlobal = "_Global_StreamMax";
        private const string kRainIntensityGlobal = "_Global_RainIntensity";
        private const string kSnowLevelGlobal = "_Global_SnowLevel";
        private const string kSnowMinMaxHeightGlobal = "_Global_SnowMinMaxHeight";
        private const string kWindParticulateStrengthGlobal = "_Global_WindParticulateStrength";

        private static readonly int GlobalWetnessParams = Shader.PropertyToID(kWetnessParamsGlobal);
        private static readonly int GlobalSnowMinMaxHeight = Shader.PropertyToID(kSnowMinMaxHeightGlobal);
        private static readonly int GlobalWindParticulateStrength = Shader.PropertyToID(kWindParticulateStrengthGlobal);
        private static readonly int GlobalRainIntensity = Shader.PropertyToID(kRainIntensityGlobal);
        private static readonly int GlobalSnowLevel = Shader.PropertyToID(kSnowLevelGlobal);

        /// <summary>
        /// Microsplat implementation of wind
        /// </summary>
        protected override float Wind
        {
            get => Shader.GetGlobalFloat(GlobalWindParticulateStrength);
            set => Shader.SetGlobalFloat(GlobalWindParticulateStrength, value);
        }

        /// <summary>
        /// Microsplat implementation of wetness
        /// </summary>
        protected override float Wetness
        {
            get => Shader.GetGlobalVector(GlobalWetnessParams).x;
            set
            {
                Shader.SetGlobalVector(GlobalWetnessParams, new Vector2(0, value));
                Shader.SetGlobalFloat(GlobalRainIntensity, value);
            }
        }

        /// <summary>
        /// Microsplat implementation of overlay
        /// </summary>
        protected override float Overlay
        {
            get => Shader.GetGlobalFloat(GlobalSnowLevel);
            set => Shader.SetGlobalFloat(GlobalSnowLevel, value);
        }
    }
}
#endif