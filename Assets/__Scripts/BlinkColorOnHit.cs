using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class BlinkColorOnHit : MonoBehaviour
{
    private static float blinkDuration = 0.1f;  // # seconds to show damage
    private static Color blinkColor     = Color.red;

    [Header("Dynamic")]
    public bool         showingColor = false;
    public float        blinkCompleteTime;  // Time to stop showing the color

    private Material[]  materials;  // All the Materials of this & its children
    private Color[]     originalColors;
    private BoundsCheck bndCheck;

    void Awake() {
        bndCheck = GetComponentInParent<BoundsCheck>();
        // Get materials and colors for this GameObject and its children
        materials = Utils.GetAllMaterials( gameObject );
        Debug.Log("Materials count: " + materials.Length);
        foreach (Material m in materials) {
            Debug.Log("Material: " + m.name);
        }
        originalColors = new Color[materials.Length];
        for (int i=0; i <materials.Length; i++) {
            originalColors[i] = materials[i].color;
        }
    }

    void Update() {
        if ( showingColor && Time.time > blinkCompleteTime ) RevertColors();
    }

    void OnCollisionEnter( Collision coll ) {
        Debug.Log("Collision detected");
        // Check for collisions with ProjectileHero
        ProjectileHero p = coll.gameObject.GetComponent<ProjectileHero>();
        if ( p != null ) {
            if ( bndCheck != null && !bndCheck.isOnScreen ) {
                return; // Don't show damage if this is off screen
            }
            SetColors();
        }
    }

    void SetColors() {
        foreach (Material m in materials) {
            m.color = blinkColor;
            Debug.Log("Material color set to red");
        }
        showingColor = true;
        blinkCompleteTime = Time.time + blinkDuration;
    }

    void RevertColors() {
        for ( int i=0; i <materials.Length; i++) {
            materials[i].color = originalColors[i];
        }
        showingColor = false;
    }
}
