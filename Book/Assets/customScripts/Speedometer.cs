using UnityEngine;
using System.Collections;

public class Speedometer : MonoBehaviour {

    public Texture2D dialTex;
    public Texture2D needleTex;
    public Vector2 dialPos;
    public float topSpeed;
    public float stopAngle;
    public float topSpeedAngle;
    public float speed;
    public float size = 2;
    private Vector2 sizeVDial;
    private Vector2 sizeVNeedle;

    void Start()
    {
       // RectTransform objectRectTransform = this.transform.parent.GetComponent<RectTransform>();
       // dialPos.x = dialPos.x + objectRectTransform.rect.width / 2;
       // dialPos.y = dialPos.y + objectRectTransform.rect.height / 2;
        Debug.Log(dialPos.x);
        sizeVDial.x = dialTex.width / size;
        sizeVDial.y = dialTex.height / size;
        sizeVNeedle.x = needleTex.width / size;
        sizeVNeedle.y = needleTex.height / size;
    }
    void OnGUI()
    {
      
        GUI.DrawTexture(new Rect(dialPos.x, dialPos.y, sizeVDial.x, sizeVDial.y), dialTex);
        var centre = new Vector2(dialPos.x + sizeVDial.x / 2, dialPos.y + sizeVDial.y / 2);
        var savedMatrix = GUI.matrix;
        var speedFraction = speed / topSpeed;
        var needleAngle = Mathf.Lerp(stopAngle, topSpeedAngle, speedFraction);
        GUIUtility.RotateAroundPivot(needleAngle, centre);
        GUI.DrawTexture(new Rect(centre.x, centre.y - sizeVNeedle.y / 2, sizeVNeedle.x, sizeVNeedle.y), needleTex);
        GUI.matrix = savedMatrix;
    }
}
