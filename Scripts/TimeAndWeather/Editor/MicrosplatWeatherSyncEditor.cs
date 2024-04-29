#if __MICROSPLAT__
using DaftAppleGames.TimeAndWeather.Core.Weather;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector.Editor;
#endif
using UnityEditor;

namespace DaftAppleGames.TimeAndWeather.Editor
{
    [CustomEditor(typeof(MicrosplatWeatherSync))]
#if ODIN_INSPECTOR
    public class MicrosplatWeatherSyncEditor : OdinEditor
#else
    public class MicrosplatWeatherSyncEditor : UnityEditor.Editor
#endif
    {
        public override void OnInspectorGUI()
        {
#if !__MICROSPLAT_SNOW__
            EditorGUILayout.HelpBox("WARNING: MicroSplat Snow module is not installed. Snow sync will have no effect.", MessageType.Warning);
#endif
#if !__MICROSPLAT_STREAMS__
            EditorGUILayout.HelpBox("WARNING: MicroSplat Streams and Water module is not installed. Wetness sync will have no effect.", MessageType.Warning);
#endif
#if !__MICROSPLAT_WINDGLITTER__
            EditorGUILayout.HelpBox("WARNING: MicroSplat Wind and Glitter module is not installed. Wind sync will have no effect.", MessageType.Warning);
#endif

#if ODIN_INSPECTOR
            DrawDefaultInspector();
#else
            base.OnInspectorGUI();
#endif
        }
    }
}
#endif