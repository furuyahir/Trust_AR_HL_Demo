using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ARCondition
{
    public float HFOV = 60f;
    public float VFOV = 60f;
    
    public float mipLevel = 1;

    // offset variables
    public float xOffset = 0;
    public float yOffset = 0;
    public float zOffset = 0;
    public float xOrient = 0;
    public float yOrient = 0;
    public float zOrient = 0;

    // opacity variables
    public float opacityLevel = 1f;
}
