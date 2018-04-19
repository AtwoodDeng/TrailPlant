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
    Vector3 m_velocity;
    Vector3 m_acceleration;
    Vector3 lastPos;
    Vector3 m_pos;

    public Vector3 DeltaPos { get { return Position - lastPos; } }
    public Vector3 Position { get { return m_pos; } }
	// Use this for initialization
	void Start () {
        m_pos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        UpdatePosition();
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
        m_velocity = Vector3.ClampMagnitude(m_velocity, vel);

        lastPos = m_pos;
        m_pos = lastPos + m_velocity * Time.deltaTime;
        transform.position = m_pos;
        // Vector3.SmoothDamp( transform.position , trans )
    }
}
