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

}

}