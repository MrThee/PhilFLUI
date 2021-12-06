using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Phil.Attributes;

namespace Phil.FLUI {

[CreateAssetMenu(menuName = "UI Styles/Dynamic Text/Interactive", order = 1, fileName = "Interactive Dynamic Text Style")]
public class InteractiveDynamicTextStyle : ScriptableObject {

    [InlineCorral] public Inline data = new Inline();

    public DynamicTextStyle.CharBehaviour rollout => data.rollout;
    public DynamicTextStyle.CharBehaviour idle => data.idle;
    public DynamicTextStyle.CharBehaviour recede => data.recede;
    public DynamicTextStyle.CharBehaviour highlighted => data.highlighted;
    public DynamicTextStyle.CharBehaviour confirmed => data.confirmed;
    public float crossfadePeriod => data.crossfadePeriod;

    public DynamicTextStyle.CharBehaviour GetCharBehaviour(InteractiveState state){
        return data.GetCharBehaviour(state);
    }

    public Vector3 CalcBlendedTransformLocalPoint(float priorStateTimer, InteractiveState priorState, float newStateTimer, InteractiveState newState,
            Vector3 quadPoint, int charIndex
    ) {
        return data.CalcBlendedTransformLocalPoint(priorStateTimer, priorState, newStateTimer, newState, quadPoint, charIndex);
    }

    public Color CalcWordBlendedColor(InteractiveStateMachine iTextStateMachine){
        return data.CalcWordBlendedColor(iTextStateMachine);
    }

    [System.Serializable]
    public class Inline {
        public DynamicTextStyle.CharBehaviour rollout = new DynamicTextStyle.CharBehaviour();
        public DynamicTextStyle.CharBehaviour idle = new DynamicTextStyle.CharBehaviour();
        public DynamicTextStyle.CharBehaviour recede = new DynamicTextStyle.CharBehaviour();
        public DynamicTextStyle.CharBehaviour highlighted = new DynamicTextStyle.CharBehaviour();
        public DynamicTextStyle.CharBehaviour confirmed = new DynamicTextStyle.CharBehaviour();
        public float crossfadePeriod = 0.3f;

        public DynamicTextStyle.CharBehaviour GetCharBehaviour(InteractiveState state){
            switch(state){
                case InteractiveState.Rollout: return rollout;
                case InteractiveState.Idle: return idle;
                case InteractiveState.Recede: return recede;
                case InteractiveState.Highlighted: return highlighted;
                case InteractiveState.Confirmed: return confirmed;
                default: return null;
            }
        }

        public Vector3 CalcBlendedTransformLocalPoint(InteractiveStateMachine ism, Vector3 quadPoint, int charIndex){
            if(ism.currentState.HasValue == false){
                return Vector3.zero;
            }
            var curState = ism.currentState.Value;
            var priorState = ism.priorState ?? curState;
            return CalcBlendedTransformLocalPoint(ism.priorStateTimer, priorState, ism.currentStateTimer, curState, quadPoint, charIndex);
        }

        public Vector3 CalcBlendedTransformLocalPoint(float priorStateTimer, InteractiveState priorState, float newStateTimer, InteractiveState newState,
            Vector3 quadPoint, int charIndex
        ) {
            var aState = GetCharBehaviour(priorState);
            var bState = GetCharBehaviour(newState);
            float t = (crossfadePeriod == 0f) ? 1f : newStateTimer / crossfadePeriod;
            Vector3 aPoint = aState.TransformLocalPoint(quadPoint, charIndex, priorStateTimer);
            Vector3 bPoint = bState.TransformLocalPoint(quadPoint, charIndex, newStateTimer);
            return Vector3.Lerp(aPoint, bPoint, t);
        }
        
        public Color CalcWordBlendedColor(InteractiveStateMachine iTextStateMachine){
            var curState = iTextStateMachine.currentState.Value;
            var priorState = iTextStateMachine.priorState ?? curState;
            float t = (crossfadePeriod == 0f) ? 1f : iTextStateMachine.currentStateTimer / crossfadePeriod;
            var aBehave = GetCharBehaviour(priorState);
            var bBehave = GetCharBehaviour(curState);
            Color aColor = aBehave.wordGradient.Evaluate(iTextStateMachine.priorStateTimer/aBehave.wordColorPeriod);
            Color bColor = bBehave.wordGradient.Evaluate(iTextStateMachine.currentStateTimer/bBehave.wordColorPeriod);
            Color c = Color.Lerp(aColor, bColor, t);
            return c;
        }
    }
}

}