# Benchmarker

<img width="2214" height="60" alt="Benchmarker_using" src="https://github.com/user-attachments/assets/d044ddad-da56-41e1-a2ef-d7d64ce5627a" />

## Description
This tool displays FPS, memory usage, garbage collection counts, and draw calls directly on the game screen as benchmarks.<br/>
It is designed for high performance with zero additional GC.Alloc, ensuring smooth operation even in Development Builds.<br/>
In production builds, it will always be disabled automatically.<br/>

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
```
