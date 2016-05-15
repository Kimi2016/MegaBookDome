using UnityEngine;
using UnityEngine.UI;

public class CameraFacingBillboard : MonoBehaviour
{
    public Camera m_Camera;
    public MegaBookBuilder book;
    private Text pageNumberText;
    private bool isShowing;
    private bool isOverIt;


    void Start()
    {
        pageNumberText = transform.FindChild("PageNum").GetComponent<Text>();
        int i = transform.childCount;
        for (int x = 0;  x < i; x++)
        {
            transform.GetChild(x).gameObject.SetActive(false);
        }
        isShowing = false;
        isOverIt = false;
    }

    public void setChildrensActive(bool a)
    {
        int i = transform.childCount;
        for (int x = 0; x < i; x++)
        {
            transform.GetChild(x).gameObject.SetActive(a);
        }
    }

    private Ray ray;
    private RaycastHit hit;

void Update()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            if (!isOverIt)
            {
                if (hit.collider.name == "NextPageHit" || hit.collider.name == "PrevPageHit")
                {
                    isOverIt = true;
                    setChildrensActive(isOverIt);
                }
            }
        }
        else if (isOverIt)
        {
            isOverIt = false;
            setChildrensActive(isOverIt);

        }
        

        int temp = Mathf.RoundToInt(book.GetPage());
        pageNumberText.text = "" + temp;
        transform.LookAt(transform.position + m_Camera.transform.rotation * Vector3.forward,
            m_Camera.transform.rotation * Vector3.up);
    }
}