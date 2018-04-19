using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantCreator : MonoBehaviour {

    [SerializeField] GameObject prefab;
    [SerializeField] MinMax timeInterval = new MinMax(1f, 5f);
    [SerializeField] MinMax distanceInterval = new MinMax(1f, 5f);

    [Space(10f)]
    [Header("Status")]
    [SerializeField] float timer;
    [SerializeField] float distancer;

    private void Update()
    {
        timer -= Time.deltaTime;
        distancer -= MPlayer.Instance.DeltaPos.magnitude;

        if (timer < 0 || distancer < 0)
        {
            CreatePlant();
            timer = timeInterval.Rand;
            distancer = distanceInterval.Rand;
        }
    }

    public void CreatePlant()
    {
        var plant = Instantiate(prefab) as GameObject;
        var com = plant.GetComponent<PlantNode>();
        plant.transform.position = MPlayer.Instance.Position + Vector3.forward;

        float size = Random.RandomRange(0.5f, 1f);
        Vector3 dir = MPlayer.Instance.DeltaPos.normalized + (Vector3) Random.insideUnitCircle * 0.5f ;

        com.Init(PlantNode.Type.Root, dir, size);
    }

}
