#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

// Lets us change the neutral and heavy attacks on the monster part and have it change to the subclass in the editor. This lets us edit subclass vars in the editor
[CustomPropertyDrawer(typeof(NeutralAttack), true)]
[CustomPropertyDrawer(typeof(HeavyAttack), true)]
public class BaseAttackDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        float y = position.y;
        float lineHeight = EditorGUIUtility.singleLineHeight;
        float spacing = 4f;

        // Get the actual attack instance
        BaseAttack baseAttack = property.managedReferenceValue as BaseAttack;

        if (baseAttack == null)
        {
            EditorGUI.LabelField(position, "Null Base Attack");
            EditorGUI.EndProperty();
            return;
        }

        // Get enum type and value depending on subclass type
        System.Enum currentEnum = null;
        if (baseAttack is NeutralAttack neutral)
        {
            currentEnum = neutral.Attack;
        }
        else if (baseAttack is HeavyAttack heavy)
        {
            currentEnum = heavy.Attack;
        }

        if (currentEnum == null)
        {
            EditorGUI.LabelField(position, "Unknown Attack Enum");
            EditorGUI.EndProperty();
            return;
        }

        // Draw the enum popup manually
        Rect enumRect = new Rect(position.x, y, position.width, lineHeight);
        EditorGUI.BeginChangeCheck();

        System.Enum newEnum = EditorGUI.EnumPopup(enumRect, "Attack", currentEnum);
        y += lineHeight + spacing;

        if (EditorGUI.EndChangeCheck())
        {
            // Create a new subclass instance with the new enum selected
            BaseAttack newAttack = null;

            if (baseAttack is NeutralAttack neutralAttack)
            {
                neutralAttack.Attack = (NeutralAttack.AttackType)newEnum;
                newAttack = neutralAttack.GetAttack();
                if (newAttack != null)
                    ((NeutralAttack)newAttack).Attack = (NeutralAttack.AttackType)newEnum;
            }
            else if (baseAttack is HeavyAttack heavyAttack)
            {
                heavyAttack.Attack = (HeavyAttack.HeavyAttackType)newEnum;
                newAttack = heavyAttack.GetAttack();
                if (newAttack != null)
                    ((HeavyAttack)newAttack).Attack = (HeavyAttack.HeavyAttackType)newEnum;
            }

            if (newAttack != null)
            {
                property.managedReferenceValue = newAttack;
                property.serializedObject.ApplyModifiedProperties();
            }

            EditorGUI.EndProperty();
            return; // We return early here because the instance changed, we redraw next frame
        }

        // Draw the rest of the fields
        SerializedProperty iterator = property.Copy();
        SerializedProperty endProperty = iterator.GetEndProperty();
        bool enterChildren = true;

        while (iterator.NextVisible(enterChildren) && !SerializedProperty.EqualContents(iterator, endProperty))
        {
            // Skip the base field since we drew the enum manually
            if (iterator.name == "Attack")
                continue;

            float height = EditorGUI.GetPropertyHeight(iterator, true);
            Rect propRect = new Rect(position.x, y, position.width, height);
            EditorGUI.PropertyField(propRect, iterator, true);
            y += height + spacing;
            enterChildren = false;
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = EditorGUIUtility.singleLineHeight + 4f;

        SerializedProperty iterator = property.Copy();
        SerializedProperty endProperty = iterator.GetEndProperty();
        bool enterChildren = true;

        while (iterator.NextVisible(enterChildren) && !SerializedProperty.EqualContents(iterator, endProperty))
        {
            if (iterator.name == "Attack")
                continue;

            height += EditorGUI.GetPropertyHeight(iterator, true) + 2f;
            enterChildren = false;
        }

        return height;
    }
}
#endif
