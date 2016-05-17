using UnityEngine;
using System.Timers;

public class SpeedONeedle : MonoBehaviour {

    public float lowestSpeed = 200f;
    public float highestSpeed = 20f;
    public int maxTurnSpeed = 10;
    public int needleSpeed = 10;
    //The speed to get to.
    public int speed = 0;
    private float speedOfOne;
    private Timer speedDecreaseTimer;


    void Start()
    {
        //Set the speed of each point on the speedometer.
        speedOfOne = (lowestSpeed - highestSpeed) / maxTurnSpeed;
        initializeSpeedDecrease();
    }

    /// <summary>
    /// Activate the reducing of the speed.
    /// </summary>
    private void initializeSpeedDecrease()
    {
        speedDecreaseTimer = new Timer(1000); //Set Timer intervall 
        speedDecreaseTimer.Elapsed += decreaseSpeed; // Hook up the method to the timer
        speedDecreaseTimer.Enabled = true;
    }
    
    void Update()
    {
        //So speed doesn't exceed max speed.
        if (speed > maxTurnSpeed)
            speed = maxTurnSpeed;
        //Speed can never be negative.
        if (speed < 0)
            speed = 0;
        //Let it go up to a certian point or down.
            if (lowestSpeed - (speed * speedOfOne) >= transform.eulerAngles.z)
                transform.RotateAround(transform.parent.position, transform.forward, Time.deltaTime * needleSpeed);
            else if (lowestSpeed - (speed * speedOfOne) <= transform.eulerAngles.z)
                transform.RotateAround(transform.parent.position, -transform.forward, Time.deltaTime * needleSpeed);
    }

    private void decreaseSpeed(object sender, ElapsedEventArgs e)
    {
        if (speed > 0)
            speed -= 1;
        else
            speedDecreaseTimer.Enabled = false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="speed"></param>
    internal void AddSpeed(int speed)
    {
        this.speed += speed;
        if (!speedDecreaseTimer.Enabled)
            initializeSpeedDecrease();
    }
}
