using UnityEngine;

public class ProbabilityTracker
{
    private int rightHandLeftUp = 0;
    private int rightHandRightUp = 0;
    private int rightHandRightDown = 0;
    private int rightHandLeftDown = 0;
    private int forwardCircle = 0;
    private int backwardCircle = 0;

    public int getForwardCircle()
    {
        return forwardCircle;
    }
    public void decreaseForwardCircle()
    {
        forwardCircle--;
    }

    public void setForwardCircle(int setTo)
    {
        forwardCircle = setTo;
    }

    public void increaseForwardCircle()
    {
        forwardCircle++;
    }

    public int getBackwardCircle()
    {
        return backwardCircle;
    }
    public void decreaseBackwardCircle()
    {
        backwardCircle--;
    }

    public void setBackwardCircle(int setTo)
    {
        backwardCircle = setTo;
    }

    public void increaseBackwardCircle()
    {
        backwardCircle++;
    }

    public void nullRightHand()
    {
        rightHandLeftUp = 0;
        rightHandRightUp = 0;
        rightHandRightDown = 0;
        rightHandLeftDown = 0;
    }

    //increase
    public void increaseRightHandLeftUp()
    {
        rightHandLeftUp++;
    }
    public void increaseRightHandRightUp()
    {
        rightHandRightUp++;
    }

    public void increaseRightHandRightDown()
    {
        rightHandRightDown++;
    }

    public void increaseRightHandLeftDown()
    {
        rightHandLeftDown++;
    }
    //decrease
    public void decreaseRightHandLeftUp()
    {
        if (rightHandLeftUp >= 0)
            rightHandLeftUp--;
    }

    public void decreaseRightHandRightUp()
    {
        if (rightHandRightUp >= 0)
            rightHandRightUp--;
    }

    public void decreaseRightHandRightDown()
    {
        if (rightHandRightDown >= 0)
            rightHandRightDown--;
    }

    public void decreaseRightHandLeftDown()
    {
        if (rightHandLeftDown >= 0)
            rightHandLeftDown--;
    }

    
    //setters
    public void setRightHandLeftUp(int rightHandLeftUp)
    {
        this.rightHandLeftUp = rightHandLeftUp;
    }
    public void setRightHandRightUp(int rightHandRightUp)
    {
        this.rightHandRightUp = rightHandRightUp;
    }
    public void setRightHandRightDown(int rightHandRightDown)
    {
        this.rightHandRightDown = rightHandRightDown;
    }
    public void setRightHandLeftDown(int rightHandLeftDown)
    {
        this.rightHandLeftDown = rightHandLeftDown;
    }


    //getters
    public int getRightHandLeftUp()
    {
        return rightHandLeftUp;
    }
    public int getRightHandRightUp()
    {
        return rightHandRightUp;
    }
    public int getRightHandRightDown()
    {
        return rightHandRightDown;
    }
    public int getRightHandLeftDown()
    {
        return rightHandLeftDown;
    }
}