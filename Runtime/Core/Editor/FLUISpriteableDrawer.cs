using UnityEditor;
using UnityEngine;

namespace Phil.FLUI {

[CustomPropertyDrawer(typeof(FLUISpriteable))]
public class FLUISpriteableDrawer : PropertyDrawer {

    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty( rect, label, property );
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        rect = EditorGUI.PrefixLabel(rect, GUIUtility.GetControlID(FocusType.Passive), label);
        Vector2 wholeFieldSize = new Vector2(rect.width, EditorGUIUtility.singleLineHeight);

        FLUISpriteable dummy;

        var cTypeProp = property.FindPropertyRelative( nameof(dummy.type) );
        var cType = (FLUISpriteable.Type)cTypeProp.enumValueIndex;

        // # Draw Component Field
        {
            switch(cType){
            case FLUISpriteable.Type.UIImage: {
                var imgProp = property.FindPropertyRelative( nameof(dummy.image) );
                imgProp.objectReferenceValue = EditorGUI.ObjectField( new Rect(rect.position, wholeFieldSize), "Image", imgProp.objectReferenceValue, typeof(UnityEngine.UI.Image), true );
            } break;
            case FLUISpriteable.Type.SpriteRenderer: {
                var spriteRendererProp = property.FindPropertyRelative( nameof(dummy.spriteRenderer) );
                spriteRendererProp.objectReferenceValue = EditorGUI.ObjectField( new Rect(rect.position, wholeFieldSize), "Sprite", spriteRendererProp.objectReferenceValue, typeof(SpriteRenderer), true );
            } break;
            }
            rect.position += EditorGUIUtility.singleLineHeight*Vector2.up;
        } 

        // # Draw Enum Field
        {
            cTypeProp.enumValueIndex = System.Convert.ToInt32(
                EditorGUI.EnumPopup( new Rect(rect.position, wholeFieldSize), 
                "Type", 
                (FLUISpriteable.Type)cTypeProp.enumValueIndex )
            );
            rect.position += EditorGUIUtility.singleLineHeight*Vector2.up;
        }


        // Wrap it up!
        property.serializedObject.ApplyModifiedProperties();
        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 2 * EditorGUIUtility.singleLineHeight;
    }

}

}