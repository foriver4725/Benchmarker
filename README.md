# Benchmarker

<img width="1283" height="29" alt="image" src="https://github.com/user-attachments/assets/e634d0c7-b3c4-405b-bcac-599bc39f2119" />

## Description
This tool displays benchmark information directly on the game screen, including CPU frame time, memory usage, per-frame GC.Alloc, total GC.Collect count, SetPass calls, and Draw calls.<br/>
CPU frame time is supported only when VSync is disabled, and Per-frame GC.Alloc is supported only in Mono builds.<br/>
It is designed for high performance with zero additional GC.Alloc, ensuring smooth operation even in Development Builds.<br/>
In production builds, it will always be disabled automatically.<br/>

<img width="1596" height="212" alt="Benchmarker_profiling" src="https://github.com/user-attachments/assets/acad94b5-cb26-41b8-87c4-35b31c6ad739" />

## How to Use

### 1. Install the dependent libraries
First, you need to install the libraries used by this feature.<br/>
- Unity Profiling Core API
- TextMeshPro
- [ZString](https://github.com/Cysharp/ZString)

### 2. Import the asset package
Download the asset package from [the latest release](https://github.com/foriver4725/Benchmarker/releases) and import it into your project.<br/>

## Supplement
- If you want to check profiling during a build, enable `Development Build`.<br/>
- The colors of the profiled values are determined based on threshold values.<br/>
  Depending on the game you are creating, you may want to adjust these thresholds.<br/>
  If so, check the [`UpdateUI()`](https://github.com/foriver4725/Benchmarker/blob/main/Assets/foriver4725/Benchmarker/Assets/Benchmarker.cs#L122) method in the script and modify the values accordingly.<br/>

```cs
string frameTimeColorText = ColorHtmlTexts[frameTime switch
{
    < 21.0f => ColorGreen,
    < 28.0f => ColorYellow,
    _       => ColorRed
}];
string memoryUsingColorText = ColorHtmlTexts[allocatedMemory switch
{
    < 800  => ColorGreen,
    < 1200 => ColorYellow,
    _      => ColorRed
}];
string gcAllocatedInFrameColorText = ColorHtmlTexts[gcAllocatedInFrame switch
{
    < 15.0f => ColorGreen,
    < 20.0f => ColorYellow,
    _       => ColorRed
}];
string gcCountColorText = ColorHtmlTexts[gcCount switch
{
    0   => ColorGreen,
    < 4 => ColorYellow,
    _   => ColorRed
}];
string setPassCallsColorText = ColorHtmlTexts[setPassCalls switch
{
    < 80  => ColorGreen,
    < 120 => ColorYellow,
    _     => ColorRed
}];
string drawCallsColorText = ColorHtmlTexts[drawCalls switch
{
    < 120 => ColorGreen,
    < 180 => ColorYellow,
    _     => ColorRed
}];
```
