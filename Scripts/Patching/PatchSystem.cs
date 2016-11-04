using UnityEngine;
using System.Collections;
using System.IO;					//for reading files
using System;						//for try catch exception
using System.Collections.Generic; 	//for <string, string>
using System.Diagnostics;			//for running external exe with command line arguments
using Octodiff.CommandLine;			//implements Octodiff core files in an easy to use way
using Octodiff.CommandLine.Support;	//for passing arguments to a command
using Octodiff.Core;				//Octodiff command line needs these 

namespace GameDevRepo {
	public class PatchSystem {
		private WWW www;

		public string projectRootDir = "";
		public string remoteVersionURL = "";
		public string localVersionFileName = "";
		public string localVersionFileLocation = "";
		public bool updateAvailable = false;
		public string currentException = "";
		public string patchOldFilePath = "";
		public string patchSigFilePath = "";
		public string patchDeltaFilePath = "";
		public string currentProcess = "";
		public float progress = 0;
		public bool curProcessDone = true;

		void Update() {
			progress = www.progress;
		}

		/// <summary>
		/// Initializes variables: save Path, root Directory, Version File Name.
		/// </summary>
		public void InitializeVariables () 
		{
			curProcessDone = false;
			remoteVersionURL = remoteVersionURL.Trim ();											//remove line endings

			//generate a save path based on OS (Mac or Windows)
			projectRootDir = Application.dataPath;
			switch (Application.platform) {
				case RuntimePlatform.OSXPlayer:
				case RuntimePlatform.OSXEditor:
					projectRootDir += "/../../Contents/";
					break;
				case RuntimePlatform.WindowsPlayer:
				case RuntimePlatform.WindowsEditor:
					projectRootDir += "/../";
					break;
			}
			if (string.IsNullOrEmpty (localVersionFileName) == true) {
				localVersionFileName = "version.txt";
			}
			if (string.IsNullOrEmpty (localVersionFileLocation) == true) {
				localVersionFileLocation = projectRootDir + localVersionFileName;
			}
			curProcessDone = true;
		}
			
		/// <summary>
		/// Download everything from URL. Will optionally unzip the downloaded file.
		/// </summary>
		/// <returns>The from UR.</returns>
		/// <param name="url">URL to download remote files</param>
		/// <param name="saveLocation">Local Location to save downloaded remote files.</param>
		/// <param name="unzip">If you want to unzip the final file that is produced.</param>
		public IEnumerator downloadFromURL(string url, string saveLocation, bool unzip=false) {
			curProcessDone = false;
			currentProcess = "Downloading files...";
			www = new WWW(url);
			UnityEngine.Debug.Log (www.progress);
			yield return www;
			byte[] data = www.bytes;
			System.IO.File.WriteAllBytes (saveLocation, data);
			if (unzip == true) {
				ZipUtil.Unzip (saveLocation, saveLocation.Replace(Path.GetFileName(saveLocation),Path.GetFileNameWithoutExtension(saveLocation)));
				File.Delete (saveLocation);
			}
			currentProcess = "";
			curProcessDone = true;
			yield return null;
		}
		/// <summary>
		/// Calls a url and returns the contents from it as a string.
		/// </summary>
		/// <returns>The response from UR.</returns>
		/// <param name="url">URL.</param>
		public string getResponseFromURL(string url){
			curProcessDone = false;
			www = new WWW(url);		
			while (www.isDone == false) {
				progress = www.progress;
			}
			curProcessDone = true;
			return www.text;
		}
		/// <summary>
		/// Get contents of local version file.
		/// </summary>
		/// <returns>The local version.</returns>
		/// <param name="overrideLocation">Optional. Specify another location where your version file is.</param>
		public string GetLocalVersion(string overrideLocation="") 						//return version from version
		{		
			curProcessDone = false;
			currentProcess = "Identifying Version File";
			string version = "";
			string versionPath = "";
			if (string.IsNullOrEmpty (overrideLocation) == false) {
				versionPath = overrideLocation;
			} else {
				versionPath = localVersionFileLocation;
			}
			try 
			{
				currentProcess = "Reading Version";
				StreamReader fileReader = new StreamReader (versionPath);				//open file in executable location
				version = fileReader.ReadLine().Trim();									//remove line endings
				fileReader.Close();
			}
			catch (Exception ex)
			{
				currentProcess = "";
				currentException = ex.Message;
			}
			currentProcess = "";
			curProcessDone = true;
			return version;
		}
		/// <summary>
		/// This will scan the contents of a folder and compare it with another outdated copy of that folder
		///  and generate all the delta (patch) files needed to upgrade the old folder to the new folder. You must
		/// specify a location to save the output of the delta files.
		/// </summary>
		/// <returns>The project patches.</returns>
		public IEnumerator generateProjectPatches(string currentFolder, string outdatedFolder, string saveLocation) {
			curProcessDone = false;
			foreach (string file in Directory.GetFiles(outdatedFolder)) {
				yield return generateSignatureFile (file, saveLocation + Path.DirectorySeparatorChar + Path.GetFileName (file).Replace (".cs", ".sig"));
			}
			foreach (string file in Directory.GetFiles(currentFolder)) {
				yield return generateDeltaFile (saveLocation +Path.DirectorySeparatorChar+Path.GetFileName (file).Replace (".cs", ".sig"),
									file,
									saveLocation +Path.DirectorySeparatorChar+ Path.GetFileName (file).Replace (".cs", ".delta"));
			}
			currentProcess = "Scanning and removing un-needed signature files";
			foreach (string file in Directory.GetFiles(saveLocation)) {
				if (Path.GetExtension (file) == ".sig") {
					File.Delete (file);
				}
			}
			currentProcess = "";
			curProcessDone = true;
			yield return true;
		}

		/// <summary>
		/// Will generate a signature file for the file that you provide and save it at your desired location. If save location isn't specified then it will save it to the same location as the file.
		/// </summary>
		/// <returns>The single signature file.</returns>
		/// <param name="oldFilePath">Outdated file path.</param>
		/// <param name="saveLocation">Override save location.</param>

		public string generateSignatureFile(string oldFilePath, string saveLocation="")
		{
			curProcessDone = false;
			currentProcess = "Generating Signature File for " + Path.GetFileName (oldFilePath);
			try 
			{
				if(string.IsNullOrEmpty(saveLocation) == true) 					//if no save location specified make one
				{
					string[] fileInfo = octodiffFilePathInfo(oldFilePath);
					saveLocation = fileInfo[0];
				}
				saveLocation = saveLocation.Trim();
				if(Path.GetExtension(saveLocation) != ".sig") {					//add correct file ending (if applicable)
					saveLocation = saveLocation+".sig";
				}
				//Usage: Octodiff.exe signature <basis-file> [<signature-file>] [<options>]
				string [] commands = {oldFilePath,saveLocation};				//Add correct arguments for signature command
				SignatureCommand command = new SignatureCommand();				
				command.Execute(commands);										//make the signature file
			}
			catch(Exception ex) 
			{
				currentProcess = "";
				currentException = ex.Message;
				return ex.Message;
			}
			currentProcess = "";
			curProcessDone = true;
			return "Success";
		}

		/// <summary>
		/// Generates the delta file. Based off the new file and signature file.
		/// </summary>
		/// <returns>The delta file.</returns>
		/// <param name="signaturePath">Signature path.</param>
		/// <param name="newFilePath">New file path.</param>
		/// <param name="saveDeltaLocation">Save delta location.</param>
		public string generateDeltaFile(string signaturePath, string newFilePath, string saveDeltaLocation="")
		{
			curProcessDone = false;
			currentProcess = "Generating delta file for " + Path.GetFileName (newFilePath);
			//newFilePath could be a remote file as well
			try 
			{
				if(string.IsNullOrEmpty(saveDeltaLocation) == true) 			//generate save location
				{
					string[] fileInfo = octodiffFilePathInfo(newFilePath);
					saveDeltaLocation = fileInfo[1];
				}
				saveDeltaLocation = saveDeltaLocation.Trim();					//remove line endings

				//Usage: Octodiff delta <signature-file> <new-file> [<delta-file>] [<options>]
				string[] commands = {signaturePath,newFilePath,saveDeltaLocation};//add correct arguments
				DeltaCommand command = new DeltaCommand();
				command.Execute(commands);										//generate delta file
			}
			catch (Exception ex) 
			{
				currentProcess = "";
				currentException = ex.Message;
				return ex.Message;
			}
			currentProcess = "";
			curProcessDone = true;
			return "Success";
		}

		/// <summary>
		/// Applies the delta file to an outdated version to update it.
		/// </summary>
		/// <returns>The patch.</returns>
		/// <param name="oldFilePath">Old file path.</param>
		/// <param name="deltaFilePath">Delta file path.</param>
		/// <param name="outputFilePath">Output file path(new file).</param>
		public string applyPatch(string oldFilePath, string deltaFilePath, string outputFilePath="")
		{
			curProcessDone = false;
			try 
			{
				if(string.IsNullOrEmpty(outputFilePath) == true) 			//generate save location
				{
					outputFilePath = oldFilePath;
				}
				outputFilePath = outputFilePath.Trim();						//remove line endings
				outputFilePath = outputFilePath+".new";						//append a .new as to not get errors with existing files
				//Usage: Octodiff.exe patch <basis-file> <delta-file> <new-file> [<options>]
				string[] commands = {oldFilePath,deltaFilePath,outputFilePath};//add correct arguments for patching
				PatchCommand command = new PatchCommand();
				command.Execute(commands);									//apply the patch to the file
			}
			catch (Exception ex) 
			{
				currentProcess = "";
				currentException = ex.Message;
				curProcessDone = true;
				return ex.Message;
			}
			currentProcess = "";
			curProcessDone = true;
			return "Success";
		}

		/// <summary>
		/// Removes .delta and .sig files. Also replaces all files with their .new versions if they exist. 
		/// Will do this based on your supplied directory or will default to the root of your project.
		/// </summary>
		/// <returns>The old files.</returns>
		/// <param name="directoryPath">Directory path.</param>
		public string removeOldFiles(string directoryPath="")
		{
			curProcessDone = false;
			currentProcess = "Cleaning up directory...";
			if (string.IsNullOrEmpty (directoryPath) == true) {
				directoryPath = projectRootDir;
			} 
			foreach (string file in Directory.GetFiles(directoryPath)) 
			{
				if (Path.GetExtension (file) == ".new" && File.Exists ((file.Substring (0, file.Length - 4)))) 
				{
					File.Delete ((file.Substring (0, file.Length - 4)));
					File.Move (file, (file.Substring (0, file.Length - 4)));
				} 
				else if (Path.GetExtension (file) == ".delta" || Path.GetExtension (file) == ".sig") 
				{
					File.Delete (file);
				}
			}
			currentProcess = "";
			curProcessDone = true;
			return "Success";
		}
		/// <summary>
		/// Compares your local and remote "version" files and sets the "updateAvailable" variable to true or false.
		/// </summary>
		/// <returns>The for updates.</returns>
		public IEnumerator CheckForUpdates() 
		{
			curProcessDone = false;
			currentProcess = "Checking for Updates";
			www = new WWW(remoteVersionURL);													//check remote version
			yield return www;
			progress = 0;
			string availableVersion = www.text.Trim ();									//remove line endings from file
			string line;
			try 																		//check local version
			{
				if (System.IO.File.Exists(projectRootDir+localVersionFileName)) 
				{
					StreamReader fileReader = new StreamReader (projectRootDir+localVersionFileName);	//open localVersionFileName
					using (fileReader) 														//will close the file when done
					{
						line = fileReader.ReadLine().Trim();								//read only the first line
						line = line.Replace(".","");										//produce a straight number
						//fileReader.Close();												//close the file
					}
				}
				else 
				{
					line = "000";
					System.IO.File.WriteAllText(projectRootDir+localVersionFileName, line);			//make version file if it doesn't exist
				}
			}
			catch (Exception ex)
			{
				line = "000";
				currentException = ex.Message;
			}
			currentProcess = "";
			updateAvailable = (float.Parse (line) < float.Parse (availableVersion.Replace(".","")))? true : false;//compare versions, update needed?
			curProcessDone = true;
			return updateAvailable;
		}

		private string[] octodiffFilePathInfo(string filePath)
		{
			string oldFileName = Path.GetFileName(filePath);								//get file to make patch from
			string ext = Path.GetExtension(filePath);										//get file extention
			string signatureFileName = "";
			string deltaFileName = "";
			if(string.IsNullOrEmpty(ext) == false) 
			{
				signatureFileName = Path.GetFileName(filePath).Replace(ext,".sig");			//signature file to use
				deltaFileName = Path.GetFileName(filePath).Replace(ext,".delta");			//name the delta file
			}
			else 
			{
				signatureFileName = Path.GetFileName(filePath)+".sig";						//signature file to use
				deltaFileName = Path.GetFileName(filePath)+".delta";						//name the delta file
			}
			string signatureFilePath = filePath.Replace(oldFileName,signatureFileName);		//signature file path
			string deltaFilePath = filePath.Replace(oldFileName,deltaFileName);				//where to save delta file (patch)

			string[] returnValues = {signatureFilePath, deltaFilePath, ext};
			return returnValues;
		}
	}
}