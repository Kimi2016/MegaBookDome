using UnityEngine;

public class Random_Pic : MonoBehaviour {
    MegaBookBuilder obj;

    private int pageTexUsed = 0; 
    private bool frontText;
    private bool picset = false;
    
	void Start () {
        obj = GameObject.Find("Book").GetComponent<MegaBookBuilder>();

        Random.seed = (int)System.DateTime.Now.Ticks;
        pageTexUsed = Random.Range(5, obj.NumPages - 1);
        frontText = (Random.value < 0.5);
    }
    
    void Update()
    {
        if (!picset)
        {
            transform.GetComponent<Renderer>().material.SetTexture("_MainTex", obj.GetPageTexture(pageTexUsed, frontText));
            picset = true;
            this.gameObject.GetComponent<Renderer>().material.DisableKeyword("_NORMALMAP");
        }
    }
}
