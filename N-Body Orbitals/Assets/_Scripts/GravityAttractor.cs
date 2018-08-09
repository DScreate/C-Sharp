using System;
using System.Collections.Generic;
using UnityEngine;

public class GravityAttractor : MonoBehaviour
{

	//private float _scaleFactor;
	public float PredictionTime;
	
	const float G = 667.4f;


	public bool OrbitManaged;


	public Rigidbody rb;

	public Vector3 Velocity;

	public int TraceCount;

	public AttractorController Controller;

	public bool LoggingEnabled;

	
	//public Vector3 Position;

	//public Dictionary<GravityAttractor, Vector3> PositionMap = new Dictionary<GravityAttractor, Vector3>();
	
	private Transform _transformCache;
	private List<Vector3> _positions;
	private bool _filled;

	private Vector3 _momentum;
	private Vector3 _position;
	private Vector3 _last;

	private int _index;

	void Start()
	{
		if (OrbitManaged)
		{
			Vector3 r = rb.position - Controller.MainRigidbody.position;
			Velocity = new Vector3((float) QuickCalc(r.x), (float) QuickCalc(r.y), (float) QuickCalc(r.z));
		}
		rb.velocity += Velocity;
		_index = 0;
		_filled = false;
		
	}

	double QuickCalc(float r)
	{
		return Math.Sqrt((G * Controller.MainRigidbody.mass * rb.mass) / r);
	}

	void Awake()
	{
		_transformCache = transform;
		_positions = new List<Vector3>(TraceCount);

	}

	
	public void CalculateOrbit ()
	{
		if (LoggingEnabled)
		{
			if (_index >= TraceCount)
			{
				_index = 0;

				_positions[_index] = _transformCache.position;
				_filled = true;

			}
			else if(_index < TraceCount && !_filled)
			{
				_positions.Add(_transformCache.position);
			}
			else
			{
				_positions[_index] = transform.position;
			}
		}


		foreach (GravityAttractor attractor in AttractorController.Attractors)
		{
			if (attractor != this)
				Attract(attractor);
		}
		
		if (LoggingEnabled)
		{
			for (int i = _index - 1; i >= 0; i--)
			{
				Debug.DrawLine(_positions[i+1], _positions[i], i % 2 == 0 ? Color.blue : Color.green);
			}
			_index++;
		}


		Velocity = rb.velocity;
	}
	

	void OnEnable ()
	{
		if (AttractorController.Attractors == null)
			AttractorController.Attractors = new List<GravityAttractor>();

		AttractorController.Attractors.Add(this);
		
		if (AttractorController.Positions == null)
			AttractorController.Positions = new Dictionary<GravityAttractor, Vector3>();

		if (AttractorController.Positions.ContainsKey(this))
			AttractorController.Positions[this] = rb.position;
		else
		{
			AttractorController.Positions.Add(this, rb.position);
		}
		//_scaleFactor = Controller.ScaleFactor;


	}

	void OnDisable ()
	{
		AttractorController.Attractors.Remove(this);
	}

	void Attract (GravityAttractor objToAttract)
	{
		Rigidbody rbToAttract = objToAttract.rb;

		Vector3 direction = rb.position - rbToAttract.position;
		float distance = direction.magnitude;

		if (distance == 0f)
			return;

		float forceMagnitude = (G * (rb.mass * rbToAttract.mass) / Mathf.Pow(distance, 2));
		Vector3 force = direction.normalized * forceMagnitude;


		
		rbToAttract.AddForce(force);
		
		//Debug.DrawRay(transformCache.position, transformCache.position + force, Color.red);

	}
	
	void OnCollisionEnter(Collision collision)
	{
		foreach (ContactPoint contact in collision.contacts)
		{
			//Debug.DrawRay(contact.point, contact.normal, Color.white);
			Debug.Log("Boom");
		}
		if (collision.relativeVelocity.magnitude > 2)
			Debug.Log("Big Collision");
		if(collision.rigidbody.mass > this.rb.mass)
			Destroy(this);
	}
	
	public void OnDestroy()
	{
		AttractorController.Attractors.Remove(this);
	}

	/*
	public void OnDrawGizmos()
	{
		//_scaleFactor = Controller.ScaleFactor;

		if (AttractorController.Attractors == null)
			AttractorController.Attractors = new List<GravityAttractor>();

		if(!AttractorController.Attractors.Contains(this))
			AttractorController.Attractors.Add(this);
		
		if (AttractorController.Positions.ContainsKey(this))
			AttractorController.Positions[this] = rb.position;
		else
		{
			AttractorController.Positions.Add(this, rb.position);
		}

		Gizmos.color = Color.red;
		Gizmos.DrawRay(transform.position, Velocity);

	
		_momentum = Velocity;
		_position = gameObject.transform.position;
		_last = gameObject.transform.position;
		
		
		Gizmos.color = Color.cyan;
		for (int i = 0; i < (int) (PredictionTime * 60); i++)
		{
			foreach (GravityAttractor attractor in AttractorController.Attractors)
			{
				if (attractor != this)
				{
					Rigidbody rbToAttract = attractor.rb;

					Vector3 direction = _position - AttractorController.Positions[attractor];
					float distance = direction.magnitude;

					if (distance == 0f)
						return;

					float forceMagnitude = (G * (rb.mass * rbToAttract.mass) / Mathf.Pow(distance, 2));
					Vector3 force = -direction.normalized * forceMagnitude;

					_momentum += force;

				}


			}
			_position += _momentum;
			Gizmos.DrawLine(_last, _position);
			_last = _position;
			AttractorController.Positions[this] = _position;

		}
	}
	*/
	

}
