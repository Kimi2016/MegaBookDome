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

public class LeapPageTurner : MonoBehaviour
{

    MegaBookBuilder book;
    AudioSource pageTurnSound;
    private AudioSource pageTurnSoundSlow;
    private AudioSource pageTurnSoundFast;
    private Leap.Controller controller;
    private Timer gesturesTimer;
    private Timer fastPageTurnerTimer;
    private bool gesturesEnabled;
    private bool turnNextPage;
    private bool turnForward;
    private bool turnBackward;
    private bool grabbing;

    public LeapPageTurner(MegaBookBuilder book, Leap.Controller controller, AudioSource pageTurnSoundSlow, AudioSource pageTurnSoundFast)
    {
        this.book = book;
        this.controller = controller;
        this.pageTurnSoundSlow = pageTurnSoundSlow;
        this.pageTurnSoundFast = pageTurnSoundFast;
        gesturesEnabled = true;
        turnNextPage = false;
        turnForward = false;
        turnBackward = false;
        book.SetTurnTime(0.2f);
    }

    /// <summary>
    /// Function initializes a timer which disables all Gestures used for "PageTurning" for a specific time.
    /// </summary>
    /// <param name="timeIntervall"></param>
    private void DisableGestures(int timeIntervall)
    {
        try
        {
            gesturesTimer.Close();
        }
        catch
        {
        }
        gesturesEnabled = false;
        gesturesTimer = new Timer(timeIntervall); //Set Timer intervall 
        gesturesTimer.Elapsed += EnableGestures; // Hook up the method to the timer
        gesturesTimer.Enabled = true;
    }

    /// <summary>
    /// Fuction is called by the gesturesTimer when one timeintervall is finished. It is used to unlock gestures after they have been locked by the InitDisableGesturesTimer() function.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void EnableGestures(object sender, ElapsedEventArgs e)
    {
        gesturesEnabled = true;
        gesturesTimer.Close();
    }

    /// <summary>
    /// Initializes a timer which is used for the fast page turn gesture. 
    /// The timer is necessary so not all pages are turned at once.
    /// If all page turns would be called at once in a for loop without a timer, it would not be possible to stop the turning pages during the animation.
    /// </summary>
    private void InitFastTurnTimer()
    {
        fastPageTurnerTimer = new Timer(300); //Set Timer intervall 
        fastPageTurnerTimer.Elapsed += EnablePageFastTurn; // Hook up the method to the timer
        fastPageTurnerTimer.Enabled = true;
    }

    /// <summary>
    /// Fuction is called by the FastPageTurnerTimer when one timeintervall is finished. It is used to turn pages smoothly and allow and interaction with the "stop" gesture.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void EnablePageFastTurn(object sender, ElapsedEventArgs e)
    {
        //ToDo: Do I really need the if? What is when the book is at 0?
        if (book.GetPageCount() == (int)book.GetPage())
        {
            fastPageTurnerTimer.Close();
        }
        else {
            turnNextPage = true;
        }
    }

    /// <summary>
    /// Function is used to determine the right timing for the next Page turn in case of a fast turn gestuer. 
    /// The NextPage() function of the book is only called once in a time intervall defined within the InitFastTurnTimer() function.
    /// </summary>
    private void CheckRemainingFastTurnGesture()
    {
        if (turnNextPage && turnForward)
        {
            book.NextPage(pageTurnSoundSlow);
            turnNextPage = false;
        }
        if (turnNextPage && turnBackward)
        {
            book.PrevPage(pageTurnSoundSlow);
            turnNextPage = false;
        }

    }

    private void CheckStopGesture(Hand hand)
    {
        
        if (hand.GrabStrength > 0.8)
        {
            if (turnForward || turnBackward)
            {
                turnForward = false;
                turnBackward = false;
                grabbing = true;
                fastPageTurnerTimer.Close();
            }

        }
        else {
            grabbing = false;
        }

    }


    /// <summary>
    /// Function is called by the Controller every Frame. It is used to detect Gestures and to trigger to trigger the corresponding functions.
    /// </summary>
    /// <param name="hands"></param>
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
            //Gesture for stopping Fast Page Turn
            //ToDo: Change gesture to something else.
            CheckStopGesture(rightHand);

            palmVelocityRightX = rightHand.PalmVelocity.x;

            //Gesture for starting the Fast Page Turn
            if (palmVelocityRightX < -2)
            {
                if (gesturesEnabled == true)
                {

                    InitFastTurnTimer();
                    turnForward = true;
                    turnBackward = false;
                    DisableGestures(1000);
                }
            }

            //Gesture for turning a Single Page
            if (rightHand.Fingers[1] != null)
            {
                triggerFingerRight = rightHand.Fingers[1];
                if (oldTriggerFingerDirectionRightX - triggerFingerRight.Direction.x > 0.4)
                {
                    if (gesturesEnabled == true)
                    {
                        book.NextPage(pageTurnSoundSlow);
                        DisableGestures(200);
                    }
                }
            }
        }


        if (leftHand != null)
        {
            CheckStopGesture(leftHand);
            palmVelocityLeftX = leftHand.PalmVelocity.x;
            if (palmVelocityLeftX > 4)
            {
                if (gesturesEnabled)
                {
                    InitFastTurnTimer();
                    turnForward = false;
                    turnBackward = true;
                    DisableGestures(1000);
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
                        DisableGestures(200);
                    }

                }
            }
        }
    }

}



