using UnityEngine;
using System.Collections;
using Leap;
using System.Collections.Generic;
using System.Timers;

public class LeapPageTurnerV2 : MonoBehaviour
{

    private MegaBookBuilder book;
    private AudioSource pageTurnSound;
    private Leap.Controller controller;
    private GestureQueV2 RightHandgestureQue = new GestureQueV2();
    private ProbabilityTracker probabilityTracker = new ProbabilityTracker();
    private int rightUpGestureProbability;
    private int rightDownGestureProbability;
    private int leftDownGestureProbability;
    private Timer circleProbabilityTimer;
    private Timer fastForwardTimer;
    private bool fastForward = false;
    private bool fastForwardTimerActive = false;

    public LeapPageTurnerV2(MegaBookBuilder book, Leap.Controller controller, AudioSource pageTurnSound)
    {
        this.book = book;
        this.pageTurnSound = pageTurnSound;
        this.controller = controller;
        initializeCircleProbabilityTimer();
    }

    /// <summary>
    /// Decreases the probability of a circle gesture beein currently done. Probability is increased by every "circle gesture".
    /// </summary>
    private void initializeCircleProbabilityTimer()
    {
        circleProbabilityTimer = new Timer(2000); //Set Timer intervall 
        circleProbabilityTimer.Elapsed += decreaseCircleProbability; // Hook up the method to the timer
        circleProbabilityTimer.Enabled = true;
    }

    private void decreaseCircleProbability(object sender, ElapsedEventArgs e)
    {
        if (probabilityTracker.getCircleMovement() > 0)
        {
            probabilityTracker.decreaseCircleMovement();
        }
    }

    private void initilizeFastForwardTimer()
    {
        fastForwardTimer = new Timer(100); //Set Timer intervall 
        fastForwardTimer.Elapsed += enableFastForward; // Hook up the method to the timer
        fastForwardTimer.Enabled = true;
    }

    private void enableFastForward(object sender, ElapsedEventArgs e)
    {
        fastForward = true;
    }


    private void circleChecker()
    {
        //Check forward Gesture
        if (RightHandgestureQue.getLastGesture() == "rightUp" && RightHandgestureQue.getOldGesture() == "leftUp" && RightHandgestureQue.getOldestGesture() == "leftDown")
        {
            book.NextPage(pageTurnSound);
            probabilityTracker.increaseCircleMovement();
            RightHandgestureQue.setOldGesture("done");
        }
        if (RightHandgestureQue.getLastGesture() == "rightDown" && RightHandgestureQue.getOldGesture() == "rightUp" && RightHandgestureQue.getOldestGesture() == "leftUp")
        {
            book.NextPage(pageTurnSound);
            probabilityTracker.increaseCircleMovement();
            RightHandgestureQue.setOldGesture("done");
        }
        if (RightHandgestureQue.getLastGesture() == "leftDown" && RightHandgestureQue.getOldGesture() == "rightDown" && RightHandgestureQue.getOldestGesture() == "rightUp")
        {
            book.NextPage(pageTurnSound);
            probabilityTracker.increaseCircleMovement();
            RightHandgestureQue.setOldGesture("done");
        }
        if (RightHandgestureQue.getLastGesture() == "leftUp" && RightHandgestureQue.getOldGesture() == "leftDown" && RightHandgestureQue.getOldestGesture() == "rightDown")
        {
            book.NextPage(pageTurnSound);
            probabilityTracker.increaseCircleMovement();
            RightHandgestureQue.setOldGesture("done");
        }

        //Check backward gesture
        if (RightHandgestureQue.getLastGesture() == "leftUp" && RightHandgestureQue.getOldGesture() == "rightUp" && RightHandgestureQue.getOldestGesture() == "rightDown")
        {
            book.PrevPage(pageTurnSound);
            probabilityTracker.increaseCircleMovement();
            RightHandgestureQue.setOldGesture("done");
        }
        if (RightHandgestureQue.getLastGesture() == "rightUp" && RightHandgestureQue.getOldGesture() == "rightDown" && RightHandgestureQue.getOldestGesture() == "leftDown")
        {
            book.PrevPage(pageTurnSound);
            probabilityTracker.increaseCircleMovement();
            RightHandgestureQue.setOldGesture("done");
        }
        if (RightHandgestureQue.getLastGesture() == "rightDown" && RightHandgestureQue.getOldGesture() == "leftDown" && RightHandgestureQue.getOldestGesture() == "leftUp")
        {
            book.PrevPage(pageTurnSound);
            probabilityTracker.increaseCircleMovement();
            RightHandgestureQue.setOldGesture("done");
        }
        if (RightHandgestureQue.getLastGesture() == "leftDown" && RightHandgestureQue.getOldGesture() == "leftUp" && RightHandgestureQue.getOldestGesture() == "rightUp")
        {
            book.PrevPage(pageTurnSound);
            probabilityTracker.increaseCircleMovement();
            RightHandgestureQue.setOldGesture("done");
        }

    }

    private Hand getHand(int frameID, string leftOrRight)
    {
        Hand rightHand = null;
        Hand leftHand = null;
        foreach (Hand hand in controller.Frame(frameID).Hands)
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

        if (leftOrRight == "left")
        {
            return leftHand;
        }
        else {
            return rightHand;
        }
    }

    private void updateHandMovementProbability(Hand currentHand, Hand oldHand, string leftOrRight)
    {
        Vector currentHandPosition = currentHand.PalmPosition;
        Vector oldHandPosition = oldHand.PalmPosition;

        //LeftUp Gesture
        if (currentHandPosition.x > oldHandPosition.x && currentHandPosition.z < oldHandPosition.z)
        {
            if (leftOrRight == "right")
            {
                probabilityTracker.increaseRightHandLeftUp();
            }
            if (leftOrRight == "left")
            {
                probabilityTracker.increaseLeftHandLeftUp();
            }
        }

        else if (currentHandPosition.x > oldHandPosition.x || currentHandPosition.z < oldHandPosition.z)
        {
        }
        else {
            if (leftOrRight == "right" && probabilityTracker.getRightHandLeftUp() != 0)
            {
                probabilityTracker.decreaseRightHandLeftUp();
            }
            if (leftOrRight == "left" && probabilityTracker.getLeftHandLeftUp() != 0)
            {
                probabilityTracker.decreaseLeftHandLeftUp();
            }

        }

        //rightUp Gesture
        if (currentHandPosition.x < oldHandPosition.x && currentHandPosition.z < oldHandPosition.z)
        {
            if (leftOrRight == "right")
            {
                probabilityTracker.increaseRightHandRightUp();
            }
            if (leftOrRight == "left")
            {
                probabilityTracker.increaseLeftHandRightUp();
            }
        }
        else if (currentHandPosition.x < oldHandPosition.x || currentHandPosition.z < oldHandPosition.z)
        {
        }
        else {
            if (leftOrRight == "right" && probabilityTracker.getRightHandRightUp() != 0)
            {
                probabilityTracker.decreaseRightHandRightUp();
            }
            if (leftOrRight == "left" && probabilityTracker.getLeftHandRightUp() != 0)
            {
                probabilityTracker.decreaseLeftHandRightUp();
            }
        }

        //rightDown
        if (currentHandPosition.x < oldHandPosition.x && currentHandPosition.z > oldHandPosition.z)
        {
            if (leftOrRight == "right")
            {
                probabilityTracker.increaseRightHandRightDown();
            }
            if (leftOrRight == "left")
            {
                probabilityTracker.increaseLeftHandRightDown();
            }
        }
        else if (currentHandPosition.x < oldHandPosition.x || currentHandPosition.z > oldHandPosition.z)
        {
        }
        else {
            if (leftOrRight == "right" && probabilityTracker.getRightHandRightDown() != 0)
            {
                probabilityTracker.decreaseRightHandRightDown();
            }
            if (leftOrRight == "left" && probabilityTracker.getLeftHandRightDown() != 0)
            {
                probabilityTracker.decreaseLeftHandRightDown();
            }
        }

        //leftDown
        if (currentHandPosition.x > oldHandPosition.x && currentHandPosition.z > oldHandPosition.z)
        {
            if (leftOrRight == "right")
            {
                probabilityTracker.increaseRightHandLeftDown();
            }
            if (leftOrRight == "left")
            {
                probabilityTracker.increaseLeftHandLeftDown();
            }
        }
        else if (currentHandPosition.x > oldHandPosition.x || currentHandPosition.z > oldHandPosition.z)
        {
        }
        else {
            if (leftOrRight == "right" && probabilityTracker.getRightHandLeftDown() != 0)
            {
                probabilityTracker.decreaseRightHandLeftDown();
            }
            if (leftOrRight == "left" && probabilityTracker.getLeftHandLeftDown() != 0)
            {
                probabilityTracker.decreaseLeftHandLeftDown();
            }
        }
    }

    private void updateGestureQue(Hand currentHand, Hand veryOldHand, string leftOrRight)
    {

        Vector distance = currentHand.PalmPosition - veryOldHand.PalmPosition;

        if (probabilityTracker.getRightHandLeftUp() > 14)
        {
            if (distance.Magnitude > 27)
            {
                Debug.Log("leftUp");
                probabilityTracker.nullRightHand();
                circleForcaster("leftUp");
                RightHandgestureQue.setLastGesture("leftUp");
            }
        }

        if (probabilityTracker.getRightHandRightUp() > 14)
        {
            if (distance.Magnitude > 27)
            {
                Debug.Log("rightUp");
                probabilityTracker.nullRightHand();
                circleForcaster("rightUp");
                RightHandgestureQue.setLastGesture("rightUp");
            }
        }

        if (probabilityTracker.getRightHandRightDown() > 14)
        {
            if (distance.Magnitude > 27)
            {
                Debug.Log("rightDown");
                probabilityTracker.nullRightHand();
                circleForcaster("rightDown");
                RightHandgestureQue.setLastGesture("rightDown");
            }
        }

        if (probabilityTracker.getRightHandLeftDown() > 14)
        {
            if (distance.Magnitude > 27)
            {
                Debug.Log("leftDown");
                probabilityTracker.nullRightHand();
                circleForcaster("leftDown");
                RightHandgestureQue.setLastGesture("leftDown");
            }
        }
    }

    private void circleForcaster(string lastDirection)
    {

        if (lastDirection == "leftDown")
        {
            probabilityTracker.setRightHandRightUp(-10);
        }
        if (lastDirection == "rightDown")
        {
            probabilityTracker.setRightHandLeftUp(-10);
        }
        if (lastDirection == "rightUp")
        {
            probabilityTracker.setRightHandLeftDown(-10);
        }
        if (lastDirection == "leftUp")
        {
            probabilityTracker.setRightHandRightDown(-10);
        }

        if (RightHandgestureQue.getLastGesture() == "rightUp" && RightHandgestureQue.getOldGesture() == "leftUp")
        {
            if (RightHandgestureQue.getOldestGesture() == "leftDown")
            {
                probabilityTracker.setRightHandRightDown(8);
            }
            else {
                probabilityTracker.setRightHandRightDown(5);
            }

        }
        if (RightHandgestureQue.getLastGesture() == "rightDown" && RightHandgestureQue.getOldGesture() == "rightUp")
        {
            if (RightHandgestureQue.getOldestGesture() == "leftUp")
            {
                probabilityTracker.setRightHandLeftDown(8);
            }
            else {
                probabilityTracker.setRightHandLeftDown(5);
            }
        }
        if (RightHandgestureQue.getLastGesture() == "leftDown" && RightHandgestureQue.getOldGesture() == "rightDown")
        {
            if (RightHandgestureQue.getOldestGesture() == "rightUp")
            {
                probabilityTracker.setRightHandLeftUp(8);
            }
            else {
                probabilityTracker.setRightHandLeftUp(5);
            }
            
        }
        if (RightHandgestureQue.getLastGesture() == "leftUp" && RightHandgestureQue.getOldGesture() == "leftDown")
        {
            if (RightHandgestureQue.getOldestGesture() == "rightDown")
            {
                probabilityTracker.setRightHandRightUp(8);
            }
            else {
                probabilityTracker.setRightHandRightUp(5);
            }
            
        }

    }

    public void fastPageTurnChecker(Hand currentHand)
    {
        Vector palmVelocity = currentHand.PalmVelocity;

        if ((palmVelocity.x > 1100 || palmVelocity.x < -1100) && probabilityTracker.getCircleMovement() > 2)
        {
            //Debug.Log("ultra fast Forward");
        }
        else if ((palmVelocity.x > 800 || palmVelocity.x < -800) && probabilityTracker.getCircleMovement() > 2)
        {
            if (fastForwardTimerActive == false)
            {
                initilizeFastForwardTimer();
                fastForwardTimerActive = true;
                Debug.Log("fast forward");
            }
            if (fastForward == true)
            {
                fastForward = false;
                book.NextPage(pageTurnSound);
            }
        }

        if (probabilityTracker.getCircleMovement() == 0 && fastForwardTimer != null)
        {
            fastForwardTimer.Close();
            fastForwardTimerActive = false;
        }

    }

    public void CheckPageTurnGesture(List<Hand> hands)
    {
        Hand rightCurrentHand = getHand(0, "right");
        Hand rightVeryOldHand = getHand(10, "right");
        Hand rightOldHand = getHand(1, "right");

        Hand leftCurrentHand = getHand(0, "left");
        Hand leftVeryOldHand = getHand(10, "left");
        Hand leftOldHand = getHand(1, "left");

        if (rightCurrentHand != null && rightVeryOldHand != null && rightOldHand != null)
        {

            updateHandMovementProbability(rightCurrentHand, rightOldHand, "right");
            updateGestureQue(rightCurrentHand, rightVeryOldHand, "right");
            fastPageTurnChecker(rightCurrentHand);
            circleChecker();
        }

    }
}
