using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereTargeting : MonoBehaviour {

    Vector3 _target_position;
    float speed = 1f;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        _target_position = CWorld.Instance.GetPlayerPosition();
        Vector3 targetDir = _target_position - transform.position;
        float step = speed * Time.deltaTime;
        Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDir);

    }
}
