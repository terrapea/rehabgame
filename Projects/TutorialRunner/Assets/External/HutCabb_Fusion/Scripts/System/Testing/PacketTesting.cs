using UnityEngine;
using System.Collections;
using System;
using System.Text;

public class PacketTesting : MonoBehaviour {
	
	void Start () {
		
		//To Hex
		float f = 20.0522f;
        byte[] b = BitConverter.GetBytes(f);
        StringBuilder sb = new StringBuilder();
        foreach (byte by in b) {
            sb.Append(by.ToString("X"));
        }
		print(sb.ToString());
		//0x7e,0x21,0x40,0x23,0x24,0x25
		//From Hex
		byte[] bytes = BitConverter.GetBytes(0x7e2140232425);
        if (BitConverter.IsLittleEndian)
			Array.Reverse(bytes);
            //bytes = bytes.Reverse().ToArray();
        float myFloat = BitConverter.ToSingle(bytes, 0);
		print(myFloat);
		
		print(Convert.ToChar(0x21));
	}
}
