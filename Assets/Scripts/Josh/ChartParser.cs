using System;
using System.IO;
using System.Linq;
using Ahsan;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ChartParser : MonoBehaviour
{
	/*
	looks at audio manager
	looks at chart file
	looks at active lanes

	looks at global settings for scroll speed?
	this determines travel time (scroll speed * lane length = travel time)

	json object has song position for when it should be HIT
	hit time = spawn time + travel time

	when audiomanager.songpostion = hit time - travel time: spawn note

	each lane can have its own travel time?
	how is travel time handled during frame drops? will it keep time? if unity is good this shouldnt be an issue :3

	I dont want to check the entire json file each frame, I know that the notes are in time order with some having the same time next to eachother

	I'm checking the first object in the list, if it spawns, remove it from the list and check again
	Problem. if lane 1 is slower than lane 2 its note should be spawnned earlier yet being pressed later than lane 2 should be, hense a desync. 
	Solution. check the first object for each lane
	I dont think i like this still but it is better



	Lanes are dynamic, their length will drasticly change during gameplay, if a note is traveling WHILE the length is changing, does it hit on time?
	This needs to be tested?


	When transitioning between charts it should load a new chart manager with the associated chart when the decision is made?
	That should take place in the conductor object not here ill worry about that later

	*/

	public GameObject Conductor;
	public GameObject Lanes;

	public TextAsset chartFile;

	[System.Serializable]
	public class Note
	{
		/*
			in the chart file it currently looks like this:
			{ "lane": 0.0, "songPosition": 44218, "isHold": false, "holdLength": 0 },

			when more features are needed, such as seeing the type of note and if its part of a decision will be mentioned as part of this object
		*/
		public int lane;
		public float songPosition;
		public bool isHold;
		public float holdLength;

		public bool hasSpawned = false;
	}

	[System.Serializable]
	public class ChartObject
	{
		// Head of json object

		public Note[] notes;
		// difficulty and other data would be here if needed
	}



	public ChartObject CurrentChart = new ChartObject();
	const float scrollSpeed = 20;

	void Start()
	{
		CurrentChart = JsonUtility.FromJson<ChartObject>(chartFile.text);
		// this is likely to cause lag when it happens, if there is a better way to do this please change it
    }


	// Update is called once per frame
	void Update()
	{
		// Get the next note for each lane
		// for (int i = 0; i < Lanes.transform.childCount; i++)
		// {
		// 	foreach (var note in CurrentChart.notes)
		// 	{
		// 		if (note.lane == i)
		// 		{
		// 			nextNotes.Append(note);
		// 			break;
		// 		}
		// 	}
		// }

		foreach (var note in CurrentChart.notes){
			if (note.hasSpawned){
				continue;
			}

			var lane = Lanes.transform.GetChild(note.lane % Lanes.transform.childCount).gameObject.GetComponent<Lane>();
			var laneDistance = lane.GetSplineLength();
			var travelTime = laneDistance / scrollSpeed; // 2 seconds to reach the judgement line

			if (note.songPosition <= Conductor.GetComponent<Conductor>().songPosition + 2000){
				lane.SpawnNote(travelTime);
				note.hasSpawned = true;
				// I would like to just delete the note object at this point but I am itterating through the list
			}
		}

	}
}
