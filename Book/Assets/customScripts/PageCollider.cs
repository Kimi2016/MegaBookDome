using UnityEngine;
using System.Collections;

//attached to collider within the book.


public class PageCollider : MonoBehaviour {

    public MegaBookBuilder book;
    public bool next;
    public MegaBookMouseControl mouseController;

    void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.CompareTag("pic"))
        {
            PictureDrag pd = other.gameObject.GetComponent<PictureDrag>();

            int pageNum = (int) book.GetPage();
            if (!pd.getIsInList())
            {
                setPage(other.gameObject, pageNum, next);
            }
            else if (pd.getIsInList())
            {
                bool front = next;
                foreach (GameObject value in pd.getMovingObjects())
                {
                    setPage(value, pageNum, front);

                    front = !front;

                    if (!front)
                        pageNum++;
                }
            }
        }
    }

    private void setPage(GameObject obj, int pageNum, bool front)
    {
        if (!front)
            pageNum--;
        try
        {
            if (mouseController.getStandardTexture(front) != book.GetPageTexture(pageNum, front))
                return;


            Renderer renderer = obj.GetComponent<Renderer>();
            Texture2D texture = renderer.material.GetTexture("_MainTex") as Texture2D;

            book.SetPageTexture(texture, pageNum, front);
            Destroy(obj);
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex);
        }
        Debug.Log(" Next : " + front + " Page number : " + pageNum);
    }
}
