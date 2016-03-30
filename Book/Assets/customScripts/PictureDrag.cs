using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Attach to picture you want to drag.

public class PictureDrag : MonoBehaviour {
    Vector3 dist;
    float posX;
    float posY;
    private bool isInList = false;
    static List<GameObject> moving = new List<GameObject>();

    void Update()
    {
        if (Input.GetMouseButtonDown(1) && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
        {
            moving.Clear();
        }
    }

        void OnMouseDown() {
        if (isInList)
        {
            foreach(GameObject value in moving)
            value.GetComponent<PictureDrag>().mouseDown();
        }
        else
            mouseDown();
    }
    
    //Action to call when OnMouseDown is called.
    public void mouseDown()
    {
        dist = Camera.main.WorldToScreenPoint(transform.position);
        posX = Input.mousePosition.x - dist.x;
        posY = Input.mousePosition.y - dist.y;
    }

    void OnMouseDrag() {
        if (moving.Contains(this.gameObject))
        {
            foreach (GameObject value in moving)
                value.GetComponent<PictureDrag>().mouseDrag();
        }
        else
            mouseDrag();
    }

    //Action to call when OnMouseDrag is called.
    public void mouseDrag()
    {
        Vector3 curPos = new Vector3(Input.mousePosition.x - posX, Input.mousePosition.y - posY, dist.z);
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(curPos);
        transform.position = worldPos;
    }

    void OnMouseUp()
    {
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            if (!moving.Contains(this.gameObject))
            {
                moving.Add(this.gameObject);
                isInList = true;

                this.gameObject.GetComponent<Renderer>().material.EnableKeyword("_NORMALMAP");
            }
            else
            {
                moving.Remove(this.gameObject);
                isInList = false;
                this.gameObject.GetComponent<Renderer>().material.DisableKeyword("_NORMALMAP");
            }
        }
    }

    void OnDestroy()
    {
        Debug.Log(this.gameObject + " destroyed!!!");
        moving.Remove(this.gameObject);
    }

    public List<GameObject> getMovingObjects()
    {
        return moving;
    }

    public bool getIsInList()
    {
        return isInList;
    }
}
