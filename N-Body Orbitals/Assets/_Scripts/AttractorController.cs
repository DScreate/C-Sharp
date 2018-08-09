using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttractorController : MonoBehaviour
{

	public static List<GravityAttractor> Attractors;
	public static Dictionary<GravityAttractor, Vector3> Positions;

	public float ScaleFactor;
	
	public bool PredictionOnly;

	public Rigidbody MainRigidbody;

	private void OnEnable()
	{
		if (Attractors == null)
			Attractors = new List<GravityAttractor>();
	}

	private void FixedUpdate()
	{
		foreach (GravityAttractor attractor in Attractors)
		{
			attractor.CalculateOrbit();
		}
	}
}
