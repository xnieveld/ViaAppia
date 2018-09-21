using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;

public class CameraController: MonoBehaviour {
	private Vector3 offset;

	[Header("Variables")]
	public Transform cameraHolder;
    bool vr = false;
    Vector2 startMouseLocation;
    Quaternion cameraHolderStartQuaternion;
    bool dragging = false;
    float distance = 5;

   

    private void Update()
    {
        if (Input.touchCount == 2)
        {
            // Store both touches.
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // Find the position in the previous frame of each touch.
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // Find the magnitude of the vector (the distance) between the touches in each frame.
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // Find the difference in the distances between each frame.
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
            
            distance += 0.01f * deltaMagnitudeDiff;

            transform.localPosition = new Vector3(0, 0, -distance);

        }
        else
        {
            if (Input.touches.Any(x => x.phase == TouchPhase.Began))
            {
                Touch touch = Input.GetTouch(0);
                startMouseLocation = touch.position;
                cameraHolderStartQuaternion = cameraHolder.rotation;
                dragging = true;
            }
            if (Input.touches.Any(x => x.phase == TouchPhase.Ended))
            {
                dragging = false;
            }
            if (dragging)
            {
                Touch touch = Input.GetTouch(0);
                Vector2 deltaMouse = touch.position - startMouseLocation;
                cameraHolder.transform.rotation = cameraHolderStartQuaternion * Quaternion.Euler(new Vector3(-deltaMouse.y, deltaMouse.x, 0) * 0.2f);
            }



            if (Input.GetMouseButton(0) && VR)
            {
                VR = false;
            }
        }
    }

    public bool VR
    {
        get
        {
            return vr;
        }
        set
        {
            vr = value;
            if (vr)
            {
                StartCoroutine(LoadDevice("cardboard"));
            }
            else
            {
                StartCoroutine(LoadDevice("none"));
            }
        }
    }

    IEnumerator LoadDevice(string newDevice)
    {
        if (String.Compare(XRSettings.loadedDeviceName, newDevice, true) != 0)
        {
            XRSettings.LoadDeviceByName(newDevice);
            yield return null;
            XRSettings.enabled = true;
        }
    }
}
    //    void Update() {
    //        transform.RotateAround(Vector3.zero, Vector3.up, 20 * Time.deltaTime);
    //    }
//}