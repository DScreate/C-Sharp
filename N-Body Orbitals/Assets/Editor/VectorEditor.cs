using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Vector3))]
public class VectorEditor : Editor {
	
	
	
	public void OnDrawGizmos()
	{
		GravityAttractor attractor = (GravityAttractor) target;

		Gizmos.DrawRay(attractor.transform.position, attractor.Velocity);
	}

	 
}
