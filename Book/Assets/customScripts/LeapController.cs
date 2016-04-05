using UnityEngine;
using System.Collections.Generic;
using Leap;
using LeapInternal;
using System;
using System.Threading;

public class LeapController : MonoBehaviour
{
    private Leap.Controller controller;
    private LeapProvider leapProvider;
 //   public GameObject picture;
    public MegaBookBuilder book;
    public AudioSource pageTurnSoundSlow;
    public GameObject prefabPage;
    public Texture2D frontTexture;
    public Texture2D backTexture;
    public AudioSource pageTurnSoundFast;
    private Vector3 picturePosition;
    bool pictureCurrentlyDragged;
    private LeapPageTurner leapPageTurner;

    void Start()
    {

        pictureCurrentlyDragged = false;
        controller = new Controller();
        leapProvider = FindObjectOfType<LeapProvider>() as LeapProvider;
        if (!controller.IsConnected)
        {
            Debug.LogError("no LeapMotion Device detected...");

        }
        else {
            Debug.Log("Device connected, continue");
        }
        leapPageTurner = new LeapPageTurner(book, controller, pageTurnSoundSlow, pageTurnSoundFast);

    }

    void Update()
    {
        Frame frame = leapProvider.CurrentFrame;

        if (!pictureCurrentlyDragged)
        {
            leapPageTurner.CheckPageTurnGesture(frame.Hands);
        }

        foreach (Hand hand in frame.Hands)
        {
            if (hand.IsRight)
            {
                DragAndDropChecker(hand);
            } 
            if (!pictureCurrentlyDragged && hand.GrabStrength > 0.7)
            {
                Vector3 unityLeap = hand.PalmPosition.ToUnity();
                foreach (GameObject pageCollider in GameObject.FindGameObjectsWithTag("page collider"))
                {
                    Vector3 distance = unityLeap - pageCollider.transform.position;
                    if (distance.magnitude < 0.5)
                     {  
                        if (!pageCollider.GetComponent<PageCollider>().next)
                        {
                            makeOutOfBookPicture(false, new Vector3(-1.5f, 0, 0) + pageCollider.transform.position, backTexture);
                        }
                        if (pageCollider.GetComponent<PageCollider>().next)
                        {
                            makeOutOfBookPicture(true, new Vector3(1.5f, 0, 0) + pageCollider.transform.position, frontTexture);
                        }
                    }
                }
            }
        }
    }

    //Front or back, front if true, push direction is relative to the page position, texture is the page texture if it is the standard once, it will do nothing.
    private void makeOutOfBookPicture(bool front, Vector3 pushDirection, Texture2D texture)
    {
        try
        {
            int pageNum = book.GetCurrentPage();
            if (!front)
                pageNum = pageNum - 1;

            Texture2D pageTexture = book.GetPageTexture(pageNum, front) as Texture2D;

            if (texture == pageTexture)
            {
                return;
            }

            Material material = new Material(Shader.Find("Standard"));
            material.SetTexture("_MainTex", pageTexture);


            book.SetPageTexture(texture, pageNum, front);
            material.SetTexture("_BumpMap", Resources.Load("Textures/MegaBook_Mask_Map") as Texture2D);

            prefabPage.GetComponent<Renderer>().material = material;
            Instantiate(prefabPage, pushDirection, new Quaternion(1, 0, 0, 1));
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Data);
        }
    }

    //ToDo: put function below into a own class

    private void DragAndDropChecker(Hand rightHand)
    {
        pictureCurrentlyDragged = false;
        float grabStrength = rightHand.GrabStrength;
        if (rightHand.PalmNormal.y < -0.7)
        {
            if (grabStrength > 0.6)
            {
                //Debug.Log("Grab detected... strength is:" + grabStrength);
                Vector3 unityLeap = rightHand.PalmPosition.ToUnity();
                foreach (GameObject pic in GameObject.FindGameObjectWithTag("pic").GetComponent<PictureDrag>().getCreatedObjects())
                {
                    Vector3 distance = unityLeap - pic.transform.position;
                    //Debug.Log("distance is " + distance.magnitude);
                    if (distance.magnitude < 0.7)
                    {
                        pictureCurrentlyDragged = true;
                        picturePosition.x = rightHand.PalmPosition.x;
                        picturePosition.z = rightHand.PalmPosition.z;
                        picturePosition.y = rightHand.PalmPosition.y - 0.2f;
                        pic.transform.position = picturePosition;
                        pic.GetComponent<PictureDrag>().setAsFirstInList(pic);
                        return;
                    }
                }
            }
        }
    }

}
