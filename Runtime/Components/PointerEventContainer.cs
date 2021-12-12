using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.EventSystems;

using Phil;
using Phil.Core;

namespace Phil.FLUI {

public class PointerEventContainer : MonoBehaviour,
ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler, ISubmitHandler, IPointerClickHandler
{
    private EventSystem w_eventSystem { get {
        if(w__eventSystem==null){
            w__eventSystem = Phil.Utils.GetClosestComponentFromAncestors<EventSystem>(transform);
        }
        return w__eventSystem;
    }}

    private EventSystem w__eventSystem;

    public System.Action OnSubmitCallback;
    public System.Action OnSelectCallback;
    public System.Action OnDeselectCallback;

    public bool affectedByNavigation => OnSubmitCallback!=null;

    public void OnPointerEnter(PointerEventData data){
        if(affectedByNavigation == false){
            return;
        }
        w_eventSystem.ChangeSelection(this.gameObject);
    }

    public void OnSelect(BaseEventData data){
        if(affectedByNavigation == false){
            return;
        }
        OnSelectCallback?.Invoke();
    }

    public void OnPointerExit(PointerEventData data){
        if(affectedByNavigation == false){
            return;
        }
        if(w_eventSystem.currentSelectedGameObject == this.gameObject){
            w_eventSystem.ChangeSelection(null);
        }
    }

    public void OnDeselect(BaseEventData data){
        if(affectedByNavigation == false){
            return;
        }
        OnDeselectCallback?.Invoke();
    }

    public void OnPointerClick(PointerEventData data){
        // TODO: see what this data is when confirmed w/ a gamepad...
        if(data.button != PointerEventData.InputButton.Left){
            return;
        }
        if(affectedByNavigation == false){
            return;
        }
        OnSubmit(data);
    }

    public void OnSubmit(BaseEventData data){
        if(affectedByNavigation == false){
            return;
        }
        OnSubmitCallback.Invoke();
    }

}

}
