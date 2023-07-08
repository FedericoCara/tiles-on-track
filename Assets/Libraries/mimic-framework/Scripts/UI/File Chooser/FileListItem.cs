using System.IO;

using UnityEngine;
using UnityEngine.UI;

namespace Mimic.UI{
	
	public class FileListItem : ListItemString {

		[SerializeField]
		private Image fileTypeImg;

		[SerializeField]
		private Sprite fileIcon;

		[SerializeField]
		private Sprite directoryIcon;

		protected override  void Refresh()	{
			fileTypeImg.sprite = (File.GetAttributes(Data) & FileAttributes.Directory) == FileAttributes.Directory ? directoryIcon : fileIcon;
			text.text = Path.GetFileName(Data);
		}
		
	}
}
