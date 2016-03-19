using UnityEngine;
using System.Collections;

//attached to collider within the book.


public class PageCollider : MonoBehaviour {

    public MegaBookBuilder book;
    public bool next;

    void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.CompareTag("pic"))
        {
            Renderer renderer = other.gameObject.GetComponent<Renderer>();
            Texture2D texture = renderer.material.GetTexture("_MainTex") as Texture2D;
            if(next)
            book.SetPageTexture(texture, book.GetCurrentPage(), next);
            else
                book.SetPageTexture(texture, book.GetCurrentPage() - 1, next); 
            Destroy(other.gameObject);
            Debug.Log("collision detected"); 
            
        }
    }
}
