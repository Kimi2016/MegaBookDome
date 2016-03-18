
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class MegaBookControl : MonoBehaviour
{
    public MegaBookBuilder book;
    public List<MegaBookPage> pages = new List<MegaBookPage>();
    public List<MegaBookPageParams> pageparams = new List<MegaBookPageParams>();
    public Texture2D samplePic;
   

    void Start()
	{
        Debug.Log("Starting Controller");
	}

	void Update()
	{
		if ( Input.GetKeyDown(KeyCode.RightArrow) )
			book.NextPage();

		if ( Input.GetKeyDown(KeyCode.LeftArrow) )
			book.PrevPage();

        if (Input.GetKeyDown(KeyCode.DownArrow)) {
            Debug.Log("KeyDown pressed, adding picture now.");
            StartCoroutine(book.DownloadTexture("file:///d:/test.jpg", 2, true));
        }
	}

    void OnMouseOver() {
        Debug.Log("drop,drop drop!!!!");
    }


    void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.CompareTag("pic"))
        {
            Debug.Log("My type is " +  other.gameObject.GetType());
        }
    }
}