#if ODIN_INSPECTOR
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using DaftAppleGames.TimeAndWeather.Core;
using DaftAppleGames.TimeAndWeather.Core.Weather;

namespace DaftAppleGames.TimeAndWeather.Editor
{
    /// This class is required in order to force a refresh of the [ShowInInspector] properties that are
    /// shown in the TimeAndWeatherManager inspector. While this isn't great for performance, this is not
    /// included nor executed in a build.
    [CustomEditor(typeof(ExpanseWeatherProvider))]
    #if ODIN_INSPECTOR
    public class ExpanseWeatherProviderEditor : OdinEditor
    #else
        public class TimeAndWeatherManagerEditor : UnityEditor.Editor
    #endif
    {
        public override void OnInspectorGUI()
        {
            ExpanseWeatherProvider timeAndWeatherManager = target as ExpanseWeatherProvider;

            if (timeAndWeatherManager.expanseCloudLayerInterpolator == null)
            {
                EditorGUILayout.HelpBox(
                    "No Cloud Layer Interpolator selected! Drag one in from the Expanse game object in your hierarchy.",
                    MessageType.Error);
            }

            if (timeAndWeatherManager.expanseCreativeFog == null)
            {
                EditorGUILayout.HelpBox(
                    "No Creative Fog component selected! Drag one in from the Expanse game object in your hierarchy.",
                    MessageType.Error);
            }

#if ODIN_INSPECTOR
            DrawDefaultInspector();
#else
            base.OnInspectorGUI();
#endif
        }

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