using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent( typeof(BoundsCheck) )]
public class PowerUp : MonoBehaviour
{
    [Header("Inscribed")]
    // This is unusual but handy use of Vector2s.
    [Tooltip("x holds a min value and y a max value for a Random.Range() call.")]
    public Vector2          rotMinMax = new Vector2(15, 90);
    [Tooltip("x holds a min value and y a max value for a Random.Range() call.")]
    public Vector2          driftMinMax = new Vector2(.25f,2);
    public float            lifeTime = 10; // PowerUp will exist for # seconds
    public float            fadeTime = 4;  // Then it fades over # seconds

    [Header("Dynamic")]
    public eWeaponType      type;           // type of PowerUp
    public GameObject       cube;           // Reference to the PowerCube child
    public TextMesh         letter;         // Reference to TextMesh
    public Vector3          rotPerSecond;   // Euler rotation speed for PowerCube
    public float            birthTime;      // The Time.time this was instantiated
    private Rigidbody       rigid;          
    private BoundsCheck     bndCheck;
    private Material        cubeMat;

    void Awake() {
        // Find the cube reference (there's only a single child)
        cube = transform.GetChild(0).gameObject;
        // Find the TextMesh and other components
        letter = GetComponent<TextMesh>();
        rigid = GetComponent<Rigidbody>();
        bndCheck = GetComponent<BoundsCheck>();
        cubeMat = cube.GetComponent<Renderer>().material;

        // Set random velocity
        Vector3 vel = Random.onUnitSphere;  // Get Random XYZ velocity
        vel.z = 0;          // Flatten the evl to the XY plane
        vel.Normalize();    // Normalizing a Vector3 sets its length to 1m

        vel *= Random.Range(driftMinMax.x, driftMinMax.y);
        rigid.linearVelocity = vel;

        // Set the rotation of this PowerUp GameObject to R:[ 0, 0, 0 ]
        transform.rotation = Quaternion.identity;
        // Quaternion.identity is equal to no rotation

        // Randomize rotPerSecond for PowerCube using rotMinMax x & y
        rotPerSecond = new Vector3( Random.Range( rotMinMax[0], rotMinMax[1] ),
                                    Random.Range( rotMinMax[0], rotMinMax[1] ),
                                    Random.Range( rotMinMax[0], rotMinMax[1] ) );

        birthTime = Time.time;
    }

    // Update is called once per frame
    void Update() {
        cube.transform.rotation = Quaternion.Euler( rotPerSecond*Time.time );

        // Fade out the PowerUp over time
        // Given default values, a PowerUp will exist for 10 seconds
        //      and then fade out over 4 seconds
        float u = (Time.time - (birthTime+lifeTime)) / fadeTime;
        // If u >= 1, destroy this PowerUp
        if (u >= 1) {
            Destroy( this.gameObject );
            return;
        }
        // If u>0, decrease the opacity (i.e., alpha) of the PowerCube & Letter
        if (u>0) {
            Color c = cubeMat.color;
            c.a = 1f - u;               // Set alpha of PowerCube to 1-u
            cubeMat.color = c;
            // Fade Letter too, just not as much
            c = letter.color;
            c.a = 1f - (u*0.5f);        // Set alpha of letter to 1-(u/2)
            letter.color = c;
        }

        if (!bndCheck.isOnScreen) {
            // If PowerUp has drifted entirely off screen, destroy it
            Destroy( gameObject );
        }
    }

    public eWeaponType _type { get { return _type; } set { SetType(value); } }
    
    public void SetType( eWeaponType wt ) {
        // Grab the WeaponDefinition from Main
        WeaponDefinition def = Main.GET_WEAPON_DEFINITION( wt );
        cubeMat.color = def.powerUpColor;   // Set color of PowerCube
        //letter.color = def.color;         // We could colorize the letter too
        letter.text = def.letter;           // Set the letter that is shown
        type = wt;                          // Finally actually set the type
    }

    public void AbsorbedBy( GameObject target ) {
        Destroy( this.gameObject );
    }
}
