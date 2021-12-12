using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Phil;
using Phil.Core;

namespace Phil.FLUI {

[System.Serializable]
public class FLUISpriteable : PolyObject<FLUISpriteable.Type, UnityEngine.UI.Image, SpriteRenderer> {
    public enum Type {
        UIImage,
        SpriteRenderer,
    }
    public Type type;
    public UnityEngine.UI.Image image { get => componentA; set => componentA = value; }
    public SpriteRenderer spriteRenderer { get => componentB; set => componentB = value; }

    public static FLUISpriteable Image(UnityEngine.UI.Image img){
        var fc = new FLUISpriteable();
        fc.type = Type.UIImage;
        fc.image = img;
        fc.spriteRenderer = null;
        return fc;
    }

    public static FLUISpriteable Sprite(SpriteRenderer sr){
        var fc = new FLUISpriteable();
        fc.type = Type.SpriteRenderer;
        fc.image = null;
        fc.spriteRenderer = sr;
        return fc;
    }

    public static readonly System.Action<FLUISpriteable, Sprite> SetSprite = (fc, s) => {
        switch(fc.type){
        case Type.UIImage: fc.image.sprite = s; break;
        case Type.SpriteRenderer: fc.spriteRenderer.sprite = s; break;
        }
    };
}

}