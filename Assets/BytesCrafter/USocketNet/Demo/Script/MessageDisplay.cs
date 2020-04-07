using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MessageDisplay : MonoBehaviour
{
	[Header("OPTIONS")]
	public int maxLogs = 25;

	[Header("REFERENCES")]
	public Transform content = null;
	public Transform item = null;

	public void Logs(string datas)
	{
		string data = "[" + DateTime.Now.ToLongTimeString () + "] " + datas;

		if(content.childCount >= maxLogs)
		{
			Transform reused = content.GetChild (content.childCount - 1);
			reused.SetAsFirstSibling ();
			reused.GetComponent<InputField> ().text = data;
		}

		else
		{
			Transform orig = Instantiate (item, content);
			orig.SetAsFirstSibling();
			orig.GetComponent<InputField>().text = data;
		}

		for(int index = 0; index < content.childCount; index++)
		{
			LayoutElement layEl = content.GetChild (index).GetComponent<LayoutElement> ();
			if(layEl != null) { DestroyImmediate (layEl.gameObject); }
		}
	}
}