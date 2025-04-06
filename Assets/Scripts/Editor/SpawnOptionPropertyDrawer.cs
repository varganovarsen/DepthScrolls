using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Collections.Generic;

[CustomPropertyDrawer(typeof(SpawnOption))]
public class SpawnOptionPropertyDrawer : PropertyDrawer
{
    private bool expanded = false;
    private const float SPACING = 5f;
    
    // Paths to your prefab folders - adjust these to match your project structure
    private const string ROCKS_FOLDER_PATH = "Resources/Prefabs/Rocks";
    private const string ROCK_FILLINGS_FOLDER_PATH = "Resources/Prefabs/RockFillings";
    
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (!expanded)
            return EditorGUIUtility.singleLineHeight;
            
        float height = EditorGUIUtility.singleLineHeight; // For foldout
        
        // Basic properties
        height += (EditorGUIUtility.singleLineHeight + SPACING) * 3;
        
        // Headers
        height += (EditorGUIUtility.singleLineHeight + SPACING) * 2;
        
        // Rock prefabs array
        SerializedProperty rockPrefabsProperty = property.FindPropertyRelative("rockPrefabs");
        height += EditorGUIUtility.singleLineHeight + SPACING; // Array size field
        height += (EditorGUIUtility.singleLineHeight * 2 + SPACING * 2) * rockPrefabsProperty.arraySize;
        
        // Rock filling prefabs array
        SerializedProperty rockFillingPrefabsProperty = property.FindPropertyRelative("rockFillingPrefabs");
        height += EditorGUIUtility.singleLineHeight + SPACING; // Array size field
        height += (EditorGUIUtility.singleLineHeight * 2 + SPACING * 2) * rockFillingPrefabsProperty.arraySize;
        
        height += SPACING * 2; // Some extra padding
        
        return height;
    }
    
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        
        // Foldout header
        Rect foldoutRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        expanded = EditorGUI.Foldout(foldoutRect, expanded, label, true);
        
        if (expanded)
        {
            EditorGUI.indentLevel++;
            
            float yPos = position.y + EditorGUIUtility.singleLineHeight + SPACING;
            
            // Draw basic properties
            SerializedProperty minDepthProperty = property.FindPropertyRelative("minDepth");
            SerializedProperty spawnEveryProperty = property.FindPropertyRelative("newSpawnEveryXMeters");
            SerializedProperty spawnAmountProperty = property.FindPropertyRelative("spawnAmount");
            
            Rect basicRect = new Rect(position.x, yPos, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(basicRect, minDepthProperty);
            
            yPos += EditorGUIUtility.singleLineHeight + SPACING;
            basicRect.y = yPos;
            EditorGUI.PropertyField(basicRect, spawnEveryProperty);
            
            yPos += EditorGUIUtility.singleLineHeight + SPACING;
            basicRect.y = yPos;
            EditorGUI.PropertyField(basicRect, spawnAmountProperty);
            
            yPos += EditorGUIUtility.singleLineHeight + SPACING;
            
            // Rock Prefabs section
            Rect headerRect = new Rect(position.x, yPos, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(headerRect, "Rock Prefabs", EditorStyles.boldLabel);
            yPos += EditorGUIUtility.singleLineHeight + SPACING;
            
            // Draw Rock Prefabs section
            yPos = DrawPrefabsAndChancesSection(
                position.x,
                yPos,
                position.width,
                property.FindPropertyRelative("rockPrefabs"),
                property.FindPropertyRelative("rockChanses"),
                "Rock",
                ROCKS_FOLDER_PATH
            );
            
            // Rock Filling Prefabs section
            headerRect = new Rect(position.x, yPos, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(headerRect, "Rock Filling Prefabs", EditorStyles.boldLabel);
            yPos += EditorGUIUtility.singleLineHeight + SPACING;
            
            // Draw Rock Filling Prefabs section
            DrawPrefabsAndChancesSection(
                position.x,
                yPos,
                position.width,
                property.FindPropertyRelative("rockFillingPrefabs"),
                property.FindPropertyRelative("rockFillingChanses"),
                "Rock Filling",
                ROCK_FILLINGS_FOLDER_PATH
            );
            
            EditorGUI.indentLevel--;
        }
        
        EditorGUI.EndProperty();
    }
    
    private float DrawPrefabsAndChancesSection(
        float x, 
        float yPos, 
        float width, 
        SerializedProperty prefabsProperty, 
        SerializedProperty chancesProperty, 
        string labelPrefix, 
        string folderPath)
    {
        // Load available prefabs from folder
        List<GameObject> availablePrefabs = LoadPrefabsFromFolder(folderPath);
        string[] prefabNames = availablePrefabs.Select(p => p.name).ToArray();
        
        if (prefabNames.Length == 0)
        {
            Rect warningRect = new Rect(x, yPos, width, EditorGUIUtility.singleLineHeight);
            EditorGUI.HelpBox(warningRect, $"No prefabs found in the specified {labelPrefix} folder.", MessageType.Warning);
            return yPos + EditorGUIUtility.singleLineHeight + SPACING;
        }
        
        // Ensure arrays are the same size
        if (prefabsProperty.arraySize != chancesProperty.arraySize)
        {
            chancesProperty.arraySize = prefabsProperty.arraySize;
        }
        
        // Handle array size
        Rect sizeRect = new Rect(x, yPos, width, EditorGUIUtility.singleLineHeight);
        int indentLevel = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;
        
        Rect labelRect = new Rect(x + 10, yPos, 80, EditorGUIUtility.singleLineHeight);
        EditorGUI.LabelField(labelRect, $"{labelPrefix} Count");
        
        Rect fieldRect = new Rect(x + 100, yPos, 50, EditorGUIUtility.singleLineHeight);
        EditorGUI.BeginChangeCheck();
        int newSize = EditorGUI.IntField(fieldRect, prefabsProperty.arraySize);
        if (EditorGUI.EndChangeCheck())
        {
            // Handle resizing both arrays
            prefabsProperty.arraySize = newSize;
            chancesProperty.arraySize = newSize;
            
            // Initialize any new chance values
            for (int i = 0; i < chancesProperty.arraySize; i++)
            {
                if (chancesProperty.GetArrayElementAtIndex(i).floatValue <= 0)
                {
                    chancesProperty.GetArrayElementAtIndex(i).floatValue = 1.0f / chancesProperty.arraySize;
                }
            }
            
            // Normalize chances
            NormalizeChances(chancesProperty);
        }
        
        EditorGUI.indentLevel = indentLevel;
        yPos += EditorGUIUtility.singleLineHeight + SPACING;
        
        // Display prefabs and chances
        for (int i = 0; i < prefabsProperty.arraySize; i++)
        {
            // Get current prefab
            SerializedProperty prefabProperty = prefabsProperty.GetArrayElementAtIndex(i);
            GameObject currentPrefab = prefabProperty.objectReferenceValue as GameObject;
            
            // Find index in available prefabs
            int currentIndex = -1;
            for (int j = 0; j < availablePrefabs.Count; j++)
            {
                if (availablePrefabs[j] == currentPrefab)
                {
                    currentIndex = j;
                    break;
                }
            }
            
            // Create popup options with "None" as first option
            string[] popupOptions = new string[prefabNames.Length + 1];
            popupOptions[0] = "None";
            System.Array.Copy(prefabNames, 0, popupOptions, 1, prefabNames.Length);
            
            // Display prefab selector
            Rect prefabRect = new Rect(x, yPos, width, EditorGUIUtility.singleLineHeight);
            EditorGUI.BeginChangeCheck();
            
            EditorGUI.LabelField(new Rect(x, yPos, 80, EditorGUIUtility.singleLineHeight), $"{labelPrefix} {i+1}");
            int newIndex = EditorGUI.Popup(
                new Rect(x + 80, yPos, width - 90, EditorGUIUtility.singleLineHeight),
                currentIndex + 1, 
                popupOptions
            ) - 1;
            
            if (EditorGUI.EndChangeCheck())
            {
                prefabProperty.objectReferenceValue = newIndex >= 0 ? availablePrefabs[newIndex] : null;
            }
            
            yPos += EditorGUIUtility.singleLineHeight + SPACING;
            
            // Chance slider
            SerializedProperty chanceProperty = chancesProperty.GetArrayElementAtIndex(i);
            Rect chanceRect = new Rect(x, yPos, width, EditorGUIUtility.singleLineHeight);
            
            float oldValue = chanceProperty.floatValue;
            float newValue = EditorGUI.Slider(chanceRect, $"Chance {i+1}", oldValue, 0f, 1f);
            
            if (oldValue != newValue)
            {
                chanceProperty.floatValue = newValue;
                NormalizeChances(chancesProperty, i);
            }
            
            yPos += EditorGUIUtility.singleLineHeight + SPACING * 2;
        }
        
        return yPos;
    }
    
    private List<GameObject> LoadPrefabsFromFolder(string folderPath)
    {
        List<GameObject> prefabs = new List<GameObject>();
        
        if (Directory.Exists(folderPath))
        {
            string[] guids = AssetDatabase.FindAssets("t:GameObject", new[] { folderPath });
            Debug.Log($"Found {guids.Length} assets in folder: {folderPath}");
            
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Debug.Log($"Loading asset at path: {path}");
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab != null)
                {
                    prefabs.Add(prefab);
                }
            }
        }
        else
        {
            Debug.LogError($"Folder does not exist: {folderPath}");
        }
        
        return prefabs;
    }
    
    private void NormalizeChances(SerializedProperty chancesProperty, int changedIndex = -1)
    {
        if (chancesProperty.arraySize == 0)
            return;
            
        // Get sum of all chances except the changed one
        float sum = 0f;
        for (int i = 0; i < chancesProperty.arraySize; i++)
        {
            if (i != changedIndex)
                sum += chancesProperty.GetArrayElementAtIndex(i).floatValue;
        }
        
        // If we changed a specific value and sum with other values exceeds 1
        if (changedIndex >= 0)
        {
            float changedValue = chancesProperty.GetArrayElementAtIndex(changedIndex).floatValue;
            
            if (sum + changedValue > 1f)
            {
                // Scale other values down
                float scale = sum > 0 ? (1f - changedValue) / sum : 0f;
                for (int i = 0; i < chancesProperty.arraySize; i++)
                {
                    if (i != changedIndex)
                    {
                        SerializedProperty chanceProperty = chancesProperty.GetArrayElementAtIndex(i);
                        chanceProperty.floatValue *= scale;
                    }
                }
            }
            else if (Mathf.Abs(sum + changedValue - 1f) > 0.001f)
            {
                // Scale all values to sum to 1
                float totalSum = sum + changedValue;
                
                if (totalSum > 0)
                {
                    float scale = 1f / totalSum;
                    for (int i = 0; i < chancesProperty.arraySize; i++)
                    {
                        SerializedProperty chanceProperty = chancesProperty.GetArrayElementAtIndex(i);
                        chanceProperty.floatValue *= scale;
                    }
                }
                else
                {
                    // If all values are 0, distribute evenly
                    float evenValue = 1f / chancesProperty.arraySize;
                    for (int i = 0; i < chancesProperty.arraySize; i++)
                    {
                        chancesProperty.GetArrayElementAtIndex(i).floatValue = evenValue;
                    }
                }
            }
        }
        else
        {
            // Scale all values to sum to 1
            float totalSum = 0f;
            for (int i = 0; i < chancesProperty.arraySize; i++)
                totalSum += chancesProperty.GetArrayElementAtIndex(i).floatValue;
                
            if (totalSum > 0)
            {
                float scale = 1f / totalSum;
                for (int i = 0; i < chancesProperty.arraySize; i++)
                {
                    SerializedProperty chanceProperty = chancesProperty.GetArrayElementAtIndex(i);
                    chanceProperty.floatValue *= scale;
                }
            }
            else if (chancesProperty.arraySize > 0)
            {
                // If all values are 0, distribute evenly
                float evenValue = 1f / chancesProperty.arraySize;
                for (int i = 0; i < chancesProperty.arraySize; i++)
                {
                    chancesProperty.GetArrayElementAtIndex(i).floatValue = evenValue;
                }
            }
        }
    }
}
