using System;
using UnityEditor;
using UnityEngine;

public class ClearPlayerPrefs : EditorWindow
{
    [MenuItem("EAP/Clear PlayerPrefs")]
    public static void Action()
    {
        ClearPrefs();
    }

    private static void ClearPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
}
