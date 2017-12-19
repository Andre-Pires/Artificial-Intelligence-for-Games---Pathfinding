using UnityEngine;

public class CameraControlScript : MonoBehaviour
{
    public float ScrollSpeed = 15f;
    public float ScrollEdge = 0.01f;

    private int HorizontalScroll = 1;
    private int VerticalScroll = 1;
    private int DiagonalScroll = 1;

    public float PanSpeed = 10.0f;

    public Vector2 ZoomRange = new Vector2(-5, 5);
    public float CurrentZoom = 0.0f;
    public float ZoomZpeed = 1.0f;
    public float ZoomRotation = 1.0f;

    private Vector3 InitPos;
    private Vector3 InitRotation;

    public void Start()
    {
        //Instantiate(Arrow, Vector3.zero, Quaternion.identity);

        InitPos = transform.position;
        InitRotation = transform.eulerAngles;
    }

    public void Update()
    {
        //PAN
        if (Input.GetKey(KeyCode.Mouse1))
        {
            transform.Translate(Vector3.right * Time.deltaTime * PanSpeed * (Input.mousePosition.x - Screen.width * 0.5f) / (Screen.width * 0.5f), Space.World);
            transform.Translate(Vector3.forward * Time.deltaTime * PanSpeed * (Input.mousePosition.y - Screen.height * 0.5f) / (Screen.height * 0.5f), Space.World);
        }
        else
        {
            if (Input.GetKey("d"))
            {
                transform.Translate(Vector3.right * Time.deltaTime * ScrollSpeed, Space.World);
            }
            else if (Input.GetKey("a"))
            {
                transform.Translate(Vector3.right * Time.deltaTime * -ScrollSpeed, Space.World);
            }

            if (Input.GetKey("w"))
            {
                transform.Translate(Vector3.forward * Time.deltaTime * ScrollSpeed, Space.World);
            }
            else if (Input.GetKey("s"))
            {
                transform.Translate(Vector3.forward * Time.deltaTime * -ScrollSpeed, Space.World);
            }
        }

        //ZOOM IN/OUT

        CurrentZoom -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * 1000 * ZoomZpeed;

        CurrentZoom = Mathf.Clamp(CurrentZoom, ZoomRange.x, ZoomRange.y);

        transform.position -= new Vector3(0, (transform.position.y - (InitPos.y + CurrentZoom)) * 0.1f, 0);
        transform.eulerAngles -= new Vector3((transform.eulerAngles.x - (InitRotation.x + CurrentZoom * ZoomRotation)) * 0.1f, 0, 0);
    }
}