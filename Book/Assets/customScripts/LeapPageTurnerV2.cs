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
    private GestureQueV2 gestureQue = new GestureQueV2();
    private ProbabilityTracker probabilityTracker = new ProbabilityTracker();
    private Timer circleProbabilityTimer;
    private Timer fastPageTurnTimer;
    private bool fastPageTurn = false;
    private bool fastForwardActive = false;
    private bool fastBackwardActive = false;


    //the next two variables are used in updateGestureQue as thresholds.
    private int probabilityThreshold = 13;
    private int GestureDistanceThreshold = 27;
    private int circleProbabilityThreshhold = 6;
    private int ProbabilityDistanceThreshold = 1;
    private int fastPageTurnSpeedThreshold = 9000;
    //Number of elements saved for speed estimation, -1, so if it is 21 it is actually 20.
    private List<float> speedList = new List<float>(21);
    private Queue<float> speedQue = new Queue<float>();

    public LeapPageTurnerV2(MegaBookBuilder book, Leap.Controller controller, AudioSource pageTurnSound)
    {
        this.book = book;
        this.pageTurnSound = pageTurnSound;
        this.controller = controller;
        initializeCircleProbabilityTimer();
        initializeSpeedQue();
    }

    /// <summary>
    /// Initilizes the que with stores the speed of the handpalm of the last 20 frames. Initialization means storing 20 times 0 in the que.
    /// </summary>
    private void initializeSpeedQue() {
        for (int i = 0; i < 20; i++) {
            speedQue.Enqueue(0); 
        }
    }

    /// <summary>
    /// creates and enables Timer object which is used to decrease the probability of a circle gesture beein currently done. Probability is increased by every "circle gesture".
    /// </summary>
    private void initializeCircleProbabilityTimer()
    {
        circleProbabilityTimer = new Timer(2000); //Set Timer intervall 
        circleProbabilityTimer.Elapsed += decreaseCircleProbability; // Hook up the method to the timer
        circleProbabilityTimer.Enabled = true;
    }

    /// <summary>
    /// Function called by timer every X seconds to decrease the circle probability. If the user isnt doing anything the circle probability will be 0 then...
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void decreaseCircleProbability(object sender, ElapsedEventArgs e)
    {
        if (probabilityTracker.getForwardCircle() > 0)
        {
            probabilityTracker.decreaseForwardCircle();
        }
    }

    private void initilizeFastPageTurnTimer()
    {
        fastPageTurnTimer = new Timer(200); //Set Timer intervall 
        fastPageTurnTimer.Elapsed += enableFastPageTurn; // Hook up the method to the timer
        fastPageTurnTimer.Enabled = true;
    }

    private void enableFastPageTurn(object sender, ElapsedEventArgs e)
    {
        fastPageTurn = true;
    }


    /// <summary>
    /// Funtion is used to check the last 3 "directions vectors" in the gesture que. If the direction vectors appear to be a circle which goes into the same direction, a page will be turned.
    /// </summary>
    private void circleChecker()
    {
        //Check forward Gesture
        if (gestureQue.getLastGesture() == "rightUp" && gestureQue.getOldGesture() == "leftUp" && gestureQue.getOldestGesture() == "leftDown")
        {
            book.NextPage(pageTurnSound);
            probabilityTracker.increaseForwardCircle();
            gestureQue.setOldestGesture("done");
        }
        if (gestureQue.getLastGesture() == "rightDown" && gestureQue.getOldGesture() == "rightUp" && gestureQue.getOldestGesture() == "leftUp")
        {
            book.NextPage(pageTurnSound);
            probabilityTracker.increaseForwardCircle();
            gestureQue.setOldestGesture("done");
        }
        if (gestureQue.getLastGesture() == "leftDown" && gestureQue.getOldGesture() == "rightDown" && gestureQue.getOldestGesture() == "rightUp")
        {
            book.NextPage(pageTurnSound);
            probabilityTracker.increaseForwardCircle();
            gestureQue.setOldestGesture("done");
        }
        if (gestureQue.getLastGesture() == "leftUp" && gestureQue.getOldGesture() == "leftDown" && gestureQue.getOldestGesture() == "rightDown")
        {
            book.NextPage(pageTurnSound);
            probabilityTracker.increaseForwardCircle();
            gestureQue.setOldestGesture("done");
        }

        //Check backward gesture
        if (gestureQue.getLastGesture() == "leftUp" && gestureQue.getOldGesture() == "rightUp" && gestureQue.getOldestGesture() == "rightDown")
        {
            book.PrevPage(pageTurnSound);
            probabilityTracker.increaseBackwardCircle();
            gestureQue.setOldestGesture("done");
        }
        if (gestureQue.getLastGesture() == "rightUp" && gestureQue.getOldGesture() == "rightDown" && gestureQue.getOldestGesture() == "leftDown")
        {
            book.PrevPage(pageTurnSound);
            probabilityTracker.increaseBackwardCircle();
            gestureQue.setOldestGesture("done");
        }
        if (gestureQue.getLastGesture() == "rightDown" && gestureQue.getOldGesture() == "leftDown" && gestureQue.getOldestGesture() == "leftUp")
        {
            book.PrevPage(pageTurnSound);
            probabilityTracker.increaseBackwardCircle();
            gestureQue.setOldestGesture("done");
        }
        if (gestureQue.getLastGesture() == "leftDown" && gestureQue.getOldGesture() == "leftUp" && gestureQue.getOldestGesture() == "rightUp")
        {
            book.PrevPage(pageTurnSound);
            probabilityTracker.increaseBackwardCircle();
            gestureQue.setOldestGesture("done");
        }

    }

    /// <summary>
    /// Figures out which is the left and which is the right hand and returns it.
    /// </summary>
    /// <param name="frameID"></param>
    /// <param name="leftOrRight"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Function is used to update the probabilities of all the "direction vectors". 
    /// </summary>
    /// <param name="currentHand"></param>
    /// <param name="oldHand"></param>
    /// <param name="leftOrRight"></param>
    private void updateHandMovementProbability(Hand currentHand, Hand oldHand)
    {
        Vector currentHandPosition = currentHand.PalmPosition;
        Vector oldHandPosition = oldHand.PalmPosition;

        Vector distance = currentHand.PalmPosition - oldHand.PalmPosition;

        //LeftUp Gesture
        if (currentHandPosition.x > oldHandPosition.x && currentHandPosition.z < oldHandPosition.z && distance.Magnitude > ProbabilityDistanceThreshold)
        {
                probabilityTracker.increaseRightHandLeftUp();
        }

        else if (currentHandPosition.x > oldHandPosition.x || currentHandPosition.z < oldHandPosition.z)
        {
        }
        else {

                probabilityTracker.decreaseRightHandLeftUp();
        }

        //rightUp Gesture
        if (currentHandPosition.x < oldHandPosition.x && currentHandPosition.z < oldHandPosition.z && distance.Magnitude > ProbabilityDistanceThreshold)
        {

                probabilityTracker.increaseRightHandRightUp();
        }
        else if (currentHandPosition.x < oldHandPosition.x || currentHandPosition.z < oldHandPosition.z)
        {
        }
        else {

                probabilityTracker.decreaseRightHandRightUp();
        }

        //rightDown
        if (currentHandPosition.x < oldHandPosition.x && currentHandPosition.z > oldHandPosition.z && distance.Magnitude > ProbabilityDistanceThreshold)
        {
                probabilityTracker.increaseRightHandRightDown();

        }
        else if (currentHandPosition.x < oldHandPosition.x || currentHandPosition.z > oldHandPosition.z)
        {
        }
        else {

                probabilityTracker.decreaseRightHandRightDown();
        }

        //leftDown
        if (currentHandPosition.x > oldHandPosition.x && currentHandPosition.z > oldHandPosition.z && distance.Magnitude > ProbabilityDistanceThreshold)
        {

                probabilityTracker.increaseRightHandLeftDown();
        }
        else if (currentHandPosition.x > oldHandPosition.x || currentHandPosition.z > oldHandPosition.z)
        {
        }
        else {

                probabilityTracker.decreaseRightHandLeftDown();
        }
    }

    /// <summary>
    /// updates the gesture que. If the probability of a direction vector goes over a certain threshold, the direction vector is pushed into the gesture que.
    /// </summary>
    /// <param name="currentHand"></param>
    /// <param name="veryOldHand"></param>
    private void updateGestureQue(Hand currentHand, Hand veryOldHand)
    {

        Vector distance = currentHand.PalmPosition - veryOldHand.PalmPosition;

        if (probabilityTracker.getRightHandLeftUp() > probabilityThreshold)
        {
            if (distance.Magnitude > GestureDistanceThreshold)
            {
                Debug.Log("leftUp");
                probabilityTracker.nullRightHand();
                circleForcaster("leftUp");
                gestureQue.setLastGesture("leftUp");
            }
        }

        if (probabilityTracker.getRightHandRightUp() > probabilityThreshold)
        {
            if (distance.Magnitude > GestureDistanceThreshold)
            {
                Debug.Log("rightUp");
                probabilityTracker.nullRightHand();
                circleForcaster("rightUp");
                gestureQue.setLastGesture("rightUp");
            }
        }

        if (probabilityTracker.getRightHandRightDown() > probabilityThreshold)
        {
            if (distance.Magnitude > GestureDistanceThreshold)
            {
                Debug.Log("rightDown");
                probabilityTracker.nullRightHand();
                circleForcaster("rightDown");
                gestureQue.setLastGesture("rightDown");
            }
        }

        if (probabilityTracker.getRightHandLeftDown() > probabilityThreshold)
        {
            if (distance.Magnitude > GestureDistanceThreshold)
            {
                Debug.Log("leftDown");
                probabilityTracker.nullRightHand();
                circleForcaster("leftDown");
                gestureQue.setLastGesture("leftDown");
            }
        }
    }

    /// <summary>
    /// Function forecasts the next possible direction vectors and increases or decreases their probability in advance.
    /// </summary>
    /// <param name="lastDirection"></param>
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

        if (gestureQue.getLastGesture() == "rightUp" && gestureQue.getOldGesture() == "leftUp")
        {
            if (gestureQue.getOldestGesture() == "leftDown")
            {
                probabilityTracker.setRightHandRightDown(8);
            }
            else {
                probabilityTracker.setRightHandRightDown(5);
            }

        }
        if (gestureQue.getLastGesture() == "rightDown" && gestureQue.getOldGesture() == "rightUp")
        {
            if (gestureQue.getOldestGesture() == "leftUp")
            {
                probabilityTracker.setRightHandLeftDown(8);
            }
            else {
                probabilityTracker.setRightHandLeftDown(5);
            }
        }
        if (gestureQue.getLastGesture() == "leftDown" && gestureQue.getOldGesture() == "rightDown")
        {
            if (gestureQue.getOldestGesture() == "rightUp")
            {
                probabilityTracker.setRightHandLeftUp(8);
            }
            else {
                probabilityTracker.setRightHandLeftUp(5);
            }
            
        }
        if (gestureQue.getLastGesture() == "leftUp" && gestureQue.getOldGesture() == "leftDown")
        {
            if (gestureQue.getOldestGesture() == "rightDown")
            {
                probabilityTracker.setRightHandRightUp(8);
            }
            else {
                probabilityTracker.setRightHandRightUp(5);
            }
            
        }
        
    }

    /// <summary>
    // 
    /// </summary>
    /// <param name="currentHand"></param>
    public void fastPageTurnChecker(Hand currentHand)
    {
        float handSpeed=0;
        speedQue.Dequeue();
        speedQue.Enqueue(currentHand.PalmVelocity.Magnitude);

        foreach (float speedValue in speedQue) {
            handSpeed = handSpeed + speedValue;
        }

        if (probabilityTracker.getForwardCircle() > circleProbabilityThreshhold && handSpeed > fastPageTurnSpeedThreshold) {
            if (fastForwardActive == false && fastBackwardActive == false)
            {
                initilizeFastPageTurnTimer();
                probabilityTracker.setForwardCircle(0);
                fastForwardActive = true;
                Debug.Log("fast forward");
            }
        }

        if (probabilityTracker.getBackwardCircle() > circleProbabilityThreshhold && handSpeed > fastPageTurnSpeedThreshold)
        {
            if (fastForwardActive == false && fastBackwardActive == false)
            {
                initilizeFastPageTurnTimer();
                probabilityTracker.setBackwardCircle(0);
                fastBackwardActive = true;
                Debug.Log("fast backward");
            }
        }

        if (fastPageTurn == true && fastForwardActive == true)
        {
            fastPageTurn = false;
            book.NextPage(pageTurnSound);
        }

        if (fastPageTurn == true && fastBackwardActive == true)
        {
            fastPageTurn = false;
            book.PrevPage(pageTurnSound);
        }

        if (handSpeed < 8000 && fastPageTurnTimer != null)
        {
            fastPageTurnTimer.Close();
            fastForwardActive = false;
            fastBackwardActive = false;
        }

    }

    /// <summary>
    /// Function called by the LeapController class. It is used to call the necessary functions to make the gesture work.
    /// </summary>
    /// <param name="hands"></param>
    public void CheckPageTurnGesture(List<Hand> hands)
    {
        Hand rightCurrentHand = getHand(0, "right");
        Hand rightVeryOldHand = getHand(10, "right");
        Hand rightOldHand = getHand(2, "right");

        if (rightCurrentHand != null && rightVeryOldHand != null && rightOldHand != null)
        {

            updateHandMovementProbability(rightCurrentHand, rightOldHand);
            updateGestureQue(rightCurrentHand, rightVeryOldHand);
            fastPageTurnChecker(rightCurrentHand);
            circleChecker();
        }

    }
}
