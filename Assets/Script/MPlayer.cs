using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

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
	[SerializeField] SpriteRenderer centerSprite;
	[SerializeField] GameObject hitEffect;
    [SerializeField] AudioSource m_source;
    [SerializeField] AudioClip dashSound;
    [SerializeField] AudioClip zenDashSound;
    [SerializeField] AudioClip pickSound;
	[SerializeField] AudioClip hitSound;
    [SerializeField] AudioClip errorSound;
    private MPlayerActions m_playerActions;
    public MPlayerActions PlayerAction { get { return m_playerActions; } }

    [SerializeField] float hp = 100f;

    public bool isPlant = false;

	public bool isProtect = false;

    [SerializeField][ReadOnly] PlantSeed m_seed;
    [SerializeField] float seedRenewDuration = 5f;
    [SerializeField][ReadOnly] float seedTimer = 0;
    [SerializeField] Image SeedProcessBar;
    [SerializeField] Image SeedTypeBar;
    [SerializeField] Image HealthBar;

    public Vector3 DeltaPos { get { return Position - lastPos; } }
    public Vector3 Position { get { return m_pos; } }

    public bool IsReadyPlant {  get { return seedTimer < 0;  } }

    public Vector3 Velocity {  get { return m_velocity; } }
	// Use this for initialization
	void Start () {
        m_pos = transform.position;
        m_playerActions = MPlayerActions.CreateWithDefaultBindings();
    }
	
	// Update is called once per frame
	void Update () {
        if (isDash)
            UpdateDash();
        else 
            UpdatePosition();

        if (m_playerActions.Dash.WasPressed)
        {
            if (!isDash && !isPlant)
                StartDash();
            else
            {
                m_source.clip = errorSound;
                m_source.Play();
            }
        } 

        if ( m_playerActions.Plant.WasPressed )
        {
            if (isPlant)
            {
                Plant();
            }else
            {
                m_source.clip = errorSound;
                m_source.Play();
            }
        }

        UpdateUI();

        lastPos = m_pos;
        m_pos = transform.position;
        seedTimer -= Time.deltaTime;

    }

    public void UpdateUI()
    {
        HealthBar.fillAmount = Mathf.Max(0, hp / 100f);
        SeedProcessBar.fillAmount = Mathf.Max(0, 1f - seedTimer / seedRenewDuration);

        if (m_seed != null)
            SeedTypeBar.color = PlantCreator.Instance.GetFlowerItem(m_seed.MFlowerType).color;
        else
            SeedTypeBar.color = new Color(1f, 1f, 1f, 0);
    }

    public void UpdatePosition()
    {
        Vector3 input = Vector3.zero;
        input.x = m_playerActions.Move.X;
        input.y = m_playerActions.Move.Y;

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

        m_source.clip = GameController.Instance.IsZen ? zenDashSound : dashSound;
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
        if (m_velocity.magnitude < 0.1f)
            EndDash();

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
        if (seedTimer < 0)
        {
            isPlant = false;
            m_seed.GetComponentInChildren<SpriteRenderer>().sortingLayerName = "Default";
            m_seed.GetComponentInChildren<SpriteRenderer>().sortingOrder = 0;

            m_seed.CreateRoot(m_velocity.normalized);

            m_seed.transform.parent = null;
            m_seed = null;

            seedTimer = seedRenewDuration;
        }
    }

    public void Drop()
    {
        isPlant = false;

        if (m_seed != null)
        {
            Destroy(m_seed.gameObject);
            m_seed = null;
        }
    }

	public void Attack( Enermy sender , float dmg )
	{
		if (!isProtect) {
			hp -= dmg;

			isProtect = true;

			m_source.clip = hitSound;
			m_source.Play ();

            m_velocity = Vector3.zero;

			centerSprite.transform.DOShakePosition (1f, 0.5f);

			var seq = DOTween.Sequence ();
			seq.Append (centerSprite.DOColor (Color.gray, 0.5f).SetEase (Ease.InOutCubic));
			seq.Append (centerSprite.DOColor (Color.white, 0.5f).SetEase (Ease.InOutCubic));
			seq.Append (centerSprite.DOColor (Color.gray, 0.5f).SetEase (Ease.InOutCubic));
			seq.Append (centerSprite.DOColor (Color.white, 0.5f).SetEase (Ease.InOutCubic));
			seq.Append (centerSprite.DOColor (Color.gray, 0.5f).SetEase (Ease.InOutCubic));
			seq.Append (centerSprite.DOColor (Color.white, 0.5f).SetEase (Ease.InOutCubic));
			seq.AppendCallback (delegate {
				isProtect = false;	
			});

			var hitE = Instantiate (hitEffect);
			hitE.transform.parent = transform;
			hitE.transform.localPosition = Vector3.zero;

            if (hp <= 0 && hp > -10f )
                Subtitles.show("You're died. But in this prototype. You can keep playing.");
		} else
        {
            m_source.clip = hitSound;
            m_source.Play();

            var hitE = Instantiate(hitEffect);
            hitE.transform.parent = transform;
            hitE.transform.localPosition = Vector3.zero;
        }
	}

	public void OnGUI()
	{
		GUILayout.Label (" HP : " + hp );
	}

}
