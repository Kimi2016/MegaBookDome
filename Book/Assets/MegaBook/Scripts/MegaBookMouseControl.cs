
using UnityEngine;
using System.Collections;

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
    

    void Update()
    {
        if (book)
        {
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
                            book.PrevPage(pageTurnSound);

                        if (hit.collider == nextcollider)
                            book.NextPage(pageTurnSound);

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
                        makeOutOfBookPicture(false, new Vector3(-1.5f, 0, 0) + prevcollider.transform.position, backTexture);
                    }

                    if (hit.collider == nextcollider)
                    {
                        makeOutOfBookPicture(true, new Vector3(1.5f, 0, 0) + nextcollider.transform.position, frontTexture);
                    }
                }
                else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                {
                    toMove.Clear();
                }
                else
                {
                    Debug.Log("10 pages now!");
                    book.AddPages(5);
                    book.UpdateSettings();
                    
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