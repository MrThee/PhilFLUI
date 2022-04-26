using UnityEditor;
using UnityEngine;

using Phil.Core;

#if UNITY_EDITOR

namespace Phil.FLUI {

[CustomPropertyDrawer(typeof(FLUISpriteable))]
public class FLUISpriteableDrawer : PolyObjectDrawer<FLUISpriteable, FLUISpriteable.Type, UnityEngine.UI.Image, SpriteRenderer> {

    public override string aFieldLabel => "Image";
    public override string bFieldLabel => "Sprite Renderer";
    public override FLUISpriteable.Type Int2Type(int i){ return (FLUISpriteable.Type)i; }

}

}

#endif