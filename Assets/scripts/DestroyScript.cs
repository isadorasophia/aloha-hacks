using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyScript : MonoBehaviour

{
	[SerializeField] private double DistanceThreshold;
	[SerializeField] private string DistanceFromTag;
    private GameObject MarkerObject;


	// Start is called before the first frame update
	void Start()
    {
        MarkerObject = GameObject.FindGameObjectWithTag(DistanceFromTag);
    }

    // Update is called once per frame
    void Update()
    {
		
		if (Vector3.Distance(MarkerObject.transform.position, transform.position) >= DistanceThreshold)
			Destroy(gameObject);
    }
}
