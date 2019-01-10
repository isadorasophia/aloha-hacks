using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water_Interaction : MonoBehaviour
{
    [SerializeField]
    private LayerMask _particleLayers;
    public GameObject _WaterParticlePrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collider col)
    {
        if (CheckForParticleLayer(col.gameObject.layer))
        {
            Vector3 pNewParticlePos = col.gameObject.transform.position;
            GameObject pNewParticle = Instantiate(_WaterParticlePrefab);
            pNewParticle.transform.position = pNewParticlePos;
            Destroy(pNewParticle, 2f);
        }
    }

    private bool CheckForParticleLayer(LayerMask layer)
    {
        return layer == (_particleLayers | (1 << layer));
    }
}
