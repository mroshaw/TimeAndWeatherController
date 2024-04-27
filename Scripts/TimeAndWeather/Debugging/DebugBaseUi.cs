#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#else
using DaftAppleGames.Core.MetaAttributes;
#endif
using UnityEngine;

namespace DaftAppleGames.TimeAndWeather.Debugging
{
    public abstract class DebugBaseUi : MonoBehaviour
    {

        [BoxGroup("Settings")] public DebuggerUi debugManagerUi;

        public virtual void Start()
        {
        }

        /// <summary>
        /// Show the given string in the log console
        /// </summary>
        /// <param name="logText"></param>
        public void ShowLog(string logText)
        {
            debugManagerUi.ShowLog(logText);
        }

        /// <summary>
        /// Find Debug objects given type and name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectName"></param>
        /// <returns></returns>
        public DebugBase FindDebugObject<T>(string objectName)
        {
            DebugBase[] allDebugObjects = FindObjectsOfType<DebugBase>();

            foreach (DebugBase debugObject in allDebugObjects)
            {
                if (debugObject.gameObject.name == objectName && debugObject is T)
                {
                    return debugObject;
                }
            }
            Debug.LogError($"DebugBaseUi: could not find object named {objectName} in loaded scenes! Please check: {gameObject.name}!");
            return null;
        }
    }
}