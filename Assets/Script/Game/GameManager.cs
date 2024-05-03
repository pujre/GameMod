using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{

	public GoundBackItem[,] GoundBackItemArray2D;
	private static GameManager instance;

	private void Awake()
	{
		instance = this;


	}
	public static GameManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new GameManager();
			}
			return instance;
		}
	}
	void Start()
	{

	}

	void Update()
	{

	}

	public void SetGoundBack(int x, int y)
	{
		GoundBackItemArray2D = new GoundBackItem[x, y];
		for (int i = 0; i < GoundBackItemArray2D.LongLength; i++)
		{

		}
	}

	public void EnterSurface(int x, int y)
	{

	}


}

