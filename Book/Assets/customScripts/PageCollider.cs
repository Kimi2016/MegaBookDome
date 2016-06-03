using System;
using UnityEngine;

//attached to collider within the book.


public class PageCollider : MonoBehaviour
{
    public MegaBookBuilder book;
    public bool next;
    public MegaBookMouseControl mouseController;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("pic"))
        {
            if (book.page <= 0 && !next)
                return;
            if (book.page < 0 && next)
                return;
            
                PictureDrag pd = other.gameObject.GetComponent<PictureDrag>();

                int pageNum = Mathf.RoundToInt(book.GetPage());
                if (!pd.getIsInList())
                    setPage(other.gameObject, pageNum, next, true);
                else if (pd.getIsInList())
                {
                    bool front = next;
                    foreach (GameObject value in pd.getMovingObjects())
                    {
                        bool done;
                        do
                        {
                            done = setPage(value, pageNum, front, false);
                            front = !front;
                            if (!front)
                                pageNum++;
                        } while (!done);
                    }
                }
        }
    }

    private bool setPage(GameObject obj, int pageNum, bool front, bool oneTime)
    {
        if (!front)
            pageNum--;
        try
        {
            if (mouseController.getStandardTexture(front) != book.GetPageTexture(pageNum, front))
            {
                if (oneTime)
                    try
                    {
                        mouseController.makePage(front);
                    }
                    catch (Exception ex)
                    {

                    }
                else
                    return false;
            }

            Renderer renderer = obj.GetComponent<Renderer>();
            Texture2D texture = renderer.material.GetTexture("_MainTex") as Texture2D;

            book.SetPageTexture(texture, pageNum, front);
            Destroy(obj);
        }
        catch (Exception ex)
        {
            book.AddPages(2);
            setPage(obj, pageNum, front, oneTime);
            Debug.Log("Error : " + ex);
        }
        return true;
    }
}
