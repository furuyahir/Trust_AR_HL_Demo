using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOVConditions : MonoBehaviour
{
    public Transform leftObj;
    public Transform rightObj;
    public Transform upObj;
    public Transform downObj;

    //public Transform renderQuad;

    private float depth = 0.5f;
    public float HFOV = 60f;
    public float VFOV = 60f;

    // Start is called before the first frame update
    void Start()
    {
        leftObj.localScale = new Vector3(1, 1, 0.1f);
        rightObj.localScale = new Vector3(1, 1, 0.1f);
        upObj.localScale = new Vector3(1, 1, 0.1f);
        downObj.localScale = new Vector3(1, 1, 0.1f);
        //depth = Camera.main.nearClipPlane + 0.001f;
    }
    // Update is called once per frame
    void Update()
    {
        // calculate the left position
        float xShift = Mathf.Tan(HFOV * Mathf.Deg2Rad / 2) + leftObj.localScale.x / 2;
        Vector3 xPositionOffset = new Vector3(xShift, 0, depth); // this is the local position relative to camera
        Vector3 xPositionOffset2 = new Vector3(-1 * xShift, 0, depth); // this is the local position relative to camera

        // calculate the up position
        float yShift = Mathf.Tan(VFOV * Mathf.Deg2Rad / 2) + upObj.localScale.y / 2;
        Vector3 yPositionOffset = new Vector3(0, yShift, depth);
        Vector3 yPositionOffset2 = new Vector3(0, -1 * yShift, depth);

        // convert these local positions into global positions... 
        leftObj.localPosition = xPositionOffset;
        rightObj.localPosition = xPositionOffset2;
        upObj.localPosition = yPositionOffset;
        downObj.localPosition = yPositionOffset2;

        // calculate the appropriate scale of the objects
        upObj.localScale = new Vector3(2*xShift, upObj.localScale.y, upObj.localScale.z);
        downObj.localScale = new Vector3(2*xShift, downObj.localScale.y, downObj.localScale.z);
        leftObj.localScale = new Vector3(leftObj.localScale.x, 2*yShift, leftObj.localScale.z);
        rightObj.localScale = new Vector3(rightObj.localScale.x, 2*yShift, rightObj.localScale.z);

    }
}
