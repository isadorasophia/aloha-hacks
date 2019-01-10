using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeagullSpawner : MonoBehaviour
{
    public GameObject seagullPrefab;
    private float spawnTimer = 0;
    private float flapTimer = 0;
    private bool flying = false;
    private GameObject seagull;
    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {

        seagull = Instantiate(seagullPrefab);
        seagull.transform.position = transform.position;
        seagull.transform.rotation = transform.rotation;
        anim = seagull.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        spawnTimer += Time.deltaTime;
        flapTimer += Time.deltaTime;

        if (flying)
            seagull.transform.Translate(0, 0.02f, 0.06f);

        if (flying && flapTimer > 2)
        {
            anim.Play("Flap");
            flapTimer = 0;
        }

        if (spawnTimer > 5)
        {
            anim.Play("Flap");
            flying = true;
            spawnTimer = 0;
        }
    }


}
