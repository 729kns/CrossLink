using System.Collections;
using UnityEngine;

public class DropEmitter : MonoBehaviour {
	public GameObject turningLine;
	public static bool stop;
	private float timeCount;

	// Use this for initialization
	void Start () {
		timeCount = Random.Range (0.3f, 0.7f);
	}
	
	// Update is called once per frame
	void Update () {
		timeCount -= Time.deltaTime;
		if (timeCount <= 0) {
            Instantiate(turningLine, transform);
			timeCount = Random.Range (0.3f, 0.7f);
		}
	}

    public void StopEmit()
    {
        stop = true;
        enabled = false;
    }

    public void StartEmit()
    {
        stop = false;
    }
}
