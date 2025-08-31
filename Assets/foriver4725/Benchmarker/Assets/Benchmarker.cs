using System;
using System.Runtime.CompilerServices;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Profiling;
using TMPro;
using Cysharp.Text;

namespace foriver4725.Benchmarker
{
    internal sealed class Benchmarker : MonoBehaviour
    {
        [SerializeField] private GameObject root;
        [SerializeField] private TextMeshProUGUI text;

#if UNITY_EDITOR || DEVELOPMENT_BUILD

#if DEVELOPMENT_BUILD
        // Since the garbage collection ends up being counted when the initial scene load doesn’t finish in time, wait a bit at the beginning.
        private static readonly int OnFirstSceneIgnoreFrames = 16;
        private static int onFirstSceneFrames = 0;
        private static bool doEnableOnFirstScene = false;
#endif

        // Measure at fixed intervals.
        private float time = 0f;
        private float baseTime = 0f;

        private int fpsCount = 0;
        private float fps = -1f;

        private float allocatedMemory = -1f;
        private float reservedMemory = -1f;
        private float unusedReservedMemory = -1f;
        private float memoryUsingRate = -1f;

        private int gcCountInit = -1;
        private int gcCount = -1;

        private ProfilerRecorder setPassCallsRecorder;
        private ProfilerRecorder drawCallsRecorder;
        private long setPassCalls = -1;
        private long drawCalls = -1;

        // HTML color codes for green, yellow, and red.
        private static readonly string[] ColorHtmlTexts = new string[3]
        {
            Color.green.ToHtmlStringRGB(),
            Color.yellow.ToHtmlStringRGB(),
            Color.red.ToHtmlStringRGB()
        };
        private static readonly int ColorGreen = 0;
        private static readonly int ColorYellow = 1;
        private static readonly int ColorRed = 2;

        private void Awake()
        {
            setPassCallsRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "SetPass Calls Count");
            drawCallsRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Draw Calls Count");
        }

        private void OnDestroy()
        {
            setPassCallsRecorder.Dispose();
            drawCallsRecorder.Dispose();
        }

        private void Update()
        {
#if DEVELOPMENT_BUILD
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == 0)
            {
                if (!doEnableOnFirstScene)
                {
                    onFirstSceneFrames++;
                    if (onFirstSceneFrames > OnFirstSceneIgnoreFrames)
                        doEnableOnFirstScene = true;
                    else
                        return;
                }
            }
#endif

            fpsCount++;
            time = Time.realtimeSinceStartup - baseTime;
            if (time >= 0.5f)
            {
                UpdateStats();
                UpdateUI();
            }
        }

        private void UpdateStats()
        {
            fps = fpsCount / time;
            fpsCount = 0;
            baseTime = Time.realtimeSinceStartup;

            allocatedMemory = Profiler.GetTotalAllocatedMemoryLong().ByteToMegabyte();
            reservedMemory = Profiler.GetTotalReservedMemoryLong().ByteToMegabyte();
            unusedReservedMemory = Profiler.GetTotalUnusedReservedMemoryLong().ByteToMegabyte();
            memoryUsingRate = allocatedMemory / reservedMemory;

            if (gcCountInit == -1)
                gcCountInit = GC.CollectionCount(0);
            gcCount = GC.CollectionCount(0) - gcCountInit;

            if (setPassCallsRecorder.Valid)
            {
                long lastValue = setPassCallsRecorder.LastValue;
                if (lastValue > 0)
                    setPassCalls = lastValue;
            }
            if (drawCallsRecorder.Valid)
            {
                long lastValue = drawCallsRecorder.LastValue;
                if (lastValue > 0)
                    drawCalls = lastValue;
            }
        }

        private void UpdateUI()
        {
            if (text == null) return;

            string fpsColorText = ColorHtmlTexts[fps switch
            {
                > 54 => ColorGreen,
                > 42 => ColorYellow,
                _ => ColorRed
            }];
            string memoryUsingColorText = ColorHtmlTexts[allocatedMemory switch
            {
                < 800 => ColorGreen,
                < 1200 => ColorYellow,
                _ => ColorRed
            }];
            string gcCountColorText = ColorHtmlTexts[gcCount switch
            {
                0 => ColorGreen,
                < 4 => ColorYellow,
                _ => ColorRed
            }];
            string setPassCallsColorText = ColorHtmlTexts[setPassCalls switch
            {
                < 80 => ColorGreen,
                < 120 => ColorYellow,
                _ => ColorRed
            }];
            string drawCallsColorText = ColorHtmlTexts[drawCalls switch
            {
                < 120 => ColorGreen,
                < 180 => ColorYellow,
                _ => ColorRed
            }];

            using var sb = ZString.CreateStringBuilder();
            sb.AppendFormat("FPS : <color=#{0}>{1:F2}</color>,    ",
                fpsColorText, fps);
            sb.AppendFormat("Memory(MB) : <color=#{0}>{1:F2}/{2:F2} ({3:P2}, {4:F2} unused)</color>,    ",
                memoryUsingColorText, allocatedMemory, reservedMemory, memoryUsingRate, unusedReservedMemory);
            sb.AppendFormat("GC.Collect : <color=#{0}>{1}</color>,    ",
                gcCountColorText, gcCount);
            sb.AppendFormat("Calls : SetPass=<color=#{0}>{2}</color>,Draw=<color=#{1}>{3}</color>,    ",
                setPassCallsColorText, drawCallsColorText, setPassCalls, drawCalls);
            text.SetText(sb);
        }

#else
        private void Awake()
        {
            root.SetActive(false);
            this.enabled = false;
        }
#endif
    }

    internal static class BenchmarkerEx
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static float ByteToMegabyte(this long n) => (n >> 10) / 1024f;

        // Extension methods for ColorUtility.ToHtmlString()-style methods that replace the internal processing with ZString.

        // Returns the color as a hexadecimal string in the format "RRGGBB".
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string ToHtmlStringRGB(this Color color)
        {
            Color32 color2 = new Color32((byte)Mathf.Clamp(Mathf.RoundToInt(color.r * 255f), 0, 255), (byte)Mathf.Clamp(Mathf.RoundToInt(color.g * 255f), 0, 255), (byte)Mathf.Clamp(Mathf.RoundToInt(color.b * 255f), 0, 255), 1);
            return ZString.Format("{0:X2}{1:X2}{2:X2}", color2.r, color2.g, color2.b);
        }
        // Returns the color as a hexadecimal string in the format "RRGGBBAA".
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string ToHtmlStringRGBA(this Color color)
        {
            Color32 color2 = new Color32((byte)Mathf.Clamp(Mathf.RoundToInt(color.r * 255f), 0, 255), (byte)Mathf.Clamp(Mathf.RoundToInt(color.g * 255f), 0, 255), (byte)Mathf.Clamp(Mathf.RoundToInt(color.b * 255f), 0, 255), (byte)Mathf.Clamp(Mathf.RoundToInt(color.a * 255f), 0, 255));
            return ZString.Format("{0:X2}{1:X2}{2:X2}{3:X2}", color2.r, color2.g, color2.b, color2.a);
        }
    }
}
