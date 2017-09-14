using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Linq;

namespace FlowAI
{
	[System.Serializable]
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
				_summary = "ENTRY";
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
		[SerializeField]
		List<FlowAINode> _nodes;    //ノードリスト
		EntryPointNode _entryPoint; //エントリポイント
		FlowAINode _currentNode;    //現在のノード

		float _elapsed;
		bool _isStopped;
		#endregion

		#region properties
		/// <summary>
		/// 登録されているノード
		/// Resisted nodes.
		/// </summary>
		public List<FlowAINode> nodes { get { return _nodes; } }
		/// <summary>
		/// 現在のノード
		/// Current node.
		/// </summary>
		public FlowAINode currentNode { get { return _currentNode; } }

		/// <summary>AIの動作を停止させるか is AI stopped?</summary>
		public bool isStopped { get { return _isStopped; }set { _isStopped = value; } }

		/// <summary>elapsed time.</summary>
		public float elapsed { get { return _elapsed; } }
		#endregion

		/// <summary>
		/// AIのエントリポイント
		/// Entry point of FlowAI.
		/// </summary>
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
		/// <summary>
		/// AI開始
		/// Transition at entry point.
		/// </summary>
		public void Entry()
		{
			_elapsed = 0f;
			Transition(0);
		}

		/// <summary>
		/// 更新処理
		/// Update.
		/// </summary>
		/// <param name="delta">デルタ Delta time.</param>
		public void Update(float delta)
		{
			//停止中だった場合
			if (_isStopped)
			{
				//	Debug.Log("[UPDT]FlowAIは停止しています");
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
				//	Debug.LogFormat("[UPDT]遷移中 LID{0} >>> LID{1}", _currentNode.localId, _currentNode.GetNextNode().localId);
			}
		}

		/// <summary>遷移 Transition</summary>
		/// <param name="localId">遷移先のノードID LocalID of next node</param>
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

		/// <summary>ノード追加 Add node.</summary>
		/// <param name="node">ノード node.</param>
		public void AddNode(FlowAINode node)
		{
			node.localId = _idCount++;
			_nodes.Add(node);
		}

		/// <summary>ノード追加(可変長) Add nodes.</summary>
		/// <param name="args">ノード node.</param>
		public void AddNode(params FlowAINode[] args)
		{
			foreach (var item in args)
			{
				AddNode(item);
			}
		}

		/// <summary>dummy</summary>
		/// <param name="fromId">dummy</param>
		/// <param name="toId">dummy</param>
		public void Swap(int fromId, int toId)
		{

		}

		/// <summary>擬似的にノードを交換する. Swap two nodes for imitative.</summary>
		/// <param name="fromId"></param>
		/// <param name="toId"></param>
		public void ImitativeSwap(int fromId, int toId)
		{
			if (fromId == toId)
				return;

			var temp1 = _nodes.Find(item => item.localId == fromId);
			var temp2 = _nodes.Find(item => item.localId == toId);

			var from = temp1 as ProcessNode;
			var to = temp2 as ProcessNode;

			//エラー処理
			bool isFailed = false;
			//fromが不明
			if (temp1 == null)
			{
				TFDebug.Log("FlowAIBasis", "fromノードが見つかりません");
				isFailed = true;
			}
			//toが不明
			if (temp2 == null)
			{
				TFDebug.Log("FlowAIBasis", "toノードが見つかりません");
				isFailed = true;
			}
			//交換するノードが処理ノードではない
			if (from == null || to == null)
			{
				TFDebug.Log("FlowAIBasis", "処理ノード以外をSwapすることはできません");
				isFailed = true;
			}
			//交換失敗
			if (isFailed)
			{
				TFDebug.Log("FlowAIBasis", "Swapに失敗しました");
				return;
			}

			var ft = from.Copy();
			var tt = to.Copy();

			from.Imitate(tt);
			to.Imitate(ft);


			TFDebug.Log("FlowAIBasis", "Swapping finished! from LID:{0} to LID:{1}", from.localId, to.localId);

		}
		#endregion
	}
}