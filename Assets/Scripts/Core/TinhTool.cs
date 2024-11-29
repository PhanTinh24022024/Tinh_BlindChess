#if UNITY_EDITOR

using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tinh.Tool
{
    public class TinhTool : EditorWindow
    {
        [Obsolete("Obsolete")]
        private void OnGUI()
        {
            GUILayout.Label("Delete Data");
            if (GUILayout.Button("Delete Data"))
            {
                PlayerPrefs.DeleteAll();
                if (File.Exists(Application.persistentDataPath + "/Timer.json"))
                {
                    File.Delete(Application.persistentDataPath + "/Timer.json");
                }
            }

            GUILayout.Label("Load Scene");
            for (var i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
                if (GUILayout.Button(SceneUtility.GetScenePathByBuildIndex(i)))
                {
                    EditorSceneManager.SaveOpenScenes();
                    EditorSceneManager.OpenScene(SceneUtility.GetScenePathByBuildIndex(i));
                }
        }

        [MenuItem("Tools/TinhTool")]
        public static void ShowWindow()
        {
            GetWindow<TinhTool>("TinhTool");
        }
    }
}

#endif
