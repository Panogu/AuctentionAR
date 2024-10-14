using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsoleController : MonoBehaviour
{

    string output = string.Empty;
    string stack = string.Empty;
    public TMPro.TMP_Text consoleOutput;

    // Start is called before the first frame update
    void Awake()
    {
        consoleOutput.text = "";
        Application.logMessageReceived += Log;
    }

    public void Log(string logString, string stackTrace, LogType type)
    {
        output = logString;
        stack = stackTrace;
        consoleOutput.text = output + "\n" + consoleOutput.text;
        if (consoleOutput.text.Length > 1000)
        {
            consoleOutput.text = consoleOutput.text.Substring(0, 950);
        }
    }
}
