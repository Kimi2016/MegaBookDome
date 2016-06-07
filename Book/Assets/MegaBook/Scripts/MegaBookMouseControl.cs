
using UnityEngine;
using System.Collections;
using System.IO;


// Very simple script to allow mouse clicks to turn pages


public class MegaBookMouseControl : MonoBehaviour
{
    public MegaBookBuilder book;
    public Collider prevcollider;
    public Collider nextcollider;
    public GameObject prefabPage;
    public Texture2D frontTexture;
    public Texture2D backTexture;
    public int speedPage;
    public AudioSource pageTurnSound;
    public bool forward = true;
    public ArrayList toMove = new ArrayList();
    private SpeedONeedle needle;
    private bool picNotAdded = true;
    public bool disableOnStart = false;
    
    void Start()
    {
        gameObject.SetActive(!disableOnStart);
        this.needle = book.transform.parent.Find("SpeedCanvas").transform.Find("SpeedDial").GetChild(0).GetComponent<SpeedONeedle>();
        
    }

    void Update()
    {   
        if (book)
        {
            //pageNumberText.text = "" + (int) book.GetPage();
            if (Input.GetMouseButtonDown(0))
            {
                if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                {
                    RaycastHit hit;
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
                    {
                        if (hit.collider.gameObject.tag == "pic")
                        {
                            GameObject thisGameObj = hit.collider.gameObject;

                            bool picIsInList = toMove.Contains(thisGameObj);
                            if (!picIsInList)
                                toMove.Add(thisGameObj);
                        }
                    }
                }
                else if (prevcollider && nextcollider)
                {
                    RaycastHit hit;
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
                    {
                        if (hit.collider == prevcollider)
                        {
                            book.PrevPage(pageTurnSound);
                            needle.AddSpeed(1);
                        }

                        if (hit.collider == nextcollider)
                        {
                            book.NextPage(pageTurnSound);
                            needle.AddSpeed(1);
                        }

                    }
                }
                
            }
            else if (Input.GetMouseButtonDown(1))
            {
                RaycastHit hit;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
                {
                    if (hit.collider == prevcollider)
                    {
                        makePage(false);
                    }

                    if (hit.collider == nextcollider)
                    {
                        makePage(true);
                    }
                }
                else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                {
                    toMove.Clear();
                }
                else
                {
                    Debug.Log("10 pages now!");
                    book.AddPages(-2);
                    //book.UpdateSettings();
                    
                    //book.rebuild = true;
                  //  book.BuildPages();

                }
            }
            //Doesn't work on the front cover.
            else if (Input.GetMouseButtonDown(2))
            {
                cycleFast(speedPage, forward);
            }
        }
    }
    
	private void cycleFast(int pages, bool dirForward)
	{
	if(dirForward)
	book.SetPage(pages + (int) book.GetPage(), false, pageTurnSound);
	
	if(!dirForward)
	book.SetPage((int) book.GetPage() - pages, false, pageTurnSound);
	}

    public void makePage(bool next)
    {
        if (!next)
        {
            makeOutOfBookPicture(false, new Vector3(-1.5f, 0, 0) + prevcollider.transform.position, backTexture);
        }

        if (next)
        {
            makeOutOfBookPicture(true, new Vector3(1.5f, 0, 0) + nextcollider.transform.position, frontTexture);
        }
    }

    //Front or back, front if true, push direction is relative to the page position, texture is the page texture if it is the standard once, it will do nothing.
    private void makeOutOfBookPicture(bool front, Vector3 pushDirection, Texture2D texture)
    {
        try
        {
            int pageNum = book.GetCurrentPage();
            if (!front)
                pageNum = pageNum-1;

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

            book.RemoveUnnecessaryPages(front, frontTexture, backTexture, pageNum);
        }
        catch(System.Exception e)
        {
            Debug.Log(e.Data);
        }
    }

    public Texture2D getStandardTexture(bool front)
    {
        if (front)
            return frontTexture;
        else
            return backTexture;
    }
}