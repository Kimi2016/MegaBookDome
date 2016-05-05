using UnityEngine;
using UnityEngine.UI;

public class CameraFacingBillboard : MonoBehaviour
{
    public Camera m_Camera;
    public MegaBookBuilder book;
    private Text pageNumberText;


    void Start()
    {
        pageNumberText = transform.FindChild("PageNum").GetComponent<Text>();
    }

    void Update()
    {
        int temp = Mathf.RoundToInt(book.GetPage());
        Debug.Log(temp);
        pageNumberText.text = "" + temp;
        transform.LookAt(transform.position + m_Camera.transform.rotation * Vector3.forward,
            m_Camera.transform.rotation * Vector3.up);
    }
}