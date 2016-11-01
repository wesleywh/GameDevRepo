using UnityEngine;
using System.Collections;
using System.IO;					//for reading files
using System;						//for try catch exception
using System.Collections.Generic; 	//for <string, string>
using System.Diagnostics;			//for running external exe with command line arguments
using Octodiff.CommandLine;			//implements Octodiff core files in an easy to use way
using Octodiff.CommandLine.Support;	//for passing arguments to a command
using Octodiff.Core;				//Octodiff command line needs these 

namespace WesLibraries {
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

		public void InitializeVariables () 
		{
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
		}
		void Update() {
			progress = www.progress;
		}
		//---------------------------------------------//
		/// <summary>
		/// Download everything from URL.
		/// </summary>
		/// <returns>The from UR.</returns>
		/// <param name="url">URL to download remote files</param>
		/// /// <param name="saveLocation">Local Location to save downloaded remote files.</param>
		public IEnumerator downloadFromURL(string url, string saveLocation) {
			currentProcess = "Downloading files...";
			www = new WWW(url);
			yield return www;
			byte[] data = www.bytes;
			System.IO.File.WriteAllBytes (saveLocation, data);
		}
		/// <summary>
		/// Calls a url and returns the contents from it as a string.
		/// </summary>
		/// <returns>The response from UR.</returns>
		/// <param name="url">URL.</param>
		public string getResponseFromURL(string url){
			www = new WWW(url);		
			while (www.isDone == false) {
				progress = www.progress;
			}
			return www.text;
		}
		//get contents of version file
		public string GetLocalVersion(string overrideLocation="") 						//return version from version
		{						
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
				return ex.Message;
			}
			return version;
		}
		/// <summary>
		/// This will scan the contents of a folder and compare it with another outdated copy of that folder
		///  and generate all the delta (patch) files needed to upgrade the old folder to the new folder. You must
		/// specify a location to save the output of the delta files.
		/// </summary>
		/// <returns>The project patches.</returns>
		public IEnumerator generateProjectPatches(string currentFolder, string outdatedFolder, string saveLocation) {
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
				currentException = ex.Message;
				return ex.Message;
			}
			currentProcess = "";
			return "Success";
		}

		//generate a delta file (patch file)
		public string generateDeltaFile(string signaturePath, string newFilePath, string saveDeltaLocation="")
		{
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
				currentException = ex.Message;
				return ex.Message;
			}
			currentProcess = "";
			return "Success";
		}

		//apply a patch file to update a local file
		public string applyPatch(string oldFilePath, string deltaFilePath, string outputFilePath="")
		{
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
				currentException = ex.Message;
				return ex.Message;
			}
			currentProcess = "";
			return "Success";
		}

		public string removeOldFiles(string directoryPath="")
		{
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

			return "Success";
		}
		//return true or false. Compare local version file with remote one
		public IEnumerator CheckForUpdates() 
		{
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
			updateAvailable = (float.Parse (line) < float.Parse (availableVersion.Replace(".","")))? true : false;//compare versions, update needed?
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