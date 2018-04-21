using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantBase : MonoBehaviour {

    public enum Type
    {
        Root,
        Stem,
        FlowerPetal,
        Seed,

    }

    [Header("Plant")]
    [SerializeField] List<PlantBase> m_children = new List<PlantBase>();
    [SerializeField] PlantBase m_parent;
    [SerializeField] protected Type m_type;


    public void Init( PlantBase parent   )
    {
        if (parent != null)
            parent.m_children.Add(this);
    }

    public virtual Type GetPlantType()
    {
        return m_type;
    }


}
