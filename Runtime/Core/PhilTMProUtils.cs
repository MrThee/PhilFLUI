using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Phil {

public static class TMProUtils {

    static System.Func< TMPro.TextMeshProUGUI, int, Vector3[] > S_GetVertArr;
    static System.Func< TMPro.TextMeshProUGUI, int, Color32[] > S_GetC32Arr;
    static System.Action< TMPro.TextMeshProUGUI > S_UpdateVerts;
    static System.Action< TMPro.TextMeshProUGUI > S_UpdateColors;

    static TMProUtils(){
        S_GetVertArr = (tf, mi) => tf.textInfo.meshInfo[mi].vertices;
        S_GetC32Arr = (tf, mi) => tf.textInfo.meshInfo[mi].colors32;
        S_UpdateVerts = tf => tf.UpdateVertexData(TMPro.TMP_VertexDataUpdateFlags.Vertices);
        S_UpdateColors = tf => tf.UpdateVertexData(TMPro.TMP_VertexDataUpdateFlags.Colors32);
    }

    public static void GetPositions(TMPro.TextMeshProUGUI textField, List<Vector3> dstBuf){
        GetBaseAttributes(textField, S_GetVertArr, dstBuf);
    }

    public static void SetPositions(TMPro.TextMeshProUGUI textField, IReadOnlyList<Vector3> srcBuf){
        SetAttributes(textField, srcBuf, S_GetVertArr, S_UpdateVerts);
    }

    public static void GetColors(TMPro.TextMeshProUGUI textField, List<Color> dstBuf){
        GetBaseAttributes<Color,Color32>(textField, S_GetC32Arr,
            dstBuf, c32 => { Color c= c32; return c; }
        );
    }

    public static void SetColors(TMPro.TextMeshProUGUI textField, IReadOnlyList<Color> srcBuf){
        SetAttributes<Color, Color32>(textField, srcBuf, S_GetC32Arr,
            color => new Color32(
                (byte)(Mathf.RoundToInt(color.r*255)),
                (byte)Mathf.RoundToInt(color.g*255),
                (byte)Mathf.RoundToInt(color.b*255),
                (byte)Mathf.RoundToInt(color.a*255)
            ), S_UpdateColors
        );
    }

    public static void GetBoundsMatchingParameters(TMPro.TextMeshProUGUI source, out Vector2 anchoredPosition, out Vector2 size){
        var bounds = source.bounds;
        anchoredPosition = source.rectTransform.anchoredPosition + new Vector2(bounds.center.x, bounds.center.y);
        size = new Vector2(bounds.size.x, bounds.size.y);
    }

    public static void GetTextBoundsMatchingParameters(TMPro.TextMeshProUGUI source, out Vector2 anchoredPosition, out Vector2 sizeDelta){
        var bounds = source.textBounds;
        anchoredPosition = source.rectTransform.anchoredPosition + new Vector2(bounds.center.x, bounds.center.y);
        sizeDelta = new Vector2(bounds.size.x, bounds.size.y);
    }

    public static void GetBaseAttributes<T>(
        TMPro.TextMeshProUGUI textField,
        System.Func<TMPro.TextMeshProUGUI, int, T[]> GetSrcBuf,
        List<T> dstBuf
    ) {
        TMPro.TMP_TextInfo textInfo = textField.textInfo;
        TMPro.TMP_CharacterInfo[] charInfomation = textInfo.characterInfo;
        int charCount = textInfo.characterCount; // charInfomation.Length;
        dstBuf.Clear();

        for(int c = 0; c < charCount; c++){
            TMPro.TMP_CharacterInfo charInfo = charInfomation[c];
            if(charInfo.isVisible == false){
                continue;
            }
            int matIndex = charInfo.materialReferenceIndex;
            T[] src = GetSrcBuf(textField, matIndex);
            int startIndex = charInfo.vertexIndex;

            dstBuf.Add(src[startIndex+0]);
            dstBuf.Add(src[startIndex+1]);
            dstBuf.Add(src[startIndex+2]);
            dstBuf.Add(src[startIndex+3]);
        }
    }

    public static void GetBaseAttributes<T,N>(
        TMPro.TextMeshProUGUI textField,
        System.Func<TMPro.TextMeshProUGUI, int, N[]> GetSrcBuf,
        List<T> dstBuf,
        System.Func<N,T> Caster
    ) {
        TMPro.TMP_TextInfo textInfo = textField.textInfo;
        TMPro.TMP_CharacterInfo[] charInformation = textInfo.characterInfo;
        int charCount = charInformation.Length;
        dstBuf.Clear();

        for(int c = 0; c < charCount; c++){
            TMPro.TMP_CharacterInfo charInfo = textInfo.characterInfo[c];
            if(charInfo.isVisible == false){
                continue;
            }
            int matIndex = charInfo.materialReferenceIndex;
            N[] src = GetSrcBuf(textField, matIndex);
            int startIndex = charInfo.vertexIndex;

            dstBuf.Add( Caster(src[startIndex+0]) );
            dstBuf.Add( Caster(src[startIndex+1]) );
            dstBuf.Add( Caster(src[startIndex+2]) );
            dstBuf.Add( Caster(src[startIndex+3]) );
        }
    }

    public static void SetAttributes<T>( TMPro.TextMeshProUGUI textField,
        IReadOnlyList<T> srcBuf,
        System.Func<TMPro.TextMeshProUGUI, int, T[]> GetArr,
        System.Action<TMPro.TextMeshProUGUI> UpdateBufferAction
    ) {
        TMPro.TMP_TextInfo textInfo = textField.textInfo;
        TMPro.TMP_CharacterInfo[] charInformation = textInfo.characterInfo;
        int charCount = charInformation.Length;
        int v = 0;
        // Debug.Log("Before: " + GetArr(textField, 0)[0]);
        for(int c = 0; c < charCount; c++){
            TMPro.TMP_CharacterInfo charInfo = charInformation[c];
            if(charInfo.isVisible == false){
                continue;
            }
            int matIndex = charInfo.materialReferenceIndex;
            int startIndex = charInfo.vertexIndex;

            T[] arr = GetArr(textField, matIndex);
            if(startIndex + 3 < arr.Length && v+3 < srcBuf.Count){
                // I SOMETIMES get errors where these are invalid indices if the conditional above
                // is turned off... i wonder what causes that.
                arr[startIndex+0] = srcBuf[v++];
                arr[startIndex+1] = srcBuf[v++];
                arr[startIndex+2] = srcBuf[v++];
                arr[startIndex+3] = srcBuf[v++];
            }
        }
        // Debug.Log("After: " + GetArr(textField, 0)[0]);
        UpdateBufferAction(textField);
        // Debug.Log("AfterAfter: " + GetArr(textField, 0)[0]);
    }

    public static void SetAttributes<T,N>(
        TMPro.TextMeshProUGUI textField,
        IReadOnlyList<T> srcBuf,
        System.Func<TMPro.TextMeshProUGUI, int, N[]> GetNatArr,
        System.Func<T,N> CastToNative,
        System.Action<TMPro.TextMeshProUGUI> UpdateBufferAction
    ) {
        TMPro.TMP_TextInfo textInfo = textField.textInfo;
        TMPro.TMP_CharacterInfo[] charInformation = textInfo.characterInfo;
        int charCount = charInformation.Length;
        int v = 0;
        // Debug.Log("Before: " + GetArr(textField, 0)[0]);
        for(int c = 0; c < charCount; c++){
            TMPro.TMP_CharacterInfo charInfo = charInformation[c];
            if(charInfo.isVisible == false){
                continue;
            }
            int matIndex = charInfo.materialReferenceIndex;
            int startIndex = charInfo.vertexIndex;

            N[] nArr = GetNatArr(textField, matIndex);
            nArr[startIndex+0] = CastToNative(srcBuf[v++]);
            nArr[startIndex+1] = CastToNative(srcBuf[v++]);
            nArr[startIndex+2] = CastToNative(srcBuf[v++]);
            nArr[startIndex+3] = CastToNative(srcBuf[v++]);
        }
        // Debug.Log("After: " + GetArr(textField, 0)[0]);
        UpdateBufferAction(textField);
    }

}

// Example:

// (Old) General PseudoCode:
// 0. In Late Update:
// 1. Force the TextRend component to update
// 2. See if text has been rendered yet
// 3. If Yes, cache the base vertices of the rendered component
// 4. If the base vertices have been cached, use them as reference 
// to calc a new transformed position.
// 5. Update the TMPro char vertex positions

// Alternativey, a more-clear design would involve only Forcing a Mesh Update
// when the base references vertices haven't been cache
// OR when their data has been transformed.

	// void LateUpdate(){
	// 	kTextRend.ForceMeshUpdate();
	// 	if(transCharStagger > 0f){
	// 		_PerCharPositioning();
	// 	}
	// 	if(colorCharStagger > 0f){
	// 		_PerCharColoring();
	// 	}
	// }

	// private List<Vector3> _srcVerts;
	// private List<Vector3> _dstVerts;

	// private List<Color> _colors;

	// void _PerCharPositioning(){
	// 	float t = transCurves.stateTimer / transCurves.onTime;

	// 	// Have verts been rendered yet?
	// 	int vCount = kTextRend.textInfo.meshInfo[0].vertexCount;
	// 	if(vCount == 0){
	// 		return;
	// 	}
	// 	// There are verts to grab!
	// 	if(_srcVerts.Count == 0){
	// 		PhilTMProUtils.GetPositions(kTextRend, _srcVerts);
	// 	} else {
	// 		// Set _dst verts every frame.
	// 		_dstVerts.Clear();
	// 		int srcCount = _srcVerts.Count;
	// 		for(int i = 0; i < srcCount; i++){
	// 			int c = i / 4;
	// 			Vector3 srcVert = _srcVerts[i];
	// 			float tc = t - (c*transCharStagger);
	// 			Vector3 offset = transCurves.translationCurves.Evaluate(tc);
	// 			_dstVerts.Add(srcVert + offset);
	// 		}
	// 		PhilTMProUtils.SetPositions(kTextRend, _dstVerts);
	// 	}
	// }

	// void _PerCharColoring(){
	// 	float t = gradientPlayer.stateTimer / gradientPlayer.period;

	// 	int vertCount = kTextRend.textInfo.meshInfo[0].vertexCount;
	// 	if(vertCount == 0){
	// 		return;
	// 	}

	// 	// Assign them
	// 	_colors.Clear();
	// 	for(int i = 0; i < vertCount; i++){
	// 		int c = i / 4;
	// 		float tc = t - c*colorCharStagger;
	// 		Color color = gradientPlayer.gradient.Evaluate(tc);
	// 		_colors.Add(color);
	// 	}
	// 	PhilTMProUtils.SetColors(kTextRend, _colors);
	// }

}