using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using FlowAI;
using System;

public class FlowAITest
{
	public class DummyNode : FlowAINode
	{
		FlowAINode _nextNode;
		public FlowAINode nextNode
		{
			get { return _nextNode; }
			set { _nextNode = value; }
		}

		public override void Processing()
		{
			Debug.LogFormat("[NODE]ダミープロセス完了(LID:{0})", localId);
		}

		public override FlowAINode GetNextNode()
		{
			return _nextNode;
		}
	}

	public class RandomBranchNode : FlowAINode
	{
		FlowAINode _nextNode1;
		public FlowAINode nextNode1
		{
			get { return _nextNode1; }
			set { _nextNode1 = value; }
		}

		FlowAINode _nextNode2;
		public FlowAINode nextNode2
		{
			get { return _nextNode2; }
			set { _nextNode2 = value; }
		}

		FlowAINode _selected;

		public override void Processing()
		{
			bool flg = UnityEngine.Random.Range(0, 1) == 0;

			if (flg)
			{
				_selected = _nextNode1;
				//Debug.LogFormat("[LID:{0}]次のノードはLID:{1}が選択されました", localId, _nextNode1.localId);
				Debug.LogFormat("[LID:{0}]Decision: next node >> LID:{1}", localId, _nextNode1.localId);
			}
			else
			{
				_selected = _nextNode2;
				Debug.LogFormat("[LID:{0}]Decision: next node >> LID:{1}", localId, _nextNode2.localId);
			}
		}

		public override FlowAINode GetNextNode()
		{
			return _selected;
		}
	}

	[Test]
	public void BasisTest()
	{
		Debug.Log("---initial---");

		var basis = new FlowAIBasis();

		var dummy1 = new DummyNode();
		var dummy2 = new DummyNode();
		var dummy3 = new DummyNode();
		var branch = new RandomBranchNode();

		dummy1.duration = 0.5f;
		dummy1.nextNode = branch;

		branch.duration = 0.2f;
		branch.nextNode1 = dummy2;
		branch.nextNode2 = dummy3;

		dummy2.duration = 0.5f;
		dummy2.nextNode = dummy1;

		dummy3.duration = 0.5f;
		dummy3.nextNode = dummy1;

		basis.AddNode(dummy1);
		basis.AddNode(branch);
		basis.AddNode(dummy2);
		basis.AddNode(dummy3);

		basis.entryPointNode.nextNode = dummy1;


		basis.Entry();
		for(int f1=0;f1<30;f1++)
		{
			Debug.Log("\n");
			Debug.LogFormat("---{0}[sec]---", (f1+1) * 0.1f);
			basis.Update(0.1f);
		}
	}

	[Test]
	public void ProcessNodeTest()
	{
		var basis = new FlowAIBasis();

		var proc1 = new ProcessNode();
		var proc2 = new ProcessNode();
		var proc3 = new ProcessNode();
		var terminal = new ProcessNode();
		var branch = new RandomBranchNode();

		proc1.Initialize(0.3f, branch, () => Debug.Log("completed process1"));


		branch.nextNode1 = proc2;
		branch.nextNode2 = proc3;
		proc2.Initialize(0.1f, terminal, () => Debug.Log("completed process2"));
		proc3.Initialize(0.2f, terminal, () => Debug.Log("completerd process3"));
		terminal.Initialize(0.0f, null);
	
		basis.AddNode(proc1, proc2, proc3, terminal, branch);

		basis.entryPointNode.nextNode = proc1;

		basis.Entry();

		for(int f1=0;f1<50;f1++)
		{
			Debug.LogFormat("---{0}sec---", (f1 + 1) * 0.1f);
			basis.Update(0.1f);
		}
	}

	[Test]
	public void BranchNodeTest()
	{
		float elapsed = 0f;

		var basis = new FlowAIBasis();

		var branch = new BranchNode();
		var proc1 = new ProcessNode();
		var proc2 = new ProcessNode();

		branch.Initialize(
			proc1, 0.2f,
			proc2, 0.2f,
			() =>
			{
				return elapsed < 0.5f;
			});

		proc1.Initialize(0.2f, branch, () => Debug.Log("プロセス1完了"));
		proc2.Initialize(0.2f, branch, () => Debug.Log("プロセス2完了"));

		basis.entryPointNode.nextNode = branch;
		basis.AddNode(branch, proc1, proc2);

		basis.Entry();

		for(int f1=0;f1<50;f1++)
		{
			elapsed += 0.1f;
			Debug.LogFormat("---{0:0.0}sec---", elapsed);
			basis.Update(0.1f);
		}
	}
}
