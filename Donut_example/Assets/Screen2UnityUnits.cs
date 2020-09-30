using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screen2UnityUnits : MonoBehaviour
{
    public Camera cam; // note: camera is set to orthographic view
    public Vector2 WorldUnitsInCamera;
    private GameObject DisplayText;
    public GameObject Quad;
  
    // Start is called before the first frame update
    void Start()
    {
        WorldUnitsInCamera.y = cam.GetComponent<Camera>().orthographicSize * 2; // because orthographic  size is measured from the center of the screen
        WorldUnitsInCamera.x = WorldUnitsInCamera.y *Screen.width / Screen.height;

        DisplayText = GameObject.Find("DisplayText");
        DisplayText.gameObject.GetComponent<TMPro.TextMeshPro>().lineSpacing = 10;
       
        DisplayText.gameObject.GetComponent<MeshRenderer>().enabled = true;
        var videoPlayer = Quad.GetComponent<UnityEngine.Video.VideoPlayer>();
        float videoWidth = videoPlayer.clip.width;
        float videoHeight = videoPlayer.clip.height;
        DisplayText.gameObject.GetComponent<TMPro.TextMeshPro>().text = "Unity units are (w,h): " + WorldUnitsInCamera.x.ToString() + "," + WorldUnitsInCamera.y.ToString() + "\n\n" + Screen.width.ToString() + "pixels," + Screen.height.ToString() + "pixels," + "\n\n" + "Video Clip in pixels(w, h):\n\n" + videoWidth.ToString() + "," + videoHeight.ToString();

        Quad.SetActive(false);

    }

}
