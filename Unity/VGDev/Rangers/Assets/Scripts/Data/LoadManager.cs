using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Runtime.Serialization.Formatters.Binary;
using Assets.Scripts.Util;

namespace Assets.Scripts.Data
{
    /// <summary>
    /// Class will load in data from player prefs or from disk
    /// </summary>
    public class LoadManager : DataManager
	{
        /// <summary>
        ///  Use a singleton instance to make sure there is only one
        /// </summary>
		public static LoadManager instance;

        // Sets up singleton instance. Will remain if one does not already exist in scene
		protected override void Init()
		{
			if(instance == null)
			{
				DontDestroyOnLoad(gameObject);
				instance = this;
			}
			else if (instance != this)
			{
				Destroy(gameObject);
			}
		}

        /// <summary>
        /// Loads the audio data for this game
        /// </summary>
        /// <returns>The audio settings saved to disk or player prefs</returns>
		public static AudioData LoadAudio()
		{
            // Get a default audiodata in case none exists
			AudioData data = new AudioData();
#if UNITY_WEBPLAYER
            // If there is data related to audio, get those values
			if(PlayerPrefs.HasKey(audioHash + 0))
			{
				data.SFXVol = PlayerPrefs.GetFloat(audioHash + 0);
				data.MusicVol = PlayerPrefs.GetFloat(audioHash + 1);
                data.VoiceVol = PlayerPrefs.GetFloat(audioHash + 2);
			}
            // Otherwise save the default data
			else
			{
				SaveManager.SaveAudio(new AudioData());
			}
			return data;
#else
            // If a file exists
            if (File.Exists(audioDataPath))
			{
                // Open the file and get all the data from the file to load
				BinaryFormatter bf = new BinaryFormatter();
				FileStream file = File.Open(audioDataPath, FileMode.Open);

				data = (AudioData)bf.Deserialize(file);

				file.Close();

				return data;
			}
            // No file exists, so save the default data
			else
			{
				SaveManager.SaveAudio(data);
				return data;
			}
#endif
		}

        /// <summary>
        /// Loads the video data for this game
        /// </summary>
        /// <returns>The video settings saved to disk or player prefs</returns>
		public static VideoData LoadVideo()
		{
            // Get a default videodata in case none exists
            VideoData data = new VideoData();
#if UNITY_WEBPLAYER
            // If there is data related to video, get those values
			if(PlayerPrefs.HasKey(videoHash + 0))
			{
				data.ResolutionIndex = PlayerPrefs.GetInt(videoHash + 0);
				data.QualityIndex = PlayerPrefs.GetInt(videoHash + 1);
				int full = PlayerPrefs.GetInt(videoHash + 2);
				data.Fullscreen = (full == 1) ? true : false;
			}
            // Otherwise save the default data
			else
			{
				SaveManager.SaveVideo(new VideoData());
			}
			return data;
#else
            // If a file exists
            if (File.Exists(videoDataPath))
			{
				BinaryFormatter bf = new BinaryFormatter();
				FileStream file = File.Open(videoDataPath, FileMode.Open);
				
				data = (VideoData)bf.Deserialize(file);
				
				file.Close();
				
				return data;
			}
            // No file exists, so save the default data
            else
            {
				SaveManager.SaveVideo(data);
				return data;
			}
#endif
		}

        /// <summary>
        /// Loads the game settings from a certain file
        /// </summary>
        /// <param name="extension">The extension where the desired settings are saved</param>
        /// <returns>The certain game settings at a specific location</returns>
        public static GameSettings LoadGameSettings(string extension)
        {
            // Get a default settings in case none exists
            GameSettings data = new GameSettings();
            // If a file exists
            Debug.Log(settingsDataPath + extension);
            if (File.Exists(settingsDataPath + extension))
            {
                // Open the file and get all the data from the file to load
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(settingsDataPath + extension, FileMode.Open);

                data = (GameSettings)bf.Deserialize(file);
                file.Close();

                return data;
            }
            // No file exists, so save the default data
            else
            {
                SaveManager.SaveGameSettings(data, extension);
                return data;
            }            
        }

        /// <summary>
        /// Gets a file of game settings from the file name
        /// </summary>
        /// <param name="extension">Where the file is located</param>
        /// <returns>The game settings from resources</returns>
        public static GameSettings LoadGameSettingsXML(string extension)
        {
            // Get a default settings in case none exists
            GameSettings data = new GameSettings();

            TextAsset xmlFile = (TextAsset)Resources.Load(xmlSettingsDataPath + extension);
			Debug.Log(xmlFile);
			if(xmlFile != null) {
	            MemoryStream assetStream = new MemoryStream(xmlFile.bytes);
	            XmlReader reader = XmlReader.Create(assetStream);

	            bool endofSettings = false;
	            while (reader.Read() && !endofSettings)
	            {
	                if (reader.IsStartElement())
	                {
	                    switch (reader.LocalName)
	                    {
	                        case "Type":
	                            data.Type = (Enums.GameType)System.Enum.Parse(typeof(Enums.GameType), reader.ReadElementContentAsString());
	                            break;
	                        case "TimeLimitEnabled":
	                            data.TimeLimitEnabled = reader.ReadElementContentAsBoolean();
	                            break;
	                        case "TimeLimit":
	                            data.TimeLimit = reader.ReadElementContentAsFloat();
	                            break;
	                        case "KillLimit":
	                            data.KillLimit = reader.ReadElementContentAsFloat();
	                            break;
	                        case "StockLimit":
	                            data.StockLimit = reader.ReadElementContentAsFloat();
	                            break;
	                        case "ArrowLimit":
	                            data.ArrowLimit = reader.ReadElementContentAsFloat();
	                            break;
	                        case "DamageModifier":
	                            data.DamageModifier = reader.ReadElementContentAsFloat();
	                            break;
	                        case "GravityModifier":
	                            data.GravityModifier = reader.ReadElementContentAsFloat();
	                            break;
	                        case "SpeedModifier":
	                            data.SpeedModifier = reader.ReadElementContentAsFloat();
	                            break;
	                        case "TokenSpawnFreq":
	                            data.TokenSpawnFreq = reader.ReadElementContentAsFloat();
	                            break;
	                        case "PlayerSpawnFreq":
	                            data.PlayerSpawnFreq = reader.ReadElementContentAsFloat();
	                            break;
	                        case "EnabledTokens":
	                            Dictionary<Enums.Tokens, Enums.Frequency> dict = new Dictionary<Enums.Tokens, Enums.Frequency>();
	                            XmlReader inner = reader.ReadSubtree();
	                            while (inner.Read())
	                            {
	                                if (inner.IsStartElement())
	                                {
	                                    if (inner.LocalName.Equals("Token"))
	                                    {
	                                        Enums.Tokens t = (Enums.Tokens)System.Enum.Parse(typeof(Enums.Tokens), inner.ReadElementContentAsString());
	                                        inner.ReadToFollowing("Frequency");
	                                        Enums.Frequency f = (Enums.Frequency)System.Enum.Parse(typeof(Enums.Frequency), inner.ReadElementContentAsString());
	                                        dict.Add(t, f);
	                                    }
	                                }
	                            }
	                            data.EnabledTokens = dict;
	                            inner.Close();
	                            break;
	                        case "DefaultTokens":
	                            List<Enums.Tokens> defaultTokens = new List<Enums.Tokens>();
	                            XmlReader tokens = reader.ReadSubtree();
	                            while (tokens.Read())
	                            {
	                                if (tokens.IsStartElement())
	                                {
	                                    if (tokens.LocalName.Equals("Token"))
	                                    {
	                                        Enums.Tokens t = (Enums.Tokens)System.Enum.Parse(typeof(Enums.Tokens), tokens.ReadElementContentAsString());
	                                        defaultTokens.Add(t);
	                                    }
	                                }
	                            }
	                            data.DefaultTokens = defaultTokens;
	                            tokens.Close();
	                            break;
	                        case "GameSettings":
	                            break;
	                        default:
	                            endofSettings = true;
	                            break;
	                    }
	                }
	            }
	            reader.Close();
			}
            return data;
        }

        /// <summary>
        /// Gets a file of target settings from the file name
        /// </summary>
        /// <param name="extension">Where the file is located</param>
        /// <returns>The game settings from resources</returns>
        public static TargetSettings LoadTargetSettingsXML(string extension)
        {
            // Get a default settings in case none exists
            TargetSettings data = new TargetSettings();
            Debug.Log(xmlSettingsDataPath + extension);
            TextAsset xmlFile = (TextAsset)Resources.Load(xmlSettingsDataPath + extension);
            MemoryStream assetStream = new MemoryStream(xmlFile.bytes);
            XmlReader reader = XmlReader.Create(assetStream);

            bool endofSettings = false;
            while (reader.Read() && !endofSettings)
            {
                if (reader.IsStartElement())
                {
                    switch (reader.LocalName)
                    {
                        case "Platinum":
                            data.PlatinumTime = reader.ReadElementContentAsFloat();
                            break;
                        case "Gold":
                            data.GoldTime = reader.ReadElementContentAsFloat();
                            break;
                        case "Silver":
                            data.SilverTime = reader.ReadElementContentAsFloat();
                            break;
                        case "Bronze":
                            data.BronzeTime = reader.ReadElementContentAsFloat();
                            break;
                        case "TargetSettings":
                            break;
                        default:
                            endofSettings = true;
                            break;
                    }
                }
            }
            reader.Close();
            return data;
        }
    }
}