using UnityEngine;
using Leap;
using System.Collections.Generic;
using System.Timers;

public class LeapPageTurnerV2 : MonoBehaviour
{

    private MegaBookBuilder book;
    private SpeedONeedle needle;
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
    private int circleProbabilityThreshhold = 3;
    private int ProbabilityDistanceThreshold = 1;
    private int fastPageTurnSpeedThreshold = 10000;
    private Queue<float> speedQue = new Queue<float>();

    public LeapPageTurnerV2(MegaBookBuilder book, Leap.Controller controller, AudioSource pageTurnSound)
    {
        this.book = book;
        this.needle = book.transform.parent.Find("SpeedCanvas").transform.Find("SpeedDial").GetChild(0).GetComponent<SpeedONeedle>();
        this.pageTurnSound = pageTurnSound;
        this.controller = controller;
        initializeCircleProbabilityTimer();
        initializeSpeedQue();
    }

    /// <summary>
    /// Initilizes the que with stores the speed of the handpalm of the last 20 frames. Initialization means storing 20 times 0 in the que.
    /// </summary>
    private void initializeSpeedQue()
    {
        for (int i = 0; i < 20; i++)
        {
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
            probabilityTracker.decreaseForwardCircle();
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
    /// Funtion is used to check the last 3 "directions vectors" in the gesture que. If the direction vectors appear to be a circle which goes into the same direction, it will call updateprobability.
    /// </summary>
    private void circleChecker()
    {
        //Check forward Gesture
        if (checkOldGestures("rightUp", "leftUp", "leftDown"))
            updateProbabilityNTurnPage(true);
        if (checkOldGestures("rightDown", "rightUp", "leftUp"))
            updateProbabilityNTurnPage(true);
        if (checkOldGestures("leftDown", "rightDown", "rightUp"))
            updateProbabilityNTurnPage(true);
        if (checkOldGestures("leftUp", "leftDown", "rightDown"))
            updateProbabilityNTurnPage(true);

        //Check backward gesture
        if (checkOldGestures("leftUp", "rightUp", "rightDown"))
            updateProbabilityNTurnPage(false);
        if (checkOldGestures("rightUp", "rightDown", "leftDown"))
            updateProbabilityNTurnPage(false);
        if (checkOldGestures("rightDown", "leftDown", "leftUp"))
            updateProbabilityNTurnPage(false);
        if (checkOldGestures("leftDown", "leftUp", "rightUp"))
            updateProbabilityNTurnPage(false);
    }

    /// <summary>
    /// Check if the 3 input strings are equal to the 3 last gesture directions.
    /// </summary>
    /// <param name="last"></param>
    /// <param name="old"></param>
    /// <param name="oldest"></param>
    /// <returns></returns>
    private bool checkOldGestures(string last, string old, string oldest)
    {
        if (gestureQue.getLastGesture() == last && gestureQue.getOldGesture() == old && gestureQue.getOldestGesture() == oldest)
            return true;
        return false;
    }

    /// <summary>
    /// Update the probability for forward or backwards.
    /// And also turn a page and update speed of the needle.
    /// </summary>
    /// <param name="forward"></param>
    private void updateProbabilityNTurnPage(bool forward)
    {
        if (forward)
        {
            book.NextPage(pageTurnSound);
            probabilityTracker.increaseForwardCircle();
        }
        else if (!forward)
        {
            book.PrevPage(pageTurnSound);
            probabilityTracker.increaseBackwardCircle();
        }
        gestureQue.setOldestGesture("done");
        needle.AddSpeed(1);
    }

    /// <summary>
    /// Figures out which is the left and which is the right hand and returns it.
    /// Returns null if the specifik hand isn't there.
    /// </summary>
    /// <param name="frameID"></param>
    /// <param name="leftOrRight"></param>
    /// <returns></returns>
    private Hand getHand(int frameID, string leftOrRight)
    {
        foreach (Hand hand in controller.Frame(frameID).Hands)
        {
            if (hand.IsLeft && leftOrRight == "left")
                return hand;
            if (hand.IsRight && leftOrRight == "right")
                return hand;
        }
        return null;
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
            probabilityTracker.increaseRightHandLeftUp();

        else if (currentHandPosition.x > oldHandPosition.x || currentHandPosition.z < oldHandPosition.z)
        {
        }
        else
            probabilityTracker.decreaseRightHandLeftUp();

        //rightUp Gesture
        if (currentHandPosition.x < oldHandPosition.x && currentHandPosition.z < oldHandPosition.z && distance.Magnitude > ProbabilityDistanceThreshold)
            probabilityTracker.increaseRightHandRightUp();
        else if (currentHandPosition.x < oldHandPosition.x || currentHandPosition.z < oldHandPosition.z)
        {
        }
        else
            probabilityTracker.decreaseRightHandRightUp();

        //rightDown
        if (currentHandPosition.x < oldHandPosition.x && currentHandPosition.z > oldHandPosition.z && distance.Magnitude > ProbabilityDistanceThreshold)
            probabilityTracker.increaseRightHandRightDown();
        else if (currentHandPosition.x < oldHandPosition.x || currentHandPosition.z > oldHandPosition.z)
        {
        }
        else
            probabilityTracker.decreaseRightHandRightDown();

        //leftDown
        if (currentHandPosition.x > oldHandPosition.x && currentHandPosition.z > oldHandPosition.z && distance.Magnitude > ProbabilityDistanceThreshold)
            probabilityTracker.increaseRightHandLeftDown();
        else if (currentHandPosition.x > oldHandPosition.x || currentHandPosition.z > oldHandPosition.z)
        {
        }
        else
            probabilityTracker.decreaseRightHandLeftDown();
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
            forcastCall("leftUp", distance);

        if (probabilityTracker.getRightHandRightUp() > probabilityThreshold)
            forcastCall("rightUp", distance);

        if (probabilityTracker.getRightHandRightDown() > probabilityThreshold)
            forcastCall("rightDown", distance);

        if (probabilityTracker.getRightHandLeftDown() > probabilityThreshold)
            forcastCall("leftDown", distance);
    }

    /// <summary>
    /// Check if the distance magnitude is over the threshold, if it is it will do a forcast.
    /// </summary>
    /// <param name="dir"></param>
    /// <param name="distance"></param>
    void forcastCall(string dir, Vector distance)
    {
        if (distance.Magnitude > GestureDistanceThreshold)
        {
            Debug.Log(dir);
            probabilityTracker.nullRightHand();
            circleForcaster(dir);
            gestureQue.setLastGesture(dir);
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
            else
            {
                probabilityTracker.setRightHandRightDown(5);
            }

        }
        if (gestureQue.getLastGesture() == "rightDown" && gestureQue.getOldGesture() == "rightUp")
        {
            if (gestureQue.getOldestGesture() == "leftUp")
            {
                probabilityTracker.setRightHandLeftDown(8);
            }
            else
            {
                probabilityTracker.setRightHandLeftDown(5);
            }
        }
        if (gestureQue.getLastGesture() == "leftDown" && gestureQue.getOldGesture() == "rightDown")
        {
            if (gestureQue.getOldestGesture() == "rightUp")
            {
                probabilityTracker.setRightHandLeftUp(8);
            }
            else
            {
                probabilityTracker.setRightHandLeftUp(5);
            }

        }
        if (gestureQue.getLastGesture() == "leftUp" && gestureQue.getOldGesture() == "leftDown")
        {
            if (gestureQue.getOldestGesture() == "rightDown")
            {
                probabilityTracker.setRightHandRightUp(8);
            }
            else
            {
                probabilityTracker.setRightHandRightUp(5);
            }

        }

    }

    /// <summary>
    /// Returns the speed of the Hand. The speed is determined by the last 10 Frames.
    /// </summary>
    /// <param name="currentHand"></param>
    /// <returns></returns>
    private float getHandSpeed(Hand currentHand)
    {
        float handSpeed = 0;
        speedQue.Dequeue();
        speedQue.Enqueue(currentHand.PalmVelocity.Magnitude);

        foreach (float speedValue in speedQue)
        {
            handSpeed = handSpeed + speedValue;
        }

        return handSpeed;
    }

    /// <summary>
    /// Function sets the page turn speed of the book, according to the speed of the hand.
    /// </summary>
    /// <param name="currentHand"></param>
    private void setPageTurnSpeed(Hand currentHand)
    {
        float handSpeed = getHandSpeed(currentHand);
        if (handSpeed < 4000)
        {
            book.SetTurnTime(0.8f);
        }
        if (handSpeed > 4000)
        {
            book.SetTurnTime(0.2f);
        }
    }

    /// <summary>
    //  Function triggers fast page turns if the hand velocity is higher as an specific threshold AND if the user is doing a circle gesture.
    /// </summary>
    /// <param name="currentHand"></param>
    private void fastPageTurnChecker(Hand currentHand)
    {
        float handSpeed = getHandSpeed(currentHand);

        if (probabilityTracker.getForwardCircle() >= circleProbabilityThreshhold && handSpeed > fastPageTurnSpeedThreshold)
        {
            
            if (fastForwardActive == false && fastBackwardActive == false)
            {
                Debug.Log("fast forward");
                initilizeFastPageTurnTimer();
                probabilityTracker.setForwardCircle(0);
                fastForwardActive = true;
                
            }
        }

        if (probabilityTracker.getBackwardCircle() >= circleProbabilityThreshhold && handSpeed > fastPageTurnSpeedThreshold)
        {
            if (fastForwardActive == false && fastBackwardActive == false)
            {
                Debug.Log("fast backward");
                initilizeFastPageTurnTimer();
                probabilityTracker.setBackwardCircle(0);
                fastBackwardActive = true;
               
            }
        }

        if (fastPageTurn && fastForwardActive)
        {
            fastPageTurn = false;
            book.NextPage(pageTurnSound);
            needle.AddSpeed(1);
        }

        if (fastPageTurn && fastBackwardActive)
        {
            fastPageTurn = false;
            book.PrevPage(pageTurnSound);
            needle.AddSpeed(1);
        }

        if (handSpeed < 5000 && fastPageTurnTimer != null)
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

            setPageTurnSpeed(rightCurrentHand);
            updateHandMovementProbability(rightCurrentHand, rightOldHand);
            updateGestureQue(rightCurrentHand, rightVeryOldHand);
            fastPageTurnChecker(rightCurrentHand);
            circleChecker();
        }

    }
}
