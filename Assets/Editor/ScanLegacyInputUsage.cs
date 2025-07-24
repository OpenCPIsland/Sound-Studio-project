using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public static class ScanScriptsForWebAccess
{
    static readonly string[] patterns = new string[]
    {
        @"standalone"
    };

    [MenuItem("Project/Scan Scripts for Web Access")]
    public static void Scan()
    {
        string scriptsRoot = Path.Combine(Application.dataPath, "Scripts");
        if (!Directory.Exists(scriptsRoot))
        {
            Debug.LogWarning("Assets/Scripts/ does not exist!");
            return;
        }

        var csFiles = Directory.GetFiles(scriptsRoot, "*.cs", SearchOption.AllDirectories);
        var foundMatches = new List<string>();

        for (int fileIndex = 0; fileIndex < csFiles.Length; fileIndex++)
        {
            string file = csFiles[fileIndex];
            float progress = (float)fileIndex / csFiles.Length;
            bool canceled = EditorUtility.DisplayCancelableProgressBar(
                "Scanning for Web/Network Access",
                $"Scanning file {fileIndex + 1} of {csFiles.Length}\n{file.Replace(Application.dataPath, "Assets")}",
                progress
            );
            if (canceled)
            {
                EditorUtility.ClearProgressBar();
                Debug.LogWarning("Scan canceled.");
                return;
            }

            string[] lines = File.ReadAllLines(file);
            for (int i = 0; i < lines.Length; i++)
            {
                foreach (var pattern in patterns)
                {
                    if (Regex.IsMatch(lines[i], pattern))
                    {
                        string msg = $"[Web Access] {file.Replace(Application.dataPath, "Assets")} (Line {i + 1}): {lines[i].Trim()}";
                        Debug.Log(msg);
                        foundMatches.Add(msg);
                    }
                }
            }
        }

        EditorUtility.ClearProgressBar();

        string txtPath = Path.Combine(Application.dataPath, "WebAccessScan.txt");
        if (foundMatches.Count == 0)
        {
            Debug.Log("No web/network access usages found in Assets/Scripts/!");
            File.WriteAllText(txtPath, "No web/network access usages found in Assets/Scripts/!\n");
        }
        else
        {
            Debug.Log($"Found web/network access usages in {foundMatches.Count} places in Assets/Scripts/. See above for details.");
            File.WriteAllLines(txtPath, foundMatches);
        }
        AssetDatabase.Refresh(); // Show the new/updated .txt file in Project window
        Debug.Log($"Web access scan results written to: Assets/WebAccessScan.txt");
    }
}
