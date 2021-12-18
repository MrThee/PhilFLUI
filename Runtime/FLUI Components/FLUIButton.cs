using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Phil;
using Phil.Core;

namespace Phil.FLUI {

    public class FLUIButton : MonoBehaviour {

        [Header("Components")]
        public ISMText text;
        public ISMSprite background;
        public ISMSprite foreground;
        public FLUIEventContainer eventContainer = new FLUIEventContainer();

        [Header("Config")]
        public DynamicButtonStyle defaultStyle;
        public IDynamicButtonStyle parentOverrideStyle { 
            get => m_overrideStorage;
            set {
                m_overrideStorage = value;
                // update children overrides!
                if(text){
                    text.overrideStyle = value?.iTextBehaviour ?? defaultStyle?.iTextBehaviour ?? null;
                }
                if(background){
                    background.overrideStyle = value?.iBackgroundBehaviour ?? defaultStyle?.iBackgroundBehaviour ?? null;
                }
                if(foreground){
                    foreground.overrideStyle = value?.iForegroundBehaviour ?? defaultStyle?.iForegroundBehaviour ?? null;
                }
            }
        }

        private IDynamicButtonStyle m_overrideStorage;

        public void Init(InteractiveState? optInitialState, InteractiveState? optPostConfirmState, int stringBufferCapacity){
            this.parentOverrideStyle = null; // actually an initial hydration
            // Where the interfaces REALLY come in handy!
            text?.Init(optInitialState, optPostConfirmState, stringBufferCapacity);
            background?.Init(optInitialState, optPostConfirmState);
            foreground?.Init(optInitialState, optPostConfirmState);
            // need to set this at least once for the components to be overrwritten if the default style is assigned.
        }

        public void ChangeState(InteractiveState? istate){
            text?.ChangeState(istate);
            background?.ChangeState(istate);
            foreground?.ChangeState(istate);
        }

    }

}