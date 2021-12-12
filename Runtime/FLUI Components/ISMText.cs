using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Phil;

namespace Phil.FLUI {

public class ISMText : MonoBehaviour, InteractiveStateMachine.IChangeStateCallback {

    [Header("Components")]
    public TMPro.TextMeshProUGUI textField;

    [Header("Default Config")]
    public InteractiveDynamicTextStyle style;

    private InteractiveStateMachine m_ism;
    private PerCharOps<Vector3> m_pco;
    private System.Text.StringBuilder m_strbuf;

    public void Init(InteractiveState? optInitialState, int strBufCapacity){
        this.m_strbuf = new System.Text.StringBuilder(strBufCapacity);
        // TODO: migrate first arg to the style
        this.m_pco = PerCharOps.Positions(true, strBufCapacity, CalcGlyphPosition);
        ChangeState(optInitialState);

        Canvas.willRenderCanvases += this.TextUpdate;
    }

    void OnDestroy(){
        Canvas.willRenderCanvases -= this.TextUpdate;
    }

    // TODO: implement this
    Vector3 CalcGlyphPosition(Vector3 quadPoint, int gi){
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