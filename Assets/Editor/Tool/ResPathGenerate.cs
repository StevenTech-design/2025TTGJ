using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace TTGJ.Editor
{
    public class ResPathGenerate : EditorWindow
    {
        private string className = "ResPathConfig";
        private string outputPath = "Assets/Scripts/Generate/ResPathConfig.cs";
        private static string resFolderPath = "Assets/Res";

        [MenuItem("Tools/Generate ResPathConfig")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(ResPathGenerate), false, "ResPathConfig Generator");
        }

        private void OnGUI()
        {
            GUILayout.Label("资源路径配置生成器", EditorStyles.boldLabel);

            className = EditorGUILayout.TextField("类名", className);
            outputPath = EditorGUILayout.TextField("输出路径", outputPath);
            resFolderPath = EditorGUILayout.TextField("Res路径", resFolderPath);

            if (GUILayout.Button("生成 ResPathConfig 文件"))
            {
                GenerateResPathConfig();
            }
        }

        private void GenerateResPathConfig()
        {
            if (!Directory.Exists(resFolderPath))
            {
                Debug.LogError($"指定的 Res 文件夹不存在: {resFolderPath}");
                return;
            }

            string resFolderFullPath = Path.GetFullPath(resFolderPath).Replace("\\", "/");

            StringBuilder builder = new StringBuilder();
            builder.AppendLine("namespace Dm.TwistedFate.TexasHoldem");
            builder.AppendLine("{");
            builder.AppendLine($"    public static class {className}");
            builder.AppendLine("    {");

            HashSet<string> usedFieldNames = new HashSet<string>();

            string[] directories = Directory.GetDirectories(resFolderFullPath, "*", SearchOption.AllDirectories);
            foreach (var dir in directories)
            {
                string relativePath = dir.Replace("\\", "/").Replace(resFolderFullPath + "/", "");
                string folderName = Path.GetFileName(dir);
                string fieldName = GenerateUniqueFieldName(relativePath, folderName, usedFieldNames);

                builder.AppendLine($"        public const string {fieldName} = \"{relativePath}\";");
                usedFieldNames.Add(fieldName);
            }

            string[] files = Directory.GetFiles(resFolderFullPath, "*.*", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                if (file.EndsWith(".meta", StringComparison.OrdinalIgnoreCase)) continue;

                string relativePath = file.Replace("\\", "/").Replace(resFolderFullPath + "/", "");
                int lastDot = relativePath.LastIndexOf('.');
                if (lastDot > -1)
                    relativePath = relativePath.Substring(0, lastDot);

                string rawName = Path.GetFileNameWithoutExtension(file);
                string fieldName = GenerateUniqueFieldName(relativePath, rawName, usedFieldNames);

                builder.AppendLine($"        public const string {fieldName} = \"{relativePath}\";");
                usedFieldNames.Add(fieldName);
            }

            builder.AppendLine("    }");
            builder.AppendLine("}");

            outputPath = outputPath.Replace("\\", "/");
            File.WriteAllText(outputPath, builder.ToString(), Encoding.UTF8);
            Debug.Log($"ResPathConfig 文件已生成：{outputPath}");
            AssetDatabase.Refresh();
        }


        private string SanitizePart(string part)
        {
            string s = Regex.Replace(part, "[^a-zA-Z0-9_]", "_");
            if (string.IsNullOrEmpty(s)) s = "_";
            if (char.IsDigit(s[0])) s = "File_" + s;
            return s;
        }

        private string GenerateUniqueFieldName(string relativePath, string rawName, HashSet<string> usedNames)
        {
            string[] parts = relativePath.Split('/');
            int parentIdx = parts.Length - 2;

            string candidate;
            if (parentIdx >= 0)
            {
                candidate = $"{SanitizePart(parts[parentIdx])}_{SanitizePart(rawName)}";
                parentIdx--;
            }
            else
            {
                candidate = SanitizePart(rawName);
            }

            while (usedNames.Contains(candidate) && parentIdx >= 0)
            {
                candidate = $"{SanitizePart(parts[parentIdx])}_{candidate}";
                parentIdx--;
            }

            int count = 1;
            string unique = candidate;
            while (usedNames.Contains(unique))
            {
                unique = $"{candidate}_{count++}";
            }

            return unique;
        }
    }
}