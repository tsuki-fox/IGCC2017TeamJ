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
				Debug.LogFormat("[LID:{0}]次のノードはLID:{1}が選択されました", localId, _nextNode1.localId);
			}
			else
			{
				_selected = _nextNode2;
				Debug.LogFormat("[LID:{0}]次のノードはLID:{1}が選択されました", localId, _nextNode2.localId);
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
}
