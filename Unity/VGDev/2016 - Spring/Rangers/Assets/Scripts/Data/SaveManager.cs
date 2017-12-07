using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Assets.Scripts.Data
{
    /// <summary>
    /// This class will save all the data for the game
    /// </summary>
    public class SaveManager : DataManager
	{
        /// <summary>
        /// Use a singleton instance to make sure there is only one
        /// </summary>
        public static SaveManager instance;

        // Sets up singleton instance. Will remain if one does not already exist in scene
        protected override void Init()
		{
			if(instance == null)
			{
//				DontDestroyOnLoad(this);
				instance = this;
			}
			else if(instance != this)
			{
				Destroy(gameObject);
			}
		}

        /// <summary>
        /// Saves the audio settings for this game
        /// </summary>
        /// <param name="data">The data to be saved</param>
		public static void SaveAudio(AudioData data)
		{
#if UNITY_WEBPLAYER
            // Set appropriate hash values
			PlayerPrefs.SetFloat(audioHash + 0, data.SFXVol);
			PlayerPrefs.SetFloat(audioHash + 1, data.MusicVol);
            PlayerPrefs.SetFloat(audioHash + 2, data.VoiceVol);
#else
            // Create a new save file
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Create(audioDataPath);

			bf.Serialize(file, data);
			file.Close();
#endif
		}

        /// <summary>
        /// Saves the video settings for this game
        /// </summary>
        /// <param name="data">The data to be saved</param>
        public static void SaveVideo(VideoData data)
		{
#if UNITY_WEBPLAYER
            // Set appropriate hash values
			PlayerPrefs.SetInt(videoHash + 0, data.ResolutionIndex);
			PlayerPrefs.SetInt(videoHash + 1, data.QualityIndex);
			int full = data.Fullscreen ? 1 : 0;
			PlayerPrefs.SetInt(videoHash + 2, full);
#else
            // Create a new save file
            BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Create(videoDataPath);
			
			bf.Serialize(file, data);
			file.Close();
#endif
		}

        /// <summary>
        /// Saves the Game settings for this game
        /// </summary>
        /// <param name="data">The data to be saved</param>
        /// <param name="extension">The name of the path/file to save</param>
        public static void SaveGameSettings(GameSettings data, string extension)
        {
            // Create a new save file
            if (!Directory.Exists(settingsDataPath)) Directory.CreateDirectory(settingsDataPath);
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(settingsDataPath + extension);

            bf.Serialize(file, data);
            file.Close();

			bf = new BinaryFormatter();
			file = File.Open(settingsDataPath + extension, FileMode.Open);

			data = (GameSettings)bf.Deserialize(file);
			file.Close();
        }
    }
}
