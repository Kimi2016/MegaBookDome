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
            int pageNum = (int) book.GetPage();

            if (!next)
                pageNum -= 1;
            try
            {
                if (mouseController.getStandardTexture(next) != book.GetPageTexture(pageNum, next))
                return;
            

            Renderer renderer = other.gameObject.GetComponent<Renderer>();
            Texture2D texture = renderer.material.GetTexture("_MainTex") as Texture2D;
            
                book.SetPageTexture(texture, pageNum, next);
                Destroy(other.gameObject);
            }
            catch(System.Exception ex)
            {
                Debug.Log(ex);
            }
            
        }
    }
}
