using UnityEngine;
using System.Collections;

public class ModelGenderController : IxoMonoSingleton<ModelGenderController> {

	public Skeleton maleModelPrefab;
	public Skeleton femaleModelPrefab;
	
	private Skeleton _skeletonInstance;
	
	public Skeleton SkeletonInstance
	{
		get { 
			if(_skeletonInstance == null)
			{
				if(UserDataUtil.isMale)
				{
					_skeletonInstance = Instantiate(maleModelPrefab) as Skeleton;
					_skeletonInstance.transform.position = new Vector3(0f,1.2f,0f);
				}
				else
				{
					_skeletonInstance = Instantiate(femaleModelPrefab) as Skeleton;
					_skeletonInstance.transform.position = new Vector3(0f,1.0f,0f);
				}
			}
			return _skeletonInstance;
		}
	}
}
