using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GestureQueV2
{

    private List<string> que = new List<string>();

    public GestureQueV2()
    {
        que.Add("dummy");
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

    public string getOldestGesture() {
        return que[2];
    }

    public void setOldGesture(string oldGesture)
    {
        que[1] = oldGesture;
    }

    public void setOldestGesture(string oldestGesture) {
        que[2] = oldestGesture;
    }

    public void setLastGesture(string lastGesture)
    {
        if (lastGesture != que[0])
        {
            que[2] = que[1];
            que[1] = que[0];
            que[0] = lastGesture;
        }
    }


}
