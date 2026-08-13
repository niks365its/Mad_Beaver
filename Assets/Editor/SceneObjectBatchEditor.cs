using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneObjectBatchEditor : EditorWindow
{
    private enum NameMatchMode
    {
        Exact,
        Contains
    }

    private enum PositionMode
    {
        World,
        Local
    }

    private GameObject targetObject;
    private string targetName = "";
    private NameMatchMode nameMatchMode = NameMatchMode.Exact;

    private bool applyPosition = true;
    private PositionMode positionMode = PositionMode.World;
    private Vector3 newPosition;

    private bool applyActiveState = true;
    private bool newActiveState = true;

    private bool applyTransparency = true;
    [Range(0f, 1f)]
    private float newAlpha = 1f;
    private bool createMaterialInstances = true;

    private Vector2 scrollPosition;
    private readonly List<SearchResult> lastResults = new List<SearchResult>();

    private struct SearchResult
    {
        public string ScenePath;
        public string ObjectPath;
    }

    [MenuItem("Tools/Scene Object Batch Editor")]
    public static void OpenWindow()
    {
        GetWindow<SceneObjectBatchEditor>("Scene Object Editor");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Пошук об'єкта на всіх сценах", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        EditorGUI.BeginChangeCheck();
        targetObject = (GameObject)EditorGUILayout.ObjectField("Об'єкт або prefab", targetObject, typeof(GameObject), true);
        if (EditorGUI.EndChangeCheck() && targetObject != null)
        {
            targetName = targetObject.name;
        }

        targetName = EditorGUILayout.TextField("Назва об'єкта", targetName);
        nameMatchMode = (NameMatchMode)EditorGUILayout.EnumPopup("Порівняння назви", nameMatchMode);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Зміни", EditorStyles.boldLabel);

        applyPosition = EditorGUILayout.ToggleLeft("Змінити координати", applyPosition);
        using (new EditorGUI.DisabledScope(!applyPosition))
        {
            positionMode = (PositionMode)EditorGUILayout.EnumPopup("Тип координат", positionMode);
            newPosition = EditorGUILayout.Vector3Field("Нова позиція", newPosition);
        }

        applyActiveState = EditorGUILayout.ToggleLeft("Змінити стан об'єкта", applyActiveState);
        using (new EditorGUI.DisabledScope(!applyActiveState))
        {
            newActiveState = EditorGUILayout.Toggle("Увімкнений", newActiveState);
        }

        applyTransparency = EditorGUILayout.ToggleLeft("Змінити прозорість", applyTransparency);
        using (new EditorGUI.DisabledScope(!applyTransparency))
        {
            newAlpha = EditorGUILayout.Slider("Alpha", newAlpha, 0f, 1f);
            createMaterialInstances = EditorGUILayout.Toggle("Окремі матеріали", createMaterialInstances);
        }

        EditorGUILayout.Space();

        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Знайти"))
            {
                FindObjectsInAllScenes();
            }

            if (GUILayout.Button("Застосувати зміни"))
            {
                ApplyChangesToAllScenes();
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField($"Знайдено: {lastResults.Count}", EditorStyles.boldLabel);

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        foreach (SearchResult result in lastResults)
        {
            EditorGUILayout.LabelField(Path.GetFileNameWithoutExtension(result.ScenePath), result.ObjectPath);
        }
        EditorGUILayout.EndScrollView();
    }

    private void FindObjectsInAllScenes()
    {
        if (!ValidateInput())
        {
            return;
        }

        lastResults.Clear();
        string[] scenePaths = GetScenePaths();
        SceneSetup[] previousSetup = EditorSceneManager.GetSceneManagerSetup();

        try
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return;
            }

            foreach (string scenePath in scenePaths)
            {
                Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                foreach (GameObject sceneObject in GetSceneGameObjects(scene))
                {
                    if (MatchesTarget(sceneObject))
                    {
                        lastResults.Add(new SearchResult
                        {
                            ScenePath = scenePath,
                            ObjectPath = GetHierarchyPath(sceneObject.transform)
                        });
                    }
                }
            }
        }
        finally
        {
            RestorePreviousScenes(previousSetup);
        }

        Debug.Log($"Scene Object Batch Editor: знайдено {lastResults.Count} об'єкт(ів).");
    }

    private void ApplyChangesToAllScenes()
    {
        if (!ValidateInput())
        {
            return;
        }

        if (!applyPosition && !applyActiveState && !applyTransparency)
        {
            EditorUtility.DisplayDialog("Немає змін", "Увімкни хоча б одну зміну для застосування.", "OK");
            return;
        }

        string[] scenePaths = GetScenePaths();
        if (!EditorUtility.DisplayDialog("Застосувати зміни?",
            $"Буде перевірено сцен: {scenePaths.Length}. Зміни буде збережено у сценах, де знайдено об'єкт \"{targetName}\".",
            "Застосувати", "Скасувати"))
        {
            return;
        }

        int changedObjects = 0;
        int changedScenes = 0;
        lastResults.Clear();
        SceneSetup[] previousSetup = EditorSceneManager.GetSceneManagerSetup();

        try
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return;
            }

            foreach (string scenePath in scenePaths)
            {
                Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                bool sceneChanged = false;

                foreach (GameObject sceneObject in GetSceneGameObjects(scene))
                {
                    if (!MatchesTarget(sceneObject))
                    {
                        continue;
                    }

                    Undo.RegisterFullObjectHierarchyUndo(sceneObject, "Batch Edit Scene Object");
                    ApplyToObject(sceneObject);
                    EditorUtility.SetDirty(sceneObject);

                    lastResults.Add(new SearchResult
                    {
                        ScenePath = scenePath,
                        ObjectPath = GetHierarchyPath(sceneObject.transform)
                    });

                    sceneChanged = true;
                    changedObjects++;
                }

                if (sceneChanged)
                {
                    EditorSceneManager.MarkSceneDirty(scene);
                    EditorSceneManager.SaveScene(scene);
                    AssetDatabase.SaveAssets();
                    changedScenes++;
                }
            }
        }
        finally
        {
            RestorePreviousScenes(previousSetup);
        }

        Debug.Log($"Scene Object Batch Editor: змінено {changedObjects} об'єкт(ів) у {changedScenes} сцен(ах).");
    }

    private bool ValidateInput()
    {
        if (string.IsNullOrWhiteSpace(targetName))
        {
            EditorUtility.DisplayDialog("Немає назви", "Вкажи об'єкт або введи назву для пошуку.", "OK");
            return false;
        }

        return true;
    }

    private static string[] GetScenePaths()
    {
        string[] guids = AssetDatabase.FindAssets("t:Scene", new[] { "Assets" });
        string[] paths = new string[guids.Length];

        for (int i = 0; i < guids.Length; i++)
        {
            paths[i] = AssetDatabase.GUIDToAssetPath(guids[i]);
        }

        return paths;
    }

    private static IEnumerable<GameObject> GetSceneGameObjects(Scene scene)
    {
        foreach (GameObject rootObject in scene.GetRootGameObjects())
        {
            foreach (Transform child in rootObject.GetComponentsInChildren<Transform>(true))
            {
                yield return child.gameObject;
            }
        }
    }

    private bool MatchesTarget(GameObject sceneObject)
    {
        if (targetObject != null)
        {
            GameObject prefabSource = PrefabUtility.GetCorrespondingObjectFromSource(sceneObject);
            if (prefabSource == targetObject)
            {
                return true;
            }
        }

        return nameMatchMode == NameMatchMode.Exact
            ? sceneObject.name == targetName
            : sceneObject.name.IndexOf(targetName, System.StringComparison.OrdinalIgnoreCase) >= 0;
    }

    private void ApplyToObject(GameObject sceneObject)
    {
        if (applyPosition)
        {
            if (positionMode == PositionMode.World)
            {
                sceneObject.transform.position = newPosition;
            }
            else
            {
                sceneObject.transform.localPosition = newPosition;
            }
        }

        if (applyActiveState)
        {
            sceneObject.SetActive(newActiveState);
        }

        if (applyTransparency)
        {
            ApplyAlpha(sceneObject, newAlpha);
        }
    }

    private void ApplyAlpha(GameObject sceneObject, float alpha)
    {
        foreach (Renderer renderer in sceneObject.GetComponentsInChildren<Renderer>(true))
        {
            Material[] materials = createMaterialInstances ? renderer.materials : renderer.sharedMaterials;
            foreach (Material material in materials)
            {
                if (material == null)
                {
                    continue;
                }

                SetMaterialAlpha(material, alpha);
                EditorUtility.SetDirty(material);
            }

            if (createMaterialInstances)
            {
                renderer.sharedMaterials = materials;
            }

            EditorUtility.SetDirty(renderer);
        }

        foreach (SpriteRenderer spriteRenderer in sceneObject.GetComponentsInChildren<SpriteRenderer>(true))
        {
            Color color = spriteRenderer.color;
            color.a = alpha;
            spriteRenderer.color = color;
            EditorUtility.SetDirty(spriteRenderer);
        }

        foreach (Graphic graphic in sceneObject.GetComponentsInChildren<Graphic>(true))
        {
            Color color = graphic.color;
            color.a = alpha;
            graphic.color = color;
            EditorUtility.SetDirty(graphic);
        }

        foreach (CanvasGroup canvasGroup in sceneObject.GetComponentsInChildren<CanvasGroup>(true))
        {
            canvasGroup.alpha = alpha;
            EditorUtility.SetDirty(canvasGroup);
        }
    }

    private static void SetMaterialAlpha(Material material, float alpha)
    {
        if (material.HasProperty("_Color"))
        {
            Color color = material.color;
            color.a = alpha;
            material.color = color;
        }

        if (material.HasProperty("_BaseColor"))
        {
            Color color = material.GetColor("_BaseColor");
            color.a = alpha;
            material.SetColor("_BaseColor", color);
        }

        SetupTransparentMaterial(material, alpha);
    }

    private static void SetupTransparentMaterial(Material material, float alpha)
    {
        if (alpha >= 1f)
        {
            return;
        }

        if (material.HasProperty("_Mode"))
        {
            material.SetFloat("_Mode", 3f);
        }

        if (material.HasProperty("_Surface"))
        {
            material.SetFloat("_Surface", 1f);
        }

        material.SetOverrideTag("RenderType", "Transparent");
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
    }

    private static string GetHierarchyPath(Transform transform)
    {
        string path = transform.name;
        while (transform.parent != null)
        {
            transform = transform.parent;
            path = transform.name + "/" + path;
        }

        return path;
    }

    private static void RestorePreviousScenes(SceneSetup[] previousSetup)
    {
        if (previousSetup == null || previousSetup.Length == 0)
        {
            return;
        }

        EditorSceneManager.RestoreSceneManagerSetup(previousSetup);
    }
}
