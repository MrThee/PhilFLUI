using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Phil;

namespace Phil.FLUI {

[System.Serializable]
public struct FLUITransformable {
    public RectTransform offsetThis;
    public Transform rotateThis;
    public RectTransform scaleThis;

    public static FLUITransformable Same(RectTransform shared){
        FLUITransformable ft;
        ft.offsetThis = shared;
        ft.rotateThis = shared;
        ft.scaleThis = shared;
        return ft;
    }

    public static FLUITransformable TRS(RectTransform offsetThis, Transform rotateThis, RectTransform scaleThis){
        FLUITransformable ft;
        ft.offsetThis = offsetThis;
        ft.rotateThis = rotateThis;
        ft.scaleThis = scaleThis;
        return ft;
    }
}
    
}