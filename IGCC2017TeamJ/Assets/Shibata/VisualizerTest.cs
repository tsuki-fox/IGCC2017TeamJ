using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FlowAI;

public class VisualizerTest : MonoBehaviour
{
	FlowAIBasis _flowAI;

	ProcessNode proc1;
	ProcessNode proc2;
	ProcessNode proc3;
	ProcessNode proc4;
	ProcessNode proc5;

	BranchNode rand1;

	void Start()
	{
		_flowAI = GetComponent<FlowAIHolder>().flowAI;

		proc1 = new ProcessNode();
		proc2 = new ProcessNode();
		proc3 = new ProcessNode();
		proc4 = new ProcessNode();
		proc5 = new ProcessNode();

		rand1 = new BranchNode();

		proc1.Initialize(1.0f, rand1, () => TFDebug.Log("visualizer", "proc1 finished"),"PROCESS1");
		proc2.Initialize(1.0f, proc3, () => TFDebug.Log("visualizer", "proc2 finished"),"PROCESS2");
		proc3.Initialize(1.0f, proc1, () => TFDebug.Log("visualizer", "proc3 finished"),"PROCESS3");
		proc4.Initialize(1.0f, proc5, () => TFDebug.Log("visualizer", "proc4 finished"),"PROCESS4");
		proc5.Initialize(1.0f, proc1, () => TFDebug.Log("visualizer", "proc5 finished"),"PROCESS5");

		rand1.Initialize(proc2, 1.0f, proc4, 1.0f, () =>
		{
			bool result = Random.Range(0, 2) == 0;
			TFDebug.Log("visualizer", "rand1 {0}", result.ToString());
			return result;
		});

		rand1.summary = "RANDOM";

		_flowAI.AddNode(proc1, proc2, proc3, proc4, proc5, rand1);
		_flowAI.entryPointNode.nextNode = proc1;
		_flowAI.Entry();
	}

	// Update is called once per frame
	void Update()
	{
		_flowAI.Update(Time.deltaTime);
	}
}