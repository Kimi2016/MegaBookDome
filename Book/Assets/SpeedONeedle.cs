using UnityEngine;
using System.Collections;

public class SpeedONeedle : MonoBehaviour {

    public float lowestSpeed = 200f;
    public float highestSpeed = 20f;
    public int maxTurnSpeed = 10;
    public int needleSpeed = 10;
    //The speed to get to.
    public int speed = 0;
    private float speedOfOne;

    void Start()
    {
        speedOfOne = (lowestSpeed - highestSpeed) / maxTurnSpeed;
    }

    void Update()
    {
        // if (lowestSpeed >= transform.eulerAngles.z-1 && transform.eulerAngles.z+1 >= highestSpeed)
        if (speed > maxTurnSpeed)
            speed = maxTurnSpeed;
        if (speed < 0)
            speed = 0;
            if (lowestSpeed - (speed * speedOfOne) >= transform.eulerAngles.z)
                transform.RotateAround(transform.parent.position, transform.forward, Time.deltaTime * needleSpeed);
            else if (lowestSpeed - (speed * speedOfOne) <= transform.eulerAngles.z)
                transform.RotateAround(transform.parent.position, -transform.forward, Time.deltaTime * needleSpeed);
            
        
    }
    
}
