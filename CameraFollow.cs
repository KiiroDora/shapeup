using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    public Transform target; //target's 3D transform

    private Vector3 velocity = Vector3.zero; //reference variable for smoothdamp of camera position
    public float smoothTime = 0.3f; //speed for linear movement
    public Vector3 offset; //offset for camera
    
    public static float zoom; //variable to deposit orthographic size of the camera
    public float zoomvel = 0f; //reference variable for the smoothdamp of orthographic size

    void Start()
    {
        zoom = Camera.main.orthographicSize;
    }

    void FixedUpdate()
    {
        Vector3 targetpos = target.position + offset; //set up initial position with offset
        Vector3 smoothpos = Vector3.SmoothDamp(transform.position, targetpos, ref velocity, smoothTime); //move from initial position to target's position with linear speed
        
        smoothpos.z = transform.position.z; //since this is 2D don't change the z axis
        transform.position = smoothpos; //update position

    }

    void LateUpdate()
    {
        if (Input.GetKey(KeyCode.C)) //when C is held
        {
            float zoomset = Mathf.SmoothDamp(Camera.main.orthographicSize, 10f, ref zoomvel, 1f); //zoom out to orthographic size 10 with linear speed
            Camera.main.orthographicSize = zoomset; //update orthographic size
        }
        
        if (Input.GetKeyUp(KeyCode.C) && !GameController.winscr.activeSelf && !GameController.pausscr.activeSelf) //when C is released and no pause/win screen is active
        {
            Camera.main.orthographicSize = zoom; //reset orthographic size to 6
        }

    }

    public static void ResetCamera() //static function to remotely reset camera view from other scripts
    {
        Camera.main.orthographicSize = zoom;
    }
}
