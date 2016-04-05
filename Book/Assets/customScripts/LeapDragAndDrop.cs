using UnityEngine;
using System.Collections;
using Leap;

/// <summary>
/// Autor: Tobi Rohrer
/// Class is used for the drag and drop gesture.
/// </summary>
public class LeapDragAndDrop : MonoBehaviour
{
    private GameObject picture;
    private Vector3 currentPicturePosition;

    public LeapDragAndDrop(GameObject picture)
    {
        this.picture = picture;
    }

    /// <summary>
    /// Function called by LeapController every frame. Function is used to determine whether the grap gesture is done by the user.
    /// </summary>
    /// <param name="rightHand"></param>
    public void CheckDragAndDropGesture(Hand rightHand)
    {
        if (rightHand.PalmNormal.y < -0.7) //if hand is "looking" down to the picture
        {
            if (rightHand.GrabStrength > 0.6) //if hand is grabbing
            {
                Vector3 distance = rightHand.PalmPosition.ToUnity() - picture.transform.position; //distance between hand and picture
                if (distance.magnitude < 0.7)
                {
                    currentPicturePosition.x = rightHand.PalmPosition.x;
                    currentPicturePosition.z = rightHand.PalmPosition.z;
                    currentPicturePosition.y = rightHand.PalmPosition.y - 0.2f; //picture will be placed directly below the right hand
                    picture.transform.position = picture.transform.position;
                }
            }
        }
    }
}
