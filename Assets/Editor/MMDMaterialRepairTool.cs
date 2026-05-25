using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;

public static class MMDMaterialRepairTool
{
    private const string DefaultModelFolder = "Assets/Naxida";

    [MenuItem("Tools/MMD/Repair Materials From MMD XML")]
    public static void RepairSelectedOrDefault()
    {
        var selectedPath = GetSelectedAssetPath();
        var modelFolder = ResolveModelFolder(selectedPath) ?? DefaultModelFolder;
        var xmlPath = ResolveModelXmlPath(selectedPath, modelFolder);
        if (string.IsNullOrEmpty(xmlPath) || !File.Exists(xmlPath))
        {
            Debug.LogError($"MMD material repair failed. MMDModel XML not found in: {modelFolder}");
            return;
        }

        modelFolder = Path.GetDirectoryName(xmlPath)?.Replace('\\', '/');
        if (string.IsNullOrEmpty(modelFolder))
        {
            Debug.LogError($"MMD material repair failed. Could not resolve model folder for: {xmlPath}");
            return;
        }

        var materialFolder = Path.Combine(modelFolder, "Mat").Replace('\\', '/');
        if (!Directory.Exists(materialFolder))
        {
            Debug.LogError($"MMD material repair failed. Material folder not found: {materialFolder}");
            return;
        }

        var document = XDocument.Load(xmlPath);
        var texturePaths = document.Descendants("textureList")
            .Descendants("Texture")
            .Select(texture => NormalizeTexturePath(texture.Element("fileName")?.Value))
            .ToList();

        var urpShader = Shader.Find("Universal Render Pipeline/Lit");
        var standardShader = Shader.Find("Standard");
        if (urpShader == null && standardShader == null)
        {
            Debug.LogError("MMD material repair failed. Could not find URP Lit or Standard shader.");
            return;
        }

        var repairedCount = 0;
        foreach (var materialElement in document.Descendants("materialList").Descendants("Material"))
        {
            var materialName = materialElement.Element("materialName")?.Value;
            if (string.IsNullOrWhiteSpace(materialName))
            {
                continue;
            }

            var material = LoadMaterial(materialFolder, materialName);
            if (material == null)
            {
                Debug.LogWarning($"MMD material repair skipped missing material: {materialName}");
                continue;
            }

            material.shader = urpShader != null ? urpShader : standardShader;

            var diffuse = ReadColor(materialElement.Element("diffuse"), Color.white);
            SetColorIfPresent(material, "_BaseColor", diffuse);
            SetColorIfPresent(material, "_Color", diffuse);

            var texture = LoadTexture(modelFolder, texturePaths, ReadInt(materialElement.Element("textureID"), -1));
            if (texture != null)
            {
                SetTextureIfPresent(material, "_BaseMap", texture);
                SetTextureIfPresent(material, "_MainTex", texture);
            }

            var isTransparent = diffuse.a < 0.999f;
            ConfigureSurface(material, isTransparent, ReadInt(materialElement.Element("isDrawBothFaces"), 0) == 1);

            EditorUtility.SetDirty(material);
            repairedCount++;
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"MMD material repair completed. Repaired {repairedCount} materials from {xmlPath}.");
    }

    private static string GetSelectedAssetPath()
    {
        var selected = Selection.activeObject;
        if (selected == null)
        {
            return null;
        }

        var path = AssetDatabase.GetAssetPath(selected);
        return string.IsNullOrEmpty(path) ? null : path;
    }

    private static string ResolveModelFolder(string selectedPath)
    {
        if (string.IsNullOrEmpty(selectedPath))
        {
            return null;
        }

        var folder = Directory.Exists(selectedPath)
            ? selectedPath
            : Path.GetDirectoryName(selectedPath)?.Replace('\\', '/');

        if (string.IsNullOrEmpty(folder))
        {
            return null;
        }

        return Path.GetFileName(folder).Equals("Mat", StringComparison.OrdinalIgnoreCase)
            ? Path.GetDirectoryName(folder)?.Replace('\\', '/')
            : folder;
    }

    private static string ResolveModelXmlPath(string selectedPath, string modelFolder)
    {
        if (!string.IsNullOrEmpty(selectedPath)
            && selectedPath.EndsWith(".xml", StringComparison.OrdinalIgnoreCase)
            && IsMmdModelXml(selectedPath))
        {
            return selectedPath;
        }

        return Directory.Exists(modelFolder)
            ? Directory.GetFiles(modelFolder, "*.xml", SearchOption.TopDirectoryOnly)
                .Select(path => path.Replace('\\', '/'))
                .FirstOrDefault(IsMmdModelXml)
            : null;
    }

    private static bool IsMmdModelXml(string path)
    {
        try
        {
            return XDocument.Load(path).Root?.Name.LocalName == "MMDModel";
        }
        catch (Exception)
        {
            return false;
        }
    }

    private static Material LoadMaterial(string materialFolder, string materialName)
    {
        var directPath = Path.Combine(materialFolder, materialName + ".mat").Replace('\\', '/');
        var material = AssetDatabase.LoadAssetAtPath<Material>(directPath);
        if (material != null)
        {
            return material;
        }

        return AssetDatabase.FindAssets("t:Material", new[] { materialFolder })
            .Select(AssetDatabase.GUIDToAssetPath)
            .Where(path => string.Equals(Path.GetFileNameWithoutExtension(path), materialName, StringComparison.OrdinalIgnoreCase))
            .Select(AssetDatabase.LoadAssetAtPath<Material>)
            .FirstOrDefault(found => found != null);
    }

    private static Texture LoadTexture(string modelFolder, IReadOnlyList<string> texturePaths, int textureId)
    {
        if (textureId < 0 || textureId >= texturePaths.Count)
        {
            return null;
        }

        var relativePath = texturePaths[textureId];
        if (string.IsNullOrWhiteSpace(relativePath))
        {
            return null;
        }

        var candidates = new[]
        {
            Path.Combine(modelFolder, relativePath).Replace('\\', '/'),
            Path.Combine("Assets/MMD4Mecanim/Textures", Path.GetFileName(relativePath)).Replace('\\', '/')
        };

        foreach (var path in candidates)
        {
            var texture = AssetDatabase.LoadAssetAtPath<Texture>(path);
            if (texture != null)
            {
                return texture;
            }
        }

        Debug.LogWarning($"MMD material repair could not find texture: {relativePath}");
        return null;
    }

    private static string NormalizeTexturePath(string path)
    {
        return string.IsNullOrWhiteSpace(path) ? null : path.Trim().Replace('\\', '/');
    }

    private static Color ReadColor(XElement element, Color fallback)
    {
        if (element == null)
        {
            return fallback;
        }

        return new Color(
            ReadFloat(element.Element("r"), fallback.r),
            ReadFloat(element.Element("g"), fallback.g),
            ReadFloat(element.Element("b"), fallback.b),
            ReadFloat(element.Element("a"), fallback.a));
    }

    private static int ReadInt(XElement element, int fallback)
    {
        return int.TryParse(element?.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value)
            ? value
            : fallback;
    }

    private static float ReadFloat(XElement element, float fallback)
    {
        return float.TryParse(element?.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var value)
            ? value
            : fallback;
    }

    private static void SetColorIfPresent(Material material, string propertyName, Color color)
    {
        if (material.HasProperty(propertyName))
        {
            material.SetColor(propertyName, color);
        }
    }

    private static void SetTextureIfPresent(Material material, string propertyName, Texture texture)
    {
        if (material.HasProperty(propertyName))
        {
            material.SetTexture(propertyName, texture);
        }
    }

    private static void ConfigureSurface(Material material, bool transparent, bool doubleSided)
    {
        if (material.HasProperty("_Surface"))
        {
            material.SetFloat("_Surface", transparent ? 1f : 0f);
        }

        if (material.HasProperty("_Blend"))
        {
            material.SetFloat("_Blend", 0f);
        }

        if (material.HasProperty("_Cull"))
        {
            material.SetFloat("_Cull", doubleSided ? 0f : 2f);
        }

        material.renderQueue = transparent ? (int)UnityEngine.Rendering.RenderQueue.Transparent : -1;
        material.SetOverrideTag("RenderType", transparent ? "Transparent" : "Opaque");
        SetKeyword(material, "_SURFACE_TYPE_TRANSPARENT", transparent);
    }

    private static void SetKeyword(Material material, string keyword, bool enabled)
    {
        if (enabled)
        {
            material.EnableKeyword(keyword);
        }
        else
        {
            material.DisableKeyword(keyword);
        }
    }
}
