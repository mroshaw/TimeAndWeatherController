#if ODIN_INSPECTOR
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using DaftAppleGames.TimeAndWeather.Core;

namespace DaftAppleGames.TimeAndWeather.Editor
{
    /// <summary>
    /// This class is required in order to force a refresh of the [ShowInInspector] properties that are
    /// shown in the TimeAndWeatherManager inspector. While this isn't great for performance, this is not
    /// included nor executed in a build.
    /// </summary>
    [CustomEditor(typeof(TimeAndWeatherManager))]
    public class TimeAndWeatherManagerEditor : OdinEditor
    {
        /// <summary>
        /// This forces the OnInspectorGUI to run each frame, refreshing the Inspector
        /// </summary>
        /// <returns></returns>
        public override bool RequiresConstantRepaint()
        {
            return true;
        }
    }
}
#endif