using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Phil;

namespace Phil.FLUI {

[System.Serializable]
public struct FLUISpriteable {
    public enum Type {
        UIImage,
        SpriteRenderer,
    }
    public Type type;
    public UnityEngine.UI.Image image;
    public SpriteRenderer spriteRenderer;

    public static FLUISpriteable Image(UnityEngine.UI.Image img){
        FLUISpriteable fc;
        fc.type = Type.UIImage;
        fc.image = img;
        fc.spriteRenderer = null;
        return fc;
    }

    public static FLUISpriteable Sprite(SpriteRenderer sr){
        FLUISpriteable fc;
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