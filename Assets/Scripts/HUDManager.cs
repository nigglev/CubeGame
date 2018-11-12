using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//public class HUDManager : MonoBehaviour {

//    public Text scoreLabel;

//    // Use this for initialization
//    void Start () {
//        CWorld.Instance.SetHUDManager(this);
//        Refresh();
//	}
	
//	// Update is called once per frame
//	void Update () {
		
//	}

//    public void Refresh()
//    {
//        scoreLabel.text = "Score: " + CWorld.Instance.Score;
//    }

//}


public class CHUDManager
{
    GameObject _canvas;
    Text _score_label;
    Text _velocity_label;

    public CHUDManager()
    {
        _canvas = GameObject.Find("Canvas");
        //_score_label = Object.FindObjectOfType<Text>();
        Transform scoretr = _canvas.transform.Find("Score Label");
        Transform velocitytr = _canvas.transform.Find("Velocity");
        _score_label = scoretr.GetComponent<Text>();
        _velocity_label = velocitytr.GetComponent<Text>();
    }

    public void Refresh()
    {
        _score_label.text = "Score: " + CWorld.Instance.Score;
    }

    public void RefreshVelocity(Vector3 in_vel)
    {
        _velocity_label.text = "Velocity: " + in_vel;
    }

}
