using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Phil;

namespace Phil.FLUI {

[System.Serializable]
public class FLUIColorable : Phil.Core.PolyObject<FLUIColorable.Type, UnityEngine.UI.Image, SpriteRenderer> {
    public enum Type {
        UIImage,
        SpriteRenderer,
    }

    public UnityEngine.UI.Image image {
        get => componentA;
        set => componentA = value;
    }

    public SpriteRenderer spriteRenderer {
        get => componentB;
        set => componentB = value;
    }

    public static FLUIColorable Image(UnityEngine.UI.Image img){
        var fc = new FLUIColorable();
        fc.type = Type.UIImage;
        fc.image = img;
        fc.spriteRenderer = null;
        return fc;
    }

    public static FLUIColorable Sprite(SpriteRenderer sr){
        var fc = new FLUIColorable();
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