using UnityEngine;
using System.Collections;

public class ColliderTest : MonoBehaviour {

    public MegaBookBuilder book;

    void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.CompareTag("pic"))
        {
            Renderer renderer = other.gameObject.GetComponent<Renderer>();
            Texture2D texture = renderer.material.GetTexture("_MainTex") as Texture2D;
            book.SetPageTexture(texture, book.GetCurrentPage(), true);
            other.gameObject.SetActive(false);
            Debug.Log("collision detected"); 
            
        }
    }
}
