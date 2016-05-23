using UnityEngine;
using System.Collections;
using Leap;

/// <summary>
/// Autor: Tobi Rohrer
/// Class is used for the drag and drop gesture.
/// </summary>
public class LeapDragAndDrop : MonoBehaviour
{

    private LeapProvider leapProvider;

    public LeapDragAndDrop(LeapProvider leapProvider)
    {
        this.leapProvider = leapProvider;
    }

    /// <summary>
    /// Return the hand if it is the right, else null.
    /// </summary>
    /// <returns></returns>
    private Hand GetRightHand()
    {
        Frame frame = leapProvider.CurrentFrame;
        foreach (Hand hand in frame.Hands)
        {
            if (hand.IsRight)
                return hand;
        }
        return null;
    }

    /// <summary>
    /// Function called to check wether to drag and drop the picture if the user is "looking" at it. If so, the picture is dragged.
    /// </summary>
    /// <param name="rightHand"></param>
    public void CheckDragAndDropGesture(GameObject picture)
    {
        Hand rightHand = GetRightHand();
        Vector3 newPicturePosition;
        if (rightHand.GrabStrength > 0.6) //if hand is grabbing
        {
            newPicturePosition.x = rightHand.PalmPosition.x;
            newPicturePosition.z = rightHand.PalmPosition.z;
            newPicturePosition.y = rightHand.PalmPosition.y - 0.2f; //picture will be placed directly below the right hand
            picture.transform.position = newPicturePosition;
        }
    }

}
