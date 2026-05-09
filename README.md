# Custom Encoder / Decoder

A .NET 10 console application that encodes and decodes text using two custom alphabet-based cipher techniques, individually or combined. Logic is build based on India Competitive Exam

---

## Table of Contents

- [User Guide](#user-guide)
  - [Running the App](#running-the-app)
  - [Menu Options](#menu-options)
  - [Examples](#examples)
- [Developer Guide](#developer-guide)
  - [Project Structure](#project-structure)
  - [How the Ciphers Work](#how-the-ciphers-work)
    - [1. Reverse Pair](#1-reverse-pair)
    - [2. Forward-Backward](#2-forward-backward)
    - [3. Both Combined](#3-both-combined)
  - [API Reference](#api-reference)
  - [Key Design Decisions](#key-design-decisions)

---

## User Guide

### Running the App

```bash
dotnet run --project CompetitveExam
```

### Menu Options

When launched, you will see:

```
Welcome to the Custom Encoder/Decoder!

0. Exit
1. Reverse Pair      (e.g. HELLO is SVOOL and Hello is Svool)
2. Forward-Backward  (e.g. HELLO is JDNKQ and Hello is Jgnnq)
3. Both #1 and #2    (e.g. HELLO is UUQNN and Hello is Uuqnn)

Select option (0/1/2/3):
```

| Option | Description |
|--------|-------------|
| `0` | Exit the application |
| `1` | Encode / decode using the **Reverse Pair** cipher |
| `2` | Encode / decode using the **Forward-Backward** cipher |
| `3` | Encode / decode using **both** ciphers applied in sequence |

- Select an option, then type your text and press **Enter**
- The app prints the **encoded** text, then immediately **decodes** it back to verify
- Press any key to return to the main menu
- Entering an invalid option shows an error and loops back to the menu

### Examples

| Input | Option 1 — Reverse Pair | Option 2 — Forward-Backward | Option 3 — Both |
|-------|------------------------|-----------------------------|-----------------|
| `HELLO` | `SVOOL` | `JDNKQ` | `UUQNN` |
| `Hello` | `Svool` | `Jgnnq` | `Uuqnn` |
| `ABC` | `ZYX` | `CDB` | `VWU` |

> **Note:** Non-letter characters (spaces, digits, punctuation) are passed through unchanged in all modes.

---

## Developer Guide

### Project Structure

```
CompetitveExam/
├── Program.cs      # Entry point (menu loop) + CustomEncoder class (all cipher logic)
├── README.md       # This file
└── CompetitveExam.csproj
```

> All logic lives in a single `Program.cs` file. `Program` handles the console UI loop and `CustomEncoder` (defined in the same file) contains the encoding/decoding methods.

---

### How the Ciphers Work

#### 1. Reverse Pair

Each letter is mapped to its **mirror position** in the alphabet. Case is preserved.

```
A ↔ Z,  B ↔ Y,  C ↔ X,  …  M ↔ N
a ↔ z,  b ↔ y,  c ↔ x,  …  m ↔ n
```

**Formula:** `encoded = start + (25 - position)`

where `position` is the 0-based index of the letter in its case range (`A`/`a` = 0, `B`/`b` = 1, …).

Step-by-step for `HELLO`:
```
H (pos 7)  → S (25 - 7  = 18)
E (pos 4)  → V (25 - 4  = 21)
L (pos 11) → O (25 - 11 = 14)
L (pos 11) → O (25 - 11 = 14)
O (pos 14) → L (25 - 14 = 11)
HELLO → SVOOL
```

This cipher is **self-inverse** — applying it twice always restores the original text, so `DecodeReversePair` simply calls `EncodeReversePair`.

---

#### 2. Forward-Backward

Each letter is shifted within its alphabet range (`A–Z` or `a–z`) by an amount that **alternates based on the character's 0-based index**:

| Index parity | Encode shift | Decode shift |
|-------------|-------------|-------------|
| Even (0, 2, 4, …) | **+2** (forward) | **−2** |
| Odd  (1, 3, 5, …) | **−1** (backward) | **+1** |

Shifts wrap around using modular arithmetic (`% 26`), and case is preserved.

Step-by-step for `HELLO`:
```
H (index 0, +2) → J
E (index 1, -1) → D
L (index 2, +2) → N
L (index 3, -1) → K
O (index 4, +2) → Q
HELLO → JDNKQ
```

---

#### 3. Both Combined

Applies the two ciphers **in sequence**:

| Direction | Step 1 | Step 2 |
|-----------|--------|--------|
| **Encode** | `EncodeReversePair` | `EncodeForwardBackward` |
| **Decode** | `DecodeForwardBackward` | `DecodeReversePair` |

Decode order is the exact **reverse** of encode order to correctly restore the original.

```
HELLO
  → (ReversePair)     → SVOOL
  → (ForwardBackward) → UUQNN   ← encoded result

UUQNN
  → (DecodeForwardBackward) → SVOOL
  → (DecodeReversePair)     → HELLO  ← restored
```

---

### API Reference

All methods are `public static` on the `CustomEncoder` class in `Program.cs`.

```csharp
// Option 1 — Reverse Pair
string CustomEncoder.EncodeReversePair(string input);
string CustomEncoder.DecodeReversePair(string input);   // self-inverse; calls EncodeReversePair

// Option 2 — Forward-Backward
string CustomEncoder.EncodeForwardBackward(string input);
string CustomEncoder.DecodeForwardBackward(string input);

// Option 3 — Both combined (ReversePair → ForwardBackward)
string CustomEncoder.EncodeBoth(string input);
string CustomEncoder.DecodeBoth(string input);
```

**All methods guarantee:**
- Accept any string (including empty strings)
- Non-letter characters (`0–9`, spaces, punctuation) are passed through unchanged
- Original letter casing (upper / lower) is always preserved
- Stateless and thread-safe (no shared mutable state)

---

### Key Design Decisions

| Decision | Reason |
|----------|--------|
| `DecodeReversePair` reuses `EncodeReversePair` | Mirroring is mathematically self-inverse — a separate implementation would be identical code |
| `EncodeBoth` applies `ReversePair` before `ForwardBackward` | `ForwardBackward` operates on positions, so the mirror substitution must happen first; swapping the order produces a different (non-decodable) result |
| Shifts use `(pos + shift + 26) % 26` | Adding 26 before mod ensures negative shifts never produce a negative remainder in any runtime |
| All logic in `Program.cs` | Single-file simplicity for a small console project; no external dependencies beyond `System.Text` |
