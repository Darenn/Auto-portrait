using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuseumCamera : MonoBehaviour {

    public Transform Target;

    public float RotationSpeed;

	void Update () {
        if (Target != null)
        {
            transform.LookAt(Target);
            transform.Translate(Vector3.right * RotationSpeed * Time.deltaTime);
        }
	}
}
