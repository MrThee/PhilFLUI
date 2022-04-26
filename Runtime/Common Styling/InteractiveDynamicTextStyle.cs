using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Phil.Attributes;

namespace Phil.FLUI {

[CreateAssetMenu(menuName = "UI Styles/Dynamic Text/Interactive", order = 1, fileName = "Interactive Dynamic Text Style")]
public class InteractiveDynamicTextStyle : ScriptableObject, IInteractiveDynamicCharStyle {

    [InlineCorral] public Inline data = new Inline();

    public FLUICharBehaviour rollout => data.rollout;
    public FLUICharBehaviour idle => data.idle;
    public FLUICharBehaviour recede => data.recede;
    public FLUICharBehaviour highlighted => data.highlighted;
    public FLUICharBehaviour confirmed => data.confirmed;
    public float GetCrossfadePeriod() { return data.GetCrossfadePeriod(); }

    public float GetStatePeriod(InteractiveState iState){
        return data.GetBehaviour(iState)?.charPeriod ?? 0f;
    }

    public FLUICharBehaviour GetBehaviour(InteractiveState state){
        return data.GetBehaviour(state);
    }

    public Vector3 CalcBlendedTransformLocalPoint(InteractiveStateMachine ism,
            Vector3 quadPoint, GlyphInfo glyphInfo
    ) {
        return data.CalcBlendedTransformLocalPoint(ism, quadPoint, glyphInfo);
    }

    public Color CalcWordBlendedColor(InteractiveStateMachine iTextStateMachine){
        return CalcWordBlendedColor(this,    iTextStateMachine);
    }
        
    public static Color CalcWordBlendedColor(IInteractiveDynamicCharStyle style, InteractiveStateMachine iTextStateMachine){
        if(iTextStateMachine.currentState.HasValue==false){
            return Color.white;
        }
        var curState = iTextStateMachine.currentState.Value;
        var priorState = iTextStateMachine.priorState ?? curState;
        float crossfadePeriod = style.GetCrossfadePeriod();
        float t = (crossfadePeriod == 0f) ? 1f : iTextStateMachine.currentStateTimer / crossfadePeriod;
        var aBehave = style.GetBehaviour(priorState);
        var bBehave = style.GetBehaviour(curState);
        Color aColor = aBehave.wordGradient.Evaluate(iTextStateMachine.priorStateTimer/aBehave.wordColorPeriod);
        Color bColor = bBehave.wordGradient.Evaluate(iTextStateMachine.currentStateTimer/bBehave.wordColorPeriod);
        Color c = Color.Lerp(aColor, bColor, t);
        return c;
    }

    void OnValidate(){
        idle.TRS.EnsureWraps();
    }

    [System.Serializable]
    public class Inline : IInteractiveDynamicCharStyle {
        public float crossfadePeriod = 0.3f;
        public FLUICharBehaviour rollout = new FLUICharBehaviour();
        public FLUICharBehaviour idle = new FLUICharBehaviour();
        public FLUICharBehaviour recede = new FLUICharBehaviour();
        public FLUICharBehaviour highlighted = new FLUICharBehaviour();
        public FLUICharBehaviour confirmed = new FLUICharBehaviour();

        public float GetCrossfadePeriod() { return crossfadePeriod; }

        public float GetStatePeriod(InteractiveState istate){ return GetBehaviour(istate).charPeriod; }

        public FLUICharBehaviour GetBehaviour(InteractiveState state){
            switch(state){
                case InteractiveState.Rollout: return rollout;
                case InteractiveState.Idle: return idle;
                case InteractiveState.Recede: return recede;
                case InteractiveState.Highlighted: return highlighted;
                case InteractiveState.Confirmed: return confirmed;
                default: return null;
            }
        }

        public Vector3 CalcBlendedTransformLocalPoint(InteractiveStateMachine ism, Vector3 quadPoint, GlyphInfo glyphInfo){
            if(ism.currentState.HasValue == false){
                return Vector3.zero;
            }
            var curState = ism.currentState.Value;
            var priorState = ism.priorState ?? curState;
            return CalcBlendedTransformLocalPoint(this, ism.priorStateTimer, priorState, ism.currentStateTimer, curState, quadPoint, glyphInfo);
        }

        public static Vector3 CalcBlendedTransformLocalPoint(IInteractiveDynamicCharStyle style, float priorStateTimer, InteractiveState priorState, float newStateTimer, InteractiveState newState,
            Vector3 quadPoint, GlyphInfo glyphInfo
        ) {
            var aState = style.GetBehaviour(priorState);
            var bState = style.GetBehaviour(newState);
            float t = (style.GetCrossfadePeriod() == 0f) ? 1f : newStateTimer / style.GetCrossfadePeriod();
            Vector3 aPoint = aState.TransformLocalPoint(quadPoint, glyphInfo, priorStateTimer);
            Vector3 bPoint = bState.TransformLocalPoint(quadPoint, glyphInfo, newStateTimer);
            return Vector3.Lerp(aPoint, bPoint, t);
        }

        public Color CalcWordBlendedColor(InteractiveStateMachine ism){
            return InteractiveDynamicTextStyle.CalcWordBlendedColor(this, ism);
        }
    }
}

}