using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace Phil.FLUI {

public struct GlyphInfo {
    public TMPro.TMP_TextInfo textInfo;
    public TMPro.TMP_CharacterInfo characterInfo;
    public int srcCharInfoIndex;
    public int glyphIndex => characterInfo.vertexIndex / 4;

    public GlyphInfo(ref TMPro.TMP_TextInfo srcTextInfo, ref TMPro.TMP_CharacterInfo srcCharInfo, int srcCharInfoIndex){
        this.textInfo = srcTextInfo;
        this.characterInfo = srcCharInfo;
        this.srcCharInfoIndex = srcCharInfoIndex;
    }
}

}