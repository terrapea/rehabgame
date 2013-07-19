using UnityEngine;
using System.Runtime.InteropServices;
using Serialization;

[System.Serializable]
[StructLayout(LayoutKind.Sequential)]
public struct FeatureInfo
{
	public double uMeanSpeed;  // upper arm mean speed
	public double fMeanSpeed;  // forearm mean speed
	public double wMeanSpeed;  // wrist mean speed
	public double vSpeed;  // var speed  wrist
	public double uMaxSpeed; // the max speed upper arm
	public double fMaxSpeed; // the max speed forearm
	public double wMaxSpeed; // the max speed wrist
	public double wRms;  // root mean square of acceleration hand
	public double fRms;  // root mean square of acceleration wrist 
	public double uRms;  // root mean square of acceleration forearm 
	public double wEntropy; // the entropy of the acceleration wrist
	public double fEntropy; // forearm
	public double uEntropy; // upper arm
	public double uMeanDis; // upper arm == elbow joint
	public double wMeanDis; //  mean distance of each point of the path from the theoretical path wrist.
	public double fMeanDis; // forearm
	public double wNPL;     // Normalized path length the ratio of: the sum of distance between 
	public double fNPL;    // forearm
	public double uNPL;    // upper arm
	public double uSmooth;  // the smoothness of the movement upper arm
	public double fSmooth;  // forearm
	public double wSmooth;  // wrist
	public double wPDE;     //  Position Directional Error (PDE),the PDE is the angle theta
	public double fPDE;
	public double uPDE;
	public double jnAng;   // the joint angle
	public double uwAccCorr; // the correlation coefficient between the upper arm and the wrist
	public double balance;  // the Balance represents the trunk movement
	public double uwPosCorrY; // the y-axis position corr  upper arm and wrist
	public double fwPosCorrY; // the y-axis position corr  forearm and wrist
	/*
	FeatureInfo()
	{
	    uMaxSpeed(0),fMaxSpeed(0),wMaxSpeed(0),
		vSpeed(0),
		wMeanSpeed(0),fMeanSpeed(0),uMeanSpeed(0),
		wRms(0), fRms(0),uRms(0),
		wEntropy(0), fEntropy(0), uEntropy(0),
		wMeanDis(0), fMeanDis(0), uMeanDis(0),
		wNPL(0), fNPL(0), uNPL(0),
		uSmooth(0), fSmooth(0),wSmooth(0),
		wPDE(0),fPDE(0),uPDE(0),
		jnAng(0),
		uwAccCorr(0),uwPosCorrY(0), fwPosCorrY(0),
		balance(0)
	}
	*/
	/*
	public void Reset()
	{
		uMaxSpeed = 0.0; fMaxSpeed = 0.0; wMaxSpeed = 0.0;
	    vSpeed     = 0.0;
		wRms      = 0.0; fRms       = 0.0; 
		wEntropy  = 0.0; fEntropy   = 0.0; uEntropy   = 0.0;
		wMeanDis  = 0.0; fMeanDis   = 0.0;
		wNPL      = 0.0; fNPL       = 0.0; uNPL       = 0.0;
		uwAccCorr = 0.0; uwPosCorrY = 0.0; fwPosCorrY = 0.0;
		balance   = 0.0; jnAng      = 0.0; 
		wPDE      = 0.0; fPDE      = 0.0;  uPDE      = 0.0;
		uSmooth   = 0.0; fSmooth    = 0.0; wSmooth    = 0.0;
	}
	*/
	/*
	public override string ToString ()
	{
		return UnitySerializer.JSONSerialize(this,true);
	}
	*/
};

public class FusionBridgeUnity { 
	
	[DllImport ("FusionBridge")]
	public static extern void CreateBaseFusionCenter();	
	
	[DllImport ("FusionBridge")]
	public static extern void DeleteBaseFusionCenter();	
	
	[DllImport ("FusionBridge")]
	public static extern void SetMotionNumber(int motionNumber);
	
	[DllImport ("FusionBridge")]
	public static extern bool IsUpperLimbInit(int motionNumber);
	
	[DllImport ("FusionBridge")]
	public static extern void FeedData(
		double [] trunk,
		double [] shoulder,
		double [] elbow,
		double [] wrist,
		bool runQuaternionConversion,
		bool runExercise);
	
	[DllImport ("FusionBridge")]
	public static extern void GetFeatureInformation(ref FeatureInfo featureInfo);
	
	[DllImport ("FusionBridge")]
	public static extern void SetUpperLimbExeRedo();
	
	[DllImport ("FusionBridge")]
	public static extern bool IsUpperLimbExeValid();
	
	[DllImport ("FusionBridge")]
	public static extern void GetQuaternion(int skeletonNum, double [] result);
}
