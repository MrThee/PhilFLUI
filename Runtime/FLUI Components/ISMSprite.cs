using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Phil;

namespace Phil.FLUI {

public class ISMSprite : MonoBehaviour, InteractiveStateMachine.IChangeStateCallback {

    [Header("Components")]
    public RectTransform localTransformable;

    public FLUIColorable colorable;

    [Header("Default Config")]
    public InteractiveDynamicSpriteStyle style;

    private InteractiveStateMachine m_ism;

    public void Init(InteractiveState? optInitialState){
        ChangeState(optInitialState);
    }

    public void ChangeState(InteractiveState? optNIState){
        m_ism.ChangeState(optNIState, this);
    }

    public void DidChangeState(ref InteractiveStateMachine ism){
        this.gameObject.SetActive(ism.currentState.HasValue);
        Reapply();
    }

    void FixedUpdate(){
        m_ism.UpdateState(Time.fixedDeltaTime, style, this);
    }

    void Reapply(){
        FLUISpriteable spriteable;
        switch(this.colorable.type){
        default:
        case FLUIColorable.Type.SpriteRenderer: {
            spriteable = FLUISpriteable.Sprite(this.colorable.spriteRenderer);
        } break;
        case FLUIColorable.Type.UIImage: {
            spriteable = FLUISpriteable.Image(this.colorable.image);
        } break;
        }
        style.BlendedApplyAll( m_ism, 
            FLUITransformable.Same(localTransformable),
            this.colorable,  spriteable
        );
    }
}

}