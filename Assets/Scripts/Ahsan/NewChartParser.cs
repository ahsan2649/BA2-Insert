using System;
using System.Collections.Generic;
using Ahsan.ScriptableObjects;
using UnityEngine;

namespace Ahsan
{
	public class NewChartParser : MonoBehaviour
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

		// public GameObject Conductor;
		public NewConductor Conductor;
		public List<Lane> Lanes;

		// This should be an editable setting with range like 5-30
		const float scrollSpeed = 20;

		private Chart currentChart;
		
		int noteIndex = 0;

		public void SetCurrentChart(SongChartPair songChartPair)
		{
			currentChart = songChartPair.chart;
			noteIndex = 0;
		}

		// Update is called once per frame
		void Update()
		{
			if (!currentChart)
			{
				print("no chart");
				return;
			}
			var note = currentChart.notes[Mathf.Min(noteIndex, currentChart.notes.Length - 1)];
			var lane = Lanes[note.lane % Lanes.Count];

			if (note.songPosition <= Conductor.songPosition + (scrollSpeed*100)){
				lane.SpawnNote(scrollSpeed/10);
				note.hasSpawned = true;
				noteIndex++;
			}
		}
	}
}
