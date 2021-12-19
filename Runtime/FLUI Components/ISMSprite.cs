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
    public InteractiveDynamicSpriteStyle defaultStyle;
    public IInteractiveDynamicSpriteStyle overrideStyle;
    public IInteractiveDynamicSpriteStyle style => overrideStyle ?? defaultStyle;

    public InteractiveState? currentState => m_ism.currentState;
    private InteractiveStateMachine m_ism;

    public void Init(InteractiveState? optInitialState, InteractiveState? postConfirmedState){
        m_ism = new InteractiveStateMachine(optInitialState, postConfirmedState);
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
        Reapply();
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
            this.colorable, spriteable
        );
    }
}

}