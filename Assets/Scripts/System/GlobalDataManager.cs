using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public static class GlobalDataManager
{
//		public enum Languages
//		{
//				English,
//				Portuguese,
//		}
	
		public const string SAVE_FILE_NAME = "PantsOffData.xml";
		public static bool initializeDataFile = true;
		public const bool enableSaving = true;
		public const bool allLevelsUnlocked = true;
		public const bool enableCheats = true;
		//public static AdsManager adsManager = AdsManager.Instance;

		public static List<int> UnlockableStages = new List<int>{1, 3, 5, 6, 7, 8, 10, 13, 14, 15, 16, 21};

        public static SaveLoad.Data GameData { get { return SaveLoad.Instance.data; } }


//		public static readonly int LanguagesCount = Enum.GetNames (typeof(Languages)).Length;
//		public static int ActiveLanguage = (int)Enum.Parse (typeof(Languages), Application.systemLanguage.ToString ());
}