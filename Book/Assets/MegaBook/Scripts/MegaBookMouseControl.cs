
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
    public int fastCycle;
public bool forward = true;
    

    void Update()
    {
        if (book)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (prevcollider && nextcollider)
                {
                    RaycastHit hit;
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
                    {
                        if (hit.collider == prevcollider)
                            book.PrevPage();

                        if (hit.collider == nextcollider)
                            book.NextPage();
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
                        Debug.Log("Prev hit confirm.");
                        makeOutOfBookPicture(false, new Vector3(-1.5f, 0, 0) + prevcollider.transform.position, backTexture);
                    }

                    if (hit.collider == nextcollider)
                    {
                        makeOutOfBookPicture(true, new Vector3(1.5f, 0, 0) + nextcollider.transform.position, frontTexture);
                    }
                }
            }
            //Doesn't work on the front cover.
            else if (Input.GetMouseButtonDown(2))
            {
                cycleFast(fastCycle, forward);
            }
            }
    }
    
	private void cycleFast(int pages, bool dirForward)
	{
	if(dirForward)
	book.SetPage(1 + pages + (int) book.GetPage(), false);
	
	if(!dirForward)
	book.SetPage((int) book.GetPage() - pages + 1, false);
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

            Material material = new Material(Shader.Find("Transparent/Diffuse"));
            material.mainTexture = pageTexture;

            book.SetPageTexture(texture, pageNum, front);

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