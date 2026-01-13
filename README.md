# Match3 - The "Worst" Match3 Game 🎮

> "En kötü match3 yaparız" - We'll make the worst match3
> 
> Mission accomplished! ✅

## Overview

This is a minimal, functional Match3 game implementation in Unity. While ironically calling itself "the worst," it actually contains a clean, well-tested core game logic following Match3 game mechanics.

## Features

### Core Mechanics
- **Grid-based board system** with configurable width and height
- **Item swapping** mechanism between adjacent cells
- **Match detection** for horizontal and vertical matches (minimum 3)
- **Special items** that can be created from larger matches
- **Multiple item types**:
  - Blast items (Red, Blue, Green, Yellow)
  - Special items (Types 1, 2, 3)
  - Obstacles

### Game Rules

1. **Basic Match**: Match 3 or more items of the same color in a row (horizontal or vertical)
2. **Vertical Blast**: Match 3+ items vertically
3. **Horizontal Blast**: Match 3+ items horizontally
4. **Special Blast**: Match items in both directions to create special items
5. **Invalid Moves**: Moves that don't result in a match are reverted

## Architecture

### Core Components

```
Assets/
├── Module/
│   ├── Game/
│   │   └── Core/
│   │       ├── Board/         # Board management logic
│   │       │   ├── Board.cs
│   │       │   ├── BoardCell.cs
│   │       │   └── BoardUpdateResult.cs
│   │       ├── Item/          # Item types and definitions
│   │       │   ├── Item.cs
│   │       │   └── ItemType.cs
│   │       ├── Level/         # Level configuration
│   │       └── Util/          # Game utilities
│   │           └── BlastRule.cs
│   └── Util/
│       └── Enum/
│           └── Direction.cs   # Movement directions
└── Test/
    └── BoardTest/             # Unit tests
        └── Editor/
            └── BlastRuleTests.cs
```

### Key Classes

#### `Board`
The main game board class that handles:
- Board initialization with configurable dimensions
- Item swapping and validation
- Match detection using blast rules
- Board state updates

#### `Item`
Represents game items with:
- `ItemType`: The type of item (Blast, Special, Obstacle)
- `health`: Item durability

#### `BlastRule`
Defines matching rules:
- Direction requirements
- Minimum match sizes
- Priority ordering
- Special item generation

## Usage Example

```csharp
// Create a 5x5 board
var boardConfig = new BoardConfig 
{ 
    width = 5, 
    height = 5 
};

var board = new Board(boardConfig);

// Make a move (swap cell at index with direction)
var random = new Random(seed);
var result = board.OnBoardUpdate(cellIndex, Direction.Right, ref random);

// Check if match occurred
if (!result.isReturn)
{
    // Match found! Items were blasted
    Debug.Log($"Match type: {result.blastConditionDebugName}");
    Debug.Log($"Blasted tiles: {result.blastedTileIndexes.Length}");
}
else
{
    // No match - move was reverted
    Debug.Log("No match found, move reverted");
}
```

## Item Types

### Blast Items (ItemType.Blast)
Regular matchable items that can be destroyed:
- `Blast_Red` (17)
- `Blast_Blue` (18)
- `Blast_Green` (20)
- `Blast_Yellow` (21)

### Special Items (ItemType.Special)
Created from larger matches:
- `Special_1` (33)
- `Special_2` (34)
- `Special_3` (35)

### Obstacles (ItemType.Obstacle)
Non-matchable blocking items:
- `Obstacle_1` (65)

## Testing

The project includes comprehensive unit tests covering:
- Vertical blast detection
- Horizontal blast detection
- Match validation
- Move reversal on invalid matches
- Item swapping logic

Run tests in Unity Test Runner:
1. Open Unity
2. Go to Window > General > Test Runner
3. Select EditMode
4. Click "Run All"

## Development

### Requirements
- Unity 6000.3.2f1 or compatible
- .NET compatible with Unity

### Building
This is a Unity project. Open it in Unity Editor to build:
1. Open Unity Hub
2. Add project from disk
3. Open the project
4. Build via File > Build Settings

## Why "The Worst"?

Despite the ironic title, this Match3 implementation demonstrates:
- ✅ Clean separation of concerns
- ✅ Testable architecture
- ✅ Efficient match detection algorithms
- ✅ Extensible blast rule system
- ✅ Type-safe item management

It's not the worst—it's actually quite decent! 😄

## License

See [LICENSE](LICENSE) file for details.

## Contributing

Feel free to make it even "worse" (better) by:
1. Adding more special item types
2. Implementing gravity and cascading
3. Adding power-ups and combos
4. Creating UI and visual effects
5. Adding sound effects and animations

---

*"En iyi senaryo minecraft yaparız"* - Best case scenario, we make Minecraft next! 🎮⛏️
