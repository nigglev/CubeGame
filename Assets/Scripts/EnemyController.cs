using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {


    public float rangeX = 0.5f;
    public float speed = 1f;
    public float direction = 1f;
    

    // Use this for initialization
    void Start () {
        CWorld.Instance.SetEnemy(this);
        SRVector pos = new SRVector(transform);
        CWorld.Instance.SetEnemyStartPosition(pos);
    }
	
	// Update is called once per frame
	void Update () {

        //float movementX = direction * speed * Time.deltaTime;
        //float newX = transform.position.x + movementX;

        //if (Mathf.Abs(newX - CWorld.Instance.EnemyStartPosition.position.x) > rangeX)
        //{
            
        //    direction *= -1;
        //}
        //else
        //{   
        //    transform.Translate(new Vector3(newX, 0, 0));
        //}
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "CubePlayer")
        {
            Debug.Log("Game Over");
            CWorld.Instance.Restart();  
        }
    }

}
