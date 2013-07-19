using UnityEngine;
using System.Collections;

public class I2rSkeletonController : IxoMonoSingleton<I2rSkeletonController> {
	
	public bool [] EnabledSensors {
		get { return _enabledSensors; }
	}
	private bool [] _enabledSensors = new bool[4];
	
	public Skeleton userSkeleton;
	
	private bool skeletonCalibrated = false;
	private I2rSensor _i2rSensor;
	//private TrainingActionsSelectionWindow _actionsSelectionWindow;
	
	private Quaternion [] initialSensorRotation = new Quaternion[4];
	
	override protected void Awake()
	{
		base.Awake();
		
		//grabbing i2r sensor singleton instance 
		_i2rSensor = I2rSensor.Instance;
		
		/// START Non-Engine Implementation
		/*if(_actionsSelectionWindow == null)
			_actionsSelectionWindow = TrainingActionsSelectionWindow.Instance;
		
		_actionsSelectionWindow.OnCallibrateBtnPressed += OnCallibrateBtnPressed;
		_actionsSelectionWindow.OnDisconnectBtnPressed += OnDisconnectBtnPressed;
		*//// END Non-Engine Implementation
		
		for(int i = 0; i < _enabledSensors.Length; i++)
			_enabledSensors[i] = true;
	}
	
	public void CalibrateAndResetSensor()
	{
		OnCallibrateBtnPressed();
	}
	
	void OnCallibrateBtnPressed()
	{
		for(int i = 0; i < 4; i++)
			initialSensorRotation[i] = _i2rSensor.GetQuaternion(i);
		
		skeletonCalibrated = true;
	}
	
	void OnDisconnectBtnPressed()
	{
		skeletonCalibrated = false;
	}
	
	public void DisconnectSkeleton()
	{
		skeletonCalibrated = false;
	}
		
	void Update()
	{
		if(skeletonCalibrated == false)
			return;
		
		int rightPositive = UserDataUtil.isRightHand ? 1 : -1;
		if(UserDataUtil.isRightHand)
		{
			if(_enabledSensors[0] && _i2rSensor.IsReceiving(0))
			{
				Quaternion q = FromToRotation(initialSensorRotation[0] * Quaternion.Euler(0f,rightPositive * 90f,0f),_i2rSensor.GetQuaternion(0) * Quaternion.Euler(0f,rightPositive * 90f,0f));
				userSkeleton.UpdateJointBasedOnCalibPose(IxoSkeletonJoint.RightWrist, q);
			}
			if(_enabledSensors[1] && _i2rSensor.IsReceiving(1))
			{
				Quaternion q = FromToRotation(initialSensorRotation[1] * Quaternion.Euler(0f,rightPositive * 90f,0f),_i2rSensor.GetQuaternion(1) * Quaternion.Euler(0f,rightPositive * 90f,0f));
				userSkeleton.UpdateJointBasedOnCalibPose(IxoSkeletonJoint.RightElbow, q);
			}
			
			if(_enabledSensors[2] && _i2rSensor.IsReceiving(2))
			{
				Quaternion q = FromToRotation(initialSensorRotation[2] * Quaternion.Euler(0f,rightPositive * 90f,0f),_i2rSensor.GetQuaternion(2) * Quaternion.Euler(0f,rightPositive * 90f,0f));
				userSkeleton.UpdateJointBasedOnCalibPose(IxoSkeletonJoint.RightShoulder, q);
			}
		}
		else
		{
			if(_enabledSensors[0] && _i2rSensor.IsReceiving(0))
			{
				Quaternion q = FromToRotation(initialSensorRotation[0] * Quaternion.Euler(0f,rightPositive * 90f,0f),_i2rSensor.GetQuaternion(0) * Quaternion.Euler(0f,rightPositive * 90f,0f));
				userSkeleton.UpdateJointBasedOnCalibPose(IxoSkeletonJoint.LeftWrist, q);
			}
			if(_enabledSensors[1] && _i2rSensor.IsReceiving(1))
			{
				Quaternion q = FromToRotation(initialSensorRotation[1] * Quaternion.Euler(0f,rightPositive * 90f,0f),_i2rSensor.GetQuaternion(1) * Quaternion.Euler(0f,rightPositive * 90f,0f));
				userSkeleton.UpdateJointBasedOnCalibPose(IxoSkeletonJoint.LeftElbow, q);
			}
			
			if(_enabledSensors[2] && _i2rSensor.IsReceiving(2))
			{
				Quaternion q = FromToRotation(initialSensorRotation[2] * Quaternion.Euler(0f,rightPositive * 90f,0f),_i2rSensor.GetQuaternion(2) * Quaternion.Euler(0f,rightPositive * 90f,0f));
				userSkeleton.UpdateJointBasedOnCalibPose(IxoSkeletonJoint.LeftShoulder, q);
			}
		}
	
		if(_enabledSensors[3] && _i2rSensor.IsReceiving(3))
			userSkeleton.UpdateJointBasedOnCalibPose(IxoSkeletonJoint.Torso, FromToRotation(initialSensorRotation[3], _i2rSensor.GetQuaternion(3)));
	}
				
	public static Quaternion FromToRotation(Quaternion a, Quaternion b) {
		return Quaternion.Inverse(a) * b;
	}
}
