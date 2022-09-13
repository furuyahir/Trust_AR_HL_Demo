using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AEPostProcess : MonoBehaviour
{
    private RenderTexture arTexture;
    public Material arMat;
    public Camera arCam;
    private Transform mainCamT;

    // error/condition variables
    public float mipLevel = 1f;
    public float opacityLevel = 1f;
    public float xOffset = 0f;
    public float yOffset = 0f;
    public float zOffset = 0f;
    public float xOrient = 0f;
    public float yOrient = 0f;
    public float zOrient = 0f;


    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        // send the main camera's imagery to the material/shader
        Graphics.Blit(source,arMat);
        // now the shader will combine the Camera texture with the AR imagery texture

        
    }

    // Start is called before the first frame update
    void Start()
    {
        mainCamT = Camera.main.transform;
        arTexture = new RenderTexture(Camera.main.scaledPixelWidth, Camera.main.scaledPixelHeight, (int)Camera.main.depth);
        arTexture.useMipMap = true;
        arTexture.autoGenerateMips = true;
        arMat.SetTexture("_ARTex", arTexture);

        float cachedAspect = Camera.main.aspect;
        arCam.targetTexture = arTexture;
        arCam.aspect = cachedAspect; //Camera.main.pixelWidth / Camera.main.pixelHeight;

    }

    // Update is called once per frame
    void Update()
    {
        // scale the renderTexture
        arCam.aspect = Camera.main.aspect;


        // set opacity in the shader
        if (opacityLevel < 0)
            opacityLevel = 0;
        else if (opacityLevel > 1)
            opacityLevel = 1;
        arMat.SetFloat("_Opacity", opacityLevel);

        // set mip level in the shader
        if (mipLevel < 0)
            mipLevel = 0;
        else if (mipLevel > 6)
            mipLevel = 6;
        arMat.SetFloat("_MipLevel", mipLevel);

        // update the ar camera's position and orientatoin for registration errors
        arCam.transform.position = mainCamT.position + new Vector3(xOffset, yOffset, zOffset);
        arCam.transform.rotation = Quaternion.Euler(xOrient+ mainCamT.rotation.eulerAngles.x, 
            yOrient + mainCamT.rotation.eulerAngles.y, 
            zOrient + mainCamT.rotation.eulerAngles.z);
    }
}
