using UnityEngine;
using System.Collections.Generic;
using Leap;
using LeapInternal;

public class LeapController : MonoBehaviour
{
    Leap.Controller controller;
    LeapProvider leapProvider;
    public GameObject picture;
    Vector3 picturePosition;

    void Start()
    {
        controller = new Controller();
        leapProvider = FindObjectOfType<LeapProvider>() as LeapProvider;
        if (!controller.IsConnected)
        {
            Debug.LogError("no LeapMotion Device detected...");
        }
        else {
            Debug.Log("Device connected, continue");
        }
        picturePosition = picture.transform.position;

    }

    void Update()
    {

        Frame frame = leapProvider.CurrentFrame;
        foreach (Hand hand in frame.Hands)
        {
            if (hand.IsRight)
            {
                float grabStrength = hand.GrabStrength;
                if (grabStrength > 0.6)
                {
                    Debug.Log("Grab detected... strength is:" + grabStrength);
                    Vector3 unityLeap = hand.PalmPosition.ToUnity();
                    Vector3 distance = unityLeap - picture.transform.position;
                    Debug.Log("distance is " + distance.magnitude);
                    if (distance.magnitude < 0.7)
                    {
                        picturePosition.x = hand.PalmPosition.x;
                        picturePosition.z = hand.PalmPosition.z;
                        picturePosition.y = hand.PalmPosition.y - 0.2f;
                        picture.transform.position = picturePosition;
                    }
                }

            }
        }
    }


}
