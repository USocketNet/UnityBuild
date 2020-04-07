using UnityEngine;
using UnityEngine.UI;
using System;
using System.Runtime.InteropServices;

[Serializable]
public class Contains
{
	public string strings = "string";
	public int integers = 12;
	public float floatings = 0.24f;
}

public class USocketNetWebGL : MonoBehaviour 
{
	//#region Temporary
		[DllImport("__Internal")]
		private static extern void CallTheBrowser(string asdas);

		[DllImport("__Internal")]
		private static extern void Hello();

		[DllImport("__Internal")]
		private static extern void HelloString(string str);

		[DllImport("__Internal")]
		private static extern void PrintFloatArray(float[] array, int size);

		[DllImport("__Internal")]
		private static extern int AddNumbers(int x, int y);

		[DllImport("__Internal")]
		private static extern string StringReturnValueFunction();

		[DllImport("__Internal")]
		private static extern void BindWebGLTexture(int texture);
	//#endregion

	public delegate void CallbackResult( string data );

	[DllImport("__Internal")]
	private static extern void CallbackReturnee( string data );

	[DllImport("__Internal")]
	private static extern void CallbackReturn( string data, CallbackResult cr );

	[DllImport("__Internal")]
	private static extern string ConnectToServer();

	[DllImport("__Internal")]
	private static extern string DisconnectFromServer();
	
	public Text results = null;

	public Contains contains = new Contains();

	public CallbackResult resulted = null;

	void Start()
	{
		//UnityFunction();
		resulted = OnCallback;
	}

	public void OnCallback( string data )
	{
		Debug.Log( data );
	}

	public void Connect()
	{
		ConnectToServer();
	}

	public void Connecting(string sid)
	{
		results.text = sid;
	}

	public void Disconnect()
	{
		results.text = DisconnectFromServer();
	}

	public void OnCallbackResult(string data)
	{
		Debug.Log( "Callback: " + data );
	}

	void Update() 
	{


		if(Input.GetKeyDown(KeyCode.Space))
		{
			Debug.Log ("Unity to Browser!");
			string datas = JsonUtility.ToJson( contains );
			CallbackReturnee( datas );
		//CallbackReturn (datas, (string objs) => {
			//contains = JsonUtility.FromJson<Contains>( objs );
			//Debug.Log( "Result: " + contains.strings + ":" + contains.integers + ":" + contains.floatings );

			
			//Hello();

			//HelloString("This is a string.");

			//float[] myArray = new float[10];
			//PrintFloatArray(myArray, myArray.Length);

			//int result = AddNumbers(5, 7);
			//Debug.Log(result);

			//Debug.Log(StringReturnValueFunction());

			//var texture = new Texture2D(0, 0, TextureFormat.ARGB32, false);
			//BindWebGLTexture(texture.GetNativeTextureID());

			//Debug.Log ("Unity: StringReturnValueFunction!");
			//Debug.Log("Browser to Unity: 2");
			//UnityFunction();
		}

		if(Input.GetKeyDown(KeyCode.LeftShift))
		{
			//Debug.Log("Unity to Browser: This is from Unity!");
			CallTheBrowser("1");
		}

	}

		
	public void UnityFunction()
	{
		if(Application.platform == RuntimePlatform.WebGLPlayer)
		{
			//Debug.Log("Unity to Browser: This is from Unity!");
			CallTheBrowser("1");
		}

	}

	public void MyFunction(string data)
	{
		//HelloString("Browser: " + data);
		//Debug.Log("Browser to Unity: " + data);
		UnityFunction();
	}
}
