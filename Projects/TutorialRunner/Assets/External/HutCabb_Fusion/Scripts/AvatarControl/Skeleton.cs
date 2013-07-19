using UnityEngine;
using System.Collections;
using System;

public class Skeleton : MonoBehaviour {

	public Transform Head;
	public Transform Neck;
	public Transform Torso;
	public Transform Waist;

	public Transform LeftCollar;
	public Transform LeftShoulder;
	public Transform LeftElbow;
	public Transform LeftWrist;
	public Transform LeftHand;
	public Transform LeftFingertip;

	public Transform RightCollar;
	public Transform RightShoulder;
	public Transform RightElbow;
	public Transform RightWrist;
	public Transform RightHand;
	public Transform RightFingertip;

	public Transform LeftHip;
	public Transform LeftKnee;
	public Transform LeftAnkle;
	public Transform LeftFoot;

	public Transform RightHip;
	public Transform RightKnee;
	public Transform RightAnkle;
	public Transform RightFoot;
	
	public float RotationDamping = 15f;
	
	protected Transform[] transforms;
	protected Quaternion[] initialRotations;
	
	protected Quaternion [] additionalRotation;
	
	//public bool IsUserModel = false;
	//public bool IsMaleModel = false;
	
	void Awake()
	{
		int jointCount = Enum.GetNames(typeof(IxoSkeletonJoint)).Length + 1; // Enum starts at 1
		
		transforms = new Transform[jointCount];
		initialRotations = new Quaternion[jointCount];
		additionalRotation = new Quaternion[jointCount];
		
		transforms[(int)IxoSkeletonJoint.Head] = Head;
		transforms[(int)IxoSkeletonJoint.Neck] = Neck;
		transforms[(int)IxoSkeletonJoint.Torso] = Torso;
		transforms[(int)IxoSkeletonJoint.Waist] = Waist;
		transforms[(int)IxoSkeletonJoint.LeftCollar] = LeftCollar;
		transforms[(int)IxoSkeletonJoint.LeftShoulder] = LeftShoulder;
		transforms[(int)IxoSkeletonJoint.LeftElbow] = LeftElbow;
		transforms[(int)IxoSkeletonJoint.LeftWrist] = LeftWrist;
		transforms[(int)IxoSkeletonJoint.LeftHand] = LeftHand;
		transforms[(int)IxoSkeletonJoint.LeftFingertip] = LeftFingertip;
		transforms[(int)IxoSkeletonJoint.RightCollar] = RightCollar;
		transforms[(int)IxoSkeletonJoint.RightShoulder] = RightShoulder;
		transforms[(int)IxoSkeletonJoint.RightElbow] = RightElbow;
		transforms[(int)IxoSkeletonJoint.RightWrist] = RightWrist;
		transforms[(int)IxoSkeletonJoint.RightHand] = RightHand;
		transforms[(int)IxoSkeletonJoint.RightFingertip] = RightFingertip;
		transforms[(int)IxoSkeletonJoint.LeftHip] = LeftHip;
		transforms[(int)IxoSkeletonJoint.LeftKnee] = LeftKnee;
		transforms[(int)IxoSkeletonJoint.LeftAnkle] = LeftAnkle;
		transforms[(int)IxoSkeletonJoint.LeftFoot] = LeftFoot;
		transforms[(int)IxoSkeletonJoint.RightHip] = RightHip;
		transforms[(int)IxoSkeletonJoint.RightKnee] = RightKnee;
	    transforms[(int)IxoSkeletonJoint.RightAnkle] = RightAnkle;
		transforms[(int)IxoSkeletonJoint.RightFoot] = RightFoot;
    }
	
	void Start() 
    {
		// save all initial rotations
		// NOTE: Assumes skeleton model is in rest pose since all rotations are relative to that pose
		foreach (IxoSkeletonJoint j in Enum.GetValues(typeof(IxoSkeletonJoint)))
		{
			if (transforms[(int)j])
			{
				// we will store the relative rotation of each joint from the gameobject rotation
				// we need this since we will be setting the joint's rotation (not localRotation) but we 
				// still want the rotations to be relative to our game object
				initialRotations[(int)j] = Quaternion.Inverse(transform.rotation) * transforms[(int)j].rotation;
			}
		}
		
		// start out in calibration pose
		RotateToCalibrationRestPose();
	}
	
	public void RotateToCalibrationRestPose()
	{
		for(int i = 0; i < additionalRotation.Length; i++)
		{
			additionalRotation[i] = Quaternion.identity;
		}
		
		foreach (IxoSkeletonJoint j in Enum.GetValues(typeof(IxoSkeletonJoint)))
		{
			if (null != transforms[(int)j])
			{
				transforms[(int)j].rotation = transform.rotation * initialRotations[(int)j];
			}
		}
	}
	
	public void RotateToElbowOutPose()
	{
		RotateToCalibrationRestPose();
		
		if(UserDataUtil.isRightHand)
		{
			Quaternion rootRotation = transform.localRotation;
			transform.localRotation = Quaternion.identity;
			RightElbow.transform.Rotate(new Vector3(-80f,0f,0f),Space.World);
			transform.localRotation = rootRotation;
			additionalRotation[(int)IxoSkeletonJoint.RightElbow] = Quaternion.Euler(new Vector3(-80f,0f,0f));
			additionalRotation[(int)IxoSkeletonJoint.RightWrist] = Quaternion.Euler(new Vector3(-80f,0f,0f));
		}
		else
		{
			Quaternion rootRotation = transform.localRotation;
			transform.localRotation = Quaternion.identity;
			LeftElbow.transform.Rotate(new Vector3(-80f,0f,0f),Space.World);
			transform.localRotation = rootRotation;
			additionalRotation[(int)IxoSkeletonJoint.LeftElbow] = Quaternion.Euler(new Vector3(-80f,0f,0f));
			additionalRotation[(int)IxoSkeletonJoint.LeftWrist] = Quaternion.Euler(new Vector3(-80f,0f,0f));
		}
	}
	
	public void RotateToShoulderOutFwdPose()
	{
		RotateToCalibrationRestPose();
		
		if(UserDataUtil.isRightHand)
		{
			Quaternion rootRotation = transform.localRotation;
			transform.localRotation = Quaternion.identity;
			RightShoulder.transform.Rotate(new Vector3(-80f,0f,0f),Space.World);
			transform.localRotation = rootRotation;
			additionalRotation[(int)IxoSkeletonJoint.RightShoulder] = Quaternion.Euler(new Vector3(-80f,0f,0f));
			additionalRotation[(int)IxoSkeletonJoint.RightElbow] = Quaternion.Euler(new Vector3(-80f,0f,0f));
			additionalRotation[(int)IxoSkeletonJoint.RightWrist] = Quaternion.Euler(new Vector3(-80f,0f,0f));
		}
		else
		{
			Quaternion rootRotation = transform.localRotation;
			transform.localRotation = Quaternion.identity;
			LeftShoulder.transform.Rotate(new Vector3(-80f,0f,0f),Space.World);
			transform.localRotation = rootRotation;
			additionalRotation[(int)IxoSkeletonJoint.LeftShoulder] = Quaternion.Euler(new Vector3(-80f,0f,0f));
			additionalRotation[(int)IxoSkeletonJoint.LeftElbow] = Quaternion.Euler(new Vector3(-80f,0f,0f));
			additionalRotation[(int)IxoSkeletonJoint.LeftWrist] = Quaternion.Euler(new Vector3(-80f,0f,0f));
		}
	}
	
	public void RotateToShoulderOutSidePose()
	{
		RotateToCalibrationRestPose();
		
		if(UserDataUtil.isRightHand)
		{
			Quaternion rootRotation = transform.localRotation;
			transform.localRotation = Quaternion.identity;
			RightShoulder.transform.Rotate(new Vector3(0f,0f,90f),Space.World);
			transform.localRotation = rootRotation;
			additionalRotation[(int)IxoSkeletonJoint.RightShoulder] = Quaternion.Euler(new Vector3(0f,0f,90f));
			additionalRotation[(int)IxoSkeletonJoint.RightElbow] = Quaternion.Euler(new Vector3(0f,0f,90f));
			additionalRotation[(int)IxoSkeletonJoint.RightWrist] = Quaternion.Euler(new Vector3(0f,0f,90f));
		}
		else
		{
			Quaternion rootRotation = transform.localRotation;
			transform.localRotation = Quaternion.identity;
			LeftShoulder.transform.Rotate(new Vector3(0f,0f,-90f),Space.World);
			transform.localRotation = rootRotation;
			additionalRotation[(int)IxoSkeletonJoint.LeftShoulder] = Quaternion.Euler(new Vector3(0f,0f,-90f));
			additionalRotation[(int)IxoSkeletonJoint.LeftElbow] = Quaternion.Euler(new Vector3(0f,0f,-90f));
			additionalRotation[(int)IxoSkeletonJoint.LeftWrist] = Quaternion.Euler(new Vector3(0f,0f,-90f));
		}
	}
	
	public void UpdateJointBasedOnCalibPose(IxoSkeletonJoint joint, Quaternion rotation)
	{
		Quaternion newRotation = transform.rotation *( additionalRotation[(int)joint] * (rotation * (initialRotations[(int)joint])));
		
		if(RotationDamping <= 0)
			transforms[(int)joint].rotation = newRotation;
		else
			transforms[(int)joint].rotation = Quaternion.Slerp(transforms[(int)joint].rotation, newRotation, Time.deltaTime * RotationDamping);
	}
	
//	public void UpdateJoint(IxoSkeletonJoint joint, Quaternion rotation)
//	{
//		// Z coordinate in OpenNI is opposite from Unity
//		// Convert the OpenNI 3x3 rotation matrix to unity quaternion while reversing the Z axis
//		//Vector3 worldZVec = new Vector3(-skelTrans.Orientation.Z1, -skelTrans.Orientation.Z2, skelTrans.Orientation.Z3);
//		//Vector3 worldYVec = new Vector3(skelTrans.Orientation.Y1, skelTrans.Orientation.Y2, -skelTrans.Orientation.Y3);
//		//Quaternion jointRotation = Quaternion.LookRotation(worldZVec, worldYVec);
//		Quaternion newRotation = transform.rotation * rotation;
//		if(RotationDamping <= 0)
//			transforms[(int)joint].rotation = newRotation;
//		else
//			transforms[(int)joint].rotation = Quaternion.Slerp(transforms[(int)joint].rotation, newRotation, Time.deltaTime * RotationDamping);
//
//		/*
//		if(joint == IxoSkeletonJoint.RightShoulder)
//		{
//			Quaternion rotation = Quaternion.Slerp(initialRotations[(int)IxoSkeletonJoint.RightCollar],RightShoulder.localRotation,0.25f);
//			RightCollar.localRotation = rotation;
//			rotation = Quaternion.Slerp(initialRotations[(int)IxoSkeletonJoint.RightShoulder],RightShoulder.localRotation,0.7f);
//			RightShoulder.localRotation = rotation;
//		}
//		if(joint == IxoSkeletonJoint.LeftShoulder)
//		{
//			Quaternion rotation = Quaternion.Slerp(initialRotations[(int)IxoSkeletonJoint.LeftCollar],LeftShoulder.localRotation,0.25f);
//			LeftCollar.localRotation = rotation;
//			rotation = Quaternion.Slerp(initialRotations[(int)IxoSkeletonJoint.LeftShoulder],LeftShoulder.localRotation,0.7f);
//			LeftShoulder.localRotation = rotation;
//		}
//		*/
//	}
	
	public Quaternion GetJointRotationFromCalibPose(IxoSkeletonJoint joint)
	{
		return Quaternion.Inverse(initialRotations[(int)joint]) * (Quaternion.Inverse(transform.rotation) * transforms[(int)joint].rotation);
	}

	/*
	public void UpdateJoint(SkeletonJoint joint, SkeletonJointTransformation skelTrans)
	{
		// save raw data
		jointData[(int)joint] = skelTrans;
		
		// make sure something is hooked up to this joint
		if (!transforms[(int)joint])
		{
			return;
		}
		
		// modify orientation (if confidence is high enough)
        if (UpdateOrientation && skelTrans.Orientation.Confidence > 0.5)
        {
			// Z coordinate in OpenNI is opposite from Unity
			// Convert the OpenNI 3x3 rotation matrix to unity quaternion while reversing the Z axis
			Vector3 worldZVec = new Vector3(-skelTrans.Orientation.Z1, -skelTrans.Orientation.Z2, skelTrans.Orientation.Z3);
			Vector3 worldYVec = new Vector3(skelTrans.Orientation.Y1, skelTrans.Orientation.Y2, -skelTrans.Orientation.Y3);
			Quaternion jointRotation = Quaternion.LookRotation(worldZVec, worldYVec);
			Quaternion newRotation = transform.rotation * jointRotation * initialRotations[(int)joint];

			transforms[(int)joint].rotation = Quaternion.Slerp(transforms[(int)joint].rotation, newRotation, Time.deltaTime * RotationDamping);
        }
	}
	*/
}
