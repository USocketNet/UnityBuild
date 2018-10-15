using UnityEngine;
using System.Runtime.InteropServices;

public class USocketNetWebGL : MonoBehaviour 
{

	[DllImport("__Internal")]
	private static extern void CallTheBrowser(string str);

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

	int timer = 0;
	void Update() {
		
		timer += 1;

		if(timer > 33.33)
		{
			timer = 0;
			//CallTheBrowser();
		}

		if(Input.GetKeyDown(KeyCode.Space))
		{
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
			UnityFunction();
		}

		if(Input.GetKeyDown(KeyCode.LeftShift))
		{
			//Debug.Log("Unity to Browser: This is from Unity!");
			CallTheBrowser("1");
		}

	}

	void Start()
	{
		UnityFunction();
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
