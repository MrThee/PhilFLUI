using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Phil.Core;

namespace Phil.FLUI {

[System.Serializable]
public class TRS2D {

    public AmplitudeCurve xPos = AmplitudeCurve.Constant(0f);
    public AmplitudeCurve yPos = AmplitudeCurve.Constant(0f);
    public AmplitudeCurve zRot = AmplitudeCurve.Constant(0f);
    public AmplitudeCurve xScale = AmplitudeCurve.Constant(1f);
    public AmplitudeCurve yScale = AmplitudeCurve.Constant(1f);
    public ScaleRectMode scaleMode = ScaleRectMode.AsScale;

    public void EnsureWraps(){
        xPos.normCurve.postWrapMode = WrapMode.Loop;
        xPos.normCurve.preWrapMode = WrapMode.Loop;
        yPos.normCurve.postWrapMode = WrapMode.Loop;
        yPos.normCurve.preWrapMode = WrapMode.Loop;
        zRot.normCurve.postWrapMode = WrapMode.Loop;
        zRot.normCurve.preWrapMode = WrapMode.Loop;
        xScale.normCurve.postWrapMode = WrapMode.Loop;
        xScale.normCurve.preWrapMode = WrapMode.Loop;
        yScale.normCurve.postWrapMode = WrapMode.Loop;
        yScale.normCurve.preWrapMode = WrapMode.Loop;
    }

    public enum ScaleRectMode {
        AsScale,
        AsSize
    }

    public Vector2 GetPosition(float t){
        return new Vector2(xPos.Evaluate(t), yPos.Evaluate(t));
    }

    public Vector2 GetScale(float t){
        return new Vector2(xScale.Evaluate(t), yScale.Evaluate(t));
    }
    
    public Vector2 TransformLocalPoint(Vector2 quadPoint, float t){
        Vector2 scale = new Vector2( xScale.Evaluate(t), yScale.Evaluate(t) );
        Vector2 offset = new Vector2( xPos.Evaluate(t), yPos.Evaluate(t) );
        Vector2 SV = Phil.Utils.Multiply(quadPoint, scale);
        float rot = zRot.Evaluate(t);
        float theta = rot*Mathf.Deg2Rad;
        Vector2 RSV = new Vector2( 
            SV.x*Mathf.Cos(theta) - SV.y*Mathf.Sin(theta),
            SV.y*Mathf.Sin(theta) + SV.y*Mathf.Cos(theta)
        );
        Vector2 TRSV = RSV + offset;
        return TRSV;
    }

    public void TransformRect(float t, RectTransform rt){
        rt.anchoredPosition = GetPosition(t);
        rt.localEulerAngles = Vector3.forward * zRot.Evaluate(t);
        switch(scaleMode){
            case ScaleRectMode.AsScale: {
                rt.localScale = GetScale(t);
            } break;

            case ScaleRectMode.AsSize: {
                rt.sizeDelta = GetScale(t);
            } break;
        }
    }

    public TRS2D(){
        this.xPos = AmplitudeCurve.Constant(0f);
        this.yPos = AmplitudeCurve.Constant(0f);
        this.zRot = AmplitudeCurve.Constant(0f);
        this.xScale = AmplitudeCurve.Constant(1f);
        this.yScale = AmplitudeCurve.Constant(1f);
    }

    public TRS2D(AmplitudeCurve xPos, AmplitudeCurve yPos, AmplitudeCurve zRot, AmplitudeCurve xScale, AmplitudeCurve yScale){
        this.xPos = xPos;
        this.yPos = yPos;
        this.zRot = zRot;
        this.xScale = xScale;
        this.yScale = yScale;
    }

    public TRS2D Clone() {
        return new TRS2D(xPos.Clone(), yPos.Clone(), zRot.Clone(), xScale.Clone(), yScale.Clone());
    }

}
    
}