using UnityEngine;
using System.Collections.Generic;
using Leap;
using LeapInternal;
using System;

public class LeapController : MonoBehaviour
{
    Leap.Controller controller;
    LeapProvider leapProvider;
    public GameObject picture;
    public MegaBookBuilder book;
    public AudioSource pageTurnSound;
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
                TurnOverChecker(hand);
                

            }
        }
    }

    private void TurnOverChecker(Hand rightHand) {


        float palmVelocityX = rightHand.PalmVelocity.x;
        if (palmVelocityX > 4)
        {
            try
            {
                book.PrevPage(pageTurnSound);
            }
            catch (Exception e)
            {
                Debug.LogError(e.GetBaseException());
            }
        }
        if (palmVelocityX < -2)
        {
            try
            {
                book.NextPage(pageTurnSound);
            }
            catch (Exception e)
            {
                Debug.LogError(e.GetBaseException());
            }
        }



    }

    private void DragAndDropChecker(Hand rightHand) {
        float grabStrength = rightHand.GrabStrength;
        if (grabStrength > 0.6)
        {
            //Debug.Log("Grab detected... strength is:" + grabStrength);
            Vector3 unityLeap = rightHand.PalmPosition.ToUnity();
            Vector3 distance = unityLeap - picture.transform.position;
            //Debug.Log("distance is " + distance.magnitude);
            if (distance.magnitude < 0.7)
            {
                picturePosition.x = rightHand.PalmPosition.x;
                picturePosition.z = rightHand.PalmPosition.z;
                picturePosition.y = rightHand.PalmPosition.y - 0.2f;
                picture.transform.position = picturePosition;
            }
        }
    }


}
