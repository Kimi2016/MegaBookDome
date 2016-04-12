using UnityEngine;
using System.Collections;
using Leap;
using System.Collections.Generic;

public class LeapPageTurnerV2 : MonoBehaviour {

    private MegaBookBuilder book;
    private AudioSource pageTurnSound;
    private Leap.Controller controller;

    public LeapPageTurnerV2(MegaBookBuilder book, Leap.Controller controller, AudioSource pageTurnSound) {
        this.book = book;
        this.pageTurnSound = pageTurnSound;
        this.controller = controller;
    }

    public void CheckPageTurnGesture(List<Hand> hands) {
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

        if (rightHand != null) {
            Debug.Log(rightHand.PalmVelocity.x);
            //righ
        }

    }

}
