using UnityEngine;
using System.Collections.Generic;
using Leap;
using LeapInternal;
using System;
using System.Threading;

public class LeapController : MonoBehaviour
{
    private Leap.Controller controller;
    private LeapProvider leapProvider;
    public GameObject picture;
    public MegaBookBuilder book;
    public AudioSource pageTurnSoundSlow;
    public AudioSource pageTurnSoundFast;
    private Vector3 picturePosition;
    bool pictureCurrentlyDragged;
    private LeapPageTurner leapPageTurner;

    void Start()
    {

        pictureCurrentlyDragged = false;
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
        leapPageTurner = new LeapPageTurner(book, controller, pageTurnSoundSlow, pageTurnSoundFast);

    }

    void Update()
    {
        Frame frame = leapProvider.CurrentFrame;

        if (!pictureCurrentlyDragged)
        {
            leapPageTurner.CheckPageTurnGesture(frame.Hands);
        }

        foreach (Hand hand in frame.Hands)
        {
            if (hand.IsRight)
            {
                DragAndDropChecker(hand);
            }
        }


    }

    //ToDo: put function below into a own class

    private void DragAndDropChecker(Hand rightHand)
    {
        pictureCurrentlyDragged = false;
        float grabStrength = rightHand.GrabStrength;
        if (rightHand.PalmNormal.y < -0.7)
        {
            if (grabStrength > 0.6)
            {
                //Debug.Log("Grab detected... strength is:" + grabStrength);
                Vector3 unityLeap = rightHand.PalmPosition.ToUnity();
                Vector3 distance = unityLeap - picture.transform.position;
                //Debug.Log("distance is " + distance.magnitude);
                if (distance.magnitude < 0.7)
                {
                    pictureCurrentlyDragged = true;
                    picturePosition.x = rightHand.PalmPosition.x;
                    picturePosition.z = rightHand.PalmPosition.z;
                    picturePosition.y = rightHand.PalmPosition.y - 0.2f;
                    picture.transform.position = picturePosition;
                }
            }
        }
    }

}
