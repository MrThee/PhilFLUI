using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Phil.Attributes;

namespace Phil.FLUI {

[CreateAssetMenu(menuName = "UI Styles/Dynamic Text/Non Interactive", order = 0, fileName = "Dynamic Text Style")]
public class DynamicTextStyle : ScriptableObject, INIStatePeriod {

    [InlineCorral] public Inline data = new Inline();

    public CharBehaviour rollout => data.rollout;
    public CharBehaviour idle => data.idle;
    public CharBehaviour recede => data.recede;
    public float crossfadePeriod => data.crossfadePeriod;

    public float GetStatePeriod(NonInteractiveState _niState){
        return data.GetStatePeriod(_niState);
    }
    
    public CharBehaviour GetCharBehaviour(NonInteractiveState state){
        return data.GetCharBehaviour(state);
    }

    public Color CalcWordBlendedColor(NIStateMachine dTextState){
        return data.CalcWordBlendedColor(dTextState);
    }

    public Vector3 CalcBlendedTransformLocalPoint(NIStateMachine dTextState, Vector3 quadPoint, int charIndex){
        return data.CalcBlendedTransformLocalPoint(dTextState, quadPoint, charIndex);
    }

    public Vector3 CalcBlendedTransformLocalPoint(float priorStateTimer, NonInteractiveState priorState, float newStateTimer, NonInteractiveState newState,
        Vector3 quadPoint, int charIndex
    ) {
        return data.CalcBlendedTransformLocalPoint(priorStateTimer, priorState, newStateTimer, newState, quadPoint, charIndex);
    }

    // ================ Data Types ================ //

    [System.Serializable]
    public class CharBehaviour {

        public float charStagger = 0.05f;
        public float charPeriod = 0.25f;
        [InlineCorral] public TRS2D TRS = new TRS2D();
        
        public Vector3 TransformLocalPoint(Vector3 quadPoint, int charIndex, float timer){
            float charTimer = timer - charIndex * charStagger;
            float t = charTimer / charPeriod;
            return TRS.TransformLocalPoint(quadPoint, t);
        }

        public Gradient wordGradient = new Gradient();
        public float wordColorPeriod = 1f;
    }

    [System.Serializable]
    public class Inline : INIStatePeriod {
        [InlineCorral] public CharBehaviour rollout = new CharBehaviour();
        [InlineCorral] public CharBehaviour idle = new CharBehaviour();
        [InlineCorral] public CharBehaviour recede = new CharBehaviour();
        public float crossfadePeriod = 0.2f;

        public float GetStatePeriod(NonInteractiveState _niState){
            return crossfadePeriod;
        }
        
        public CharBehaviour GetCharBehaviour(NonInteractiveState state){
            switch(state){
                case NonInteractiveState.Rollout: return rollout;
                case NonInteractiveState.Idle: return idle;
                case NonInteractiveState.Recede: return recede;
                default: return null;
            }
        }

        public Color CalcWordBlendedColor(NIStateMachine dTextState){
            if(dTextState.currentState.HasValue == false){
                return Color.white;
            }
            var priorState = dTextState.priorState ?? dTextState.currentState.Value;
            var curState = dTextState.currentState.Value;
            float t = (crossfadePeriod == 0f) ? 1f : dTextState.currentStateTimer / crossfadePeriod;
            var aBehave = GetCharBehaviour(curState);
            var bBehave = GetCharBehaviour(priorState);
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
            return CalcBlendedTransformLocalPoint(
                dTextState.priorStateTimer, oldState, dTextState.currentStateTimer, dTextState.currentState.Value,
                quadPoint, charIndex
            );
        }

        public Vector3 CalcBlendedTransformLocalPoint(float priorStateTimer, NonInteractiveState priorState, float newStateTimer, NonInteractiveState newState,
            Vector3 quadPoint, int charIndex
        ) {
            var aState = GetCharBehaviour(priorState);
            var bState = GetCharBehaviour(newState);
            float t = (crossfadePeriod == 0f) ? 1f : newStateTimer / crossfadePeriod;
            Vector3 aPoint = aState.TransformLocalPoint(quadPoint, charIndex, priorStateTimer);
            Vector3 bPoint = bState.TransformLocalPoint(quadPoint, charIndex, newStateTimer);
            return Vector3.Lerp(aPoint, bPoint, t);
        }
    }

}

}