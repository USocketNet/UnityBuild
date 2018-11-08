
using UnityEngine;
using UnityEngine.UI;

namespace BytesCrafter.USocketNet.Overrides 
{
	public class USN_UIBlocker 
	{
		private GameObject localBlocker = null;

		public void Show( bool yes )
		{
			if(yes)
			{
				if(localBlocker == null)
				{
					localBlocker = new GameObject ("Blocker");
					Canvas cvs = localBlocker.AddComponent<Canvas> ();
					cvs.renderMode = RenderMode.ScreenSpaceOverlay;
					cvs.sortingOrder = 100;
					localBlocker.AddComponent<CanvasScaler> ();
					Image img = localBlocker.AddComponent<Image> ();
					img.color = new Color (0f, 0f, 0f, 0.49f);
				}

				else
				{
					localBlocker.SetActive(true);
				}
			}

			else
			{
				if(localBlocker != null)
				{
					localBlocker.SetActive(false);
				}
			}
		}
	}
}