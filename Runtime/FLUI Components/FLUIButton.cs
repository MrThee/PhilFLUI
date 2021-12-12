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

        public void Init(InteractiveState? optInitialState, int stringBufferCapacity){
            // Where the interfaces REALLY come in handy!
            text?.Init(optInitialState, stringBufferCapacity);
            background?.Init(optInitialState);
            foreground?.Init(optInitialState);
            // need to set this at least once for the components to be overrwritten if the default style is assigned.
            this.parentOverrideStyle = null;
        }

        public void ChangeState(InteractiveState? istate){
            text?.ChangeState(istate);
            background?.ChangeState(istate);
            foreground?.ChangeState(istate);
        }

    }

}