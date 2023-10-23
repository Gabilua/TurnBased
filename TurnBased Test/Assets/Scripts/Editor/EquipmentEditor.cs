//we need that using statement
using UnityEditor;
using UnityEngine;

//We connect the editor with the Weapon SO class
[CustomEditor(typeof(EquipmentInfo))]
//We need to extend the Editor
public class EquipmentEditor : Editor
{
    //Here we grab a reference to our Weapon SO
    EquipmentInfo equipment;

    private void OnEnable()
    {
        //target is by default available for you
        //because we inherite Editor
        equipment = target as EquipmentInfo;
    }

    //Here is the meat of the script
    public override void OnInspectorGUI()
    {
        //Draw whatever we already have in SO definition
        base.OnInspectorGUI();
        //Guard clause
        if (equipment.equipmentIcon == null)
            return;

        //Convert the weaponSprite (see SO script) to Texture
        Texture2D texture = AssetPreview.GetAssetPreview(equipment.equipmentIcon);
        //We crate empty space 80x80 (you may need to tweak it to scale better your sprite
        //This allows us to place the image JUST UNDER our default inspector
        GUILayout.Label("", GUILayout.Height(90), GUILayout.Width(90));
        //Draws the texture where we have defined our Label (empty space)

        if (texture != null)
            texture.filterMode = FilterMode.Point;

        GUI.DrawTexture(GUILayoutUtility.GetLastRect(), texture);
    }
}