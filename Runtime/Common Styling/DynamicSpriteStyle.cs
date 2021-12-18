using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Phil.Attributes;

namespace Phil.FLUI {

[CreateAssetMenu(menuName = "UI Styles/Dynamic Sprite/Non Interactive", order = 0, fileName = "Dynamic Sprite Style")]
public class DynamicSpriteStyle : ScriptableObject, INIStatePeriod, IDynamicSpriteStyle {
    [InlineCorral] public Inline data = new Inline();

    public FLUISpriteBehaviour rollout => data.rollout;
    public FLUISpriteBehaviour idle => data.idle;
    public FLUISpriteBehaviour recede => data.recede;
    public float crossfadePeriod => data.crossfadePeriod;

    public float GetStatePeriod(NonInteractiveState niState){
        return data.GetStatePeriod(niState);
    }

    public float GetCrossfadePeriod(){ return data.crossfadePeriod; }

    public FLUISpriteBehaviour GetBehaviour(NonInteractiveState state){
        return data.GetBehaviour(state);
    }
    
    public void BlendedApplyAll(NIStateMachine nism, RectTransform rectTrans, Image image){
        data.BlendedApplyAll(nism, FLUITransformable.Same(rectTrans), FLUIColorable.Image(image), FLUISpriteable.Image(image));
    }

    public void BlendedApplyAll(NIStateMachine nism, RectTransform rectTrans, SpriteRenderer spr){
        data.BlendedApplyAll(nism, FLUITransformable.Same(rectTrans), FLUIColorable.Sprite(spr), FLUISpriteable.Sprite(spr));
    }

    public void BlendedApplyAll(NIStateMachine nism, FLUITransformable transformThis, FLUIColorable colorThis, FLUISpriteable spriteThis){
        data.BlendedApplyAll(nism, transformThis, colorThis, spriteThis);
    }

    void OnValidate(){
        idle.rectBehaviour.EnsureWraps();
    }

    [System.Serializable]
    public class Inline : INIStatePeriod, IDynamicSpriteStyle {
        [InlineCorral] public FLUISpriteBehaviour rollout = new FLUISpriteBehaviour();
        [InlineCorral] public FLUISpriteBehaviour idle = new FLUISpriteBehaviour();
        [InlineCorral] public FLUISpriteBehaviour recede = new FLUISpriteBehaviour();
        public float crossfadePeriod = 0.3f;

        public float GetStatePeriod(NonInteractiveState niState){
            return GetBehaviour(niState).period;
        }

        public float GetCrossfadePeriod(){ return crossfadePeriod; }

        public FLUISpriteBehaviour GetBehaviour(NonInteractiveState state){
            switch(state){
                case NonInteractiveState.Rollout: return rollout;
                case NonInteractiveState.Idle: return idle;
                case NonInteractiveState.Recede: return recede;
                default: return null;
            }
        }

        public void BlendedApplyAll(NIStateMachine state, FLUITransformable fTrans, FLUIColorable fColorable, FLUISpriteable fSpriteThis){
            if(state.currentState.HasValue==false){
                return;
            }
            var prevState = state.priorState ?? state.currentState.Value;
            var curState = state.currentState.Value;
            BlendedApplyAll<FLUIColorable,FLUISpriteable>(this, state.priorStateTimer, prevState, state.currentStateTimer, curState, 
                fTrans.offsetThis, fTrans.rotateThis, fTrans.scaleThis, 
                fColorable, FLUIColorable.SetColor, fSpriteThis, FLUISpriteable.SetSprite
            );
        }

        public static void BlendedApplyAll<C,S>(IDynamicSpriteStyle style, float priorStateTimer, NonInteractiveState priorState, float newStateTimer, NonInteractiveState newState,
            RectTransform optLoc, Transform optRot, RectTransform optScale, 
            C colorable, System.Action<C,Color> SetColor, S spriteable, System.Action<S, Sprite> SetSprite)
        {
            // Graphics
            var aBehaviour = style.GetBehaviour(priorState);
            var bBehaviour = style.GetBehaviour(newState);
            float crossfadePeriod = style.GetCrossfadePeriod();
            var aPeriod = aBehaviour.period; var bPeriod = bBehaviour.period;
            float a_t = priorStateTimer / aPeriod; float b_t = newStateTimer / bPeriod;
            float t = (crossfadePeriod == 0f) ? 1f : newStateTimer / crossfadePeriod;

            // Color
            var aColorBehav = aBehaviour.colorBehaviour; var bColorBehav = bBehaviour.colorBehaviour;
            SetColor(colorable, Color.Lerp( aColorBehav.Evaluate(a_t), bColorBehav.Evaluate(b_t), t ));

            // Rect
            var aRect = aBehaviour.rectBehaviour; var bRect = bBehaviour.rectBehaviour;
            if(optLoc){
                optLoc.anchoredPosition = Vector2.Lerp( aRect.GetPosition(a_t), bRect.GetPosition(b_t), t );
            }
            if(optRot){
                optRot.localEulerAngles = Vector3.forward * Mathf.Lerp( aRect.zRot.Evaluate(a_t), bRect.zRot.Evaluate(b_t), t );
            }
            if(optScale){
                Vector2 localScale = Vector2.Lerp( aRect.GetScale(a_t), bRect.GetScale(b_t), t );
                switch(aRect.scaleMode){
                case TRS2D.ScaleRectMode.AsScale: {
                    optScale.localScale = localScale;
                } break;
                case TRS2D.ScaleRectMode.AsSize: {
                    optScale.sizeDelta = localScale;
                } break;
                }
            }

            // Sprite (no blend, just use the new state)
            if(bBehaviour.spriteBehaviour.Count > 0){
                var sprite = bBehaviour.GetSprite(newStateTimer);
                SetSprite(spriteable, sprite);
            }
        }
    }

}

}