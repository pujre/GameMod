using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolObject : MonoBehaviour
{
	public virtual void OnCreate()
	{

	}

	//从对象池中取出调用
	public virtual void OnFetch()
	{

	}

	//回收时调用
	public virtual void OnRecycle()
	{

	}

	//销毁时调用
	public virtual void OnDestory()
	{

	}
}
