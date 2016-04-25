using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GestureQue
{

    private List<string> que = new List<string>();

    public GestureQue()
    {
        que.Add("dummy");
        que.Add("dummy");
    }

    public string getLastGesture()
    {
        return que[0];
    }

    public string getOldGesture()
    {
        return que[1];
    }

    public void setOldGesture(string oldGesture)
    {
        que[1] = oldGesture;
    }

    public void setLastGesture(string lastGesture)
    {
        if (lastGesture != que[0])
        {
            que[1] = que[0];
            que[0] = lastGesture;
        }
        else {
            Debug.Log("tried to set " + lastGesture + " but it was the last already");
        }
    }


}
