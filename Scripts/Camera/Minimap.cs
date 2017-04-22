using UnityEngine;
using System.Collections;

//[ExecuteInEditMode]
public class Minimap : MonoBehaviour
{

    // FPS KIT [www.armedunity.com]

    public float minimapSize = 2.0f;
    private float offsetX = 10.0f;
    private float offsetY = 10.0f;
    private float adjustSize = 0.0f;

    public Texture borderTexture;
    public Texture effectTexture;
    public Camera minimapCamera;

    void Start()
    {
        minimapCamera.enabled = true;
    }

    void Update()
    {
        adjustSize = Mathf.RoundToInt(Screen.width / 10);
        minimapCamera.pixelRect = new Rect(offsetX, (Screen.height - (minimapSize * adjustSize)) - offsetY, minimapSize * adjustSize, minimapSize * adjustSize);
    }

    /// <summary>
    /// Renders the UI
    /// </summary>
    void OnGUI()
    {
        minimapCamera.Render();
        GUI.DrawTexture(new Rect(offsetX, offsetY, minimapSize * adjustSize, minimapSize * adjustSize), effectTexture);
        GUI.DrawTexture(new Rect(offsetX, offsetY, minimapSize * adjustSize, minimapSize * adjustSize), borderTexture);
    }


}