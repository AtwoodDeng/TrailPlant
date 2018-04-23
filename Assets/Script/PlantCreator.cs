using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class PlantCreator : MonoBehaviour {

    public static PlantCreator Instance
    {
        get
        {
            if (m_instance == null)
                m_instance = FindObjectOfType<PlantCreator>();
            return m_instance;
        }
    }

    private static PlantCreator m_instance;

    [SerializeField] GameObject seedPrefab;
    [SerializeField] GameObject SelectPrefab;
    [SerializeField] MinMax timeInterval = new MinMax(1f, 5f);
    [SerializeField] MinMax distanceInterval = new MinMax(1f, 5f);
    [SerializeField] int maxActiveNode = 10;
    [SerializeField] float createDistance = 10f;

    [SerializeField] GameObject plantEffect;


    [Space(10f)]
    [Header("Status")]
    [SerializeField] [ReadOnly] float timer;
    [SerializeField] [ReadOnly] float distancer;
    [SerializeField] [ReadOnly] List<PlantNode> plantNodes = new List<PlantNode>();
    [SerializeField] [ReadOnly] List<PlantSeed> plantSeeds = new List<PlantSeed>();

    

    public enum FlowerType
    {
        None,
        Yellow,
        Blue,
        Red,
        Perple,
        Pink,
        White,
    }

    [System.Serializable]
    public class FlowerItem
    {
        public FlowerType type;
        public int petalNumber;
        public Color color;
        public GameObject petalPrefab;
        public GameObject budPrefab;
        public GameObject AttackPrefab;
    }

    [SerializeField] AudioSource m_source;
    [SerializeField] AudioClip errorSound;
    [Space(10f)]
    [Header("Flower Library")]
    [SerializeField] public Color greenColor;
    [SerializeField] List<FlowerItem> flowerLibs = new List<FlowerItem>();
    [SerializeField] [ReadOnly] PlantSeed m_seed;


    int temSeedIndex = 0;

    private void Update()
    {
        timer -= Time.deltaTime;
        distancer -= MPlayer.Instance.DeltaPos.magnitude;
       

        if (timer < 0 || distancer < 0)
        {
            //CreateSeed();
            timer = timeInterval.Rand;
            distancer = distanceInterval.Rand;
        }

        if (MPlayer.Instance.PlayerAction.SwitchPlant.WasPressed)
        {
            if (MPlayer.Instance.IsReadyPlant) {
                if (MPlayer.Instance.isPlant)
                    temSeedIndex = (temSeedIndex + 1) % flowerLibs.Count;

                 SelectSeed(flowerLibs[temSeedIndex]);
             }else
            {
                m_source.clip = errorSound;
                m_source.Play();
            }
        }

        

    }

    public void SelectSeed( FlowerItem item )
    {
        if ( MPlayer.Instance.isPlant )
        {
           MPlayer.Instance.Drop();

        }

        var effect = Instantiate(SelectPrefab) as GameObject;
        effect.transform.position = transform.position;
        var effCom = effect.GetComponentInChildren<SpriteRenderer>();
        effCom.color = item.color;


        var seed = Instantiate(seedPrefab) as GameObject;
        var com = seed.GetComponent<PlantSeed>();
        com.Init(item.type, 1f, FlowerType.None);
        plantSeeds.Add(com);
        MPlayer.Instance.GetSeed(com);
    }

    public int GetActiveNodeNumber()
    {
        int sum = 0;
        for (int i = 0; i < plantNodes.Count; ++i)
            if (plantNodes[i].MState == PlantNode.State.Grow)
                sum++;

        return sum;
    }

    public int GetActiveSeedNumber()
    {
        int sum = 0;
        for (int i = 0; i < plantSeeds.Count; ++i)
            if (plantSeeds[i].MState == PlantSeed.State.Active )
                sum++;

        return sum;
    }

    public void CreateSeed()
    {
        if (GetActiveSeedNumber() >= maxActiveNode)
            return;

        var seed = Instantiate(seedPrefab) as GameObject;

        var pos = MPlayer.Instance.Position + (Vector3)Random.insideUnitCircle * createDistance;
        seed.transform.position = pos;

        var com = seed.GetComponent<PlantSeed>();

        com.Init( GetRandomFlowerType().type , 1f , FlowerType.None );

        plantSeeds.Add(com);

    }

    public void CreatePlantEffect()
    {
        var eff = Instantiate(plantEffect) as GameObject;
        eff.transform.position = MPlayer.Instance.Position;


    }

    public FlowerItem GetRandomFlowerType()
    {
        return flowerLibs[Random.RandomRange(0,flowerLibs.Count)];
    }

    public FlowerItem GetFlowerItem( FlowerType type )
    {
        return flowerLibs.Find((x) => x.type == type);
    }

    //public void CreatePlant()
    //{
    //    if (GetActiveNodeNumber() >= maxActiveNode)
    //        return;

    //    var plant = Instantiate(prefab) as GameObject;
    //    var com = plant.GetComponent<PlantNode>();
    //    plant.transform.position = MPlayer.Instance.Position + Vector3.forward;

    //    float size = Random.RandomRange(0.5f, 1f);
    //    Vector3 dir = MPlayer.Instance.DeltaPos.normalized + (Vector3) Random.insideUnitCircle * 0.5f ;

    //    com.Init( dir, size , null );
    //    plantNodes.Add(com);
    //}

}
