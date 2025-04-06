using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Collections.Generic;

[CustomEditor(typeof(GameConfig))]
public class GameConfigEditor : Editor
{
    private bool[] foldoutStates;
    
    // Default paths - will be overridden by EditorPrefs if available
    private string rocksPath = "Assets/Prefabs/Rocks";
    private string rockFillingsPath = "Assets/Prefabs/RockFillings";
    
    // EditorPrefs keys for storing paths
    private const string ROCKS_PATH_PREF = "GameConfig_RocksPath";
    private const string ROCK_FILLINGS_PATH_PREF = "GameConfig_RockFillingsPath";
    
    private void OnEnable()
    {
        // Load saved paths from EditorPrefs if they exist
        if (EditorPrefs.HasKey(ROCKS_PATH_PREF))
        {
            rocksPath = EditorPrefs.GetString(ROCKS_PATH_PREF);
        }
        
        if (EditorPrefs.HasKey(ROCK_FILLINGS_PATH_PREF))
        {
            rockFillingsPath = EditorPrefs.GetString(ROCK_FILLINGS_PATH_PREF);
        }
    }
    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        GameConfig gameConfig = (GameConfig)target;
        
        // Path configuration section
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Prefab Folder Paths", EditorStyles.boldLabel);
        
        EditorGUI.BeginChangeCheck();
        rocksPath = FolderPathField("Rocks Prefabs Path", rocksPath);
        if (EditorGUI.EndChangeCheck())
        {
            EditorPrefs.SetString(ROCKS_PATH_PREF, rocksPath);
        }
        
        EditorGUI.BeginChangeCheck();
        rockFillingsPath = FolderPathField("Rock Fillings Prefabs Path", rockFillingsPath);
        if (EditorGUI.EndChangeCheck())
        {
            EditorPrefs.SetString(ROCK_FILLINGS_PATH_PREF, rockFillingsPath);
        }
        
        EditorGUILayout.Space(10);
        
        // Draw the standard fields
        EditorGUILayout.PropertyField(serializedObject.FindProperty("startMoney"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("winDepth"));
        
        // Draw spawnOptions array with custom handling
        SerializedProperty spawnOptionsProperty = serializedObject.FindProperty("spawnOptions");
        
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Spawn Options", EditorStyles.boldLabel);
        
        // Handle array size
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Size", GUILayout.Width(40));
        EditorGUI.BeginChangeCheck();
        int newSize = EditorGUILayout.IntField(spawnOptionsProperty.arraySize);
        if (EditorGUI.EndChangeCheck())
        {
            spawnOptionsProperty.arraySize = newSize;
            InitializeFoldoutStates(newSize);
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space(5);
        
        // Initialize foldout states if needed
        if (foldoutStates == null || foldoutStates.Length != spawnOptionsProperty.arraySize)
        {
            InitializeFoldoutStates(spawnOptionsProperty.arraySize);
        }
        
        // Draw each spawn option
        for (int i = 0; i < spawnOptionsProperty.arraySize; i++)
        {
            SerializedProperty spawnOptionProperty = spawnOptionsProperty.GetArrayElementAtIndex(i);
            
            EditorGUILayout.BeginVertical("box");
            
            // Foldout header with option to remove
            EditorGUILayout.BeginHorizontal();
            foldoutStates[i] = EditorGUILayout.Foldout(foldoutStates[i], $"Spawn Option {i}", true);
            
            if (GUILayout.Button("×", GUILayout.Width(20)))
            {
                spawnOptionsProperty.DeleteArrayElementAtIndex(i);
                InitializeFoldoutStates(spawnOptionsProperty.arraySize);
                serializedObject.ApplyModifiedProperties();
                return;
            }
            EditorGUILayout.EndHorizontal();
            
            if (foldoutStates[i])
            {
                // Draw the spawn option properties
                DrawSpawnOptionProperties(spawnOptionProperty);
            }
            
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(5);
        }
        
        // Add button
        if (GUILayout.Button("Add Spawn Option"))
        {
            spawnOptionsProperty.arraySize++;
            InitializeFoldoutStates(spawnOptionsProperty.arraySize);
            int newIndex = spawnOptionsProperty.arraySize - 1;
            foldoutStates[newIndex] = true;
            
            // Initialize new element with default values
            SerializedProperty newElement = spawnOptionsProperty.GetArrayElementAtIndex(newIndex);
            newElement.FindPropertyRelative("minDepth").floatValue = 0f;
            newElement.FindPropertyRelative("newSpawnEveryXMeters").floatValue = 3f;
            newElement.FindPropertyRelative("spawnAmount").intValue = 5;
            newElement.FindPropertyRelative("rockPrefabs").arraySize = 0;
            newElement.FindPropertyRelative("rockChanses").arraySize = 0;
            newElement.FindPropertyRelative("rockFillingPrefabs").arraySize = 0;
            newElement.FindPropertyRelative("rockFillingChanses").arraySize = 0;
        }
        
        serializedObject.ApplyModifiedProperties();
    }
    
    private string FolderPathField(string label, string path)
    {
        EditorGUILayout.BeginHorizontal();
        string newPath = EditorGUILayout.TextField(label, path);
        
        if (GUILayout.Button("Browse", GUILayout.Width(60)))
        {
            string selectedPath = EditorUtility.OpenFolderPanel("Select Folder", path, "");
            if (!string.IsNullOrEmpty(selectedPath))
            {
                // Convert absolute path to project relative path
                if (selectedPath.StartsWith(Application.dataPath))
                {
                    selectedPath = "Assets" + selectedPath.Substring(Application.dataPath.Length);
                }
                newPath = selectedPath;
            }
        }
        
        EditorGUILayout.EndHorizontal();
        
        // Show path validity indicator
        if (!string.IsNullOrEmpty(newPath) && !AssetDatabase.IsValidFolder(newPath))
        {
            EditorGUILayout.HelpBox($"Warning: '{newPath}' is not a valid folder path in your project.", MessageType.Warning);
        }
        
        return newPath;
    }
    
    private void InitializeFoldoutStates(int size)
    {
        foldoutStates = new bool[size];
        for (int i = 0; i < size; i++)
        {
            foldoutStates[i] = foldoutStates != null && i < foldoutStates.Length ? foldoutStates[i] : false;
        }
    }
    
    private void DrawSpawnOptionProperties(SerializedProperty spawnOptionProperty)
    {
        EditorGUI.indentLevel++;
        
        // Draw basic properties
        EditorGUILayout.PropertyField(spawnOptionProperty.FindPropertyRelative("minDepth"));
        EditorGUILayout.PropertyField(spawnOptionProperty.FindPropertyRelative("newSpawnEveryXMeters"));
        EditorGUILayout.PropertyField(spawnOptionProperty.FindPropertyRelative("spawnAmount"));
        
        EditorGUILayout.Space(10);
        
        // Draw Rock Prefabs section
        EditorGUILayout.LabelField("Rock Prefabs", EditorStyles.boldLabel);
        DrawPrefabsAndChancesSection(
            spawnOptionProperty.FindPropertyRelative("rockPrefabs"),
            spawnOptionProperty.FindPropertyRelative("rockChanses"),
            "Rock",
            rocksPath
        );
        
        EditorGUILayout.Space(10);
        
        // Draw Rock Filling Prefabs section
        EditorGUILayout.LabelField("Rock Filling Prefabs", EditorStyles.boldLabel);
        DrawPrefabsAndChancesSection(
            spawnOptionProperty.FindPropertyRelative("rockFillingPrefabs"),
            spawnOptionProperty.FindPropertyRelative("rockFillingChanses"),
            "Rock Filling",
            rockFillingsPath
        );
        
        EditorGUI.indentLevel--;
    }
    
    private void DrawPrefabsAndChancesSection(SerializedProperty prefabsProperty, SerializedProperty chancesProperty, string labelPrefix, string folderPath)
    {
        // Load available prefabs from folder
        List<GameObject> availablePrefabs = LoadPrefabsFromFolder(folderPath);
        
        // Show warning if folder path is invalid
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            EditorGUILayout.HelpBox($"The folder path '{folderPath}' is not valid. Please select a valid folder path above.", MessageType.Error);
            return;
        }
        
        // Show warning if no prefabs found
        if (availablePrefabs.Count == 0)
        {
            EditorGUILayout.HelpBox($"No prefabs found in '{folderPath}'. Make sure your prefabs are in this folder and are GameObject assets.", MessageType.Warning);
            
            // Add a button to refresh the asset database
            if (GUILayout.Button("Refresh Asset Database"))
            {
                AssetDatabase.Refresh();
            }
            
            return;
        }
        
        string[] prefabNames = availablePrefabs.Select(p => p.name).ToArray();
        
        // Ensure arrays are the same size
        if (prefabsProperty.arraySize != chancesProperty.arraySize)
        {
            chancesProperty.arraySize = prefabsProperty.arraySize;
        }
        
        // Handle array size
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField($"{labelPrefix} Count", GUILayout.Width(100));
        EditorGUI.BeginChangeCheck();
        int newSize = EditorGUILayout.IntField(prefabsProperty.arraySize);
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
        EditorGUILayout.EndHorizontal();
        
        // Show number of available prefabs
        EditorGUILayout.LabelField($"Available Prefabs: {prefabNames.Length}", EditorStyles.miniLabel);
        
        // Display prefabs and chances
        EditorGUILayout.BeginVertical("box");
        for (int i = 0; i < prefabsProperty.arraySize; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"{labelPrefix} {i+1}", GUILayout.Width(80));
            
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
            
            // Display popup
            EditorGUI.BeginChangeCheck();
            int newIndex = EditorGUILayout.Popup(currentIndex + 1, popupOptions) - 1;
            if (EditorGUI.EndChangeCheck())
            {
                prefabProperty.objectReferenceValue = newIndex >= 0 ? availablePrefabs[newIndex] : null;
            }
            
            EditorGUILayout.EndHorizontal();
            
            // Chance slider
            SerializedProperty chanceProperty = chancesProperty.GetArrayElementAtIndex(i);
            EditorGUI.BeginChangeCheck();
            float newChance = EditorGUILayout.Slider($"Chance {i+1}", chanceProperty.floatValue, 0f, 1f);
            if (EditorGUI.EndChangeCheck())
            {
                chanceProperty.floatValue = newChance;
                NormalizeChances(chancesProperty, i);
            }
            
            EditorGUILayout.Space(5);
        }
        EditorGUILayout.EndVertical();
    }
    
    private List<GameObject> LoadPrefabsFromFolder(string folderPath)
    {
        List<GameObject> prefabs = new List<GameObject>();
        
        // Skip if folder path is invalid
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            return prefabs;
        }
        
        string[] guids = AssetDatabase.FindAssets("t:GameObject", new[] { folderPath });
        
        // Debug output for troubleshooting
        // Debug.Log($"Found {guids.Length} GameObject assets in folder: {folderPath}");
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab != null)
            {
                prefabs.Add(prefab);
                // Debug.Log($"Loaded prefab: {prefab.name} from {path}");
            }
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