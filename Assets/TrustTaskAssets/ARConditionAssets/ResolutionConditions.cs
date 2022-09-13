using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResolutionConditions : MonoBehaviour
{
    public Material mipShaderMat;
    public Transform renderCameraTrans;
    //public int horizontalRes = 1920;
    //public int verticalRes = 1080;
    //private float horizontalMip;
    //private float verticalMip;
    public float mipLevel = 1;

    // offset variables
    public float xOffset = 0;
    public float yOffset = 0;
    public float zOffset = 0;
    public float xRotationOffset = 0;
    public float yRotationOffset = 0;
    public float zRotationOffset = 0;

    // opacity variables
    public float opacityRange = 1f;

    // Start is called before the first frame update
    void Start()
    {
        mipShaderMat.SetFloat("_MipLevel", mipLevel);
        renderCameraTrans.localPosition = new Vector3(xOffset,yOffset,zOffset);
        renderCameraTrans.localRotation = Quaternion.Euler(new Vector3(xRotationOffset, yRotationOffset, zRotationOffset));
    }

    // Update is called once per frame
    void Update()
    {
        mipShaderMat.SetFloat("_MipLevel", mipLevel);
        renderCameraTrans.localPosition = new Vector3(xOffset, yOffset, zOffset);
        renderCameraTrans.localRotation = Quaternion.Euler(new Vector3(xRotationOffset, yRotationOffset, zRotationOffset));
        mipShaderMat.SetFloat("_OpacityLevel", opacityRange);
    }

    public void SetMipInShader(float mip)
	{
        if (mipLevel != mip)
        {
            mipShaderMat.SetFloat("_MipLevel", mip);
            mipLevel = mip;
        }
	}

    public void SetRegistrationError(float x, float y, float z, float xRot, float yRot, float zRot)
	{
        renderCameraTrans.localPosition = new Vector3(x, y, z);
        renderCameraTrans.localRotation = Quaternion.Euler(new Vector3(xRot, yRot, zRot));
    }

    public void SetOpacity(float opacity)
	{
        if(opacity != opacityRange && opacity >= 0 && opacity <= 1f)
		{
            mipShaderMat.SetFloat("_OpacityLevel", opacity);
		}
	}

}
