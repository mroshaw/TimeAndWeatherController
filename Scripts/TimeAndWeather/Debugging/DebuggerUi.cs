using System.Collections;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#else
using DaftAppleGames.Core.MetaAttributes;
#endif
using UnityEngine;
using TMPro;

namespace DaftAppleGames.TimeAndWeather.Debugging
{
    public class DebuggerUi : MonoBehaviour
    {
        [BoxGroup("Settings")] public KeyCode debugToggleModifierKey = KeyCode.LeftControl;
        [BoxGroup("Settings")] public KeyCode debugToggleKey = KeyCode.D;
        [BoxGroup("Settings")] public bool showOnStart;
        [BoxGroup("UI Config")] public GameObject debugCanvas;
        [BoxGroup("Log Config")] public TMP_Text debuggerLogText;
        [BoxGroup("Log Config")] public float logShowDuration = 3.0f;
        [BoxGroup("Log Config")] public float logFadeDuration = 3.0f;
        
        private bool _isUiOpen = false;
        private Coroutine _logFadeCoroutine;
        private bool _logFadeIsRunning = false;

        /// <summary>
        /// Set up the canvas group and show the UI, if selected
        /// </summary>
        public void Start()
        {
            if (showOnStart)
            {
                ShowUi();
            }
            else
            {
                HideUi();
            }
        }

        /// <summary>
        /// Look for debug keycode, keep cursor visible when open
        /// </summary>
        public void Update()
        {
            // Check for keypress combo
            if (Input.GetKey(debugToggleModifierKey) && Input.GetKeyDown(debugToggleKey))
            {
                if (_isUiOpen)
                {
                    HideUi();
                }
                else
                {
                    ShowUi();
                }
            }

            // Keep the cursor visible when Debug UI is open
            if (_isUiOpen)
            {
                ForceCursor();
            }
        }

        /// <summary>
        /// Display the given log text
        /// </summary>
        /// <param name="logText"></param>
        public void ShowLog(string logText)
        {
            debuggerLogText.text = logText;

            // Check if fade already running and if so, kill it
            if (_logFadeIsRunning)
            {
                StopCoroutine(_logFadeCoroutine);
            }
            _logFadeCoroutine = StartCoroutine(FadeLogAsync());
        }

        /// <summary>
        /// Fades out the log text over time
        /// </summary>
        /// <returns></returns>
        private IEnumerator FadeLogAsync()
        {
            _logFadeIsRunning = true;
            debuggerLogText.color = new Color(debuggerLogText.color.r, debuggerLogText.color.g, debuggerLogText.color.b,
                1.0f);

            // Wait while the text is displayed
            float currentTime = 0f;
            while (currentTime < logShowDuration)
            {
                currentTime += Time.deltaTime;
                yield return null;
            }

            // After that, fade out
            currentTime = 0f;
            while (currentTime < logFadeDuration)
            {
                float alpha = Mathf.Lerp(1f, 0f, currentTime / logFadeDuration);
                debuggerLogText.color = new Color(debuggerLogText.color.r, debuggerLogText.color.g, debuggerLogText.color.b, alpha);
                currentTime += Time.deltaTime;
                yield return null;
            }

            _logFadeIsRunning = false;
        }
        
        /// <summary>
        /// Show the Debug UI
        /// </summary>
        public void ShowUi()
        {
            debugCanvas.SetActive(true);
            _isUiOpen = true;
        }

        /// <summary>
        /// Hide the Debug UI
        /// </summary>
        public void HideUi()
        {
            debugCanvas.SetActive(false);
            _isUiOpen = false;
        }

        /// <summary>
        /// Force the cursor to be active and visible
        /// </summary>
        private void ForceCursor()
        {
            if (!Cursor.visible)
            {
                Cursor.visible = true;
            }

            if (Cursor.lockState != CursorLockMode.None)
            {
                Cursor.lockState = CursorLockMode.None;
            }
        }
    }
}