using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ScriptCleaner : MonoBehaviour
{
    [MenuItem("GameObject/Automation/Remove scripts", false, 0)]
    private static void IsCanera()
    {
        foreach (GameObject obj in Selection.gameObjects)
        {
            Component[] comps = obj.GetComponentsInChildren(typeof(Component));

            for (int i = 0; i < comps.Length; i++)
            {
                if (comps[i] == null) return;
                if (comps[i] is MonoBehaviour)
                {
                    DestroyImmediate(comps[i]);
                }
            }
        }

        CustomDialog("Successfully", "All scripts have been removed successfully", "OK");
    }

    static void CustomDialog(string _title, string _msg, string _buttonText)
    {
        EditorUtility.DisplayDialog(_title, _msg, _buttonText);
    }
}
