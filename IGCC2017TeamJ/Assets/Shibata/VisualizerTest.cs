using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FlowAI;

public class VisualizerTest : MonoBehaviour
{
	FlowAIBasis _flowAI;

	void Start ()
	{
		_flowAI = GetComponent<FlowAIHolder>().flowAI;

		var proc1 = new ProcessNode();
		var proc2 = new ProcessNode();
		var proc3 = new ProcessNode();

		var rand1 = new BranchNode();
		var rand2 = new BranchNode();

		proc1.Initialize(1.0f, rand1, () =>TFDebug.Log("visualizer","proc1 finished"));
		proc2.Initialize(1.0f, rand2, () => TFDebug.Log("visualizer", "proc2 finished"));
		proc3.Initialize(1.0f, proc1, () => TFDebug.Log("visualizer", "proc3 finished"));

		rand1.Initialize(proc2, 1.0f, proc3, 1.0f, () =>
		{
			bool result = Random.Range(0, 2) == 0;
			TFDebug.Log("visualizer", "rand1 {0}", result.ToString());
			return result;
		});
		rand2.Initialize(proc1, 1.0f, rand2, 1.0f, () =>
		{
			bool result = Random.Range(0, 2) == 0;
			TFDebug.Log("visualizer", "rand2 {0}", result.ToString());
			return result;
		});

		proc1.summary = "プロセス1";
		proc2.summary = "プロセス2";
		proc3.summary = "プロセス3";

		rand1.summary = "ランダム分岐1";
		rand2.summary = "ランダム分岐2";

		_flowAI.AddNode(proc1, proc2, proc3, rand1, rand2);
		_flowAI.entryPointNode.nextNode = proc1;
		_flowAI.Entry();
	}
	
	// Update is called once per frame
	void Update ()
	{
		_flowAI.Update(Time.deltaTime);	
	}
}
