using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeagullSpawner : MonoBehaviour
{
    public GameObject seagullPrefab;
    private int spawnTimer = 0;
    private int flapTimer = 0;
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
        spawnTimer++;
        flapTimer++;

        if (flying)
            seagull.transform.Translate(0, 0.02f, 0.058f);

        if (flying && flapTimer > 150)
        {
            anim.Play("Flap");
            flapTimer = 0;
        }

        if (spawnTimer > 200)
        {
            anim.Play("Flap");
            flying = true;
            spawnTimer = 0;
        }
    }
}
