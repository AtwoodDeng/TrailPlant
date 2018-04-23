using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using HutongGames.PlayMaker;

public class GameController : MonoBehaviour {

    public static GameController Instance
    {
        get
        {
            if (m_instance == null)
                m_instance = FindObjectOfType<GameController>();
            return m_instance;
        }
    }

    private static GameController m_instance;

    public List<Enermy> enermyList = new List<Enermy>();
    public List<Enermy> deadEnermyList = new List<Enermy>();

    public void RegisterEnermy( Enermy e )
    {
        enermyList.Add(e);

    }

    public Enermy GetRandomEnermy( )
    {
        if ( enermyList.Count > 0 )
        return enermyList[Random.RandomRange(0,enermyList.Count)];
        return null;
    }

    public void EnermyDead( Enermy e )
    {
        enermyList.Remove(e);
        deadEnermyList.Add(e);
    }

    [SerializeField] AudioSource bgmSource;
    [SerializeField] AudioClip zenBGM;
    [SerializeField] AudioClip EDMBGM;
    [SerializeField] SpriteRenderer moveTips;
    [SerializeField] SpriteRenderer SelectTips;
    [SerializeField] SpriteRenderer PlantTips;
    [SerializeField] SpriteRenderer DashTips;

    [SerializeField] AudioSource m_source;
    [SerializeField] AudioClip showSFX;

    [SerializeField] GameObject plantDot;
    [SerializeField] Transform[] budsPos;
    [SerializeField] PlantCreator.FlowerType[] flowerTypes;

    [SerializeField] PlayMakerFSM fsm;

    [SerializeField] GameObject enermyMelee;
    [SerializeField] GameObject enermyRange;

    public Color enermyColor = new Color(1f, 0.5f, 0.5f, 1f);

    public bool IsZen = true;

    public void Update()
    {
        if (MPlayer.Instance.PlayerAction.SwitchPlant.WasPressed)
            fsm.SendEvent("Select");

        if (MPlayer.Instance.PlayerAction.Plant.WasPressed)
            fsm.SendEvent("Plant");

        if (enermyList.Count <= 0)
            fsm.SendEvent("AllDie");
    }

    public void Start()
    {
        moveTips.DOFade(0, 0);
        SelectTips.DOFade(0, 0);
        PlantTips.DOFade(0, 0);
        DashTips.DOFade(0, 0);
    }

    public void OnEnable()
    {
        M_Event.RegisterEvent(LogicEvents.ActivePlant, OnActive);
    }

    public void OnDisable()
    {
        M_Event.UnregisterEvent(LogicEvents.ActivePlant, OnActive);
    }

    int counter = 0;
    public void OnActive(LogicArg arg )
    {
        counter++;
        if (counter >= 4)
            fsm.SendEvent("Dash");
    }
    public void ShowMove()
    {
        IsZen = true;

        moveTips.DOFade(1f, 1f).SetEase(Ease.Linear);
        bgmSource.clip = zenBGM;
        bgmSource.Play();

        m_source.clip = showSFX;
        m_source.Play();

        Subtitles.show("Welcome to the world of Flourish.");
    }

    public void ShowSelect()
    {
        moveTips.DOFade(0f, 1f).SetEase(Ease.Linear);
        SelectTips.DOFade(1f, 1f).SetEase(Ease.Linear);

        m_source.clip = showSFX;
        m_source.Play();

        Subtitles.show("This is a game about planting.");
        Subtitles.show("Now Select a seed.");
    }

    public void ShowPlant()
    {
        SelectTips.DOFade(0f, 1f).SetEase(Ease.Linear);
        PlantTips.DOFade(1f, 1f).SetEase(Ease.Linear);

        m_source.clip = showSFX;
        m_source.Play();


        Subtitles.show("You can press the plant button to place it anywhere you like.");
        Subtitles.show("Then the magic will happen.");
    }

    public void ShowDash()
    {
        PlantTips.DOFade(0f, 1f).SetEase(Ease.Linear);
        DashTips.DOFade(1f, 1f).SetEase(Ease.Linear);

        m_source.clip = showSFX;
        m_source.Play();

        for( int i = 0; i< budsPos.Length; ++ i )
        {
            var plant = Instantiate(plantDot) as GameObject;
            var com = plant.GetComponent<PlantDot>();

            plant.transform.position = budsPos[i].position;

            com.Init(flowerTypes[i], 1f, PlantCreator.FlowerType.None);
        }

        Subtitles.show("You can dash to the flower buds to make them bloom.");
    }

    public void EnterPlay()
    {
        DashTips.DOFade(0, 1f).SetEase(Ease.Linear);

        Subtitles.show("Enjoy.");
    }

    public void EnterEDM()
    {

        Subtitles.show("Hey, bro, what the fuck are you doing?", 15f, enermyColor);
        Subtitles.show("Why are you here playing a kid's toy?", 15f, enermyColor);

    }

    public void ShowEDM()
    {
        IsZen = false;

        Subtitles.show("Come on join the Party", 15f, enermyColor);

        bgmSource.clip = EDMBGM;
        bgmSource.Play();

        StartCoroutine(CreateEnermyInLine());
    }

    IEnumerator CreateEnermyInLine()
    { 
        Vector3 pos = MPlayer.Instance.Position;
        // create Enermy Around Player
        for( int i = 0; i < 5; ++ i )
        {
            var en = Instantiate(enermyMelee) as GameObject;

            en.transform.position = pos + new Vector3((-5 + 2 * i) * 2.56f, 5f, 0);

            yield return new WaitForSeconds(0.1f);
        }

        for (int i = 0; i < 5 ; ++i)
        {
            var en = Instantiate(enermyMelee) as GameObject;

            en.transform.position = pos + new Vector3((5 - 2 * i) * 2.56f, -5f, 0);

            yield return new WaitForSeconds(0.1f);
        }

       
    }

    public void EnterDefense()
    {
        Subtitles.show("Our System is hacked.");
        Subtitles.show("Protect yourself. The flowers can HELP~!!!");
    }

    public void EnterSuccessI()
    {
        IsZen = true;

        Subtitles.show("You did it.");

        Subtitles.show("I'm sorry about the inconvinence.");
        Subtitles.show("The bug is fixed. Now you can enjoy the game.");

        bgmSource.clip = zenBGM;
        bgmSource.time = Random.RandomRange(0.25f * zenBGM.length, 0.5f * zenBGM.length);
        bgmSource.Play();
    }

    public void EnterEDMII()
    {

        Subtitles.show("Do you think I've gone?", 15f, enermyColor);
        Subtitles.show("No !!!", 15f, enermyColor);

        IsZen = false;

        bgmSource.clip = EDMBGM;
        bgmSource.time = Random.RandomRange(0.25f * EDMBGM.length, 0.5f * EDMBGM.length);
        bgmSource.Play();
    }

    public void ShowEDMII()
    {

        Subtitles.show("I'm back", 15f, enermyColor);
        Subtitles.show("with a larger army", 15f, enermyColor);

        StartCoroutine(CreateEnermyInCircle());
    }


    IEnumerator CreateEnermyInCircle()
    {
        Vector3 pos = MPlayer.Instance.Position;

        int count = 12;
        float angle = Mathf.PI * 2f / count;
        float radius = 8f;

        // create Enermy Around Player
        for (int i = 0; i < count; ++i)
        {
            Vector3 thispos = pos + new Vector3(Mathf.Cos(angle * i ), Mathf.Sin(angle * i ), 0) * radius;

            var en = Instantiate(enermyRange) as GameObject;

            en.transform.position = thispos;

            yield return new WaitForSeconds(0.2f);
        }
            



    }

    public void EnterSucessfulII()
    {
        IsZen = true;

        Subtitles.show("You did it again!");

        Subtitles.show("I'm so sorry.");
        Subtitles.show("We may have to restart the machine to clean the virus.");

        bgmSource.clip = zenBGM;
        bgmSource.time = Random.RandomRange(0.5f * zenBGM.length, 0.75f * zenBGM.length);
        bgmSource.Play();
    }

    public void EnterFreePlayMode()
    {
        Subtitles.show("(Now you enter free to play mode)");
    }

    public void EnterFPZen()
    {

        IsZen = true;

        bgmSource.clip = zenBGM;
        bgmSource.time = Random.RandomRange(0 , zenBGM.length);
        bgmSource.Play();
    }

    public void EnterFPEDM()
    {
        IsZen = false;

        bgmSource.clip = EDMBGM;
        bgmSource.time = Random.RandomRange(0, EDMBGM.length);
        bgmSource.Play();

        StartCoroutine(CreateEnermy());
    }

    IEnumerator CreateEnermy()
    {
        Vector3 pos = MPlayer.Instance.Position;

        int count = (int)Random.RandomRange( 10f , 20f );
        float radius = 50f;

        // create Enermy Around Player
        for (int i = 0; i < count; ++i)
        {
            Vector3 thispos = new Vector3(Random.RandomRange(-radius, radius), Random.RandomRange(-radius, radius));

            var en = Instantiate(enermyRange) as GameObject;

            en.transform.position = thispos;

            yield return new WaitForSeconds(0.1f);


            var en2 = Instantiate(enermyMelee) as GameObject;

            en2.transform.position = thispos;

            yield return new WaitForSeconds(0.1f);
        }

    }
    
}
