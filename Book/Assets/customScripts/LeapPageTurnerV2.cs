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
    private int leftUpGestureProbability = 0;
    private int rightUpGestureProbability = 0;
    private int rightDownGestureProbability = 0;
    private int leftDownGestureProbability = 0;
    private GestureQue gestureQue = new GestureQue();
    private Timer pageTurnerTimer;

    public LeapPageTurnerV2(MegaBookBuilder book, Leap.Controller controller, AudioSource pageTurnSound)
    {
        this.book = book;
        this.pageTurnSound = pageTurnSound;
        this.controller = controller;
    }

    private void InitFastTurnTimer()
    {
        pageTurnerTimer = new Timer(300); //Set Timer intervall 
        pageTurnerTimer.Elapsed += EnablePageFastTurn; // Hook up the method to the timer
        pageTurnerTimer.Enabled = true;
    }

    private void EnablePageFastTurn(object sender, ElapsedEventArgs e)
    {
        //ToDo: Do I really need the if? What is when the book is at 0?
        if (book.GetPageCount() == (int)book.GetPage())
        {
            pageTurnerTimer.Close();
        }
        else {
            //turnNextPage = true;
        }
    }


    private void circleChecker()
    {
        //Check forward Gesture
        if (gestureQue.getLastGesture() == "rightUp" && gestureQue.getOldGesture() == "leftUp")
        {
            book.NextPage(pageTurnSound);
            gestureQue.setOldGesture("done");
        }
        if (gestureQue.getLastGesture() == "rightDown" && gestureQue.getOldGesture() == "rightUp")
        {
            book.NextPage(pageTurnSound);
            gestureQue.setOldGesture("done");
        }
        if (gestureQue.getLastGesture() == "leftDown" && gestureQue.getOldGesture() == "rightDown")
        {
            book.NextPage(pageTurnSound);
            gestureQue.setOldGesture("done");
        }
        if (gestureQue.getLastGesture() == "leftUp" && gestureQue.getOldGesture() == "rightDown")
        {
            book.NextPage(pageTurnSound);
            gestureQue.setOldGesture("done");
        }

        //Check backward gesture
        if (gestureQue.getLastGesture() == "leftUp" && gestureQue.getOldGesture() == "rightUp")
        {
            book.PrevPage(pageTurnSound);
            gestureQue.setLastGesture("done");
        }
        if (gestureQue.getLastGesture() == "rightUp" && gestureQue.getOldGesture() == "rightDown")
        {
            book.PrevPage(pageTurnSound);
            gestureQue.setLastGesture("done");
        }
        if (gestureQue.getLastGesture() == "rightDown" && gestureQue.getOldGesture() == "leftDown")
        {
            book.PrevPage(pageTurnSound);
            gestureQue.setLastGesture("done");
        }
        if (gestureQue.getLastGesture() == "rightDown" && gestureQue.getOldGesture() == "leftUp")
        {
            book.PrevPage(pageTurnSound);
            gestureQue.setLastGesture("done");
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

    public void CheckPageTurnGesture(List<Hand> hands)
    {
        Hand currentHand = getHand(0, "right");
        Hand veryOldHand = getHand(10, "right");
        Hand oldHand = getHand(1, "right");
        if (currentHand != null && veryOldHand != null && oldHand != null)
        {
            Vector currentHandPosition = currentHand.PalmPosition;
            Vector oldHandPosition = oldHand.PalmPosition;

            if (currentHandPosition.x > oldHandPosition.x && currentHandPosition.z < oldHandPosition.z)
            {
                leftUpGestureProbability++;
            }
            else if (currentHandPosition.x > oldHandPosition.x || currentHandPosition.z < oldHandPosition.z)
            {
            }
            else {
                if (leftUpGestureProbability != 0)
                {
                    leftUpGestureProbability--;
                }
            }

            if (leftUpGestureProbability > 14)
            {
                Debug.Log("leftUpProbability greater 12");
                veryOldHand = getHand(10, "right");
                Vector distance = currentHandPosition - veryOldHand.PalmPosition;
                if (distance.Magnitude > 27)
                {
                    Debug.Log("leftUp");
                    leftUpGestureProbability = 0;
                    gestureQue.setLastGesture("leftUp");
                }
            }
            if (currentHandPosition.x < oldHandPosition.x && currentHandPosition.z < oldHandPosition.z)
            {
                rightUpGestureProbability++;
            }
            else if (currentHandPosition.x < oldHandPosition.x || currentHandPosition.z < oldHandPosition.z)
            {
            }
            else {
                if (rightUpGestureProbability != 0)
                {
                    rightUpGestureProbability--;
                }
            }
            if (rightUpGestureProbability > 14)
            {
                veryOldHand = getHand(10, "right");
                Vector distance = currentHandPosition - veryOldHand.PalmPosition;
                if (distance.Magnitude > 27)
                {
                    Debug.Log("rightUp");
                    rightUpGestureProbability = 0;
                    gestureQue.setLastGesture("rightUp");
                }
            }
            if (currentHandPosition.x < oldHandPosition.x && currentHandPosition.z > oldHandPosition.z)
            {
                rightDownGestureProbability++;
            }
            else if (currentHandPosition.x < oldHandPosition.x || currentHandPosition.z > oldHandPosition.z)
            {
            }
            else {
                if (rightDownGestureProbability != 0)
                {
                    rightDownGestureProbability--;
                }
            }
            if (rightDownGestureProbability > 14)
            {
                veryOldHand = getHand(10, "right");
                Vector distance = currentHandPosition - veryOldHand.PalmPosition;
                if (distance.Magnitude > 27)
                {
                    Debug.Log("rightDown");
                    rightDownGestureProbability = 0;
                    gestureQue.setLastGesture("rightDown");
                }
            }

            if (currentHandPosition.x > oldHandPosition.x && currentHandPosition.z > oldHandPosition.z)
            {
                leftDownGestureProbability++;
            }
            else if (currentHandPosition.x > oldHandPosition.x || currentHandPosition.z > oldHandPosition.z)
            {
            }
            else {
                if (leftDownGestureProbability != 0)
                {
                    leftDownGestureProbability--;
                }
            }
            if (leftDownGestureProbability > 14)
            {
                veryOldHand = getHand(10, "right");
                Vector distance = currentHandPosition - veryOldHand.PalmPosition;
                if (distance.Magnitude > 27)
                {
                    Debug.Log("leftDown");
                    leftDownGestureProbability = 0;
                    gestureQue.setLastGesture("leftDown");
                }
            }

            circleChecker();
        }
    }
}
