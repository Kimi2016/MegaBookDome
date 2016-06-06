using UnityEngine;
using System.Collections;

public class AddPictures : MonoBehaviour {
    public MegaBookBuilder book;
    public bool active = true;
    // Use this for initialization
    void Start()
    {
    }

    void LateUpdate()
    {
        if (active)
        {
            int numTemp = 0;
            bool front = true;
            Object[] textures = Resources.LoadAll("Picture");
            foreach (Object obj in textures)
            {
                book.SetPageTexture(Resources.Load("Picture/" + obj.name) as Texture2D, numTemp, front);
                if (!front)
                    numTemp++;
                front = !front;
                if (book.NumPages <= numTemp)
                    break;
            }
            active = false;
        }
    }
}
