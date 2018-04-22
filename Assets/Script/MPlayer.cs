using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MPlayer : MonoBehaviour {

    public static MPlayer Instance {
        get
        {
            if (m_instance == null)
                m_instance = FindObjectOfType<MPlayer>();
            return m_instance;
        }
    }

    private static MPlayer m_instance;

    [SerializeField] float acc=1f;
    [SerializeField] float vel=1f;
    [SerializeField] float drag = 2f;
    [SerializeField] Rigidbody2D rigidbody;

    public bool isDash = false;

    Vector3 m_velocity;
    Vector3 m_acceleration;
    Vector3 lastPos;
    Vector3 m_pos;

    [SerializeField] float dashDistance = 3f;
    [SerializeField] [ReadOnly] float dashDistancer = 0;
    [SerializeField] float dashVel = 40f;
    [SerializeField] ParticleSystem dashParticle;
    [SerializeField] AudioSource m_source;
    [SerializeField] AudioClip dashSound;
    [SerializeField] AudioClip pickSound;

    public bool isPlant = false;

    [SerializeField][ReadOnly] PlantSeed m_seed;

    public Vector3 DeltaPos { get { return Position - lastPos; } }
    public Vector3 Position { get { return m_pos; } }
	// Use this for initialization
	void Start () {
        m_pos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        if (isDash)
            UpdateDash();
        else 
            UpdatePosition();

        if (Input.GetKeyDown(KeyCode.Space) && !isDash  && !isPlant )
        {
            StartDash();

        }

        if ( Input.GetKeyDown(KeyCode.Space) && isPlant )
        {
            Plant();
        }

        lastPos = m_pos;
        m_pos = transform.position;

    }

    public void UpdatePosition()
    {
        Vector3 input = Vector3.zero;
        input.x = Input.GetAxis("Horizontal");
        input.y = Input.GetAxis("Vertical");

        if (input.magnitude < Mathf.Epsilon)
            m_velocity *= (1 - drag * Time.deltaTime);
        else 
            m_velocity += input * acc * Time.deltaTime;

        m_velocity = Vector3.ClampMagnitude(m_velocity, vel * (isPlant? 0.66f : 1f ));

        //m_pos = lastPos + m_velocity * Time.deltaTime;
        //transform.position = m_pos;

        transform.up = m_velocity.normalized;

        rigidbody.velocity = m_velocity;
        // Vector3.SmoothDamp( transform.position , trans )
    }

    public void StartDash()
    {
        isDash = true;
        dashDistancer = 0;
        m_velocity = m_velocity.normalized * dashVel;

        var e = dashParticle.emission;
        e.enabled = true;

        m_source.clip = dashSound;
        m_source.Play();

        GetComponent<CircleCollider2D>().radius *= 3f;
    }

    public void EndDash()
    {

        isDash = false;
        m_velocity = Vector3.zero;
        var e = dashParticle.emission;
        e.enabled = false;

        GetComponent<CircleCollider2D>().radius /= 3f;
    }

    public void UpdateDash()
    {
        dashDistancer += m_velocity.magnitude * Time.deltaTime;

        if ( dashDistancer >= dashDistance )
        {
            EndDash();
        }

        rigidbody.velocity = m_velocity;
    }


    public bool GetSeed( PlantSeed seed )
    {
        if (!isPlant)
        {
            Debug.Log("Plant " + seed.name);

            isPlant = true;

            m_seed = seed;

            m_seed.transform.parent = transform;
            m_seed.transform.localPosition = Vector3.up * 0.33f;

            m_seed.GetComponentInChildren<SpriteRenderer>().sortingLayerName = "Player";
            m_seed.GetComponentInChildren<SpriteRenderer>().sortingOrder = 10;

            m_source.clip = pickSound;
            m_source.Play();

            return true;

        }

        return false;
    }

    public void Plant()
    {
        isPlant = false;
        m_seed.GetComponentInChildren<SpriteRenderer>().sortingLayerName = "Default";
        m_seed.GetComponentInChildren<SpriteRenderer>().sortingOrder = 0;

        m_seed.CreateRoot(m_velocity.normalized);

        m_seed.transform.parent = null;
        m_seed = null;
    }


}
