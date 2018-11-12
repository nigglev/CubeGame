using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinController : MonoBehaviour {

    public float rotationSpeed = 100f;
    public float _score = 10;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        
        float angle = rotationSpeed * Time.deltaTime;
        transform.Rotate(Vector3.up * angle, Space.World);
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "CubePlayer")
        {
            Debug.Log("Grabbing coin " + GetInstanceID());
            CWorld.Instance.AddScore(_score);
            CWorld.Instance.AddToDeadCoins(this);
            gameObject.SetActive(false);
            CWorld.Instance.HUD.Refresh();
        }
    }

    public void OnRestartGame()
    {
        gameObject.SetActive(true);
    }
}
