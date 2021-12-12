using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Phil;

namespace Phil.FLUI {

[System.Serializable]
public struct FLUIColorable {
    public enum Type {
        UIImage,
        SpriteRenderer,
    }
    public Type type;
    public UnityEngine.UI.Image image;
    public SpriteRenderer spriteRenderer;

    public static FLUIColorable Image(UnityEngine.UI.Image img){
        FLUIColorable fc;
        fc.type = Type.UIImage;
        fc.image = img;
        fc.spriteRenderer = null;
        return fc;
    }

    public static FLUIColorable Sprite(SpriteRenderer sr){
        FLUIColorable fc;
        fc.type = Type.SpriteRenderer;
        fc.image = null;
        fc.spriteRenderer = sr;
        return fc;
    }

    public static readonly System.Action<FLUIColorable, Color> SetColor = (fc, c) => {
        switch(fc.type){
        case Type.UIImage: if(fc.image) fc.image.color = c; break;
        case Type.SpriteRenderer: if(fc.spriteRenderer) fc.spriteRenderer.color = c; break;
        }
    };
}

}