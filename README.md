# Memento Erdex

[![Build Status](https://img.shields.io/badge/build-passing-brightgreen)](#)
[![Unity Version](https://img.shields.io/badge/unity-6.3%20LTS-blue)](#)
[![License](https://img.shields.io/badge/license-MIT-lightgrey)](#)
[![Platform](https://img.shields.io/badge/platform-Windows-orange)](#)
[![Development Stage](https://img.shields.io/badge/stage-alpha-yellow)](#)

## Description
**Memento Erdex** is an atmospheric 2D climbing simulator. The project combines a sophisticated procedural world-generation system with timing-based jump mechanics that reward precision and focus. Unique hazards and dynamic environmental conditions ensure that each run feels distinct

## Key Features
- **Procedural World Generation:** A seed-based recursive tree-generation algorithm designed to produce an effectively endless variety of routes
- **Precision Mechanics:** A flexible jump system that emphasizes concentration, timing, and execution
- **Visual Consistency:** Custom shaders that establish and preserve a unique visual identity
- **Performance Optimization:** Extensive use of object pooling to keep procedural parts streaming smooth and stable

## Technical Stack
- **Engine:** Unity 6.3 LTS
- **Language:** C#
- **Rendering:** Universal Render Pipeline (URP), custom shaders
- **Core Systems:** Procedural generation, object pooling, Unity Input System

## Demo
Try the latest demo build on itch.io:
- **Download:** [Demo from itch.io](https://another-living-worlds.itch.io/memento-erdex)

## Quick Start

### Prerequisites
To work with this project in Unity, you need:
- **Unity Editor:** **6.3 LTS** or newer

### Installation and Setup
1. **Clone the repository**

   ```bash
   git lfs install
   git clone https://github.com/username/memento-erdex.git
   ```

2. **Add the project to Unity Hub**
   - Open Unity Hub
   - Click **Add** and select the cloned project folder
   - Confirm that the selected editor version is **6.3 LTS**

3. **First launch**
   - On first open, Unity will generate the `Library` folder. This can take a few minutes
   - Open the startup scene at `Assets/Scenes/SampleScene.unity`

## Development Stage
Current status: **Alpha**

- **Done:**
  - ~80% of procedural tree generation
  - Outline shader system
  - Tooling for adding new tree parts
- **In Progress:**
  - Player movement
  - Tree attribute system and subsystem characteristics
- **Planned:**
  - Complete a full playable demo of the first tree stage

## Documentation
Detailed architecture documentation is currently in progress and will be added in upcoming updates

## AI Acknowledgment
- **GitHub Copilot:** Used for code suggestions in scripts and for writing some code comments
- **Gemini:** Used for information gathering and quick reference research
- **ChatGPT:** Used to edit and refine README documentation structure and wording

## Author
- **Lead Developer:** Nik Romaniuk
- **Contact:** expifel@gmail.com

## License
This project is distributed under the **MIT License**
