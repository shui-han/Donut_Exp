using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screen2UnityUnits : MonoBehaviour
{
    public Camera cam; // note: camera is set to orthographic view
   
    private GameObject DisplayText;
    public GameObject Quad;
    
    // Start is called before the first frame update
    void Start()
    {
        Vector2 WorldUnitsInCamera;
        WorldUnitsInCamera.y = cam.GetComponent<Camera>().orthographicSize * 2; // because orthographic  size is measured from the center of the screen
        float camWidthPix = cam.GetComponent<Camera>().pixelWidth;
        float camHeightPix = cam.GetComponent<Camera>().pixelHeight;
        float camWidth = WorldUnitsInCamera.y;
        WorldUnitsInCamera.x = Mathf.Round(WorldUnitsInCamera.y *(camWidthPix / camHeightPix));
       
        DisplayText = GameObject.Find("DisplayText");
        DisplayText.gameObject.GetComponent<TMPro.TextMeshPro>().lineSpacing = 10;
       
        
        var videoPlayer = Quad.GetComponent<UnityEngine.Video.VideoPlayer>();
        float videoWidth = videoPlayer.clip.width;
        float videoHeight = videoPlayer.clip.height;
        DisplayText.gameObject.GetComponent<TMPro.TextMeshPro>().text = "Unity units are (w,h): " + WorldUnitsInCamera.x.ToString("#.00") + "," + WorldUnitsInCamera.y.ToString("#.00") + "\n\n" + cam.GetComponent<Camera>().pixelWidth.ToString() + "pixels," + cam.GetComponent<Camera>().pixelHeight.ToString() + "pixels," + "\n\n" + "Video Clip in pixels(w, h):\n\n" + videoWidth.ToString() + "," + videoHeight.ToString();
        DisplayText.gameObject.GetComponent<MeshRenderer>().enabled = true;
        Quad.SetActive(false);

    }

}
