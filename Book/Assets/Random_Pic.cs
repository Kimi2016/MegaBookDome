using UnityEngine;

public class Random_Pic : MonoBehaviour {
    MegaBookBuilder obj;

    private int pageTexUsed = 0; 
    private bool frontText;
    
	void Start () {
        obj = GameObject.Find("Book").GetComponent<MegaBookBuilder>();

        Random.seed = (int)System.DateTime.Now.Ticks;
        pageTexUsed = Random.Range(5, obj.NumPages - 1);
        frontText = (Random.value < 0.5);
    }

    // Update is called once per frame
    void Update()
    { 
        transform.GetComponent<Renderer>().material.SetTexture("_MainTex", obj.GetPageTexture(pageTexUsed, frontText));
    }
}
