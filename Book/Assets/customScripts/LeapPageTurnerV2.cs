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

    //the next two variables are used in updateGestureQue as thresholds.
    public int probabilityThreshold = 13;
    public int distanceThreshold = 27;
    //Number of elements saved for speed estimation, -1, so if it is 21 it is actually 20.
    private List<float> speedList = new List<float>(21);

    public LeapPageTurnerV2(MegaBookBuilder book, Leap.Controller controller, AudioSource pageTurnSound)
    {
        this.book = book;
        this.pageTurnSound = pageTurnSound;
        this.controller = controller;
        initializeCircleProbabilityTimer();
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


    /// <summary>
    /// Funtion is used to check the last 3 "directions vectors" in the gesture que. If the direction vectors appear to be a circle which goes into the same direction, a page will be turned.
    /// </summary>
    private void circleChecker()
    {
        //Check forward Gesture
        if (RightHandgestureQue.getLastGesture() == "rightUp" && RightHandgestureQue.getOldGesture() == "leftUp" && RightHandgestureQue.getOldestGesture() == "leftDown")
        {
            book.NextPage(pageTurnSound);
            probabilityTracker.increaseCircleMovement();
            RightHandgestureQue.setOldestGesture("done");
        }
        if (RightHandgestureQue.getLastGesture() == "rightDown" && RightHandgestureQue.getOldGesture() == "rightUp" && RightHandgestureQue.getOldestGesture() == "leftUp")
        {
            book.NextPage(pageTurnSound);
            probabilityTracker.increaseCircleMovement();
            RightHandgestureQue.setOldestGesture("done");
        }
        if (RightHandgestureQue.getLastGesture() == "leftDown" && RightHandgestureQue.getOldGesture() == "rightDown" && RightHandgestureQue.getOldestGesture() == "rightUp")
        {
            book.NextPage(pageTurnSound);
            probabilityTracker.increaseCircleMovement();
            RightHandgestureQue.setOldestGesture("done");
        }
        if (RightHandgestureQue.getLastGesture() == "leftUp" && RightHandgestureQue.getOldGesture() == "leftDown" && RightHandgestureQue.getOldestGesture() == "rightDown")
        {
            book.NextPage(pageTurnSound);
            probabilityTracker.increaseCircleMovement();
            RightHandgestureQue.setOldestGesture("done");
        }

        //Check backward gesture
        if (RightHandgestureQue.getLastGesture() == "leftUp" && RightHandgestureQue.getOldGesture() == "rightUp" && RightHandgestureQue.getOldestGesture() == "rightDown")
        {
            book.PrevPage(pageTurnSound);
            probabilityTracker.increaseCircleMovement();
            RightHandgestureQue.setOldestGesture("done");
        }
        if (RightHandgestureQue.getLastGesture() == "rightUp" && RightHandgestureQue.getOldGesture() == "rightDown" && RightHandgestureQue.getOldestGesture() == "leftDown")
        {
            book.PrevPage(pageTurnSound);
            probabilityTracker.increaseCircleMovement();
            RightHandgestureQue.setOldestGesture("done");
        }
        if (RightHandgestureQue.getLastGesture() == "rightDown" && RightHandgestureQue.getOldGesture() == "leftDown" && RightHandgestureQue.getOldestGesture() == "leftUp")
        {
            book.PrevPage(pageTurnSound);
            probabilityTracker.increaseCircleMovement();
            RightHandgestureQue.setOldestGesture("done");
        }
        if (RightHandgestureQue.getLastGesture() == "leftDown" && RightHandgestureQue.getOldGesture() == "leftUp" && RightHandgestureQue.getOldestGesture() == "rightUp")
        {
            book.PrevPage(pageTurnSound);
            probabilityTracker.increaseCircleMovement();
            RightHandgestureQue.setOldestGesture("done");
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
    private void updateHandMovementProbability(Hand currentHand, Hand oldHand, string leftOrRight)
    {
        Vector currentHandPosition = currentHand.PalmPosition;
        Vector oldHandPosition = oldHand.PalmPosition;

        Vector distance = currentHand.PalmPosition - oldHand.PalmPosition;

        //LeftUp Gesture
        if (currentHandPosition.x > oldHandPosition.x && currentHandPosition.z < oldHandPosition.z && distance.Magnitude > 1)
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
        if (currentHandPosition.x < oldHandPosition.x && currentHandPosition.z < oldHandPosition.z && distance.Magnitude > 1)
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
        if (currentHandPosition.x < oldHandPosition.x && currentHandPosition.z > oldHandPosition.z && distance.Magnitude > 1)
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
        if (currentHandPosition.x > oldHandPosition.x && currentHandPosition.z > oldHandPosition.z && distance.Magnitude > 1)
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

    /// <summary>
    /// updates the gesture que. If the probability of a direction vector goes over a certain threshold, the direction vector is pushed into the gesture que.
    /// </summary>
    /// <param name="currentHand"></param>
    /// <param name="veryOldHand"></param>
    /// <param name="leftOrRight"></param>
    private void updateGestureQue(Hand currentHand, Hand veryOldHand, string leftOrRight)
    {

        Vector distance = currentHand.PalmPosition - veryOldHand.PalmPosition;
        //Debug.Log(probabilityTracker.getRightHandRightUp());

        if (probabilityTracker.getRightHandLeftUp() > probabilityThreshold)
        {
            if (distance.Magnitude > distanceThreshold)
            {
                Debug.Log("leftUp");
                probabilityTracker.nullRightHand();
                circleForcaster("leftUp");
                RightHandgestureQue.setLastGesture("leftUp");
            }
        }

        if (probabilityTracker.getRightHandRightUp() > probabilityThreshold)
        {
            if (distance.Magnitude > distanceThreshold)
            {
                Debug.Log("rightUp");
                probabilityTracker.nullRightHand();
                circleForcaster("rightUp");
                RightHandgestureQue.setLastGesture("rightUp");
            }
        }

        if (probabilityTracker.getRightHandRightDown() > probabilityThreshold)
        {
            if (distance.Magnitude > distanceThreshold)
            {
                Debug.Log("rightDown");
                probabilityTracker.nullRightHand();
                circleForcaster("rightDown");
                RightHandgestureQue.setLastGesture("rightDown");
            }
        }

        if (probabilityTracker.getRightHandLeftDown() > probabilityThreshold)
        {
            if (distance.Magnitude > distanceThreshold)
            {
                Debug.Log("leftDown");
                probabilityTracker.nullRightHand();
                circleForcaster("leftDown");
                RightHandgestureQue.setLastGesture("leftDown");
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

    /// <summary>
    /// Still beta till now... Experimenting with that.
    /// </summary>
    /// <param name="currentHand"></param>
   // private int testSize = 20;
    public void fastPageTurnChecker(Hand currentHand)
    {
        //Add to a list to get the average hand speed, also using magnitude, for all dimensions. 
            Vector palmVelocity = currentHand.PalmVelocity;
        
            speedList.Add(palmVelocity.Magnitude);
            if (speedList.Count >= speedList.Capacity - 1)
            {
                speedList.RemoveAt(0);
            }
        //  if (probabilityTracker.getCircleMovement() > 2)
        //    {
        //Debug.Log(speedList.Count + " Durrr " + (speedList.Capacity - 1));
            if (speedList.Count >= speedList.Capacity - 1)
            {
                float estimatedSpeed = 0;
                for(int i = 0; i < speedList.Capacity - 1; i++)
                {
                    estimatedSpeed += speedList[i];
                }
            estimatedSpeed = estimatedSpeed / speedList.Capacity - 1;
            estimatedSpeed -= 400;
            estimatedSpeed=  estimatedSpeed/200;
            int speed = Mathf.RoundToInt(estimatedSpeed);
            if (speed < 1)
                speed = 1;
            Debug.Log(speed);
            }
        
    /*        else if ((palmVelocity.x > 800 || palmVelocity.x < -800) && probabilityTracker.getCircleMovement() > 2)
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
            }*/
   //     }

        if (probabilityTracker.getCircleMovement() == 0 && fastForwardTimer != null)
        {
            fastForwardTimer.Close();
            fastForwardTimerActive = false;
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

        Hand leftCurrentHand = getHand(0, "left");
        Hand leftVeryOldHand = getHand(10, "left");
        Hand leftOldHand = getHand(1, "left");

        if (rightCurrentHand != null && rightVeryOldHand != null && rightOldHand != null)
        {

            updateHandMovementProbability(rightCurrentHand, rightOldHand, "right");
            updateGestureQue(rightCurrentHand, rightVeryOldHand, "right");
            fastPageTurnChecker(rightCurrentHand);
            circleChecker();
            Debug.Log(probabilityTracker.getCircleMovement());
        }

    }
}
