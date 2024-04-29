#if ODIN_INSPECTOR
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using DaftAppleGames.TimeAndWeather.Core;
using DaftAppleGames.TimeAndWeather.Core.TimeOfDay;

namespace DaftAppleGames.TimeAndWeather.Editor
{
    /// This class is required in order to force a refresh of the [ShowInInspector] properties that are
    /// shown in the TimeAndWeatherManager inspector. While this isn't great for performance, this is not
    /// included nor executed in a build.
    [CustomEditor(typeof(ExpanseTimeProvider))]
    #if ODIN_INSPECTOR
    public class ExpanseTimeProviderEditor : OdinEditor
    #else
        public class TimeAndWeatherManagerEditor : UnityEditor.Editor
    #endif
    {
        public override void OnInspectorGUI()
        {
            ExpanseTimeProvider timeProvider = target as ExpanseTimeProvider;

            if (timeProvider.expanseDateTimeController == null)
            {
                EditorGUILayout.HelpBox(
                    "No Date Time Controller component selected! Drag one in from the Expanse game object in your hierarchy.",
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