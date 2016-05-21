using UnityEngine;
using System.Collections;

public class GestureReporter : MonoBehaviour {

    private int forwardLeftUp = 0;
    private int forwardRightUp = 0;
    private int forwardRightDown = 0;
    private int forwardLeftDown = 0;
    private int backwardLeftUp = 0;
    private int backwardRightUp = 0;
    private int backwardRightDown = 0;
    private int backwardLeftDown = 0;

    public int increaseGestureCount(string gesture, string direction) {
        if (direction == "Forward") {
            if (gesture == "leftUp") {
                forwardLeftUp++;
                return forwardLeftUp;
            }
            if (gesture == "rightUp") {
                forwardRightUp++;
                return forwardRightUp;
            }
            if (gesture == "rightDown")
            {
                forwardRightDown++;
                return forwardRightDown;
            }
            if (gesture == "leftDown")
            {
                forwardLeftDown++;
                return forwardLeftDown;
            }
        }
        else if (direction == "Backward")
        {
            if (gesture == "leftUp")
            {
                backwardLeftUp++;
                return backwardLeftUp;
            }
            if (gesture == "rightUp")
            {
                backwardRightUp++;
                return backwardRightUp;
            }
            if (gesture == "rightDown")
            {
                backwardRightDown++;
                return backwardRightDown;
            }
            if (gesture == "leftDown")
            {
                backwardLeftDown++;
                return backwardLeftDown;
            }
        }

        return 0;
    }

}
