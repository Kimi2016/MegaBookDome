using UnityEngine;
using System.Collections;
using System;

public class PageTurner : MonoBehaviour {

    MegaBookBuilder book;
    AudioSource pageTurnSound;
    LeapController leapController;

    public PageTurner(MegaBookBuilder book, AudioSource pageTurnSound, LeapController leapController)
    {
        this.book = book;
        this.pageTurnSound = pageTurnSound;
        this.leapController = leapController;
    }

	void Start () {
	
	}


}
