using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water_Interaction : MonoBehaviour
{
    [SerializeField]
    private LayerMask _particleLayers;
    private LayerMask layer_index;
    public GameObject _WaterParticlePrefab;

    // Start is called before the first frame update
    void Start()
    {
        layer_index = LayerMask.GetMask("Balls");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.layer == layer_index)
        {
            Vector3 pNewParticlePos = col.gameObject.transform.position;
            GameObject pNewParticle = Instantiate(_WaterParticlePrefab);
            pNewParticle.transform.position = pNewParticlePos;
            Destroy(pNewParticle, 2f);
        }
    }
}
