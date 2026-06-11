using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ConsoleMonitor : MonoBehaviour
{
    public TextMeshProUGUI logText;

    List<string> lines = new List<string>();
    int maxLines = 12;

    public void Log(string message)
    {
        lines.Add("> " + message);
        if (lines.Count > maxLines)
            lines.RemoveAt(0);

        logText.text = string.Join("\n", lines);
    }
}