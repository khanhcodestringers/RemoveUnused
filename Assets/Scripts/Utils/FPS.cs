using UnityEngine;
using System.Collections;

public class FPS : MonoBehaviour {

    float deltaTime = 0.0f;
    GUIStyle style;
    int w, h;
    Rect rect;

    void Awake() {
        w = Screen.width;
        h = Screen.height;
        rect = new Rect(0, 0, w, h * 2 / 100);
        style = new GUIStyle();
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 100;
        style.normal.textColor = Color.red;
        m_Instance = this;
        m_Instance.enabled = false;
    }

    void Update() {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
    }

    void OnGUI() {
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        GUI.Label(rect, text, style);
    }

#region GMCommand
    private static FPS m_Instance;
    public static void SetVisible(bool visible) {
        if(m_Instance != null) {
            m_Instance.enabled = visible;
        }
    }
#endregion

}
