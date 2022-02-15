using UnityEngine;

public class Rotate : MonoBehaviour {
    public Vector3 speed;
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(speed);
	}
}