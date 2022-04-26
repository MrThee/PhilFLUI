using UnityEditor;
using UnityEngine;

using Phil.Core;

#if UNITY_EDITOR

namespace Phil.FLUI{

[CustomPropertyDrawer(typeof(FLUIEventContainer))]
public class FLUIEventContainerDrawer : 
    PolyObjectDrawer<FLUIEventContainer, FLUIEventContainer.Type, PointerEventContainer, SmartPointerEventContainer>
{
    public override string aFieldLabel => "Pointer";
    public override string bFieldLabel => "Smart";
    public override FLUIEventContainer.Type Int2Type(int i)
    {
        return (FLUIEventContainer.Type)i;
    }
    
}

}

#endif
