using UnityEngine;

namespace Phil.FLUI {

public interface IDynamicSpriteStyle : INIStatePeriod {
    FLUISpriteBehaviour GetBehaviour(NonInteractiveState nistate);
    float GetCrossfadePeriod();
    void BlendedApplyAll( NIStateMachine ism, 
        FLUITransformable ft, FLUIColorable fc, FLUISpriteable fs
    );
}

public interface IInteractiveDynamicSpriteStyle : IInteractiveStatePeriod {
    FLUISpriteBehaviour GetBehaviour(InteractiveState istate);
    float GetCrossfadePeriod();
    void BlendedApplyAll( InteractiveStateMachine ism, 
        FLUITransformable ft, FLUIColorable fc, FLUISpriteable fs
    );
}

public interface IDynamicCharStyle : INIStatePeriod {
    FLUICharBehaviour GetBehaviour(NonInteractiveState nistate);
    float GetCrossfadePeriod();
    Vector3 CalcBlendedTransformLocalPoint(NIStateMachine nism, Vector3 quadPoint, GlyphInfo gi);
    Color CalcWordBlendedColor(NIStateMachine nism);
}

public interface IInteractiveDynamicCharStyle : IInteractiveStatePeriod {
    FLUICharBehaviour GetBehaviour(InteractiveState nistate);
    float GetCrossfadePeriod();
    Vector3 CalcBlendedTransformLocalPoint(InteractiveStateMachine ism, Vector3 quadPoint, GlyphInfo gi);
    Color CalcWordBlendedColor(InteractiveStateMachine nism);
}

}
