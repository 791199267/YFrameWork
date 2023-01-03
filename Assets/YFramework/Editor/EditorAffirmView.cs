using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EditorAffirmView : EditorWindow
{
    public static Action AffirmAction;

    public static void ShowWindow(Action afirmAction)
    {
        AffirmAction = afirmAction;
        EditorWindow thisWindow = EditorWindow.GetWindow(typeof(EditorAffirmView));
        thisWindow.titleContent = new GUIContent("确认操作");
        thisWindow.position = new Rect(Screen.width / 2, Screen.height / 2, 400, 250);
    }

    private void OnGUI()
    {

        GUI.Label(new Rect(100, 50, 200, 30), "已经存在的资源，是否覆盖！");

        if (GUI.Button(new Rect(50, 150, 100, 30), "关闭"))
        {
            Close();
        }


        if (GUI.Button(new Rect(250, 150, 100, 30), "确认")) 
        {
            AffirmAction?.Invoke();
            Close();
        }
    }
}
