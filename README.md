# Wheel of Fortune – Vertigo Case Study

A compact, production-style implementation of the Vertigo Games “Wheel of Fortune” task.  
The project demonstrates clean architecture, ScriptableObject-driven data, scalable UI, and a fully functional risk-reward loop.

---

## 🎮 Gameplay Summary
- Spin the wheel to earn rewards; hitting a bomb wipes everything.  
- Zones progress automatically after each spin.  
- **Safe Zones (every 5th):** Silver Spin, no bomb, boosted rewards.  
- **Super Zones (every 30th):** Gold Spin, no bomb, higher boost.  
- Player can exit only when the wheel is idle.

---

## 🧱 Architecture

### Core Managers
- **GameManager** — main flow (spin, reward, bomb handling, zone progression).  
- **ZoneManager** — loads/sorts ZoneConfig, tracks zone types, handles bomb removal.  
- **RewardManager** — stores totals and formats final results.  
- **WheelLogic** — pure spin calculation; UI-independent.

### Data (ScriptableObjects)
- `ZoneConfig` + `ZoneSlice`  
- `WheelItemData` (rewards/bomb)  
- `WheelSkinData`  
- `RewardUIMap`

---

## 🖥 UI Systems
- **UIManager** — updates UI, handles buttons, plays VFX.  
- **RewardsUI** — efficient incremental updates; no destroy/rebuild spam.  
- **RewardsUIItem** — animated count-up; bump effect on reward.  
- **ZoneProgressUI** — animated sliding strip of zone numbers.  
- **BombPopupUI** — DOTween intro/outro.

---

## 🛠 Designing Wheel Content (Important)
Wheel contents are **fully editable in the Unity Editor**, both manually and procedurally:

- Each `ZoneConfig` asset represents a complete wheel layout.  
- Designers can **assign any WheelItemData** to any slice.  
- Rewards can be **auto-generated** by the Zone Generator tool or **hand-crafted** per zone.  
- Slice reward amounts can be overridden individually (e.g., per-zone scaling, item promotions).  
- Changing wheel structure requires **no code changes** — the system reads all data from ScriptableObjects.

This makes the wheel system flexible and production-ready for live-ops, A/B tests, and frequent balancing changes.

---

## ❗ Notes on Design Decisions

### 1. End-of-Progress Handling  
The current implementation does not include a special flow for when the player reaches the final zone.  
This was a deliberate decision: the demo’s primary purpose is to demonstrate the mechanic loop (spin → reward → risk → zone progression), not long-form progression termination.  
Adding an end-state is straightforward, but outside the scope of what needed to be showcased for this case study.

### 2. Bomb “Pay to Continue” Economy  
The system includes a spending mechanism inside `RewardManager`, but the demo does not connect it to a dedicated monetization UI or deduct-reward flow.  
The continue/quit buttons function correctly, and adding a subtraction step would not meaningfully enrich the demonstration—it would simply remove a portion of accumulated rewards.
Because the assignment focuses on architecture, UI clarity, data-driven wheels, and the risk/reward loop, this part was intentionally kept minimal.

---

## ⚙ Wheel & VFX
- DOTween rotation with overspin + final alignment.  
- VFX icons burst from winning slice and fly to inventory.  
- Arrival triggers bump + animated amount change.

---

## 🛠 Editor Tools
- **Reward Icon Map Builder** — auto-fills `RewardUIMap`.  
- **Zone Generator Window** — generates Safe/Super zones with scaling rewards.

---

## 📦 Build
APK included under GitHub Releases.

---
