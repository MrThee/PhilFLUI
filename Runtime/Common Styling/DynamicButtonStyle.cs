using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Phil.Attributes;

namespace Phil.FLUI {

[CreateAssetMenu(menuName = "UI Styles/Dynamic Button", order = 2, fileName = "Dynamic Button Style")]
public class DynamicButtonStyle : ScriptableObject, IDynamicButtonStyle {

    [InlineCorral] public Inline data = new Inline();

    public InteractiveDynamicSpriteStyle.Inline iBackgroundBehaviour => data.iBackgroundBehaviour;
    public InteractiveDynamicSpriteStyle.Inline iForegroundBehaviour => data.iForegroundBehaviour;
    public InteractiveDynamicTextStyle.Inline iTextBehaviour => data.iTextBehaviour;

    void OnValidate(){
        data.backgroundBehaviour.idle.rectBehaviour.EnsureWraps();
        data.foregroundBehaviour.idle.rectBehaviour.EnsureWraps();
        data.textBehaviour.idle.TRS.EnsureWraps();
    }

    [System.Serializable]
    public class Inline : IDynamicButtonStyle {
        [InlineCorral] public InteractiveDynamicSpriteStyle.Inline backgroundBehaviour = new InteractiveDynamicSpriteStyle.Inline();
        [InlineCorral] public InteractiveDynamicSpriteStyle.Inline foregroundBehaviour = new InteractiveDynamicSpriteStyle.Inline();
        [InlineCorral] public InteractiveDynamicTextStyle.Inline textBehaviour = new InteractiveDynamicTextStyle.Inline();

        public InteractiveDynamicSpriteStyle.Inline iBackgroundBehaviour => backgroundBehaviour;
        public InteractiveDynamicSpriteStyle.Inline iForegroundBehaviour => foregroundBehaviour;
        public InteractiveDynamicTextStyle.Inline iTextBehaviour => textBehaviour;
    }

}

public interface IDynamicButtonStyle {
    InteractiveDynamicSpriteStyle.Inline iBackgroundBehaviour { get; }
    InteractiveDynamicSpriteStyle.Inline iForegroundBehaviour { get; }
    InteractiveDynamicTextStyle.Inline iTextBehaviour { get; }
}

}