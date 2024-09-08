using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class EnvReader
{
    private static readonly Dictionary<string, string> envVariables = new Dictionary<string, string>();

    static EnvReader()
    {
        LoadEnvFile();
    }

    private static void LoadEnvFile()
    {
        string envPath = Path.Combine(Application.dataPath, ".env");

        if (!File.Exists(envPath))
        {
            Debug.LogError(".env file not found at " + envPath);
            return;
        }

        string[] lines = File.ReadAllLines(envPath);
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                continue; // Skip empty lines and comments

            var keyValuePair = line.Split(new[] { '=' }, 2);
            if (keyValuePair.Length != 2)
                continue; // Skip lines that don't have exactly one '=' character

            string key = keyValuePair[0].Trim();
            string value = keyValuePair[1].Trim();

            if (!envVariables.ContainsKey(key))
            {
                envVariables.Add(key, value);
            }
        }
    }

    public static string GetEnvVariable(string key)
    {
        if (envVariables.TryGetValue(key, out string value))
        {
            return value;
        }

        Debug.LogWarning($"Environment variable '{key}' not found.");
        return null;
    }
}
