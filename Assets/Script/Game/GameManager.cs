using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	private static GameManager instance;
	public GoundBackItem[,] GoundBackItemArray2D;

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

	/// <summary>
	/// ��ʼ��һ��2ά����
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	public void SetGoundBack(int width, int height)
	{
		GoundBackItemArray2D = new GoundBackItem[width, height];
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				GoundBackItemArray2D[i, j] = new GoundBackItem(i,j,$"{i},{j}");
			}
		}
	}

	/// <summary>
	/// ����ѵ��߼�
	/// </summary>
	public void CalculateElimination(int x, int y) {
		if(GoundBackItemArray2D!=null){
		}
	}




	//���ұ�дһ��������Ϸ�����������߼�
}

