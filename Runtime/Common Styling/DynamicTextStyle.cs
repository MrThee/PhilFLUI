using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Phil.Attributes;

namespace Phil.FLUI {

[CreateAssetMenu(menuName = "UI Styles/Dynamic Text/Non Interactive", order = 0, fileName = "Dynamic Text Style")]
public class DynamicTextStyle : ScriptableObject, IDynamicCharStyle {

    [InlineCorral] public Inline data = new Inline();

    public FLUICharBehaviour rollout => data.rollout;
    public FLUICharBehaviour idle => data.idle;
    public FLUICharBehaviour recede => data.recede;
    public float GetCrossfadePeriod(){ return data.GetCrossfadePeriod(); }

    public float GetStatePeriod(NonInteractiveState _niState){
        return data.GetStatePeriod(_niState);
    }
    
    public FLUICharBehaviour GetBehaviour(NonInteractiveState state){
        return data.GetBehaviour(state);
    }

    public Color CalcWordBlendedColor(NIStateMachine dTextState){
        return Inline.CalcWordBlendedColor(this, dTextState);
    }

    public Vector3 CalcBlendedTransformLocalPoint(NIStateMachine dTextState, Vector3 quadPoint, int charIndex){
        return data.CalcBlendedTransformLocalPoint(dTextState, quadPoint, charIndex);
    }

    public Vector3 CalcBlendedTransformLocalPoint(float priorStateTimer, NonInteractiveState priorState, float newStateTimer, NonInteractiveState newState,
        Vector3 quadPoint, int charIndex
    ) {
        return Inline.CalcBlendedTransformLocalPoint(this, priorStateTimer, priorState, newStateTimer, newState, quadPoint, charIndex);
    }

    void OnValidate(){
        idle.TRS.EnsureWraps();
    }

    [System.Serializable]
    public class Inline : IDynamicCharStyle {
        public float crossfadePeriod = 0.2f;
        [InlineCorral] public FLUICharBehaviour rollout = new FLUICharBehaviour();
        [InlineCorral] public FLUICharBehaviour idle = new FLUICharBehaviour();
        [InlineCorral] public FLUICharBehaviour recede = new FLUICharBehaviour();

        public float GetCrossfadePeriod(){ return crossfadePeriod; }

        public float GetStatePeriod(NonInteractiveState _niState){
            return GetBehaviour(_niState).charPeriod;
        }
        
        public FLUICharBehaviour GetBehaviour(NonInteractiveState state){
            switch(state){
                case NonInteractiveState.Rollout: return rollout;
                case NonInteractiveState.Idle: return idle;
                case NonInteractiveState.Recede: return recede;
                default: return null;
            }
        }

        public Color CalcWordBlendedColor(NIStateMachine nism){
            return CalcWordBlendedColor(this, nism);
        }

        public static Color CalcWordBlendedColor(IDynamicCharStyle style, NIStateMachine dTextState){
            if(dTextState.currentState.HasValue == false){
                return Color.white;
            }
            var priorState = dTextState.priorState ?? dTextState.currentState.Value;
            var curState = dTextState.currentState.Value;
            float crossfadePeriod = style.GetCrossfadePeriod();
            float t = (crossfadePeriod == 0f) ? 1f : dTextState.currentStateTimer / crossfadePeriod;
            var aBehave = style.GetBehaviour(curState);
            var bBehave = style.GetBehaviour(priorState);
            Color aColor = aBehave.wordGradient.Evaluate(dTextState.priorStateTimer/aBehave.wordColorPeriod);
            Color bColor = bBehave.wordGradient.Evaluate(dTextState.currentStateTimer/bBehave.wordColorPeriod);
            Color c = Color.Lerp(aColor, bColor, t);
            return c;
        }

        public Vector3 CalcBlendedTransformLocalPoint(NIStateMachine dTextState, Vector3 quadPoint, int charIndex){
            if(dTextState.currentState.HasValue==false){
                return Vector3.zero;
            }
            var oldState = dTextState.priorState ?? dTextState.currentState.Value;
            return CalcBlendedTransformLocalPoint( this,
                dTextState.priorStateTimer, oldState, dTextState.currentStateTimer, dTextState.currentState.Value,
                quadPoint, charIndex
            );
        }

        public static Vector3 CalcBlendedTransformLocalPoint(IDynamicCharStyle style, float priorStateTimer, NonInteractiveState priorState, float newStateTimer, NonInteractiveState newState,
            Vector3 quadPoint, int charIndex
        ) {
            var aState = style.GetBehaviour(priorState);
            var bState = style.GetBehaviour(newState);
            float crossfadePeriod = style.GetCrossfadePeriod();
            float t = (crossfadePeriod == 0f) ? 1f : newStateTimer / crossfadePeriod;
            Vector3 aPoint = aState.TransformLocalPoint(quadPoint, charIndex, priorStateTimer);
            Vector3 bPoint = bState.TransformLocalPoint(quadPoint, charIndex, newStateTimer);
            return Vector3.Lerp(aPoint, bPoint, t);
        }
    }

}

}