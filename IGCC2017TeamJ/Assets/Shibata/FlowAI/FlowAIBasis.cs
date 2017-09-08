using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Linq;

namespace FlowAI
{
	public class FlowAIBasis
	{
		#region inner class
		/// <summary>FlowAIのエントリポイント用ノード</summary>
		public class EntryPointNode : FlowAINode
		{
			FlowAINode _nextNode;
			public FlowAINode nextNode
			{
				get { return _nextNode; }
				set { _nextNode = value; }
			}

			public EntryPointNode()
			{
				_nextNode = null;
			}

			public override FlowAINode GetNextNode()
			{
				return _nextNode;
			}
			public override void Processing()
			{
				Debug.LogFormat("[NODE]エントリ完了(LID:{0})", localId);
			}
		}


		#endregion

		#region private fields
		int _idCount;   //ID生成用カウンタ
		List<FlowAINode> _nodes;    //ノードリスト
		EntryPointNode _entryPoint;	//エントリポイント
		FlowAINode _currentNode;    //現在のノード

		float _elapsed;
		bool _isStopped;
		#endregion

		public EntryPointNode entryPointNode
		{
			get { return _entryPoint; }
		}


		#region ctor
		public FlowAIBasis()
		{
			_idCount = 0;
			_nodes = new List<FlowAINode>();

			//エントリポイント生成
			_entryPoint = new EntryPointNode();
			_entryPoint.duration = 0f;
			AddNode(_entryPoint);
			_currentNode = _entryPoint;

			_elapsed = 0f;
			_isStopped = false;
		}
		#endregion

		#region public methods
		public void Entry()
		{
			Transition(0);
		}

		public void Update(float delta)
		{
			//停止中だった場合
			if (_isStopped)
			{
				Debug.Log("[UPDT]FlowAIは停止しています");
				return;
			}

			_elapsed += delta;

			//遷移時間を終えた場合
			if (_elapsed >= _currentNode.duration)
			{
				Transition(_currentNode.GetNextNode().localId);
				_elapsed = 0f;
			}
			//遷移中の場合
			else
			{
				Debug.LogFormat("[UPDT]遷移中 LID{0} >>> LID{1}", _currentNode.localId, _currentNode.GetNextNode().localId);
			}
		}

		public void Transition(int localId)
		{
			var node = _nodes.FirstOrDefault(item => item.localId == localId);

			//そのIDのノードが存在しない場合
			if (node == null)
			{
				Debug.LogFormat("[TRNS]この基盤に存在しないローカルID:{0}", localId);
				_isStopped = true;
				return;
			}

			_currentNode = node;
			_currentNode.Processing();

			//終端ノードだった場合
			if (_currentNode.GetNextNode() == null)
			{
				_isStopped = true;
				Debug.LogFormat("[TRNS]終端ノードに到達しました ローカルID:{0}", localId);
				return;
			}
		}

		public void AddNode(FlowAINode node)
		{
			node.localId = _idCount++;
			_nodes.Add(node);
		}
		#endregion
	}
}