
using UnityEngine;

// Very simple script to allow mouse clicks to turn pages

public class MegaBookMouseControl : MonoBehaviour
{
	public MegaBookBuilder book;
	public Collider			prevcollider ;
    public Collider			nextcollider;

    void Update()
	{
		if ( book )
		{
			
				if ( prevcollider && nextcollider )
				{
					RaycastHit	hit;
					if ( Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit) )
					{
                    if (hit.collider == prevcollider || hit.collider == nextcollider) {
                        Debug.Log("dropdropdrop");
                    }
                    if (Input.GetMouseButtonDown(0))
                        {
                            if ( hit.collider == prevcollider )
							    book.PrevPage();

						    if ( hit.collider == nextcollider )
							    book.NextPage();
					    }
				}
			}

        }
	}
}