﻿using UnityEngine;
using System;
using System.Collections;
using Kinect2;
using System.Runtime.InteropServices;

public class KinectBehaviourScript : MonoBehaviour {

	public KinectSensor GetKinectSensor()
	{
		IntPtr sensor = IntPtr.Zero;
		var hr = NativeMethods.GetDefaultKinectSensor( ref sensor );
		return (KinectSensor)Marshal.GetObjectForIUnknown( sensor );
	}

	public KinectSensor OpenKinectSensor()
	{
		var kinect = GetKinectSensor();
		var hr = kinect.Open();
		return kinect;
	}

	public ColorFrameSource GetColorFrameSource()
	{
		KinectSensor kinect = OpenKinectSensor();
		
		IntPtr ptr = IntPtr.Zero;
		var hr = kinect.get_ColorFrameSource( out ptr );
		
		return (ColorFrameSource)Marshal.GetObjectForIUnknown( ptr );
	}

	public ColorFrameReader GetColorFrameReader()
	{
		IntPtr ptr = IntPtr.Zero;
		var colorFrame = GetColorFrameSource();
		var hr = colorFrame.OpenReader( out ptr );
		
		return new ColorFrameReader( ptr );
	}

	ColorFrameReader colorReader;
	TextMesh tm;
	TextMesh tm2;
	TextMesh tm3;
	Texture2D texture;

	// Use this for initialization
	void Start () {
		GameObject go = GameObject.Find("TextMessage");
		tm = (TextMesh)go.GetComponent("TextMesh");
		tm.text = "Kinect Initialize";
		
		go = GameObject.Find("TextMessage2");
		tm2 = (TextMesh)go.GetComponent("TextMesh");
		
		go = GameObject.Find("TextMessageFPS");
		tm3 = (TextMesh)go.GetComponent("TextMesh");

		colorReader = GetColorFrameReader();

		if (texture == null) {
			texture = new Texture2D(1920,1080, TextureFormat.BGRA32,false);
			renderer.material.mainTexture = texture;
			tm.text = "texture created.";
		}

	}

	int frameCount = 0;

	// Update is called once per frame
	void Update () {
		IntPtr colorFramePtr = IntPtr.Zero;

		try {
			if ( colorReader == null ) {
				tm.text = "colorReader == null";
				return;
			}

			using ( var colorFrame = colorReader.AcquireLatestFrame() ) {
				if ( colorFrame == null ) {
					tm.text = "colorFrame == null";
					return;
				}
				frameCount++;
				tm3.text = frameCount.ToString();
				
				
				UInt64 count = 1920*1080*4;
				var pixels = new byte[count];
				colorFrame.CopyConvertedFrameDataToArray( pixels, ColorImageFormat.Bgra );
				
				tm2.text = string.Format( "{0},{1},{2},{3}", pixels[0], pixels[1], pixels[2], pixels[3] );
				
				texture.LoadRawTextureData( pixels );
				texture.Apply();
			}
		} catch (Exception ex) {
			tm.text = ex.StackTrace;
		}
		finally{
		}
	}
}
