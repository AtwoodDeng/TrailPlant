using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
public class PlantNode : MonoBehaviour {

    public enum State
    { 
        Grow,
        Finished,
    };

    public enum Type {
        Root,
        Stem,
    }

    [Header("Plant")]
    [SerializeField] GameObject prefab;
    [SerializeField] List<PlantNode> m_plants = new List<PlantNode>();
    [Space(10f)]
    [Header("Data")]
    [SerializeField] SplineTrailRenderer trail;
    [SerializeField] float maxRadius = 5f;
    [SerializeField] float minRadius = 0.2f;
    [SerializeField] float speed = 1.5f;
    [SerializeField] GameObject controlObj;
    [SerializeField] float height = 0.1f;
    [SerializeField] float intervel = 0.005f;
    [Tooltip("The length of initial radius (The rate of maxRadius)")]
    [SerializeField] MinMax initRadius = new MinMax(0.5f, 1f);
    [Tooltip("Define the max rad offset the animation( how much detail in the end)")]
    [SerializeField] MinMax RadOffset = new MinMax(4f, 7f);
    [Tooltip("Define the speed of the radius change. related to the total length")]
    [SerializeField] MinMax RadiusSpeed = new MinMax(0.2f, 0.5f);
    [Space(10f)]
    [Header("Status")]
    [SerializeField] Type m_type;
    // The radius in invert style
    // radius = 1 / r 
    [Tooltip("The invert of radius, 1/r")]
    [SerializeField] float temInvRadius = 5f;
    [SerializeField] float temHeight = 1f;
    [SerializeField] Vector3 velocity;
    [SerializeField] float radiusSpeed = 2f;
    [SerializeField] State m_state;
    [SerializeField] float radOffset = 0;
    [Tooltip("Decide the maxLength of the plant")]
    [SerializeField] float maxRadOffset = 4f;
    float oriInvRadius = 0;

    public void Init( Type _type , Vector3 initDir , float size)
    {
        velocity = initDir.normalized * speed;
        temInvRadius = 1f / ( ( Random.RandomRange(0,1f) > 0.5f? 1f: -1f ) * initRadius.Rand * maxRadius );
        temHeight = height * size ;
        oriInvRadius = temInvRadius;

        radiusSpeed = -temInvRadius * RadiusSpeed.Rand;
        maxRadOffset = Random.RandomRange(4f, 7f) * size;
        m_state = State.Grow;

        trail.height = temHeight;
        trail.width = intervel;

    }

    private void Start()
    {
    }
    
    void Update()
    {
        if (m_state == State.Grow)
        {
            temInvRadius += radiusSpeed * Time.deltaTime;
            if (Mathf.Abs(temInvRadius) > 1f / minRadius)
                temInvRadius = 1f / minRadius * Mathf.Sign(temInvRadius);

            if ( CheckFinish() )
                m_state = State.Finished;
            else
            {
                Vector3 acc = Vector3.Cross(velocity, Vector3.forward) * velocity.sqrMagnitude *  temInvRadius;
                
                velocity += acc * Time.deltaTime;
                velocity = velocity.normalized * speed;

                radOffset += velocity.magnitude * Time.deltaTime * temInvRadius;

                //Debug.DrawRay(controlObj.transform.position, velocity);
                //Debug.DrawRay(controlObj.transform.position, Vector3.Cross(velocity, Vector3.forward) * 1f / temInvRadius);

                //Debug.Log("Tem Radius " + ( 1f / temInvRadius) );
                
                if (controlObj != null)
                    controlObj.transform.position += velocity * Time.deltaTime;
            }
        } else if (m_state == State.Finished)
        {
            trail.emit = false;
            trail.IsUpdateMesh = false;
        }
    }

    public bool CheckFinish()
    {
        if (m_type == Type.Root)
            return radOffset * oriInvRadius < 0 && Mathf.Abs(radOffset) > Mathf.PI * maxRadOffset * 0.01f;
        if (m_type == Type.Stem)
            return Mathf.Abs(radOffset) > Mathf.PI * maxRadOffset * 0.01f;
        return false;
    }



}
