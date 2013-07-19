using UnityEngine;
using System.Collections;

public class SkeletonController : MonoBehaviour {
//	
//	public Skeleton SkeletonInstance;
//	
//	// Use this for initialization
//	void Start () {
//	
//	}
//	
//	// Update is called once per frame
//	void Update () {
//	/*
//		foreach (SkeletonJoint joint in Enum.GetValues(typeof(SkeletonJoint)))
//		{
//			if (skeletonCapbility.IsJointAvailable(joint))
//			{
//				skelTrans = skeletonCapbility.GetSkeletonJoint(userId, joint);
//				skeleton.UpdateJoint(joint, skelTrans);
//			}
//		}
//		*/
//	}
//	
//	public void PlayTrainingActionOnDemoSkeleton(TrainingAction trainingAction)
//	{
//		StartCoroutine(PlayTrainingActionOnDemoSkeletonCoroutine(trainingAction));
//	}
//	
//	private IEnumerator PlayTrainingActionOnDemoSkeletonCoroutine(TrainingAction trainingAction)
//	{
//		float timePassed = 0f;
//		float animationTime = 20f;
//		float animationTimeHalved = animationTime/2;
//		while(true)
//		{
//			if(timePassed <= animationTimeHalved)
//			{
//				float t = timePassed/animationTimeHalved;
//				SkeletonInstance.UpdateJointBasedOnCalibPose(IxoSkeletonJoint.Torso, Quaternion.Slerp(trainingAction.RightBeginQuaternions[0],trainingAction.RightMiddleQuaternions[0], t));
//				SkeletonInstance.UpdateJointBasedOnCalibPose(IxoSkeletonJoint.RightShoulder, Quaternion.Slerp(trainingAction.RightBeginQuaternions[1],trainingAction.RightMiddleQuaternions[1], t));
//				SkeletonInstance.UpdateJointBasedOnCalibPose(IxoSkeletonJoint.RightElbow, Quaternion.Slerp(trainingAction.RightBeginQuaternions[2],trainingAction.RightMiddleQuaternions[2], t));
//				SkeletonInstance.UpdateJointBasedOnCalibPose(IxoSkeletonJoint.RightWrist, Quaternion.Slerp(trainingAction.RightBeginQuaternions[3],trainingAction.RightMiddleQuaternions[3], t));
//			}
//			else
//			{
//				float t = (timePassed-animationTimeHalved) / animationTimeHalved;
//				SkeletonInstance.UpdateJointBasedOnCalibPose(IxoSkeletonJoint.Torso, Quaternion.Slerp(trainingAction.RightMiddleQuaternions[0],trainingAction.RightEndQuaternions[0], t));
//				SkeletonInstance.UpdateJointBasedOnCalibPose(IxoSkeletonJoint.RightShoulder, Quaternion.Slerp(trainingAction.RightMiddleQuaternions[1],trainingAction.RightEndQuaternions[1], t));
//				SkeletonInstance.UpdateJointBasedOnCalibPose(IxoSkeletonJoint.RightElbow, Quaternion.Slerp(trainingAction.RightMiddleQuaternions[2],trainingAction.RightEndQuaternions[2], t));
//				SkeletonInstance.UpdateJointBasedOnCalibPose(IxoSkeletonJoint.RightWrist, Quaternion.Slerp(trainingAction.RightMiddleQuaternions[3],trainingAction.RightEndQuaternions[3], t));
//			}
//			if(timePassed >= animationTime)
//				yield break;
//			
//			timePassed += Time.deltaTime;
//			yield return null;
//		}
//	}
}
