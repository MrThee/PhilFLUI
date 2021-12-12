using UnityEngine;
using System.Collections.Generic;
using Phil.Core;
using Phil.Attributes;

namespace Phil.FLUI {

[System.Serializable]
public class FLUICharBehaviour {

    public float charStagger = 0.05f;
    public float charPeriod = 0.25f;
    [InlineCorral] public TRS2D TRS = new TRS2D();
    
    public Vector3 TransformLocalPoint(Vector3 quadPoint, int charIndex, float timer){
        float charTimer = timer - charIndex * charStagger;
        float t = charTimer / charPeriod;
        return TRS.TransformLocalPoint(quadPoint, t);
    }

    public Gradient wordGradient = new Gradient();
    public float wordColorPeriod = 1f;
}

}
