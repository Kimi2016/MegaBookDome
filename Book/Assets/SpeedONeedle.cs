using UnityEngine;
using System.Collections;

public class SpeedONeedle : MonoBehaviour {

    public float lowestSpeed = 201f;
    public float highestSpeed = 20f;
    public float speed = 10f;

	void Update () {
        if (lowestSpeed > transform.eulerAngles.z && transform.eulerAngles.z > highestSpeed)
        transform.RotateAround(transform.parent.position, -transform.forward, Time.deltaTime * speed);
    }
    
}
