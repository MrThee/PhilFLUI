using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

namespace Phil.FLUI {

    // This isn't designed to also encapsulate a time buffer.
    // That's something you generally wanna control from outside
    // in order to drive the output of whatever CharTransform function 
    // you pass-in here.
    public struct PerCharOps<A> {

        // By default
        public readonly AspectBuffer< Func<A,int, A> > CharTransform;
        readonly Func<A,A,A> w_AddOp;
        readonly Func<A,A,A> w_SubOp;
        readonly Func<A,float,A> w_MultOp;
        bool inLocalSpace => (w_AddOp != null);

        readonly List<A> m_baseAttr;
        readonly List<A> m_transformedAttr;

        public int currentGlyphCount => m_transformedAttr.Count / 4;

        // The transform can change, but the aspect buffer stays
        readonly Action< TMPro.TextMeshProUGUI, List<A> > w_GetAttributes;
        readonly Action< TMPro.TextMeshProUGUI, IReadOnlyList<A> > w_SetAttributes;

        public PerCharOps( Func<A,A,A> Add, Func<A,A,A> Sub, Func<A,float,A> Mult,
            Action< TMPro.TextMeshProUGUI, List<A> > GetAttr,
            Action< TMPro.TextMeshProUGUI, IReadOnlyList<A> > SetAttr, int initCharBufCapacity
        ) {
            this.w_AddOp = Add;
            this.w_SubOp = Sub;
            this.w_MultOp = Mult;
            this.m_baseAttr = new List<A>(initCharBufCapacity*4);
            this.m_transformedAttr = new List<A>(initCharBufCapacity*4);
            this.CharTransform = new AspectBuffer<Func<A, int, A>>( (a, ci) => a ); // Should this be cached?
            this.w_GetAttributes = GetAttr;
            this.w_SetAttributes = SetAttr;
        }

        private A CalcAvgPosition(A a, A b, A c, A d){
            return w_MultOp( w_AddOp( w_AddOp(a,b), w_AddOp(c,d) ), 0.25f );
        }

        private A CalcLocalPosition(A a, A b, A c, A d, A position){
            A center = CalcAvgPosition(a,b,c,d);
            A localPos = w_SubOp( position, center );
            return localPos;
        }

        public void WillRenderCanvasUpdate( TMPro.TextMeshProUGUI component ){
            if(component.gameObject.activeInHierarchy==false){
                return;
            }
            component.SetVerticesDirty();
            component.ForceMeshUpdate();
            int vCount = component.textInfo.meshInfo[0].vertexCount;
            if(vCount == 0){
                return;
            }
            
            // There are verts to take!
            w_GetAttributes( component, m_baseAttr );
            
            // Now apply our charTransform
            // One day I'd like to throw this in a job system/gpu shader
            m_transformedAttr.Clear();
            int baseAttrCount = m_baseAttr.Count;
            bool local = inLocalSpace;
            var space = local ? PerCharOps.Space.Local : PerCharOps.Space.Word;
            for(int i = 0; i < baseAttrCount; i++){
                int ci = i / 4;
                int fi = ci*4;
                A wordSpaceAttr = m_baseAttr[i];
                var a = m_baseAttr[fi];     var b = m_baseAttr[fi+1]; 
                var c = m_baseAttr[fi+2];   var d = m_baseAttr[fi+3];
                A attr = local ? CalcLocalPosition(a,b,c,d, wordSpaceAttr) : wordSpaceAttr;
                A transformedAttr = CharTransform.Get()(attr, ci);
                A newCharAttrValue = local ? w_AddOp(CalcAvgPosition(a,b,c,d), transformedAttr) : transformedAttr;
                m_transformedAttr.Add(newCharAttrValue);
            }
            w_SetAttributes(component, m_transformedAttr);
        }

        
        // Shorthands

        public static PerCharOps<B> LocalSpace<B>(  Func<B,B,B> Add, Func<B,B,B> Sub, Func<B,float,B> Mult,
            Action< TMPro.TextMeshProUGUI, List<B> > GetAttr,
            Action< TMPro.TextMeshProUGUI, IReadOnlyList<B> > SetAttr, int initCharBufCapacity )
        {
            return new PerCharOps<B>(Add, Sub, Mult, GetAttr, SetAttr, initCharBufCapacity);
        }

        public static PerCharOps<B> WordSpace<B>(
            Action< TMPro.TextMeshProUGUI, List<B> > GetAttr,
            Action< TMPro.TextMeshProUGUI, IReadOnlyList<B> > SetAttr, int initCharBufCapacity
        ) {
            return new PerCharOps<B>(null, null, null, GetAttr, SetAttr, initCharBufCapacity);
        }
    }

    public static class PerCharOps {

        public enum Space {
            Local,
            Word
        }

        public static PerCharOps<float> Float( bool charLocalSpace,
            Action< TMPro.TextMeshProUGUI, List<float> > GetAttr,
                Action< TMPro.TextMeshProUGUI, IReadOnlyList<float> > SetAttr, int initCharBufCapacity
        ) {
            if(charLocalSpace){
                return PerCharOps<float>.LocalSpace( (a,b) => a+b, (a,b) => a-b, (a,f) => a*f,
                GetAttr, SetAttr, initCharBufCapacity );
            } else {
                return PerCharOps<float>.WordSpace( GetAttr, SetAttr, initCharBufCapacity );
            }
        }

        public static PerCharOps<Vector2> V2( bool charLocalSpace,
            Action< TMPro.TextMeshProUGUI, List<Vector2> > GetAttr,
                Action< TMPro.TextMeshProUGUI, IReadOnlyList<Vector2> > SetAttr, int initCharBufCapacity
        ) {
            if(charLocalSpace){
                return PerCharOps<Vector2>.LocalSpace( (a,b) => a+b, (a,b) => a-b, (a,f) => a*f,
                GetAttr, SetAttr, initCharBufCapacity );
            } else {
                return PerCharOps<Vector2>.WordSpace( GetAttr, SetAttr, initCharBufCapacity );
            }
        }

        // TODO: make this private
        public static PerCharOps<Vector3> V3( bool charLocalSpace,
            Action< TMPro.TextMeshProUGUI, List<Vector3> > GetAttr,
                Action< TMPro.TextMeshProUGUI, IReadOnlyList<Vector3> > SetAttr, int initCharBufCapacity
        ) {
            if(charLocalSpace){
                return PerCharOps<Vector3>.LocalSpace( (a,b) => a+b, (a,b) => a-b, (a,f) => a*f,
                GetAttr, SetAttr, initCharBufCapacity );
            } else {
                return PerCharOps<Vector3>.WordSpace( GetAttr, SetAttr, initCharBufCapacity );
            }
        }

        public static PerCharOps<Color> DefaultColor( int initCharBufCapacity
        ) {
            return PerCharOps<Color>.WordSpace<Color>( Phil.TMProUtils.GetColors, Phil.TMProUtils.SetColors, initCharBufCapacity );
        }

        public static PerCharOps<Vector3> Positions( bool local, int initCharBufCapacity, System.Func<Vector3,int,Vector3> CalcGlyphPosition
        ) {
            var pco = PerCharOps.V3( local, Phil.TMProUtils.GetPositions, Phil.TMProUtils.SetPositions, initCharBufCapacity );
            pco.CharTransform.Set(CalcGlyphPosition);
            return pco;
        }
        
        public static PerCharOps<Vector3> Positions( bool local, int initCharBufCapacity
        ) {
            return PerCharOps.V3( local, Phil.TMProUtils.GetPositions, Phil.TMProUtils.SetPositions, initCharBufCapacity );
        }


        // NOOPs
        public static Vector3 Vector3_Noop (Vector3 input, int ci){
            return input;
        }
        public static Func<Color, int, Color> Const (Color color){
            return (a,b) => color;
        }

        public static Func<Vector3, int, Vector3> Const (Vector3 v3){
            return (a,b) => v3;
        }

        static Func<T, int, T> LerpFunc<T>( 
            Func<T, int, T> AFunc, Func<T, int, T> BFunc, Func<T,T,float,T> Lerp, Func<float> GetTParam
        ) {
            return (src, space) => {
                T aValue = AFunc(src, space);
                T bValue = BFunc(src, space);
                T lerpValue = Lerp(aValue, bValue, GetTParam());
                return lerpValue;
            };
        }

        static Func< Func<T, int, T> > BuildFuncLerp<T>( 
            Func<Func<T, int, T>> AFunc, Func<Func<T, int, T>> BFunc, Func<T,T,float,T> FLerp, Func<float> GetTParam ) 
        {
            var CFunc = LerpFunc(AFunc(), BFunc(), FLerp, GetTParam);
            return () => CFunc;
        }

        // WE MUST GO DEEPER //
        public static Func< Func<Vector3, int, Vector3> > BuildVector3FuncLerp( 
            Func<Func<Vector3, int, Vector3>> AFunc, Func<Func<Vector3, int, Vector3>> BFunc, Func<float> GetTParam ) 
        {
            return BuildFuncLerp(AFunc, BFunc, UnityEngine.Vector3.Lerp, GetTParam);
        }

        public static Func< float, float? > ConvergeOn( Func<Func<Vector3, int, Vector3>> GetTarget,
            AspectBuffer<Func<Vector3, int, Vector3>> buffer, Func<float> Timer, float period )
        {
            Func<float> NormTimer = () => Timer() / period;
            var Lorp = BuildVector3FuncLerp( buffer.Get, GetTarget, NormTimer );
            return (_dt) => {
                float lifeTime = Timer();
                buffer.Set( Lorp() );
                if(lifeTime >= period){
                    return period - lifeTime;
                } else {
                    return null;
                }
            };
        }

        public static Action<float> LerpTowards( Func<Func<Vector3, int, Vector3>> GetStart,
            Func<Func<Vector3, int, Vector3>> GetEnd, AspectBuffer<Func<Vector3,int,Vector3>> buf, 
            Func<float> Timer, Func<float> Period )
        {
            Func<float> NormTimer = () => Timer() / Period();
            var Lorp = BuildVector3FuncLerp(GetStart,GetEnd, NormTimer);
            return (_) => {
                float t = Timer() / Period();
                buf.Set( Lorp() );
            };
        }

        // TODO: clearly illustrate that text length should probably be the whitespaceless length.
        public static float CalcCharAnimationParameter(float timer, float charAnimPeriod, float wordAnimPeriod, int charID, int textLength){
            // NOTE: text length and ci 
            float cs = (float)charID / (textLength-1);
            float t = (timer / charAnimPeriod) - cs*wordAnimPeriod;
            return t;
        }

    }

}