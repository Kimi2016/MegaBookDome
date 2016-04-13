using UnityEngine;
using System.Collections;
using Leap;
using System.Collections.Generic;

public class LeapPageTurnerV2 : MonoBehaviour
{

    private MegaBookBuilder book;
    private AudioSource pageTurnSound;
    private Leap.Controller controller;
    private int leftUpGestureProbability = 0;
    private int rightUpGestureProbability = 0;
    private GestureQue gestureQue = new GestureQue();

    public LeapPageTurnerV2(MegaBookBuilder book, Leap.Controller controller, AudioSource pageTurnSound)
    {
        this.book = book;
        this.pageTurnSound = pageTurnSound;
        this.controller = controller;
    }


    private void circleChecker()
    {
        if (gestureQue.getLastGesture() == "rightUp" && gestureQue.getOldGesture() == "leftUp")
        {
            book.NextPage(pageTurnSound);
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

            if (currentHandPosition.x < oldHandPosition.x && currentHandPosition.y > oldHandPosition.y)
            {
                leftUpGestureProbability++;
            }
            else if (currentHandPosition.x < oldHandPosition.x || currentHandPosition.y > oldHandPosition.y)
            {
            }
            else {
                if (leftUpGestureProbability != 0)
                {
                    leftUpGestureProbability--;
                }
            }

            if (leftUpGestureProbability > 15)
            {
                veryOldHand = getHand(10, "right");
                Vector distance = currentHandPosition - veryOldHand.PalmPosition;
                if (distance.Magnitude > 20)
                {
                    Debug.Log("leftUp");
                    leftUpGestureProbability = 0;
                    gestureQue.setLastGesture("leftUp");
                }
            }

            if (currentHandPosition.x > oldHandPosition.x && currentHandPosition.y > oldHandPosition.y)
            {
                rightUpGestureProbability++;
            }
            else if (currentHandPosition.x > oldHandPosition.x || currentHandPosition.y > oldHandPosition.y)
            {
            }
            else {
                if (rightUpGestureProbability != 0)
                {
                    rightUpGestureProbability--;
                }
            }
            if (rightUpGestureProbability > 15)
            {
                veryOldHand = getHand(10, "right");
                Vector distance = currentHandPosition - veryOldHand.PalmPosition;
                if (distance.Magnitude > 40)
                {
                    Debug.Log("rightUp");
                    rightUpGestureProbability = 0;
                    gestureQue.setLastGesture("rightUp");
                }
            }
            circleChecker();
        }
    }
}
