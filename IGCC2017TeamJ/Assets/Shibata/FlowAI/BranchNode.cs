using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowAI
{
	public class BranchNode : FlowAINode
	{
		#region delegates
		public delegate bool Predicate();
		public event Predicate predicate;
		#endregion

		#region private fields
		FlowAINode _trueNode;               //真の場合のノード
		FlowAINode _falseNode;              //偽の場合のノード
		FlowAINode _selectedNode = null;    //選択されたノード

		float _trueDuration;    //真の場合の遷移時間
		float _falseDuration;   //偽の場合の遷移時間

		#endregion

		#region properties
		/// <summary>結果が真の場合の遷移先ノード Next node.In case of true.</summary>
		public FlowAINode trueNode { get { return _trueNode; } set { _trueNode = value; } }
		/// <summary>結果が偽の場合の遷移先ノード Next node.In case of false.</summary>
		public FlowAINode falseNode { get { return _falseNode; } set { _falseNode = value; } }
		/// <summary>結果が真の場合の遷移時間 Transition duration.In case of true.</summary>
		public float trueDuration { get { return _trueDuration; } set { _trueDuration = value; } }
		/// <summary>結果が偽の場合の遷移時間 Transition duration.In case of false.</summary>
		public float falseDuration { get { return _falseDuration; } set { _falseDuration = value; } }
		#endregion

		#region public methods
		/// <summary>初期化 Initialize.</summary>
		/// <param name="trueNode">結果が真の場合の遷移先ノード Next node.In case of true.</param>
		/// <param name="trueDuration">結果が真の場合の遷移時間 Transition duration.In case of true.</param>
		/// <param name="falseNode">結果が偽の場合の遷移先ノード Next node.In case of false.</param>
		/// <param name="falseDuration">結果が偽の場合の遷移先ノード Transition duration.In case of false.</param>
		/// <param name="pred">叙述関数 Predicate function.</param>
		public void Initialize(FlowAINode trueNode,float trueDuration,FlowAINode falseNode,float falseDuration,Predicate pred)
		{
			_trueNode = trueNode;
			_trueDuration = trueDuration;

			_falseNode = falseNode;
			_falseDuration = falseDuration;

			predicate += pred;
		}
		#endregion

		#region overrides
		/// <summary>処理 Processing.</summary>
		public override void Processing()
		{
			if (predicate())
			{
				_selectedNode = _trueNode;
				duration = _trueDuration;
			}
			else
			{
				_selectedNode = _falseNode;
				duration = _falseDuration;
			}
		}

		/// <summary>次のノードを取得 Get next node.</summary>
		/// <returns></returns>
		public override FlowAINode GetNextNode()
		{
			return _selectedNode;
		}
		#endregion
	}
}