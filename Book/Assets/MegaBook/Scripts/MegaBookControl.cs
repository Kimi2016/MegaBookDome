
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

	void Update()
	{
		if ( Input.GetKeyDown(KeyCode.RightArrow) )
			book.NextPage();

		if ( Input.GetKeyDown(KeyCode.LeftArrow) )
			book.PrevPage();

	}

}