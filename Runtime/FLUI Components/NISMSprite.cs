using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Phil;

namespace Phil.FLUI {

public class NISMSprite : MonoBehaviour, NIStateMachine.IChangeStateCallback {

    [Header("Components")]
    public RectTransform localTransformable;
    public FLUIColorable colorable;

    [Header("Default Config")]
    public DynamicSpriteStyle style;

    public NonInteractiveState? currentState => m_nism.currentState;
    private NIStateMachine m_nism;

    public void Init(NonInteractiveState? optInitialState){
        ChangeState(optInitialState);
    }

    public void ChangeState(NonInteractiveState? optNIState){
        m_nism.ChangeState(optNIState, this);
    }

    public void DidChangeState(ref NIStateMachine nism){
        this.gameObject.SetActive(nism.currentState.HasValue);
        Reapply();
    }

    void FixedUpdate(){
        m_nism.UpdateState(Time.fixedDeltaTime, style, this);
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
        style.BlendedApplyAll( m_nism, 
            FLUITransformable.Same(localTransformable),
            this.colorable,  spriteable
        );
    }
}

}