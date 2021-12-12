using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Phil.Attributes;

namespace Phil.FLUI {

[CreateAssetMenu(menuName = "UI Styles/Dynamic Sprite/Interactive", order = 1, fileName = "Interactive Dynamic Sprite Style")]
public class InteractiveDynamicSpriteStyle : ScriptableObject, IInteractiveStatePeriod {
    [InlineCorral] public Inline data = new Inline();
    public DynamicSpriteStyle.BehaviourSet rollout => data.rollout;
    public DynamicSpriteStyle.BehaviourSet idle => data.idle;
    public DynamicSpriteStyle.BehaviourSet recede => data.recede;
    public DynamicSpriteStyle.BehaviourSet highlighted => data.highlighted;
    public DynamicSpriteStyle.BehaviourSet confirmed => data.confirmed;
    public float stateBlendPeriod => data.stateBlendPeriod;

    public float GetStatePeriod(InteractiveState state){
        return data.GetStatePeriod(state);
    }

    public DynamicSpriteStyle.BehaviourSet GetBehaviour(InteractiveState state){
        return data.GetBehaviour(state);
    }

    public void BlendedApplyAll(InteractiveStateMachine ism, RectTransform rectTrans, SpriteRenderer spriteRenderer){
        data.BlendedApplyAll(ism, FLUITransformable.Same(rectTrans), FLUIColorable.Sprite(spriteRenderer), FLUISpriteable.Sprite(spriteRenderer) );
    }

    public void BlendedApplyAll(InteractiveStateMachine ism, RectTransform rectTrans, Image graphic){
        data.BlendedApplyAll(ism, FLUITransformable.Same(rectTrans), FLUIColorable.Image(graphic), FLUISpriteable.Image(graphic) );
    }

    public void BlendedApplyAll(InteractiveStateMachine ism,
        FLUITransformable ft, FLUIColorable fc, FLUISpriteable fs)
    {
        data.BlendedApplyAll(ism, ft, fc, fs);
    }

    [System.Serializable]
    public class Inline : IInteractiveStatePeriod {
        [InlineCorral] public DynamicSpriteStyle.BehaviourSet rollout = new DynamicSpriteStyle.BehaviourSet();
        [InlineCorral] public DynamicSpriteStyle.BehaviourSet idle = new DynamicSpriteStyle.BehaviourSet();
        [InlineCorral] public DynamicSpriteStyle.BehaviourSet recede = new DynamicSpriteStyle.BehaviourSet();
        [InlineCorral] public DynamicSpriteStyle.BehaviourSet highlighted = new DynamicSpriteStyle.BehaviourSet();
        [InlineCorral] public DynamicSpriteStyle.BehaviourSet confirmed = new DynamicSpriteStyle.BehaviourSet();
        public float stateBlendPeriod = 0.3f;

        public float GetStatePeriod(InteractiveState state){
            return GetBehaviour(state).period;
        }

        public DynamicSpriteStyle.BehaviourSet GetBehaviour(InteractiveState state){
            switch(state){
                case InteractiveState.Rollout: return rollout;
                case InteractiveState.Idle: return idle;
                case InteractiveState.Recede: return recede;
                case InteractiveState.Highlighted: return highlighted;
                case InteractiveState.Confirmed: return confirmed;
                default: return null;
            }
        }

        public void GetBlendedBehaviours(ref InteractiveStateMachine ism, 
            out DynamicSpriteStyle.BehaviourSet behaviourA, out DynamicSpriteStyle.BehaviourSet behaviourB, 
            out float a_t, out float b_t, out float t)
        {
            behaviourA = GetBehaviour(ism.priorState.Value);
            behaviourB = GetBehaviour(ism.currentState.Value);
            var aPeriod = behaviourA.period; var bPeriod = behaviourB.period;
            a_t = ism.priorStateTimer / aPeriod; b_t = ism.currentStateTimer / bPeriod;
            t = (stateBlendPeriod == 0f) ? 1f : ism.currentStateTimer / stateBlendPeriod;
        }

        public void BlendedApplyAll(InteractiveStateMachine ism,
            FLUITransformable ft, FLUIColorable fc, FLUISpriteable fs)
        {
            if(ism.currentState.TryGetValue(out var curIState) == false){
                return;
            }
            var prevIState = ism.priorState ?? curIState;
            BlendedApplyAll(ism.priorStateTimer, prevIState, ism.currentStateTimer, curIState, 
                ft.offsetThis, ft.rotateThis, ft.scaleThis, 
                fc, FLUIColorable.SetColor, fs, FLUISpriteable.SetSprite
            );
        }

        public void BlendedApplyAll<C,S>(float priorStateTimer, InteractiveState priorState, float newStateTimer, InteractiveState newState,
            RectTransform optOffsetable, Transform optRotatable, RectTransform optScalable,
            C graphic, System.Action<C,Color> SetColor, S spriteable, System.Action<S, Sprite> SetSprite)
        {
            // Graphics
            var aBehaviour = GetBehaviour(priorState);
            var bBehaviour = GetBehaviour(newState);
            var aPeriod = aBehaviour.period; var bPeriod = bBehaviour.period;
            float a_t = priorStateTimer / aPeriod; float b_t = newStateTimer / bPeriod;
            float t = (stateBlendPeriod == 0f) ? 1f : newStateTimer / stateBlendPeriod;

            // Color
            var aColorBehav = aBehaviour.colorBehaviour; var bColorBehav = bBehaviour.colorBehaviour;
            SetColor( graphic, Color.Lerp(aColorBehav.Evaluate(a_t), bColorBehav.Evaluate(b_t), t) );

            // Rect
            var aRect = aBehaviour.rectBehaviour; var bRect = bBehaviour.rectBehaviour;
            if(optOffsetable){
                optOffsetable.anchoredPosition = Vector2.Lerp( aRect.GetPosition(a_t), bRect.GetPosition(b_t), t );
            }
            if(optRotatable){
                optRotatable.localEulerAngles = Vector3.forward * Mathf.Lerp( aRect.zRot.Evaluate(a_t), bRect.zRot.Evaluate(b_t), t );
            }
            if(optScalable){
                Vector2 localScale = Vector2.Lerp( aRect.GetScale(a_t), bRect.GetScale(b_t), t );
                switch(aRect.scaleMode){
                case TRS2D.ScaleRectMode.AsScale: {
                    optScalable.localScale = localScale;
                } break;
                case TRS2D.ScaleRectMode.AsSize: {
                    optScalable.sizeDelta = localScale;
                } break;
                }
            }

            // Sprite (no blend, just use the new state)
            if(bBehaviour.spriteBehaviour.Count > 0 ){
                SetSprite( spriteable, bBehaviour.GetSprite(newStateTimer) );
            }
        }

        public void BlendedValueRetrieve(InteractiveStateMachine ism,
            out Vector3 position, out Vector3 angles, out Vector3 scale, out Color color, out Sprite sprite )
        { 
            if(ism.currentState.TryGetValue(out var curIState) == false){
                position = Vector3.zero;
                angles = Vector3.zero;
                scale = Vector3.one;
                color = Color.white;
                sprite = null;
                return;
            }
            var prevIState = ism.priorState ?? curIState;
            BlendedValueRetrieve(ism.priorStateTimer, prevIState, ism.currentStateTimer, curIState, 
                out position, out angles, out scale, out color, out sprite
            );
        }

        public void BlendedValueRetrieve(float priorStateTimer, InteractiveState priorState, float newStateTimer, InteractiveState newState, 
            out Vector3 position, out Vector3 angles, out Vector3 scale, out Color color, out Sprite sprite )
        {
            // blend parameters
            var aBehaviour = GetBehaviour(priorState);
            var bBehaviour = GetBehaviour(newState);
            var aPeriod = aBehaviour.period; var bPeriod = bBehaviour.period;
            float a_t = priorStateTimer / aPeriod; float b_t = newStateTimer / bPeriod;
            float t = (stateBlendPeriod == 0f) ? 1f : newStateTimer / stateBlendPeriod;

            var aTrans = aBehaviour.rectBehaviour; var bTrans = bBehaviour.rectBehaviour;
            
            // Position
            position = Vector2.Lerp( aTrans.GetPosition(a_t), bTrans.GetPosition(b_t), t );

            // Rotation
            angles = Vector3.forward * Mathf.Lerp( aTrans.zRot.Evaluate(a_t), bTrans.zRot.Evaluate(b_t), t );
            
            // Scale
            scale = Vector2.Lerp( aTrans.GetScale(a_t), bTrans.GetScale(b_t), t );

            // Color
            var aColorBehav = aBehaviour.colorBehaviour; var bColorBehav = bBehaviour.colorBehaviour;
            color = Color.Lerp( aColorBehav.Evaluate(a_t), bColorBehav.Evaluate(b_t), t );

            // Sprite 
            sprite = bBehaviour.spriteBehaviour.Count > 0 ? bBehaviour.GetSprite(newStateTimer) : null;
        }
    }

}

}