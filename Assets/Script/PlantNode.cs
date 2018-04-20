using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
public class PlantNode : PlantBase {

    public enum State
    { 
        Grow,
        Finished,
    };

    [Space(10f)]
    [Header("Plant Prefab")]
    [SerializeField] GameObject plantStem;
    [SerializeField] GameObject plantDot;

    [Space(10f)]
    [Header("Trail")]
    [SerializeField] SplineTrailRenderer trail;
    //[SerializeField] GameObject controlObj;
    [Space(10f)]
    [Header("Data")]
    //[SerializeField] float maxRadius = 5f;
    [SerializeField] MinMax lifeTime = new MinMax(5f, 10f);
    [SerializeField] MinMax speed = new MinMax(3f, 5f);
    [SerializeField] AnimationCurve radiusCurve;
    [SerializeField] float minRadius = 0.2f;
    [SerializeField] float height = 0.1f;
    [SerializeField] AnimationCurve heightCurve;
    [SerializeField] float intervel = 0.005f;
    [Tooltip("The length of initial radius ")]
    [SerializeField] MinMax initRadius = new MinMax(5f, 10f);
    [Tooltip("Define the max rad offset the animation( how much detail in the end)")]
    [SerializeField] MinMax RadOffset = new MinMax(4f, 7f);
    //[Tooltip("Define the speed of the radius change. related to the total length")]
    //[SerializeField] MinMax RadiusSpeed = new MinMax(0.2f, 0.5f);
    [Space(10f)]
    [Header("Status")]
    [SerializeField] [ReadOnly] float m_size;
    // The radius in invert style
    // radius = 1 / r 
    [Tooltip("The invert of radius, 1/r")]
    [SerializeField] [ReadOnly] float temInvRadius = 5f;
    [SerializeField] [ReadOnly] float lifeTimer;
    [SerializeField] [ReadOnly] float m_lifeTime = 5f;
    [SerializeField] [ReadOnly] float m_speed = 3f;
    [SerializeField] [ReadOnly] float m_InvRadius = 3f;
    [SerializeField] [ReadOnly] float m_height = 1f;
    [SerializeField] [ReadOnly] Vector3 velocity;
    //[SerializeField] [ReadOnly] float radiusSpeed = 2f;
    [SerializeField] [ReadOnly] State m_state;
    [SerializeField] [ReadOnly] float radOffset = 0;
    [Tooltip("Decide the maxLength of the plant")]
    [SerializeField] [ReadOnly] float maxRadOffset = 4f;
    [SerializeField] [ReadOnly] List<float> splitList = new List<float>();
    float oriInvRadius = 0;

    public State MState { get { return m_state; }  }
    public float LifeRatio {  get { return lifeTimer / m_lifeTime;  } }

    public void Init(Vector3 initDir, float size , PlantBase parent , float invRadius = 0 )
    {
        base.Init( parent);

        m_size = size;
        m_lifeTime = lifeTime.Rand;
        lifeTimer = 0;
        m_speed = speed.Rand;
        velocity = initDir.normalized * m_speed;
        if (invRadius != 0)
            m_InvRadius = invRadius;
        else
            m_InvRadius = 1f / ((Random.RandomRange(0, 1f) > 0.5f ? 1f : -1f) * initRadius.Rand );

        m_height = height * size ;
        oriInvRadius = temInvRadius = m_InvRadius;

        //radiusSpeed = -temInvRadius * RadiusSpeed.Rand;
        maxRadOffset = Random.RandomRange(4f, 7f) * m_size;
        m_state = State.Grow;

        trail.height = m_height;
        trail.width = intervel;

        int splitTime = Random.RandomRange(0, 5);
        for( int i = 0; i < splitTime; ++ i )
        {
            splitList.Add(Random.RandomRange(0.4f, 1.2f));
            splitList.Sort();
        }


    }
    
    void Update()
    {
        if (m_state == State.Grow)
        {
            lifeTimer += Time.deltaTime;

            UpdateGrow();
            UpdateSplit();

            if (CheckFinish())
                m_state = State.Finished;

        } else if (m_state == State.Finished)
        {
            trail.emit = false;
            trail.IsUpdateMesh = false;
        }
    }

    public void UpdateGrow()
    {
        temInvRadius = radiusCurve.Evaluate(LifeRatio) * m_InvRadius;
        if (Mathf.Abs(temInvRadius) > 1f / minRadius)
            temInvRadius = 1f / minRadius * Mathf.Sign(temInvRadius);


        Vector3 acc = Vector3.Cross(velocity, Vector3.forward) * velocity.sqrMagnitude * temInvRadius;

        velocity += acc * Time.deltaTime;
        velocity = velocity.normalized * m_speed;

        radOffset += velocity.magnitude * Time.deltaTime * temInvRadius;

        //Debug.DrawRay(controlObj.transform.position, velocity);
        //Debug.DrawRay(controlObj.transform.position, Vector3.Cross(velocity, Vector3.forward) * 1f / temInvRadius);

        //Debug.Log("Tem Radius " + ( 1f / temInvRadius) );

        if ( trail.gameObject != null)
            trail.gameObject.transform.position += velocity * Time.deltaTime;

        trail.height = heightCurve.Evaluate(LifeRatio) * m_height;

    }

    public void  UpdateSplit()
    {
        if (m_type == Type.Root)
        {
            for (int i = 0; i < splitList.Count; ++i)
            {
                if (splitList[i] < LifeRatio)
                {
                    splitList[i] = 99999f;

                    var plant = Instantiate(plantStem) as GameObject;
                    var com = plant.GetComponent<PlantNode>();
                    plant.transform.position = trail.transform.position;
                    plant.transform.parent = transform;

                    Vector3 initDir = velocity.normalized;
                    float radius = -temInvRadius * Random.RandomRange(0.5f, 1f);
                    float size = m_size * Random.RandomRange(0.6f, 1f);

                    com.Init(initDir, size, this, radius);
                }
                
            }
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
