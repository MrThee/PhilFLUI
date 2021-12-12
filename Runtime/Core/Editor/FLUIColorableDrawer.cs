using UnityEditor;
using UnityEngine;

using Phil.Core;

namespace Phil.FLUI {

[CustomPropertyDrawer(typeof(FLUIColorable))]
public class FLUIColorableDrawer : PolyObjectDrawer<FLUIColorable, FLUIColorable.Type, UnityEngine.UI.Image, SpriteRenderer> {

    public override string aFieldLabel => "Image";
    public override string bFieldLabel => "Sprite Renderer";

    public override FLUIColorable.Type Int2Type(int i){ return (FLUIColorable.Type)i; }

}

}