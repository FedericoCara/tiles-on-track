using System;
using System.IO;

using UnityEngine;
using UnityEngine.UI;

namespace Mimic.UI{

	public class FileChooser : MonoBehaviour {

		[SerializeField]
		private Text currentDirectoryTxt;

		[SerializeField]
		private ListViewString filesLst;

		[SerializeField]
		private bool showHiddenFiles = true;

		private string currentDirectory;
		public string CurrentDirectory{
			set{
				currentDirectory = value;

				if(currentDirectoryTxt != null)
					currentDirectoryTxt.text = value;

				filesLst.Clear ();

				DirectoryInfo directory = new DirectoryInfo(currentDirectory);

				DirectoryInfo[] subdirectoryEntries = directory.GetDirectories();
				foreach(DirectoryInfo subdirectory in subdirectoryEntries){
					if(showHiddenFiles || (subdirectory.Attributes & FileAttributes.Hidden) == 0)
						filesLst.Add (subdirectory.FullName);
				}
					
				FileInfo[] fileEntries = directory.GetFiles();
				foreach(FileInfo fileName in fileEntries){
					if(showHiddenFiles || (fileName.Attributes & FileAttributes.Hidden) == 0)
						filesLst.Add (fileName.FullName);
				}
			}
		}

		private Action<string> onSelectedFileAction;
		public Action<string> OnSelectedFileAction{
			set{ onSelectedFileAction = value; }
		}

		private void Start(){
			CurrentDirectory = Directory.GetCurrentDirectory ();
			CheckUp ();
		}

		protected void Update(){
			if (InputAdapter.GetButton ("Fire1") && !InputAdapter.GetMouseButton (0)) {
				OnSelectBtnClick ();
			} else if (InputAdapter.GetButton ("Cancel")) {
				OnCancelBtnClick ();
			}
		}

		public void CheckUp(){
			if (InputAdapter.GetButton ("Jump")) {
				OnUpBtnClick ();
			}
			Invoke ("CheckUp", 0.1f);
		}

		public void OnSelectBtnClick(){
			string selectedFile = filesLst.SelectedItemData;
			if (selectedFile != null) {
				if ((File.GetAttributes (selectedFile) & FileAttributes.Directory) == FileAttributes.Directory) {
					CurrentDirectory = selectedFile;
				} else if(onSelectedFileAction != null){
					onSelectedFileAction (selectedFile);
					gameObject.SetActive (false);
				}
			}
		}

		public void OnCancelBtnClick(){
			gameObject.SetActive (false);
		}

		public void OnUpBtnClick(){
			DirectoryInfo parent = Directory.GetParent (currentDirectory);
			if(parent != null)
				CurrentDirectory = parent.FullName;
		}

		public void Show(string directory = null){
			if (directory != null && Directory.Exists(directory)) {
				CurrentDirectory = directory;
			}
			gameObject.SetActive (true);
		}
		
	}

}
