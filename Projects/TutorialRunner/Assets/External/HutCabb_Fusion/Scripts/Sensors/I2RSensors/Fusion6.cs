using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Filter;
using UnityEngine;

public class Fusion6
{
    // calibrated sensor measurements
    double a_x, a_y, a_z;                                       // accelerometer measurements
    double w_x, w_y, w_z;                                       // gyroscope measurements
	double m_x, m_y, m_z;                                       // magetometer measurements(not used)

    public double AccelerationX { get { return a_x; } }
    public double AccelerationY { get { return a_y; } }
    public double AccelerationZ { get { return a_z; } }

    public double GyroscopeX { get { return w_x; } }
    public double GyroscopeY { get { return w_y; } }
    public double GyroscopeZ { get { return w_z; } }
	
	public double MagnetometerX { get { return m_x; } }
    public double MagnetometerY { get { return m_y; } }
    public double MagnetometerZ { get { return m_z; } }
	
	public double [] RawReadings {
		get { return new double[9] {a_x,a_y,a_z,w_x,w_y,w_z,m_x,m_y,m_z}; }
	}

    // sensor calibration variables and constants
    private const double a_xBias = 33000;            // accelerometer bias
    private const double a_yBias = 33000;
    private const double a_zBias = 35500;
    private const double a_xGain = 0.00006103515625 * 9.81;        // accelerometer gains
    private const double a_yGain = 0.00006103515625 * 9.81;
    private const double a_zGain = 0.00006103515625 * 9.81;
    private double w_xBias = 0;                             // gyroscope bias
    private double w_yBias = -210;
    private double w_zBias = 180;
    private const double w_xGain = 0.00763358778625954198473282442748 * 0.0174532925 * 1;         // gyroscope gains
    private const double w_yGain = 0.00763358778625954198473282442748 * 0.0174532925 * 1;
    private const double w_zGain = 0.00763358778625954198473282442748 * 0.0174532925 * 1;

    private bool initSample = true;                             // flag used to indicate the initial sample
    private ButterworthFilter butterFilterAccelX = new ButterworthFilter();//new double[] { 0.0004, 0.0012, 0.0012, 0.0004 }, new double[] { 1.0000, -2.6862, 2.4197, -0.7302 });
    private ButterworthFilter butterFilterAccelY = new ButterworthFilter();//new double[] { 0.0004, 0.0012, 0.0012, 0.0004 }, new double[] { 1.0000, -2.6862, 2.4197, -0.7302 });
    private ButterworthFilter butterFilterAccelZ = new ButterworthFilter();//new double[] { 0.0004, 0.0012, 0.0012, 0.0004 }, new double[] { 1.0000, -2.6862, 2.4197, -0.7302 });

    private ButterworthFilter butterFilterGyroX = new ButterworthFilter();
    private ButterworthFilter butterFilterGyroY = new ButterworthFilter();
    private ButterworthFilter butterFilterGyroZ = new ButterworthFilter();

    // filter variables and constants
    private double SEq_1 = 1, SEq_2 = 0, SEq_3 = 0, SEq_4 = 0;                          // estimated orientation quaternion elements with initial conditions
    public double DeltaT = 0.05;                                                // sampling period
    private const double gyroMeasError = 5;                                            // gyroscope measurement error (in degrees per second)
    private double beta = Math.Sqrt(3.0 / 4.0) * (Math.PI * (gyroMeasError / 180.0));   // compute beta

    public double EarthFluxX { get { return 0; } }
    public double EarthFluxZ { get { return 0; } }

    public double GyroBiasX { get { return 0; } }
    public double GyroBiasY { get { return 0; } }
    public double GyroBiasZ { get { return 0; } }

    public double QuarternionW { get { return SEq_1; } }
    public double QuarternionX { get { return SEq_2; } }
    public double QuarternionY { get { return SEq_3; } }
    public double QuarternionZ { get { return SEq_4; } }

    public double RadianX { get { return Math.Atan2(2 * -SEq_3 * -SEq_4 - 2 * SEq_1 * -SEq_2, 2 * SEq_1 * SEq_1 + 2 * -SEq_4 * -SEq_4 - 1); } }
    public double RadianY { get { return -Math.Asin(2 * -SEq_2 * -SEq_3 - 2 * SEq_1 * -SEq_3); } }
    public double RadianZ { get { return Math.Atan2(2 * -SEq_2 * -SEq_3 - 2 * SEq_1 * -SEq_4, 2 * SEq_1 * SEq_1 + 2 * -SEq_2 * -SEq_2 - 1); } }

    public Fusion6()
    {
    }

    public Fusion6(double w, double x, double y, double z)
    {
        SEq_1 = w; SEq_2 = x; SEq_3 = y; SEq_4 = z;
    }

    public void ResetAxis()
    {
        SEq_1 = 1; SEq_2 = 0; SEq_3 = 0; SEq_4 = 0;
    }
	
	public void InsertQuarternionData(double w, double x, double y, double z)
    {
        SEq_1 = w;
        SEq_2 = x;
        SEq_3 = y;
        SEq_4 = z;
    }
	
	public void InsertRawData(double accelX, double accelY, double accelZ,
        double gyroX, double gyroY, double gyroZ,
        double magX, double magY, double magZ)
    {
        if (accelX > 32767)
            accelX -= 65536;
        a_x = (float)accelX / 16384.0f * 9.80665f;

        if (accelY > 32767)
            accelY -= 65536;
        a_y = (float)accelY / 16384.0f * 9.80665f;

        if (accelZ > 32767)
            accelZ -= 65536;
        a_z = (float)accelZ / 16384.0f * 9.80665f;

        //a_x = (accelX - a_xBias) * a_xGain;//butterFilterAccelX.Filter((accelX - a_xBias) * a_xGain);                                       // 16-bit uint ADC acceleroemter values to m/s/s
        //a_y = (accelY - a_yBias) * a_yGain;//butterFilterAccelY.Filter((accelY - a_yBias) * a_yGain);
        //a_z = (accelZ - a_zBias) * a_zGain;//butterFilterAccelZ.Filter((accelZ - a_zBias) * a_zGain);

        if (gyroX > 32767)
            gyroX -= 65536;
        w_x = (float)gyroX / 16384.0f * 2000f * 0.0174532925f + 0.05f;

        if (gyroY > 32767)
            gyroY -= 65536;
        w_y = (float)gyroY / 16384.0f * 2000f * 0.0174532925f + 0.26f;

        if (gyroZ > 32767)
            gyroZ -= 65536;
        w_z = (float)gyroZ / 16384.0f * 2000f * 0.0174532925f - 0.04f;

        //w_x = (gyroX - w_xBias) * w_xGain;//butterFilterGyroX.Filter((gyroX - w_xBias) * w_xGain); //gyroX - w_xBias) * w_xGain;//                       // 16-bit uint ADC gyrocope values to rad/s (HP filtered)
        //w_y = (gyroY - w_yBias) * w_yGain;//butterFilterGyroY.Filter((gyroY - w_yBias) * w_yGain); //(gyroY - w_yBias) * w_yGain;//
        //w_z = (gyroZ - w_zBias) * w_zGain;//butterFilterGyroZ.Filter((gyroZ - w_zBias) * w_zGain); //(gyroZ - w_zBias) * w_zGain;//

        if (magX > 32767)
            magX -= 65536;
        m_x = (float)magX / 16384.0f * 1229f - 3f;
		
        if (magY > 32767)
            magY -= 65536;
        m_y = (float)magY / 16384.0f * 1229f - 2f;
		
        if (magZ > 32767)
            magZ -= 65536;
        m_z = (float)magZ / 16384.0f * 1229f + 10f;
    }
	
	Quaternion _driftCorrection = Quaternion.identity;
	public Vector3 Gravity;
	public Vector3 Magnet;
	public void FuseQuarternionTo9DOF()
	{
		if(MagnetometerX > 40f || MagnetometerX < -40f
			|| MagnetometerY > 40f || MagnetometerY < -40f
			|| MagnetometerZ > 40f || MagnetometerZ < -40f)
			return;
		
		Vector3 gravity = new Vector3((float)a_x,(float)a_y,(float)a_z).normalized;
		Gravity = gravity;
		Vector3 rawCompass = new Vector3((float)m_x,(float)m_y,(float)m_z);
		Magnet = rawCompass;
		Vector3 flatNorth = rawCompass - Vector3.Dot(gravity,rawCompass) * gravity;
		Quaternion compassOrientation = Quaternion.Inverse(Quaternion.LookRotation(flatNorth, -gravity));
		
		Quaternion targetCorrection = compassOrientation * Quaternion.Inverse(new Quaternion((float)QuarternionX,(float)QuarternionY,(float)QuarternionZ,(float)QuarternionW));
		if (Quaternion.Angle(_driftCorrection, targetCorrection) > 45)
            _driftCorrection = targetCorrection;
        else
            _driftCorrection = Quaternion.Slerp(_driftCorrection, targetCorrection, 0.9f);
		
		SEq_1 = _driftCorrection.w;
		SEq_2 = _driftCorrection.x;
		SEq_3 = _driftCorrection.y;
		SEq_4 = _driftCorrection.z;
	}
	
	float CalculateHeadingTiltCompensated(Vector3 mag, Vector3 acc)
	{
	  // We are swapping the accelerometers axis as they are opposite to the compass the way we have them mounted.
	  // We are swapping the signs axis as they are opposite.
	  // Configure this for your setup.
	  float accX = acc.x; // -acc.y;
	  float accY = acc.y; // -acc.x;
	  
	  float rollRadians = Mathf.Asin(accY);
	  float pitchRadians = Mathf.Asin(accX);
	  
	  // We cannot correct for tilt over 40 degrees with this algorthem, if the board is tilted as such, return 0.
	  if(rollRadians > 0.78 || rollRadians < -0.78 || pitchRadians > 0.78 || pitchRadians < -0.78)
	  {
	    return 0;
	  }
	  
	  // Some of these are used twice, so rather than computing them twice in the algorithem we precompute them before hand.
	  float cosRoll = Mathf.Cos(rollRadians);
	  float sinRoll = Mathf.Sin(rollRadians);  
	  float cosPitch = Mathf.Cos(pitchRadians);
	  float sinPitch = Mathf.Sin(pitchRadians);
	  
	  // The tilt compensation algorithem.
	  float Xh = mag.x * cosPitch + mag.z * sinPitch;
	  float Yh = mag.x * sinRoll * sinPitch + mag.y * cosRoll - mag.z * sinRoll * cosPitch;
	  
	  float heading = Mathf.Atan2(Yh, Xh);
	    
	  return heading;
	}
	
	float CalculateHeadingNotTiltCompensated(Vector3 mag)
	{
	   // Calculate heading when the magnetometer is level, then correct for signs of axis.
	   float heading = Mathf.Atan2(mag.y, mag.x);
	   return heading;
	}

    public void ProcessRawData(double accelX, double accelY, double accelZ,
        double gyroX, double gyroY, double gyroZ,
        double magX, double magY, double magZ)
    {
        ProcessRawData(accelX, accelY, accelZ, gyroX, gyroY, gyroZ);
    }

    public void ProcessRawData(double accelX, double accelY, double accelZ,
        double gyroX, double gyroY, double gyroZ)
    {
        /*
        if (initSample)
        {
            w_xBias = gyroX;
            w_yBias = gyroY;
            w_zBias = gyroZ;
            initSample = false;
        }
        */
        // calibrate sensor measurments
        a_x = butterFilterAccelX.Filter((accelX - a_xBias) * a_xGain);                                       // 16-bit uint ADC acceleroemter values to m/s/s
        a_y = butterFilterAccelY.Filter((accelY - a_yBias) * a_yGain);
        a_z = butterFilterAccelZ.Filter((accelZ - a_zBias) * a_zGain);
        w_x = butterFilterGyroX.Filter((gyroX - w_xBias) * w_xGain); //gyroX - w_xBias) * w_xGain;//                       // 16-bit uint ADC gyrocope values to rad/s (HP filtered)
        w_y = butterFilterGyroY.Filter((gyroY - w_yBias) * w_yGain); //(gyroY - w_yBias) * w_yGain;//
        w_z = butterFilterGyroZ.Filter((gyroZ - w_zBias) * w_zGain); //(gyroZ - w_zBias) * w_zGain;//

        filterUpdate(w_x, w_y, w_z, a_x, a_y, a_z);
    }

    public void InsertMetricData(double accelX, double accelY, double accelZ,
        double gyroX, double gyroY, double gyroZ,
        double magX, double magY, double magZ)
    {
        InsertMetricData(accelX, accelY, accelZ,
        gyroX, gyroY, gyroZ);
    }

    public void InsertMetricData(double accelX, double accelY, double accelZ,
                double gyroX, double gyroY, double gyroZ)
    {
        if (initSample)
        {
            w_xBias = gyroX;
            w_yBias = gyroY;
            w_zBias = gyroZ;
            initSample = false;
        }

        a_x = accelX;
        a_y = accelY;
        a_z = accelZ;
        w_x = gyroX - w_xBias;
        w_y = gyroY - w_yBias;
        w_z = gyroZ - w_zBias;

        filterUpdate(w_x, w_y, w_z, a_x, a_y, a_z);
    }

    private void filterUpdate(double w_x, double w_y, double w_z, double a_x, double a_y, double a_z)
    {
        // local system variables
        double norm;                                                            // vector norm
        double SEqDot_omega_1, SEqDot_omega_2, SEqDot_omega_3, SEqDot_omega_4;  // quaternion rate from gyroscopes elements
        double f_1, f_2, f_3;                                                   // objective function elements
        double J_11or24, J_12or23, J_13or22, J_14or21, J_32, J_33;              // objective function Jacobian elements
        double nablaf_1, nablaf_2, nablaf_3, nablaf_4;                          // objective function gradient elements

        // axulirary variables to avoid reapeated calcualtions
        double halfSEq_1 = 0.5 * SEq_1;
        double halfSEq_2 = 0.5 * SEq_2;
        double halfSEq_3 = 0.5 * SEq_3;
        double halfSEq_4 = 0.5 * SEq_4;
        double twoSEq_1 = 2.0 * SEq_1;
        double twoSEq_2 = 2.0 * SEq_2;
        double twoSEq_3 = 2.0 * SEq_3;

        // compute the quaternion rate measured by gyroscopes
        SEqDot_omega_1 = -halfSEq_2 * w_x - halfSEq_3 * w_y - halfSEq_4 * w_z;
        SEqDot_omega_2 = halfSEq_1 * w_x + halfSEq_3 * w_z - halfSEq_4 * w_y;
        SEqDot_omega_3 = halfSEq_1 * w_y - halfSEq_2 * w_z + halfSEq_4 * w_x;
        SEqDot_omega_4 = halfSEq_1 * w_z + halfSEq_2 * w_y - halfSEq_3 * w_x;

        // normalise the accelerometer measurement
        norm = Math.Sqrt(a_x * a_x + a_y * a_y + a_z * a_z);
        a_x /= norm;
        a_y /= norm;
        a_z /= norm;

        // compute the objective function and Jacobian
        f_1 = twoSEq_2 * SEq_4 - twoSEq_1 * SEq_3 - a_x;
        f_2 = twoSEq_1 * SEq_2 + twoSEq_3 * SEq_4 - a_y;
        f_3 = 1.0 - twoSEq_2 * SEq_2 - twoSEq_3 * SEq_3 - a_z;
        J_11or24 = twoSEq_3;                                                    // J_11 negated in matrix multiplication
        J_12or23 = 2 * SEq_4;
        J_13or22 = twoSEq_1;                                                    // J_12 negated in matrix multiplication
        J_14or21 = twoSEq_2;
        J_32 = 2 * J_14or21;                                                    // negated in matrix multiplication
        J_33 = 2 * J_11or24;                                                    // negated in matrix multiplication

        // compute the gradient (matrix multiplication)
        nablaf_1 = J_14or21 * f_2 - J_11or24 * f_1;
        nablaf_2 = J_12or23 * f_1 + J_13or22 * f_2 - J_32 * f_3;
        nablaf_3 = J_12or23 * f_2 - J_33 * f_3 - J_13or22 * f_1;
        nablaf_4 = J_14or21 * f_1 + J_11or24 * f_2;

        // normalise the gradient
        norm = Math.Sqrt(nablaf_1 * nablaf_1 + nablaf_2 * nablaf_2 + nablaf_3 * nablaf_3 + nablaf_4 * nablaf_4);
        nablaf_1 /= norm;
        nablaf_2 /= norm;
        nablaf_3 /= norm;
        nablaf_4 /= norm;

        // compute then integrate the estimated quaternion rate
        SEq_1 += (SEqDot_omega_1 - (beta * nablaf_1)) * DeltaT;
        SEq_2 += (SEqDot_omega_2 - (beta * nablaf_2)) * DeltaT;
        SEq_3 += (SEqDot_omega_3 - (beta * nablaf_3)) * DeltaT;
        SEq_4 += (SEqDot_omega_4 - (beta * nablaf_4)) * DeltaT;

        // normalise quaternion
        norm = Math.Sqrt(SEq_1 * SEq_1 + SEq_2 * SEq_2 + SEq_3 * SEq_3 + SEq_4 * SEq_4);
        SEq_1 /= norm;
        SEq_2 /= norm;
        SEq_3 /= norm;
        SEq_4 /= norm;
    }
}
