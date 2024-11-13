using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class particle_controller : MonoBehaviour {
	

	public List<Transform> targets = new List<Transform>(); 
	public Transform startpos;		//starting position
	public GameObject preftracer;	//prefab of a tracer
	public List<GameObject> PrefOfExpl = new List<GameObject>();	//list of prefabs for explosions
	public float speed=0.1f;		//tracer's speed
	public int rangeOfRandom=0;		//range of random explosion's expansion from target points. If = 0, then all the explosions will be in target points.
	GameObject tracer;				//object for instantiating tracer		
	float timeLerped = 0.0f;		//variables for lerp moving of a tracer
	float timeToLerp = 2; 			//lerp for two seconds.
    Vector3 startingPosition;		//temp variable for starting position
	private Vector3[] arrayOfTarget;	//array of a target points
	private GameObject[] arrayOfTracers;	//array of a tracers
	private GameObject[] arrayOfExplos;		//array of explosions
	private byte[] played;					//flag for knowing was the explosion completed
	private int amount=0; 					//amount of fireworks
	float timer=0;							//timer after explosion
	int currentexp=0;							//current type of explosion
	

   	void GenerTargets()
	{
		for (int i=0; i<=amount-1; i++) //creating array of final targets positions
		{
			arrayOfTarget[i]=targets[i].transform.position;
			arrayOfTarget[i]=new Vector3(arrayOfTarget[i].x+Random.Range(-(rangeOfRandom),(rangeOfRandom)),arrayOfTarget[i].y+Random.Range(-(rangeOfRandom),(rangeOfRandom)),arrayOfTarget[i].z+Random.Range(-(rangeOfRandom),(rangeOfRandom)));
		}
	}  
     
    void Start()
	{
		amount=targets.Count;
		played=new byte[amount];									
		arrayOfExplos=new GameObject[amount];
		arrayOfTracers=new GameObject[amount];
		arrayOfTarget=new Vector3[amount];
    	startingPosition = startpos.position;
		GenerTargets();
			
		
   }
    
	
	void Update()
	{
	if (arrayOfTracers[0]==null)
		{
		
		for (int i=0; i<=(amount-1);i++)			//creating the tracers for each of a firework in starting positions
			{
				arrayOfTracers[i] = Instantiate(preftracer,startpos.position,startpos.rotation) as GameObject;
				played[i]=1;
			}	
		}
	for (int j=0; j<=(amount-1); j++)
		{
			if (arrayOfTracers[j].transform.position.y<arrayOfTarget[j].y)							//if tracer didn't reach the target point (the y variable is the main) You can't move the firework from top point to low. Fireworks always raise up! :)
			{
				Vector3 relativePos = arrayOfTarget[j] - arrayOfTracers[j].transform.position;		//generating, rotating and moving tracer to target
			    Quaternion rotation = Quaternion.LookRotation(relativePos);
			    arrayOfTracers[j].transform.rotation = rotation;
				timeLerped += Time.deltaTime*speed;
			    arrayOfTracers[j].transform.position = Vector3.Lerp(startingPosition, arrayOfTarget[j], timeLerped / timeToLerp);  
			}
				else 		//if tracers reached the point
				{
				
						arrayOfTracers[j].GetComponent<ParticleSystem>().Stop();		//stop emitting tracer's particles
						if (played[j]==1)								//instantiating explosion only 1 time
						{
							arrayOfExplos[j] = (GameObject)Instantiate(PrefOfExpl[currentexp],arrayOfTarget[j],targets[0].rotation);
							played[j]=0;
					 	}
				}
		}
		if (played[(amount-1)]==0)			//if all the explosions were made 
		{
		timer+=Time.deltaTime;	//start to count the timer
				
			if (timer>=4)
			{
			startingPosition=startpos.position;				//all the variables to the starting values
			for (int k=0;k<=(amount-1);k++)
			{
				played[k]=1;
				Destroy(arrayOfExplos[k]);
				Destroy (arrayOfTracers[k]);  //destroing emitted explosion particles and old tracer
				
			}
			timeLerped = 0.0f;
			timeToLerp = 2;	
			timer=0;
			GenerTargets();
			currentexp+=1;
			if (currentexp==PrefOfExpl.Count)
				{
				currentexp=0;
				}
			}
		}
		
				
	}
}
