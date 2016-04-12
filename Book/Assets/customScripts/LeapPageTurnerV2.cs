using UnityEngine;
using System.Collections;
using Leap;
using System.Collections.Generic;

public class LeapPageTurnerV2 : MonoBehaviour
{

    private MegaBookBuilder book;
    private AudioSource pageTurnSound;
    private Leap.Controller controller;
    private int leftUpGestureProbability;
    List<string> gestureHistory = new List<string>(); //´ToDO build own dataStructure ! 

    public LeapPageTurnerV2(MegaBookBuilder book, Leap.Controller controller, AudioSource pageTurnSound)
    {
        gestureHistory.Add("dummy");
        this.book = book;
        this.pageTurnSound = pageTurnSound;
        this.controller = controller;
    }

    public void CheckPageTurnGesture(List<Hand> hands)
    {
        Hand rightHand = null;
        Hand leftHand = null;

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
            foreach (Hand newHand in controller.Frame(0).Hands)
            {
                if (newHand.IsRight)
                {
                    Vector currentHandPosition = newHand.PalmPosition;
                    foreach (Hand oldHand in controller.Frame(1).Hands)
                    {
                        if (oldHand.IsRight)
                        {
                            Vector oldHandPosition = oldHand.PalmPosition;
                            if (currentHandPosition.x < oldHandPosition.x && currentHandPosition.y > oldHandPosition.y)
                            {
                                leftUpGestureProbability++;
                            }
                            else {
                                leftUpGestureProbability--;
                            }
                            if (leftUpGestureProbability < 0)
                            {
                                leftUpGestureProbability = 0;
                            }
                            if (leftUpGestureProbability > 15)
                            {
                                Debug.Log("Gesture leftUp detected");
                                leftUpGestureProbability = 0;
                                if (gestureHistory[0] != "leftUp")//Wenn das letzte nicht schon diese Geste war
                                {
                                    gestureHistory.Add("leftUp");
                                    Debug.Log("first Item is " + gestureHistory[0] + " second Item is " + gestureHistory[1]);
                                    Debug.Log("added to Que");
                                }
                            }
                            
                        }
                    }
                }
            }
            //Debug.Log(rightHand.PalmVelocity);

            //for(int i)
        }

    }

}
