using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Phil;
using Phil.Core;

namespace Phil.FLUI {

[System.Serializable]
public class FLUIEventContainer : PolyObject<FLUIEventContainer.Type, PointerEventContainer, SmartPointerEventContainer> {

    public enum Type : int {
        PointerEvent,
        SmartPointerEvent
    }

    public System.Action OnSelectCallback {
        get {
            switch(this.type){
            default:
            case FLUIEventContainer.Type.PointerEvent: return componentA.OnSelectCallback;
            case FLUIEventContainer.Type.SmartPointerEvent: return componentB.OnSelectCallback;
            }
        }
        set {
            switch(this.type){
            case FLUIEventContainer.Type.PointerEvent: componentA.OnSelectCallback = value; break;
            case FLUIEventContainer.Type.SmartPointerEvent: componentB.OnSelectCallback = value; break;
            }
        }
    }

    public System.Action OnDeselectCallback {
        get {
            switch(this.type){
            default:
            case FLUIEventContainer.Type.PointerEvent: return componentA.OnDeselectCallback;
            case FLUIEventContainer.Type.SmartPointerEvent: return componentB.OnDeselectCallback;
            }
        }
        set {
            switch(this.type){
            case FLUIEventContainer.Type.PointerEvent: componentA.OnDeselectCallback = value; break;
            case FLUIEventContainer.Type.SmartPointerEvent: componentB.OnDeselectCallback = value; break;
            }
        }
    }

}

}