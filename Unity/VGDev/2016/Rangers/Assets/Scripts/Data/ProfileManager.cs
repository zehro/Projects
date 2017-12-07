using UnityEngine;
using System.Collections.Generic;

public class ProfileManager : MonoBehaviour {

	private Dictionary<PlayerID,ProfileData> loadedProfiles;

	public static ProfileManager instance;

	// Use this for initialization
	void Start () {
		if(instance == null) {
			instance = this;
			loadedProfiles = new Dictionary<PlayerID, ProfileData>();
			DontDestroyOnLoad(this.gameObject);
		} else if (instance != this) {
			Destroy(this);
		}
	}

	public void AddProfile(ProfileData data, PlayerID id = PlayerID.One) {
		loadedProfiles.Add(id,data);
	}

	/// <summary>
	/// Removes the profile with the specified player ID.
	/// </summary>
	/// <param name="id">The ID of the player to remove.</param>
	public void RemoveProfile(PlayerID id) {
		loadedProfiles.Remove(id);
		int numSignedIn = NumSignedIn();
		if ((int)id <= numSignedIn) {
			// Shift all players after the removed player back one place.
			for (int i = (int)id + 1; i <= numSignedIn + 1; i++) {
				PlayerID currentID = (PlayerID)i;
				ProfileData data = GetProfile(currentID);
				loadedProfiles.Remove(currentID);
				AddProfile(data, (PlayerID)(i - 1));
			}
		}
	}

	/// <summary>
	/// Shifts profiles forward by one. Evicts the fourth player if any.
	/// </summary>
	public void ShiftProfiles() {
		for (int i = NumSignedIn(); i > 0; i--) {
			PlayerID currentID = (PlayerID)i;
			ProfileData data = GetProfile(currentID);
			loadedProfiles.Remove(currentID);
			if (i < 4) {
				AddProfile(data, (PlayerID)(i + 1));
			}
		}
	}

	public ProfileData GetProfile(PlayerID id) {
		return loadedProfiles[id];
	}

	public bool ProfileExists(PlayerID id) {
		return loadedProfiles.ContainsKey(id);
	}

    public int NumSignedIn()
    {
        return loadedProfiles.Count;
    }
}
