using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Phil;

namespace Phil.FLUI {

public class ISMText : MonoBehaviour, InteractiveStateMachine.IChangeStateCallback {

    [Header("Components")]
    public TMPro.TextMeshProUGUI textField;

    [Header("Default Config")]
    public InteractiveDynamicTextStyle defaultStyle;
    public IInteractiveDynamicCharStyle overrideStyle;
    public IInteractiveDynamicCharStyle style => overrideStyle ?? defaultStyle;

    public InteractiveState? currentState => m_ism.currentState;
    private InteractiveStateMachine m_ism;
    private PerCharOps<Vector3> m_pco;

    public void Init(InteractiveState? optInitialState, InteractiveState? postConfirmedState, int strBufCapacity){
        // TODO: migrate first arg to the style
        this.m_pco = PerCharOps.Positions(true, strBufCapacity, CalcGlyphPosition);
        Canvas.willRenderCanvases += this.TextUpdate;

        ChangeState(optInitialState);
    }

    void OnDestroy(){
        Canvas.willRenderCanvases -= this.TextUpdate;
    }

    // TODO: implement this
    Vector3 CalcGlyphPosition(Vector3 quadPoint, GlyphInfo gi){
        return style.CalcBlendedTransformLocalPoint(m_ism, quadPoint, gi);
    }

    void TextUpdate(){
        if(m_ism.currentState.HasValue){
            m_pco.WillRenderCanvasUpdate( textField );
        }
    }

    public void ChangeState(InteractiveState? optNIState){
        m_ism.ChangeState(optNIState, this);
    }

    public void DidChangeState(ref InteractiveStateMachine nism){
        this.gameObject.SetActive(nism.currentState.HasValue);
        Reapply();
    }

    void FixedUpdate(){
        m_ism.UpdateState(Time.fixedDeltaTime, style, this);
        Reapply();
    }

    void Reapply(){
        textField.color = style.CalcWordBlendedColor(m_ism);
    }
}

}