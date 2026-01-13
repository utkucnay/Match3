using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TextFileLevelDataProvider : ILevelDataProvider
{
    private const string ResourceBasePath = "Levels";
    private static readonly IReadOnlyDictionary<string, ItemType> ItemMappings =
        new Dictionary<string, ItemType>(StringComparer.OrdinalIgnoreCase)
        {
            ["."] = ItemType.None,
            ["R"] = ItemType.Blast_Red,
            ["B"] = ItemType.Blast_Blue,
            ["G"] = ItemType.Blast_Green,
            ["Y"] = ItemType.Blast_Yellow,
            ["O"] = ItemType.Obstacle_1
        };

    public LevelData LoadLevel(int id)
    {
        return LoadLevel($"level_{id:00}");
    }

    public LevelData LoadLevel(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Level name is required.", nameof(name));
        }

        TextAsset levelAsset = Resources.Load<TextAsset>($"{ResourceBasePath}/{name}");
        if (levelAsset == null)
        {
            throw new FileNotFoundException($"Level file not found at Resources/{ResourceBasePath}/{name}.");
        }

        return ParseLevel(levelAsset.text, name);
    }

    private static LevelData ParseLevel(string rawText, string name)
    {
        if (string.IsNullOrWhiteSpace(rawText))
        {
            throw new InvalidDataException($"Level file '{name}' is empty.");
        }

        List<string> contentLines = GetContentLines(rawText);
        if (contentLines.Count == 0)
        {
            throw new InvalidDataException($"Level file '{name}' has no data.");
        }

        string[] headerTokens = Tokenize(contentLines[0]);
        if (headerTokens.Length < 3)
        {
            throw new InvalidDataException($"Level file '{name}' must start with: <width> <height> <moves>.");
        }

        int width = int.Parse(headerTokens[0]);
        int height = int.Parse(headerTokens[1]);
        int moves = int.Parse(headerTokens[2]);

        if (width <= 0 || height <= 0)
        {
            throw new InvalidDataException($"Level file '{name}' must specify positive width and height.");
        }
        if (contentLines.Count - 1 < height)
        {
            throw new InvalidDataException($"Level file '{name}' does not contain {height} rows.");
        }

        ItemType[] items = new ItemType[width * height];
        bool[] excluded = new bool[width * height];

        for (int row = 0; row < height; row++)
        {
            string[] rowTokens = TokenizeRow(contentLines[row + 1], width);
            if (rowTokens.Length != width)
            {
                throw new InvalidDataException($"Row {row} in level '{name}' must have {width} entries.");
            }

            for (int column = 0; column < width; column++)
            {
                string token = rowTokens[column];
                int index = (row * width) + column;
                if (string.Equals(token, "X", StringComparison.OrdinalIgnoreCase))
                {
                    excluded[index] = true;
                    items[index] = ItemType.None;
                    continue;
                }

                if (!ItemMappings.TryGetValue(token, out ItemType item))
                {
                    throw new InvalidDataException($"Unknown token '{token}' in level '{name}'.");
                }

                items[index] = item;
            }
        }

        return new LevelData(width, height, moves, items, excluded);
    }

    private static List<string> GetContentLines(string rawText)
    {
        List<string> lines = new();
        using StringReader reader = new(rawText);
        string line;
        while ((line = reader.ReadLine()) != null)
        {
            string trimmed = line.Trim();
            if (string.IsNullOrWhiteSpace(trimmed))
            {
                continue;
            }

            if (trimmed.StartsWith("//", StringComparison.Ordinal))
            {
                continue;
            }

            lines.Add(trimmed);
        }

        return lines;
    }

    private static string[] TokenizeRow(string line, int width)
    {
        string[] tokens = Tokenize(line);
        if (tokens.Length == 1 && tokens[0].Length == width)
        {
            string compact = tokens[0];
            string[] expanded = new string[width];
            for (int index = 0; index < width; index++)
            {
                expanded[index] = compact[index].ToString();
            }

            return expanded;
        }

        return tokens;
    }

    private static string[] Tokenize(string line)
    {
        return line.Split(new[] { ' ', '\t', ',' }, StringSplitOptions.RemoveEmptyEntries);
    }
}
