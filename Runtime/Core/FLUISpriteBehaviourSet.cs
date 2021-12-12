using UnityEngine;
using System.Collections.Generic;
using Phil.Core;

namespace Phil.FLUI {

[System.Serializable]
public class FLUISpriteBehaviour {
    public float period = 1f;
    public Gradient colorBehaviour = new Gradient();
    public TRS2D rectBehaviour = new TRS2D();
    public List<Sprite> spriteBehaviour = new List<Sprite>();

    public Sprite GetSprite(float timer){
        float t = timer / period;
        t = Mathf.Repeat(t, 1f);
        int frames = spriteBehaviour.Count;
        int frameIndex = Phil.Math.FloorToIndex(t, frames);
        return spriteBehaviour[frameIndex];
    }

    public void ApplyAll(float timer, RectTransform rectTrans, UnityEngine.UI.Image graphic){
        graphic.color = colorBehaviour.Evaluate(timer);
        rectBehaviour.TransformRect(timer/period, rectTrans);
        if(spriteBehaviour.Count > 0) {
            graphic.sprite = GetSprite(timer);
        }
    }
}

}