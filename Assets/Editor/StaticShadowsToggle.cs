using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class StaticShadowsToggle : EditorWindow
{
    private bool activeShadows = true;
    [MenuItem("Window/Static Shadows Toggle")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(StaticShadowsToggle));
    }
    
    private void OnGUI()
    {
        activeShadows = EditorGUILayout.Toggle("Active shadows", activeShadows);
        if (GUILayout.Button("Apply"))
        {
           ApplyShadowsChange();
        }
    }
    
    private void ApplyShadowsChange()
    {
        var renderers = GameObject.FindObjectsOfType<MeshRenderer>();
        Undo.RecordObjects(renderers, "Change shadows for all static objects in the scene");

        int processed = 0;
        
        foreach (var renderer in renderers)
        {
            if (renderer.gameObject.isStatic)
            {
                renderer.shadowCastingMode = activeShadows ? ShadowCastingMode.On : ShadowCastingMode.Off;
            }
            processed += 1;
            float progress = (float)processed / renderers.Length;

            EditorUtility.DisplayProgressBar("Changing shadows...", renderer.gameObject.name, progress);
            
        }
        EditorUtility.ClearProgressBar();
    }
}
