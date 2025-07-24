using System.Collections.Generic;
using UnityEngine;

public class SingleObjectPrefabPool
{
	private List<GameObject> pool;

	private string prefabPath;

	public SingleObjectPrefabPool(string prefabPath)
	{
		this.prefabPath = prefabPath;
		pool = new List<GameObject>();
	}

	public GameObject GetObject()
	{
		GameObject result;
		if (pool.Count > 0)
		{
			result = pool[0];
			pool.RemoveAt(0);
		}
		else
		{
			result = (Object.Instantiate(Resources.Load(prefabPath)) as GameObject);
		}
		return result;
	}

	public void AddObject(GameObject oldObject)
	{
		pool.Add(oldObject);
	}

	public void Clear()
	{
		foreach (GameObject item in pool)
		{
			UnityEngine.Object.Destroy(item);
		}
		pool.Clear();
	}
}
