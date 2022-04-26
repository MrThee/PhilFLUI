using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Phil;

namespace Phil.FLUI {

public class NISMText : MonoBehaviour, NIStateMachine.IChangeStateCallback {

    [Header("Components")]
    public TMPro.TextMeshProUGUI textField;

    [Header("Default Config")]
    public DynamicTextStyle defaultStyle;
    public IDynamicCharStyle overrideStyle;
    public IDynamicCharStyle style => overrideStyle ?? defaultStyle;

    public NonInteractiveState? currentState => m_nism.currentState;
    private NIStateMachine m_nism;
    private PerCharOps<Vector3> m_pco;
    // private System.Text.StringBuilder m_strbuf;

    public void Init(NonInteractiveState? optInitialState, int strBufCapacity){
        // this.m_strbuf = new System.Text.StringBuilder(strBufCapacity);
        m_nism = new NIStateMachine(optInitialState);
        this.m_pco = PerCharOps.Positions(true, strBufCapacity, CalcGlyphPosition);
        ChangeState(optInitialState);

        Canvas.willRenderCanvases += this.TextUpdate;
    }

    void OnDestroy(){
        Canvas.willRenderCanvases -= this.TextUpdate;
    }

    Vector3 CalcGlyphPosition(Vector3 quadPoint, GlyphInfo gi){
        return style.CalcBlendedTransformLocalPoint(m_nism, quadPoint, gi);
    }

    void TextUpdate(){
        if(m_nism.currentState.HasValue){
            m_pco.WillRenderCanvasUpdate( textField );
        }
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
        Reapply();
    }

    void Reapply(){
        textField.color = style.CalcWordBlendedColor(m_nism);
    }
}

}