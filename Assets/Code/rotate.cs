using UnityEngine;
using System.Collections;

public class rotate : MonoBehaviour {

	// Update is called once per frame
	void FixedUpdate () {
        transform.Rotate(0.01f, 0, 0);
	}
}
