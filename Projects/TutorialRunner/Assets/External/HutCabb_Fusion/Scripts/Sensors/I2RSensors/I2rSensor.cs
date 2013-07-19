using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Threading;
using System.IO;
using OpenNETCF.IO.Ports;

public class I2rSensor : IxoMonoSingleton<I2rSensor> {

	public delegate void OnFusionUpdateHandler(int sensorRank, Fusion6 fusion);
    public event OnFusionUpdateHandler OnFusionUpdate;
	
	static readonly byte[] HEADERS = { 126, 69, 0, 255, 255 };
    static readonly int PACKET_LENGTH = 50;//27;//39;
	static readonly int[] NODE_IDS = { 11, 12, 13, 15 };
	//static readonly int[] NODE_IDS = { 2, 3, 4, 64 };
	
    private SerialPort _comport;
    List<byte> _stream = new List<byte>(2048);
	double[][] _dataReceivedBuffer = new double[NODE_IDS.Length][];
	Thread _dataReaderThread;
	
	Fusion6[] fusions = new Fusion6[4];
	bool [] isReceiving = new bool[4];
	
	public Fusion6 [] Fusions {
		get { return fusions; }
	}
	
	public bool IsReceiving(int index) {
		return isReceiving[index];
	}
	
	public Quaternion GetQuaternion(int nodeIndex) 
	{
		return new Quaternion(
			(float)fusions[nodeIndex].QuarternionX,
			(float)fusions[nodeIndex].QuarternionY,
			(float)fusions[nodeIndex].QuarternionZ,
			(float)fusions[nodeIndex].QuarternionW);
	}
	
	public void Write(string s)
	{
		byte [] b = System.Text.Encoding.Default.GetBytes(s);
		_comport.Write(b,0,b.Length);
	}
	
	override protected void Awake()
	{
		base.Awake();
		
		for (int i = 0; i < fusions.Length; i++)
                fusions[i] = new Fusion6();

		for (int i = 0; i < _dataReceivedBuffer.Length; i++)
            _dataReceivedBuffer[i] = new double[9];
	}
	
	void Start()
	{
		StartComPort();
	}
	
	override protected void OnDestroy()
	{
		CloseComPort();
		base.OnDestroy();
	}
	
	public void StartComPort()
    {
        if (_comport != null && _comport.IsOpen)
            _comport.Close();
		
        string[] portNames = System.IO.Ports.SerialPort.GetPortNames();
        if (portNames == null || portNames.Length < 2)
            throw new UnauthorizedAccessException("No ports open");
        Debug.Log(portNames[0] + " " + portNames[1]);
		string portToOpen = portNames[portNames.Length - 1].Length <= 4 ? portNames[portNames.Length - 1] : "\\\\.\\" + portNames[portNames.Length - 1];
//		string portToOpen = portNames[0].Length <= 4 ? portNames[0] : "\\\\.\\" + portNames[0];
		
		_comport = new SerialPort(portToOpen,
			115200,
			Parity.None,
			8,
			StopBits.One);
        _comport.Open();
		
		StartReadingThread();
    }
	
	private void StartReadingThread()
	{
		_dataReaderThread = new Thread(new ThreadStart(ReadDataThread));
		_dataReaderThread.Start();
	}
	
	public void CloseComPort()
	{
		if(_dataReaderThread != null)
		{
			_dataReaderThread.Interrupt();
			_dataReaderThread = null;
		}
		
		if(_comport != null && _comport.IsOpen)
		{
			_comport.Close();
			_comport = null;
		}
	}
	
	byte [] buf = new byte[8192];
    private void ReadDataThread()
    {
        while (_comport.IsOpen)
        {
            int count = _comport.BytesToRead;
			
            if (count > 0)
            {
                _comport.Read(buf, 0, count);

                lock (_stream)
                {
                    for(int i = 0; i < count; i++)
                        _stream.Add(buf[i]);
                }
            }
		 }
    }
	
	public void Update()
	{
		ProcessStreamData();
	}
	
    public void ProcessStreamData()
    {
		while (true)
        {
	        int packLen = 0;
            int headerSequence = FindFullPktHeaderIndex(out packLen);
            if (headerSequence < 0)
			{
				//Debug.Log("break: " + _stream.Count);
                break;
			}
            int nodeId = (int)_stream[headerSequence + 10];
			//Debug.Log("Node Id: " + nodeId);
			long[] rawQuart = new long[4];
            rawQuart[0] = (long)((_stream[headerSequence + 12] << 8) | _stream[headerSequence + 13]);
            rawQuart[1] = (long)((_stream[headerSequence + 14] << 8) | _stream[headerSequence + 15]);
            rawQuart[2] = (long)((_stream[headerSequence + 16] << 8) | _stream[headerSequence + 17]);
            rawQuart[3] = (long)((_stream[headerSequence + 18] << 8) | _stream[headerSequence + 19]);
			
			ushort [] raw = new ushort[9];
			raw[0] = (ushort)((_stream[headerSequence + 20] << 8) | _stream[headerSequence + 21]);
			raw[1] = (ushort)((_stream[headerSequence + 22] << 8) | _stream[headerSequence + 23]);
			raw[2] = (ushort)((_stream[headerSequence + 24] << 8) | _stream[headerSequence + 25]);
			raw[3] = (ushort)((_stream[headerSequence + 26] << 8) | _stream[headerSequence + 27]);
			raw[4] = (ushort)((_stream[headerSequence + 28] << 8) | _stream[headerSequence + 29]);
			raw[5] = (ushort)((_stream[headerSequence + 30] << 8) | _stream[headerSequence + 31]);
			raw[6] = (ushort)((_stream[headerSequence + 32] << 8) | _stream[headerSequence + 33]);
			raw[7] = (ushort)((_stream[headerSequence + 34] << 8) | _stream[headerSequence + 35]);
			raw[8] = (ushort)((_stream[headerSequence + 36] << 8) | _stream[headerSequence + 37]);
			
            double[] data = new double[4];
            for (int i = 0; i < 4; i++)
            {
                if (rawQuart[i] > 32767)
                {
                    rawQuart[i] -= 65536;
                }
                data[i] = (float)rawQuart[i] / 16384.0f;
            }
			
			if(data[0] <= 1f && data[0] >= -1f
				&& data[1] <= 1f && data[1] >= -1f
				&& data[2] <= 1f && data[2] >= -1f
				&& data[3] <= 1f && data[3] >= -1f)
			{
	            for (int i = 0; i < fusions.Length; i++)
	            {
	                if (NODE_IDS[i] == nodeId)
	                {
	                    fusions[i].InsertQuarternionData(data[0], data[1], data[2], data[3]);
						
						fusions[i].InsertRawData(raw[0],raw[1],raw[2],raw[3],raw[4],raw[5],raw[6],raw[7],raw[8]);
						
						//fusions[i].FuseQuarternionTo9DOF();
//						if(fusions[i].MagnetometerX < 100f && fusions[i].MagnetometerX > -100f
//							&& fusions[i].MagnetometerY < 100f && fusions[i].MagnetometerY > -100f
//							&& fusions[i].MagnetometerZ < 100f && fusions[i].MagnetometerZ > -100f)
//						{
	//						if(nodeId == 12)
	//						{
	//							Debug.Log(string.Format("{0:0.000},{1:0.000},{2:0.000}",fusions[i].RawReadings[0],fusions[i].RawReadings[1],fusions[i].RawReadings[2]));
	//						}
							
							if(OnFusionUpdate != null)
								OnFusionUpdate(i,fusions[i]);
//						}
						isReceiving[i] = true;
	                }
	            }
			}
			
			
//			else
//				print ("error" + nodeId);

//			if(nodeId == 11)
//			{
//				Debug.Log(_stream[headerSequence + 40]);
//				
//				if(_stream[headerSequence + 40] > lastPackId || lastPackId == 255 && _stream[headerSequence + 40] == 0)
//				{
//					int packTime = (int)((_stream[headerSequence + 41] << 24) | (_stream[headerSequence + 42] << 16) |(_stream[headerSequence + 43] << 8) | _stream[headerSequence + 44]);
//					TimeSpan span = DateTime.Now - lastTime;
//					//Debug.Log(packTime);
//					//Debug.Log( (packTime - prevTime)/1000000f + " , " + span.TotalSeconds);
//					prevTime = packTime;
//					lastTime = DateTime.Now;
//					packCount++;
//					
//				}
//				else
//				{
//					//Debug.Log("duplicate or out of order: " + _stream[headerSequence + 40]);
//				}
//				
//				if(data[0] > 1f || data[0] < -1f
//					|| data[1] > 1f || data[1] < -1f
//					|| data[2] > 1f || data[2] < -1f
//					|| data[3] > 1f || data[3] < -1f)
//				{
//					//Debug.Log("weird data: " + _stream[headerSequence + 40]);
//				}
//				
//				lastPackId = _stream[headerSequence + 40];
//			}
			lock(_stream)
			{
				_stream.RemoveRange(0, packLen);
            	//_stream.RemoveRange(0, headerSequence + PACKET_LENGTH);
			}
		}
    }
	
	private string ByteArrayToHexString(byte[] data)
    {
        StringBuilder sb = new StringBuilder(data.Length * 3);
        foreach (byte b in data)
            sb.Append(Convert.ToString(b, 16).PadLeft(2, '0').PadRight(3, ' '));
        return sb.ToString().ToUpper();
    }

    private int FindFullPktHeaderIndex(out int packLen)
    {
        while (true)
        {
            int possibleHeaderIndex = _stream.IndexOf(HEADERS[0]);
            
            if (possibleHeaderIndex >= 0 && _stream.Count >= (possibleHeaderIndex + PACKET_LENGTH)
                && HEADERS[1] == _stream[possibleHeaderIndex + 1]
                && HEADERS[2] == _stream[possibleHeaderIndex + 2]
                && HEADERS[3] == _stream[possibleHeaderIndex + 3]
                && HEADERS[4] == _stream[possibleHeaderIndex + 4])
            {
                packLen = _stream.IndexOf(HEADERS[0], possibleHeaderIndex + 1) + 1;
                if (_stream.Count > packLen && _stream[packLen] == HEADERS[1])
                    packLen = -1;

                if (packLen > 0)
                    return possibleHeaderIndex;
                else
                    return -1;
            }

            if (possibleHeaderIndex >= 0 && _stream.Count > (possibleHeaderIndex + PACKET_LENGTH)
                && (HEADERS[1] != _stream[possibleHeaderIndex + 1]
                || HEADERS[2] != _stream[possibleHeaderIndex + 2]
                || HEADERS[3] != _stream[possibleHeaderIndex + 3]
                || HEADERS[4] != _stream[possibleHeaderIndex + 4]))
            {
                //FileConsole.WriteLine("removing extra " + (possibleHeaderIndex + 1) + " bytes");
                //Console.WriteLine(ByteArrayToHexString(_stream.ToArray()));
                _stream.RemoveRange(0, possibleHeaderIndex + 1);
                    
                continue;
            }

             break;
        }
        packLen = 0;
        return -1;
    }
    
}

public static class FileConsole
{
    private static FileStream filestream = new FileStream("out.txt", FileMode.Create);
    static StreamWriter streamwriter;

    static FileConsole()
    {
        streamwriter = new StreamWriter(filestream);
        streamwriter.AutoFlush = true;
    }

    public static void WriteLine(string message)
    {
        streamwriter.WriteLine(message);
    }
}