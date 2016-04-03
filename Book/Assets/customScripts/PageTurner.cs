using UnityEngine;
using System.Collections;
using System;
using Leap;
using System.Collections.Generic;
using System.Timers;

/// <summary>
/// Autor: Tobi Rohrer
/// Class is used to manage the gestures used to turn pages of the book.
/// </summary>

public class PageTurner : MonoBehaviour
{

    MegaBookBuilder book;
    AudioSource pageTurnSound;
    public AudioSource pageTurnSoundSlow;
    public AudioSource pageTurnSoundFast;
    private Leap.Controller controller;
    private Timer gesturesTimer;
    private Timer fastPageTurnerTimer;
    private bool gesturesEnabled;
    private bool turnNextPage;

    public PageTurner(MegaBookBuilder book, Leap.Controller controller, AudioSource pageTurnSoundSlow, AudioSource pageTurnSoundFast)
    {
        this.book = book;
        this.controller = controller;
        this.pageTurnSoundSlow = pageTurnSoundSlow;
        this.pageTurnSoundFast = pageTurnSoundFast;
        gesturesEnabled = true;
        book.SetTurnTime(0.2f);
    }

    private void InitDisableGesturesTimer(int timeIntervall)
    {
        gesturesTimer = new Timer(timeIntervall); //Set Timer intervall 
        gesturesTimer.Elapsed += EnableGestures; // Hook up the method to the timer
        gesturesTimer.Enabled = true;
    }

    private void InitFastTurnTimer()
    {
        fastPageTurnerTimer = new Timer(500); //Set Timer intervall 
        fastPageTurnerTimer.Elapsed += EnablePageFastTurn; // Hook up the method to the timer
        fastPageTurnerTimer.Enabled = true;
    }

    private void EnablePageFastTurn(object sender, ElapsedEventArgs e)
    {
        turnNextPage = true;
        if (book.GetPageCount() == (int)book.GetPage())
        {
            fastPageTurnerTimer.Close();
        }
    }

    private void EnableGestures(object sender, ElapsedEventArgs e)
    {
        gesturesEnabled = true;
        gesturesTimer.Close();
    }

    private void CheckRemainingFastTurnGesture() {
        if (turnNextPage == true){
            book.NextPage(pageTurnSoundSlow);
            turnNextPage = false;
        }
    }

    public void CheckPageTurnGesture(List<Hand> hands)
    {
        Hand rightHand = null;
        Hand leftHand = null;
        float palmVelocityRightX;
        float palmVelocityLeftX;
        Finger triggerFingerRight = null;
        Finger triggerFingerLeft = null;
        float oldTriggerFingerDirectionRightX = 0;
        float oldTriggerFingerDirectionLeftX = 0;

        CheckRemainingFastTurnGesture();

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

        //defining left and right hand
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

            if (rightHand.GrabStrength > 0.8)
            {
                fastPageTurnerTimer.Close();
                InitDisableGesturesTimer(3000);
            }

            palmVelocityRightX = rightHand.PalmVelocity.x;

            if (palmVelocityRightX < -2)
            {
                if (gesturesEnabled == true)
                {
                    
                    InitFastTurnTimer();
                    gesturesEnabled = false;
                    InitDisableGesturesTimer(1000); 
                }
            }

            if (rightHand.Fingers[1] != null)
            {
                triggerFingerRight = rightHand.Fingers[1];
                if (oldTriggerFingerDirectionRightX - triggerFingerRight.Direction.x > 0.4)
                {
                    if (gesturesEnabled == true)
                    {
                        book.NextPage(pageTurnSoundSlow);
                        gesturesEnabled = false;
                        InitDisableGesturesTimer(200);
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
                    if (gesturesEnabled == true)
                    {
                        book.PrevPage(pageTurnSoundSlow);
                        gesturesEnabled = false;
                        InitDisableGesturesTimer(200);
                    }

                }
            }
        }
    }

}



