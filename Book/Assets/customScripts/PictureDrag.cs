using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Attach to picture you want to drag.

public class PictureDrag : MonoBehaviour
{
    Vector3 dist;
    float posX;
    float posY;
    private bool isInList = false;
    static List<GameObject> selectedPicture = new List<GameObject>(); //Selected picture, used for moving multiple pictures.
    static List<GameObject> createdPictures = new List<GameObject>(); //Used for moving one picture only, and also for multiple, where it will be used for the first one.

    //Add the object to the created picture list, which is used to cycle though all objects and order them.
    void Start()
    {
        createdPictures.Add(this.gameObject);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1) && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
        {
            selectedPicture.Clear();
        }
    }

    void OnMouseDown()
    {
        if (isInList)
        {
            foreach (GameObject value in selectedPicture)
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

    void OnMouseDrag()
    {
        if (selectedPicture.Contains(this.gameObject))
        {
            foreach (GameObject value in selectedPicture)
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
            selectNDeselPic();
        }
    }
    
    /// <summary>
    /// If it retruns true it is added to the list, false for removed.
    /// </summary>
    /// <returns></returns>
    public bool selectNDeselPic()
    {
        if (!selectedPicture.Contains(this.gameObject))
        {
            selectedPicture.Add(this.gameObject);
            isInList = true;

            this.gameObject.GetComponent<Renderer>().material.EnableKeyword("_NORMALMAP");
            return true;
        }
        else
        {
            selectedPicture.Remove(this.gameObject);
            isInList = false;
            this.gameObject.GetComponent<Renderer>().material.DisableKeyword("_NORMALMAP");
            return false;
        }
    }

    //Remove the object from both lists.
    void OnDestroy()
    {
        createdPictures.Remove(this.gameObject);
        selectedPicture.Remove(this.gameObject);
    }

    public List<GameObject> getMovingObjects()
    {
        return selectedPicture;
    }

    public List<GameObject> getCreatedObjects()
    {
        return createdPictures;
    }

    public void setAsFirstInList(GameObject obj)
    {
        createdPictures.Remove(obj);
        createdPictures.Insert(0, obj);
    }

    public bool getIsInList()
    {
        return isInList;
    }
}
