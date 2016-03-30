using UnityEngine;
using System.Collections.Generic;
using Leap;
using LeapInternal;
using System;

public class LeapController : MonoBehaviour
{
    private Leap.Controller controller;
    private LeapProvider leapProvider;
    public GameObject picture;
    public MegaBookBuilder book;
    public AudioSource pageTurnSoundSlow;
    public AudioSource pageTurnSoundFast;
    private Vector3 picturePosition;
    bool singePageTurnDone;
    bool pictureCurrentlyDragged;
    int frameCount;

    void Start()
    {
        singePageTurnDone = false;
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

    }

    void Update()
    {
        Frame frame = leapProvider.CurrentFrame;
        if(!pictureCurrentlyDragged)
        TurnOverChecker(frame.Hands);
        //used to disable multiple Page Turns when the gesture for one Page turn is used.
        frameCount++;
        if (frameCount % 10 == 1)
        {
            singePageTurnDone = false;
        }

        
        foreach (Hand hand in frame.Hands)
        {
            if (hand.IsRight)
            {

                DragAndDropChecker(hand);

            }
        }
        

    }

    private void TurnOverChecker(List<Hand> hands)
    {
        Hand rightHand = null;
        Hand leftHand = null;
        float palmVelocityRightX;
        float palmVelocityLeftX;
        Finger triggerFingerRight = null;
        Finger triggerFingerLeft = null;
        float oldTriggerFingerDirectionRightX = 0;
        float oldTriggerFingerDirectionLeftX = 0;

        //Getting older Frame to determine wether the Trigger finger was moving !
        List<Hand> oldHandList = controller.Frame(4).Hands;
        foreach (Hand hand in oldHandList)
        {
            if (hand.IsRight)
            {
                oldTriggerFingerDirectionRightX = hand.Fingers[1].Direction.x;
            }
            if (hand.IsLeft)
            {
                oldTriggerFingerDirectionLeftX = hand.Fingers[1].Direction.x;
            }
        }

        foreach (Hand hand in hands)
        {
            if (hand.IsLeft)
            {
                leftHand = hand;
            }
            if (hand.IsRight)
            {
                rightHand = hand;
            }
        }


        if (rightHand != null)
        {
            palmVelocityRightX = rightHand.PalmVelocity.x;
            if (palmVelocityRightX < -2)
            {
                try
                {
                    book.NextPage(pageTurnSoundFast);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.GetBaseException());
                }
            }
            if (rightHand.Fingers[1] != null)
            {
                triggerFingerRight = rightHand.Fingers[1];
                if (oldTriggerFingerDirectionRightX - triggerFingerRight.Direction.x > 0.4)
                {
                    if (singePageTurnDone == false)
                    {
                        book.NextPage(pageTurnSoundSlow);
                        singePageTurnDone = true;
                    }
                }
            }
        }

        if (leftHand != null)
        {
            palmVelocityLeftX = leftHand.PalmVelocity.x;
            if (palmVelocityLeftX > 4)
            {
                try
                {
                    book.PrevPage(pageTurnSoundFast);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.GetBaseException());
                }
            }
            if (leftHand.Fingers[1] != null)
            {
                triggerFingerLeft = leftHand.Fingers[1];
                if (oldTriggerFingerDirectionLeftX - triggerFingerLeft.Direction.x < -0.4)
                {
                    if (singePageTurnDone == false)
                    {   
                        book.PrevPage(pageTurnSoundSlow);
                        singePageTurnDone = true;
                    }
                }
            }
        }

    }

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
