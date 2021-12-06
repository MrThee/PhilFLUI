using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Phil.Attributes;

namespace Phil.FLUI {

[CreateAssetMenu(menuName = "UI Styles/Dynamic Sprite/Non Interactive", order = 0, fileName = "Dynamic Sprite Style")]
public class DynamicSpriteStyle : ScriptableObject, INIStatePeriod {
    [InlineCorral] public Inline data = new Inline();

    public BehaviourSet rollout => rollout;
    public BehaviourSet idle => idle;
    public BehaviourSet recede => recede;
    public float crossfadePeriod => crossfadePeriod;

    public float GetStatePeriod(NonInteractiveState niState){
        return data.GetStatePeriod(niState);
    }

    public BehaviourSet GetBehaviourState(NonInteractiveState state){
        return GetBehaviourState(state);
    }

    public void ApplyAll(float timer, NonInteractiveState state,
        RectTransform rectTransform, Image graphic)
    {
        data.ApplyAll(timer, state, rectTransform, graphic);
    }

    public void BlendedApplyAll(NIStateMachine state, RectTransform rectTrans, Image graphic){
        data.BlendedApplyAll(state, rectTrans, graphic);
    }
        
    public void BlendedApplyAll(float priorStateTimer, NonInteractiveState priorState, float newStateTimer, NonInteractiveState newState,
    RectTransform rectTrans, Image graphic)
    {
        data.BlendedApplyAll(priorStateTimer, priorState, newStateTimer, newState, rectTrans, graphic);
    }

    // ================ Data Types ================ //

    [System.Serializable]
    public class BehaviourSet {
        public float period = 1f;
        public Gradient colorBehaviour = new Gradient();
        public TRS2D rectBehaviour = new TRS2D();
        public List<Sprite> spriteBehaviour = new List<Sprite>();

        public Sprite GetSprite(float timer){
            float t = timer / period;
            int frames = spriteBehaviour.Count;
            int frameIndex = Phil.Math.FloorToIndex(t, frames);
            return spriteBehaviour[frameIndex];
        }

        public void ApplyAll(float timer, RectTransform rectTrans, Image graphic){
            graphic.color = colorBehaviour.Evaluate(timer);
            rectBehaviour.TransformRect(timer/period, rectTrans);
            if(spriteBehaviour.Count > 0) {
                graphic.sprite = GetSprite(timer);
            }
        }
    }

    [System.Serializable]
    public class Inline : INIStatePeriod {
        [InlineCorral] public BehaviourSet rollout = new BehaviourSet();
        [InlineCorral] public BehaviourSet idle = new BehaviourSet();
        [InlineCorral] public BehaviourSet recede = new BehaviourSet();
        public float crossfadePeriod = 0.3f;

        public float GetStatePeriod(NonInteractiveState niState){
            return GetBehaviour(niState).period;
        }

        public BehaviourSet GetBehaviour(NonInteractiveState state){
            switch(state){
                case NonInteractiveState.Rollout: return rollout;
                case NonInteractiveState.Idle: return idle;
                case NonInteractiveState.Recede: return recede;
                default: return null;
            }
        }

        public void ApplyAll(float timer, NonInteractiveState state,
            RectTransform rectTransform, Image graphic)
        {
            var behave = GetBehaviour(state);
            behave.ApplyAll(timer, rectTransform, graphic);
        }

        public void BlendedApplyAll(NIStateMachine state, RectTransform rectTrans, Image graphic){
            var prevState = state.priorState ?? state.currentState.Value;
            var curState = state.currentState.Value;
            BlendedApplyAll(state.priorStateTimer, prevState, state.currentStateTimer, curState, rectTrans, graphic);
        }
        
        public void BlendedApplyAll(float priorStateTimer, NonInteractiveState priorState, float newStateTimer, NonInteractiveState newState,
        RectTransform rectTrans, Image graphic)
        {
            // Graphics
            var aBehaviour = GetBehaviour(priorState);
            var bBehaviour = GetBehaviour(newState);
            var aPeriod = aBehaviour.period; var bPeriod = bBehaviour.period;
            float a_t = priorStateTimer / aPeriod; float b_t = newStateTimer / bPeriod;
            float t = (crossfadePeriod == 0f) ? 1f : newStateTimer / crossfadePeriod;

            // Color
            var aColorBehav = aBehaviour.colorBehaviour; var bColorBehav = bBehaviour.colorBehaviour;
            graphic.color = Color.Lerp( aColorBehav.Evaluate(a_t), bColorBehav.Evaluate(b_t), t );

            // Rect
            var aRect = aBehaviour.rectBehaviour; var bRect = bBehaviour.rectBehaviour;
            rectTrans.anchoredPosition = Vector2.Lerp( aRect.GetPosition(a_t), bRect.GetPosition(b_t), t );
            rectTrans.localEulerAngles = Vector3.forward * Mathf.Lerp( aRect.zRot.Evaluate(a_t), bRect.zRot.Evaluate(b_t), t );
            Vector2 localScale = Vector2.Lerp( aRect.GetScale(a_t), bRect.GetScale(b_t), t );
            switch(aRect.scaleMode){
            case TRS2D.ScaleRectMode.AsScale: {
                rectTrans.localScale = localScale;
            } break;
            case TRS2D.ScaleRectMode.AsSize: {
                rectTrans.sizeDelta = localScale;
            } break;
            }

            // Sprite (no blend, just use the new state)
            if(bBehaviour.spriteBehaviour.Count > 0){
                graphic.sprite = bBehaviour.GetSprite(newStateTimer);
            }
        }
    }

}

}