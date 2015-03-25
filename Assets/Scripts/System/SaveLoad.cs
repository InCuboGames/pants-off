using System;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.IO;
using System.Linq;

public class SaveLoad : Singleton<SaveLoad>
{
	readonly string filepath = Path.Combine (Application.persistentDataPath, GlobalDataManager.SAVE_FILE_NAME);

    #region Save variables
    public class StageRecord : IComparable<StageRecord>
    {
    	[XmlAttribute ("Stage")]
    	public int
    			Stage;

    	[XmlAttribute ("BestTime")]
    	public int
    			BestTime;

    	[XmlAttribute ("TimePin")]
    	public bool
    		TimePin;

    	[XmlAttribute ("ExposurePin")]
        public bool
    		ExposurePin;

    	[XmlAttribute ("NotesPin")]
        public bool
    		NotesPin;
    	
    	public StageRecord ()
    	{

    	}

    	public StageRecord (int stage, int bestTime, bool timePin, bool exposurePin, bool notesPin)
    	{
    			Stage = stage;
    			BestTime = bestTime;
                TimePin = timePin;
                ExposurePin = exposurePin;
                NotesPin = notesPin;
    	}
    	
         public int CompareTo (StageRecord older)
    	{
                return older.BestTime - BestTime;
    	}
    }

    	[XmlRoot("Data")]
    	public class Data
    	{
				[XmlAttribute("FirstBoot")]
    			public bool
				FirstBoot = true;

    //						[XmlAttribute("SoundMuted")]
    //						public bool
    //								SoundMuted;

    			[XmlArray("StageRecords")]
    			[XmlArrayItem("StageRecord")]
    			public List<StageRecord>
    					StageRecords;

                [XmlArray("UnlockedStages")]
    			public List<int>
    					UnlockedStages;

                 public Data ()
    			{
    			}

                public Data (bool init)
    			{
    					if (init) {
    							FirstBoot = false;
    							StageRecords = new List<StageRecord> ();

								if(GlobalDataManager.allLevelsUnlocked)
								{
									UnlockedStages = GlobalDataManager.UnlockableStages;
								}
								else
								{
									UnlockedStages = new List<int>{1};
								}
    					}
    			}
    	}
    #endregion

	public Data data;
	public Data GetData ()
	{
			return GlobalDataManager.GameData;
	}


	// Use this for initialization
	public void Awake ()
	{
			Load ();
			//data.FirstBoot = true;
			if (data.FirstBoot) {
				data = new Data (true);
				Save ();
			}
			/*
if (save.MyData == null || !save.MyData.GameStarted) 
{
	data = new Data (true);
	Save ();
}
else {
	data = save.MyData;
	//data = save.MyData;
}
*/
	}

	public void ClearData ()
	{
		data = null;
		data = new Data (true);
		Save ();
	}

	public void Save (string path)
	{		
			Debug.Log ("Trying to save...");
			try {
					var serializer = new XmlSerializer (typeof(Data));
					using (var stream = new FileStream(path, FileMode.Create)) {
							serializer.Serialize (stream, data);
							Debug.Log ("SAVED!");
					}
			} catch (Exception e) {
					Debug.LogError ("SAVE ERROR: " + e.Message);
			}
	}

	public Data Load (string path)
	{
			Debug.Log ("Trying to load...");
			try {
					var serializer = new XmlSerializer (typeof(Data));
					using (var stream = new FileStream(path, FileMode.Open)) {
							return serializer.Deserialize (stream) as Data;
							Debug.Log ("LOADED!");
					}
			} catch (Exception e) {
					var message = e.Message;
					Debug.LogError ("LOAD ERROR: " + e.Message);
					data = new Data (true);
					Save ();
					return data;
			}
	}

//	//Loads the xml directly from the given string. Useful in combination with www.text.
//	public static Data LoadFromText (string text)
//	{
//			var serializer = new XmlSerializer (typeof(Data));
//			return serializer.Deserialize (new StringReader (text)) as Data;
//	}


	public void Save ()
	{
			Save (filepath);
			
			//Save (Application.streamingAssetsPath + "\\" + GlobalDataManager.SAVE_FILE_NAME);
			/*
			var serializer = new XmlSerializer (typeof(Data));
			var stream = new FileStream (Path.Combine (Application.persistentDataPath, GlobalDataManager.SAVE_FILE_NAME), FileMode.Create);
			serializer.Serialize (stream, data);
			stream.Close ();
			*/
			//save = new XMLSaveLoad<Data> (GlobalDataManager.SAVE_FILE_NAME, data);
			//save.Save();

	}

	public void Load ()
	{
			data = Load (filepath);

			//data = Load (Application.streamingAssetsPath + "\\" + GlobalDataManager.SAVE_FILE_NAME);
			/*
			var serializer = new XmlSerializer (typeof(Data));
			var stream = new StreamReader (filepath);
			var container = serializer.Deserialize (stream) as Data;
			stream.Close ();
			*/
			//save.Load (true);
	}


	/*
	private static FileManager instance; // instace of the fileManager
	private string path; // holds the application path
	
	/// <summary>
	/// Constructor, creates an instance of fileManager if one does not exist
	/// </summary>
	/// <value>The instance.</value>
	public static FileManager Instance {
			get {
					if (instance == null) {
							instance = new GameObject ("fileManager").AddComponent ("fileManager") as FileManager;
					}
			
					return instance;
			}
	}
	
	/// <summary>
	/// Destroys the file manager instance.
	/// </summary>
	public void DestroyInstance ()
	{
			instance = null;
	}
	
	/// <summary>
	/// initializes the file manager.
	/// </summary>
	public void Initialize ()
	{
			path = Application.dataPath;
		
			print ("Path: " + path);
		
		
			// Check for and create the gamedata directory
			if (!CheckDirectory ("gamedata")) {
					CreateDirectory ("gamedata");
			}
		
			if (!CheckFile ("gamedata/saves/mysave.xml")) {
					// Create the XML file
					CreateXMLFile ("gamedata/saves", "mysave", "xml", BuildXMLData (), "encrypt");
					Debug.LogWarning ("Novo Arquivo");
			}
		
		
			ParseXMLFile ("gamedata/saves", "mysave", "xml", "encrypt");
		
		
	}
	
	/// <summary>
	/// Checks to see whether the passed directory exists, returning true of false.
	/// </summary>
	/// <returns><c>true</c>, if directory was checked, <c>false</c> otherwise.</returns>
	/// <param name="directory">Directory.</param>
	private bool CheckDirectory (string directory)
	{
			if (Directory.Exists (path + "/" + directory)) {
					return true;
			} else {
					return false;
			}
	}
	
	/// <summary>
	/// Creates a new directory.
	/// </summary>
	/// <param name="directory">Directory.</param>
	private void CreateDirectory (string directory)
	{
			if (!CheckDirectory (directory)) {
					Directory.CreateDirectory (path + "/" + directory);
			} else {
					Debug.LogError ("Error: You are trying to create the directory " + directory + " but it already exists!");
			}
	}
	
	/// <summary>
	/// Deletes the directory.
	/// </summary>
	/// <param name="directory">Directory.</param>
	private void DeleteDirectory (string directory)
	{
			if (CheckDirectory (directory)) {
					Directory.Delete (path + "/" + directory, true);
			} else {
					Debug.LogError ("Error: You are trying to delete the directory " + directory + " but it does not exist!");
			}
	}
	
	/// <summary>
	/// Moves the directory.
	/// </summary>
	/// <param name="originalDestination">Original destination.</param>
	/// <param name="newDestination">New destination.</param>
	private void MoveDirectory (string originalDestination, string newDestination)
	{
			if (CheckDirectory (originalDestination) && !CheckDirectory (newDestination)) {
					Directory.Move (path + "/" + originalDestination, path + "/" + newDestination);
			} else {
					Debug.LogError ("Error: You are trying to move a directory but it either does not exist or a folder of the same name already exists");
			}
	}
	
	/// <summary>
	/// Finds subdirectories of a given directory.
	/// </summary>
	/// <returns>The sub directories.</returns>
	/// <param name="directory">Directory.</param>
	public string[] FindSubDirectories (string directory)
	{
			string[] subDirectoryList = Directory.GetDirectories (path + "/" + directory);
			return subDirectoryList;
	}
	
	/// <summary>
	/// Returns the files within a given directory.
	/// </summary>
	/// <returns>The files.</returns>
	/// <param name="directory">Directory.</param>
	public string[] FindFiles (string directory)
	{
			string[] fileList = Directory.GetFiles (path + "/" + directory);
			return fileList;
	}
	
	/// <summary>
	/// checks to see whether a file exists
	/// </summary>
	/// <returns>The file.</returns>
	/// <param name="filePath">File path.</param>
	public bool CheckFile (string filePath)
	{
			if (File.Exists (path + "/" + filePath)) {
					return true;
			} else {
					return false;
			}
	}
	
	/// <summary>
	/// Creates a new file.
	/// </summary>
	/// <param name="directory">Directory.</param>
	/// <param name="fileName">File name.</param>
	/// <param name="fileType">File type.</param>
	/// <param name="fileData">File data.</param>
	public void CreateFile (string directory, string fileName, string fileType, string fileData)
	{
			if (CheckDirectory (directory)) {
					if (!CheckFile (directory + "/" + fileName + "." + fileType)) {
							// create the file
							File.WriteAllText (path + "/" + directory + "/" + fileName + "." + fileType, fileData);
					} else {
							Debug.LogError ("The file " + fileName + " already exists in " + path + "/" + directory);
					}
			} else {
					Debug.LogError ("Unable to create file as the directory " + directory + " does not exist");
			}
	}
	
	/// <summary>
	/// Reads a file and returns it's contents.
	/// </summary>
	/// <returns>The file.</returns>
	/// <param name="directory">Directory.</param>
	/// <param name="fileName">File name.</param>
	/// <param name="fileType">File type.</param>
	public string ReadFile (string directory, string fileName, string fileType)
	{
			if (CheckDirectory (directory)) {
					if (CheckFile (directory + "/" + fileName + "." + fileType)) {
							// read the file
							string fileContents = File.ReadAllText (path + "/" + directory + "/" + fileName + "." + fileType);
							return fileContents;
					} else {
							Debug.LogError ("The file " + fileName + " does not exist in " + path + "/" + directory);
							return null;
					}
			} else {
					Debug.LogError ("Unable to read the file as the directory " + directory + " does not exist");
					return null;
			}
	}
	
	/// <summary>
	/// Deletes a specified file.
	/// </summary>
	/// <param name="filePath">File path.</param>
	public void DeleteFile (string filePath)
	{
			if (File.Exists (path + "/" + filePath)) {
					File.Delete (path + "/" + filePath);
			} else {
					Debug.LogError ("Unable to delete file as it does not exist");
			}
	}
	
	/// <summary>
	/// Updates a files contents.
	/// </summary>
	/// <param name="directory">Directory.</param>
	/// <param name="fileName">File name.</param>
	/// <param name="fileType">File type.</param>
	/// <param name="fileData">File data.</param>
	/// <param name="mode">Mode.</param>
	public void UpdateFile (string directory, string fileName, string fileType, string fileData, string mode)
	{
			if (CheckDirectory (directory)) {
					if (CheckFile (directory + "/" + fileName + "." + fileType)) {
							if (mode == "replaced") {
									File.WriteAllText (path + "/" + directory + "/" + fileName + "." + fileType, fileData);
							} else if (mode == "append") {
									File.AppendAllText (path + "/" + directory + "/" + fileName + "." + fileType, fileData);
							}
					} else {
							Debug.LogError ("The file " + fileName + " does not exist in " + path + "/" + directory);
					}
			} else {
					Debug.LogError ("Unable to create file as the directory " + directory + " does not exist");
			}
	}
	
	/// <summary>
	/// Processes an opened file.
	/// </summary>
	/// <param name="filePath">File path.</param>
	public void ProcessFile (string filePath)
	{
			string fileContents = File.ReadAllText (filePath);
	}
	
	/// <summary>
	/// Creates a new XML file
	/// </summary>
	/// <param name="directory">Directory.</param>
	/// <param name="fileName">File name.</param>
	/// <param name="fileType">File type.</param>
	/// <param name="fileData">File data.</param>
	/// <param name="mode">Mode.</param>
	public void CreateXMLFile (string directory, string fileName, string fileType, string fileData, string mode)
	{	
			if (CheckDirectory (directory) == true) {
					if (mode == "plaintext") {
							File.WriteAllText (path + "/" + directory + "/" + fileName + "." + fileType, fileData);
					}
			
					if (mode == "encrypt") {
							fileData = EncryptData (fileData);
							File.WriteAllText (path + "/" + directory + "/" + fileName + "." + fileType, fileData);
					}
			} else {
					Debug.LogError ("Unable to create file as the directory " + directory + " does not exist");
			}
	}

	public class StageRecord : IComparable<StageRecord>
	{
			[XmlAttribute ("PlayerName")]
			public string
					PlayerName;

			[XmlAttribute ("Score")]
			public int
					Score;

			public int CompareTo (StageRecord other)
			{
					return other.Score - Score;
			}
	}

	[XmlRoot("Data")]
	public class Data
	{
			[XmlAttribute("GameStarted")]
			public bool
					GameStarted;
			[XmlAttribute("SoundMuted")]
			public bool
					SoundMuted;

			[XmlElement("StageRecord")]
			[XmlArrayItem("StageRecords")]
			public List<StageRecord>
					StageRecords;

			[XmlAttribute("EarnedCoins")]
			public int
					EarnedCoins;

			[XmlArrayItem("BoughtItens")]
			public List<int>
					BoughtItens;

			[XmlAttribute("EquippedFlower")]
			public int
					EquippedFlower;

			[XmlAttribute("EquippedHoneyPot")]
			public int
					EquippedHoneyPot;

			[XmlAttribute("AdsRemoved")]
			public bool
					AdsRemoved;

			public Data ()
			{
			}
			public Data (bool init)
			{
					if (init) {
							GameStarted = true;
							StageRecords = new List<StageRecord> ();
							BoughtItens = new List<int>{0, 13};
							EquippedFlower = 0;
							EquippedHoneyPot = 13;
							SoundMuted = false;
							EarnedCoins = 0;
							AdsRemoved = false;
					}
			}
	
			/// <summary>
			/// Creates and returns some XML data.
			/// </summary>
			/// <returns>The XML data.</returns>
			public string BuildXMLData ()
			{
					// Create a variable called xml as a new XmlDocument object
					XmlDocument xml = new XmlDocument ();
		
					// Create player element
					XmlElement rootElement = CreateXmlElement (xml, "SaveGame", "", null, null);		
					xml.AppendChild (rootElement);
		
					XmlElement levelRecords = CreateXmlElement (xml, "LevelRecords", "", null, null);

					foreach (var item in Data.StageRecords) {
							// Create profile element
							XmlElement rankData = CreateXmlElement (xml, "LevelRecord", "", null, null);	
			
							// Add the child elements	
							profileElement.AppendChild (CreateXmlElement (xml, "Level", item.Level, null, null));
							profileElement.AppendChild (CreateXmlElement (xml, "Stars", item.Stars, null, null));
							// Apply the profile element to the root element (player)	
							StageRecords.AppendChild (rankData);
					}
					rootElement.AppendChild (StageRecords);
		
		
					// Create inventory element
					XmlElement inventoryElement = CreateXmlElement (xml, "inventory", "", null, null);
		
					// Add the inventory items			
					inventoryElement.AppendChild (CreateXmlElement (xml, "item", "", new string[] { "name", "qty"}, new string[] {
							"item 1",
							"2"
					}));
					inventoryElement.AppendChild (CreateXmlElement (xml, "item", "", new string[] { "name", "qty"}, new string[] {
							"item 2",
							"11"
					}));
					inventoryElement.AppendChild (CreateXmlElement (xml, "item", "", new string[] { "name", "qty"}, new string[] {
							"item 3",
							"1"
					}));
					inventoryElement.AppendChild (CreateXmlElement (xml, "item", "", new string[] { "name", "qty"}, new string[] {
							"item 4",
							"5"
					}));
		
					rootElement.AppendChild (inventoryElement);
		
					return xml.OuterXml;
		
			}
	
			/// <summary>
			/// Parses the XML file.
			/// </summary>
			/// <param name="directory">Directory.</param>
			/// <param name="filename">Filename.</param>
			/// <param name="filetype">Filetype.</param>
			/// <param name="mode">Mode.</param>
			public void ParseXMLFile (string directory, string fileName, string fileType, string mode)
			{
					XmlDocument xmlDoc = new XmlDocument (); 
		
					// Read plain text XML file
					if (mode == "plaintext") {
							xmlDoc.Load (path + "/" + directory + "/" + fileName + "." + fileType); 
					}
		
					if (mode == "encrypt") {			
							// Read the encrypted file into filedata
							string filedata = ReadFile (directory, fileName, fileType);
			
							// Decrypt the data
							filedata = DecryptData (filedata);
			
							// Create a temporary file
							CreateFile (directory + "/", "tmp_" + fileName, fileType, filedata);
			
							// Read the temporary file
							xmlDoc.Load (path + "/" + directory + "/tmp_" + fileName + "." + fileType); 	
					}
		
		
					// First we will parse the profile data
					XmlNodeList profileList = xmlDoc.GetElementsByTagName ("profile");
		
					foreach (XmlNode profileInfo in profileList) {
							XmlNodeList profileContent = profileInfo.ChildNodes;
			
							foreach (XmlNode profileItems in profileContent) {
									if (profileItems.Name == "playername") {					
											print ("playername: " + profileItems.InnerText);
									}
									if (profileItems.Name == "hp") {					
											print ("hp: " + profileItems.InnerText);
									}
									if (profileItems.Name == "mp") {					
											print ("mp: " + profileItems.InnerText);
									}
									if (profileItems.Name == "level") {					
											print ("level: " + profileItems.InnerText);
									}
							}
					}
		
					// Secondly we will parse the inventory data
					XmlNodeList inventoryList = xmlDoc.GetElementsByTagName ("inventory");
		
					foreach (XmlNode inventoryInfo in inventoryList) {
							XmlNodeList inventoryContent = inventoryInfo.ChildNodes;
			
							foreach (XmlNode inventoryItems in inventoryContent) {
									print ("Item: " + inventoryItems.Attributes ["name"].Value + " x" + inventoryItems.Attributes ["qty"].Value);
							}	
					}
		
		
					// Delete the temporary file
					if (mode == "encrypt") {
							DeleteFile (directory + "/" + "tmp_" + fileName + "." + fileType);
					}
			}
	
			/// <summary>
			/// Creates an XML Element.
			/// </summary>
			/// <returns>The xml element.</returns>
			/// <param name="xmlObject">Xml object.</param>
			/// <param name="elementName">Element name.</param>
			/// <param name="innerValue">Inner value.</param>
			/// <param name="attributeList">Attribute list.</param>
			/// <param name="attributeValues">Attribute values.</param>
			private XmlElement CreateXmlElement (XmlDocument xmlObject, string elementName, string innerValue, 
	                                    string[] attributeList, string[] attributeValues)
			{
					XmlElement element = xmlObject.CreateElement (elementName);
		
					int i = 0;
		
					if (innerValue != null) {
							element.InnerText = innerValue;
					}
		
					if (attributeList != null) {
							foreach (string attribute in attributeList) {
									element.SetAttribute (attribute, attributeValues [i]);
				
									i++;
							}
					}
		
					return element;
			}
	
			/// <summary>
			/// Encrypts the data.
			/// </summary>
			/// <returns>The data.</returns>
			/// <param name="toEncrypt">To encrypt.</param>
			public string EncryptData (string toEncrypt)
			{
					byte[] keyArray = UTF8Encoding.UTF8.GetBytes ("12345678901234567890123456789012");
		
					// 256-AES key
					byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes (toEncrypt);
					RijndaelManaged rDel = new RijndaelManaged ();
		
					rDel.Key = keyArray;
					rDel.Mode = CipherMode.ECB;
		
					rDel.Padding = PaddingMode.PKCS7;
		
					ICryptoTransform cTransform = rDel.CreateEncryptor ();
					byte[] resultArray = cTransform.TransformFinalBlock (toEncryptArray, 0, toEncryptArray.Length);
		
					return Convert.ToBase64String (resultArray, 0, resultArray.Length);
			}
	
			/// <summary>
			/// Decrypts the data.
			/// </summary>
			/// <returns>The data.</returns>
			/// <param name="toDecrypt">To decrypt.</param>
			public string DecryptData (string toDecrypt)
			{
					byte[] keyArray = UTF8Encoding.UTF8.GetBytes ("12345678901234567890123456789012");
		
					// AES-256 key
					byte[] toEncryptArray = Convert.FromBase64String (toDecrypt);
					RijndaelManaged rDel = new RijndaelManaged ();
					rDel.Key = keyArray;
					rDel.Mode = CipherMode.ECB;
		
		
					rDel.Padding = PaddingMode.PKCS7;
		
					// better lang support
					ICryptoTransform cTransform = rDel.CreateDecryptor ();
		
					byte[] resultArray = cTransform.TransformFinalBlock (toEncryptArray, 0, toEncryptArray.Length);
		
					return UTF8Encoding.UTF8.GetString (resultArray);
			}				*/
}




