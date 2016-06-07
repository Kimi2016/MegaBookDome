using UnityEngine;
using Leap;
using System.Timers;
using System;
using System.Collections.Generic;

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
   //private LeapPageTurner leapPageTurner;
  //  private LeapDragAndDrop leapDragAndDrop;
    private LeapPageTurnerV2 leapPageTurnerV2;
    private bool notTapping = true;
    private bool notTappingNTimer = true;
    private Timer tapTimer;

    void Start()
    {
        pictureCurrentlyDragged = false;
        controller = new Controller();
        leapProvider = FindObjectOfType<LeapProvider>() as LeapProvider;
        if (!controller.IsConnected)
            Debug.LogError("no LeapMotion Device detected...");
        else 
            Debug.Log("Device connected, continue");
            
     //   leapPageTurner = new LeapPageTurner(book, controller, pageTurnSoundSlow, pageTurnSoundFast);
      //  leapDragAndDrop = new LeapDragAndDrop(leapProvider);
        leapPageTurnerV2 = new LeapPageTurnerV2(book, controller, pageTurnSoundSlow);
    }

   /* public LeapDragAndDrop GetLeapDragAndDrop() {
        return leapDragAndDrop;
    }*/

    void Update()
    {
        Frame frame = leapProvider.CurrentFrame;

        if (!pictureCurrentlyDragged)
        {
            //leapPageTurner.CheckPageTurnGesture(frame.Hands);
            leapPageTurnerV2.CheckPageTurnGesture(frame.Hands);
        }

        foreach (Hand hand in frame.Hands)
        {
            if (hand.IsRight)
            {
                DragAndDropChecker(hand);
                SelectChecker(frame.Hands);
                //leapDragAndDrop.CheckDragAndDropGesture(hand);
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

    private void SelectChecker(List<Hand> hands)
    {
        if (hands.Count > 1)
        {
            Hand leftHand = null;
            Hand rightHand = null;
            foreach (Hand hand in hands)
            {
                if (hand.IsRight)
                    rightHand = hand;
                else if (hand.IsLeft)
                    leftHand = hand;
            }
            if (leftHand != null && rightHand != null)
            {
                if (leftHand.PalmNormal.y > 0.7)
                {
                    if (rightHand.GrabStrength > 0.7 && notTapping)
                    {
                        Ray ray;
                        RaycastHit hit;
                        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                        if (Physics.Raycast(ray, out hit))
                        {
                            if (hit.collider.tag == "pic")
                            {
                                hit.transform.GetComponent<PictureDrag>().selectNDeselPic();
                            }
                        }
                        notTapping = false;
                    }
                    else if (rightHand.GrabStrength < 0.6)
                    {
                        notTapping = true;
                    }
                }
            }
        }
    }

    private void initializeSpeedDecrease()
    {
        tapTimer = new Timer(1000); //Set Timer intervall 
        tapTimer.Elapsed += tapTimerReset; // Hook up the method to the timer
        tapTimer.Enabled = true;
    }

    void tapTimerReset(object sender, ElapsedEventArgs e)
    {
        if(notTapping)
        {
            notTappingNTimer = true;
            tapTimer.Enabled = false;
        }
    }

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
                try
                {
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
                            foreach (GameObject sPic in pic.GetComponent<PictureDrag>().getMovingObjects())
                            {
                                Vector3 relativeToMasterPic = sPic.transform.position + picturePosition;
                                sPic.transform.position = new Vector3(relativeToMasterPic.x - pic.transform.position.x, relativeToMasterPic.y - pic.transform.position.y, relativeToMasterPic.z - pic.transform.position.z);
                                /* Vector3 relativeToMasterPic = sPic.transform.position - picturePosition;
                                 sPic.transform.position = relativeToMasterPic + pic.transform.position;*/
                            }
                            pic.transform.position = picturePosition;
                            
                            pic.GetComponent<PictureDrag>().setAsFirstInList(pic);
                            return;
                        }
                    }
                }
                catch
                {

                }
            }
        }
    }
}
