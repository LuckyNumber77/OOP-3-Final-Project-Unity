using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class DeckAutoFiller
{
    [MenuItem("Tools/Auto-Fill Card Deck")]
    public static void FillDeck()
    {
        // Locate DeckManager in the scene
        DeckManager deckManager = GameObject.FindObjectOfType<DeckManager>();
        if (deckManager == null)
        {
            Debug.LogError("DeckManager not found in the scene.");
            return;
        }

        Undo.RecordObject(deckManager, "Auto-Fill Card Deck");
        deckManager.cardDeck.Clear();

        // Find all Sprites inside the specified folder
        string[] guids = AssetDatabase.FindAssets("t:Sprite", new[] { "Assets/Cards/Playing Cards/Playing Cards/PNG-cards-1.3" });
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);

            // Skip null or duplicate sprite entries
            if (sprite == null || deckManager.cardDeck.Exists(c => c.image == sprite))
                continue;

            DeckManager.CardData card = new DeckManager.CardData
            {
                name = sprite.name,
                image = sprite,
                value = ExtractCardValue(sprite.name)
            };

            deckManager.cardDeck.Add(card);
        }

        EditorUtility.SetDirty(deckManager);
        Debug.Log($"✅ Deck filled with {deckManager.cardDeck.Count} cards.");
    }

    private static int ExtractCardValue(string cardName)
    {
        cardName = cardName.ToLower();

        if (cardName.StartsWith("ace")) return 11;
        if (cardName.StartsWith("king") || cardName.StartsWith("queen") || cardName.StartsWith("jack")) return 10;

        string[] parts = cardName.Split('_');
        if (parts.Length > 0 && int.TryParse(parts[0], out int numericValue))
            return numericValue;

        Debug.LogWarning($"⚠️ Couldn't determine value for card: {cardName}");
        return 0;
    }
}
