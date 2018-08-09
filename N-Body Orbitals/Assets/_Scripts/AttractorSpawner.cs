using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttractorSpawner : MonoBehaviour {

	public float timeBetweenSpawns;

	public float spawnDistance;

	public GravityAttractor[] attractorPrefabs;
	
	float timeSinceLastSpawn;

	void FixedUpdate () {
		timeSinceLastSpawn += Time.deltaTime;
		if (timeSinceLastSpawn >= timeBetweenSpawns) {
			timeSinceLastSpawn -= timeBetweenSpawns;
			SpawnAttractor();
		}
	}
	
	void SpawnAttractor () {
		GravityAttractor prefab = attractorPrefabs[Random.Range(0, attractorPrefabs.Length)];
		GravityAttractor spawn = Instantiate<GravityAttractor>(prefab);
		spawn.transform.localPosition = Random.onUnitSphere * spawnDistance;
	}
}
