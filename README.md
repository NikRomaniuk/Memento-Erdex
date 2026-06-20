# Memento Erdex

[![Unity Version](https://img.shields.io/badge/Unity-6_(LTS)-blue?style=flat-square)](https://unity.com/)
[![Platform](https://img.shields.io/badge/Platform-WebGL_/_Windows-orange?style=flat-square)](#)
[![Development Stage](https://img.shields.io/badge/Stage-Alpha-yellow?style=flat-square)](#)
[![License](https://img.shields.io/badge/License-MIT-lightgrey?style=flat-square)](#)

## ![Description icon](https://api.iconify.design/heroicons/ellipsis-horizontal-circle.svg?color=%23ffffff&width=22&height=22) Description
**Memento Erdex** is an atmospheric 2D climbing simulator. The project combines a sophisticated procedural world-generation system with timing-based jump mechanics that reward precision and focus. Unique hazards and dynamic environmental conditions ensure that each run feels distinct

## ![Demo icon](https://api.iconify.design/heroicons/play-circle.svg?color=%23ffffff&width=22&height=22) Demo
Try the latest demo build on itch.io:
[![Play on Itch.io](https://img.shields.io/badge/Play_on-Itch.io-FA5C5C?style=flat-square&logo=itch.io)](https://another-living-worlds.itch.io/memento-erdex)

---

## ![Features icon](https://api.iconify.design/heroicons/sparkles.svg?color=%23ffffff&width=22&height=22) Key Features
- **Procedural World Generation:** A seed-based recursive tree-generation algorithm designed to produce an effectively endless variety of routes
- **Visual Consistency:** Custom shaders that establish and preserve a unique visual identity ("Soft" Pixel-art)
- **Performance Optimization:** Extensive use of object pooling to keep procedural parts streaming smooth and stable
- **Player Controls:** A responsive jump and movement with smooth acceleration

## ![Tech icon](https://api.iconify.design/heroicons/code-bracket.svg?color=%23ffffff&width=22&height=22) Technical Stack
- **Language:** C#
- **Engine:** Unity 6 (LTS)
- **Design Patterns**: Event-Driven Architecture (EDA), Object Pooling, Component-Based Design, Deterministic Algorithms
- **Graphics Pipeline:** Universal Render Pipeline (URP), Custom HLSL/ShaderGraph shaders

---

## ![Start icon](https://api.iconify.design/heroicons/rocket-launch.svg?color=%23ffffff&width=22&height=22) Quick Start

### Prerequisites
To work with this project, ensure you have the following installed:
- **Unity Editor:** **Unity 6 (LTS)** or newer
- **Git Large File Storage (LFS):** Required for project assets

### Installation and Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/NikRomaniuk/Memento-Erdex.git
   cd Memento-Erdex
   git lfs pull
   ```
2. **Add the project to Unity Hub**
   - Open Unity Hub
   - Click **Add project from disk** and select the cloned project folder
   - Verify that the project opens with **Unity 6 (LTS)**
3. **First launch**
   - Upon the first launch, Unity will generate the local `Library` folder (this process may take a few minutes)
   - Open and run the startup scene at `Assets/_Project/Scenes/Bootstrap.unity` to ensure all systems initialize correctly

---

## ![Stage icon](https://api.iconify.design/heroicons/wrench-screwdriver.svg?color=%23ffffff&width=22&height=22) Development Stage
Current status: **Alpha**

- **Done:**
  - Procedural world-generation (Slightly configurable)
  - Basic player movement (Movement & Casual jump)
  - Custom shader system (Soft pixels/Outline)
  - Internal content automation pipeline (Data serialization tools for asset baking)
- **In Progress:**
  - Tree's Local area attribute system
- **Planned:**
  - Complete a full playable demo of the first tree stage (Level)

---

## ![Documentation icon](https://api.iconify.design/heroicons/book-open.svg?color=%23ffffff&width=22&height=22) Documentation
Detailed architecture documentation is currently in progress and will be added in upcoming updates

---

## ![AI icon](https://api.iconify.design/heroicons/information-circle.svg?color=%23ffffff&width=22&height=22) AI Acknowledgment
- **GitHub Copilot:** Used for code suggestions in scripts and for writing some code comments
- **Gemini:** Used for information gathering and quick reference research
- **ChatGPT:** Used to edit and refine README documentation structure and wording

---

## ![About Me icon](https://api.iconify.design/heroicons/user.svg?color=%23ffffff&width=22&height=22) Author
- **Developer:** Nik Romaniuk

## ![Contact icon](https://api.iconify.design/heroicons/chat-bubble-left-right.svg?color=%23ffffff&width=22&height=22) Contact Me
<p>
  <a href="https://www.linkedin.com/in/nik-romaniuk"><img src="https://img.shields.io/badge/LinkedIn-Nik%20Romaniuk-0A66C2?style=flat-square&logo=linkedin&logoColor=white" alt="LinkedIn"></a><br>
  <a href="mailto:g00468889@atu.ie"><img src="https://img.shields.io/badge/Student%20Email-g00468889%40atu.ie-0F8F8F?style=flat-square&logo=gmail&logoColor=white" alt="Student Email"></a><br>
  <a href="mailto:expifel@gmail.com"><img src="https://img.shields.io/badge/Personal%20Email-expifel%40gmail.com-8B6BCF?style=flat-square&logo=gmail&logoColor=white" alt="Personal Email"></a>
</p>

---

## ![License icon](https://api.iconify.design/heroicons/scale.svg?color=%23ffffff&width=22&height=22) License
This project is distributed under the **MIT License**
