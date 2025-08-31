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
