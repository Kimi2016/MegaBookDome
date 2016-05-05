
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class MegaBookControl : MonoBehaviour
{
    public MegaBookBuilder book;
    void Start()
	{
        Debug.Log("Starting Controller");
	}

    void OnGUI()
    {
        
     
    }


    void Update()
	{
		if ( Input.GetKeyDown(KeyCode.RightArrow) )
			book.NextPage(new AudioSource());

		if ( Input.GetKeyDown(KeyCode.LeftArrow) )
			book.PrevPage(new AudioSource());

	}

}