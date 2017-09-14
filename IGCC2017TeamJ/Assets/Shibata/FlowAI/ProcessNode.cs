using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowAI
{
    [System.Serializable]
	public class ProcessNode : FlowAINode
	{
		#region events
		public delegate void ProcessingEventHandler();
		public event ProcessingEventHandler onProcess = delegate { };
		#endregion

		#region private fields
		FlowAINode _nextNode = null;
		ProcessNode _initial;
		#endregion

		#region properties
		/// <summary>次に遷移するノード Next node.</summary>
		public FlowAINode nextNode
		{
			get { return _nextNode; }
			set { _nextNode = value; }
		}

		#endregion

		#region public methods
		/// <summary>初期化 Initialize.</summary>
		/// <param name="duration">遷移時間 Transition duration.</param>
		/// <param name="next">遷移先ノード Next node.</param>
		public void Initialize(float duration,FlowAINode next)
		{
			this.duration = duration;
			_nextNode = next;
		}

		/// <summary>初期化 Initialize.</summary>
		/// <param name="duration">遷移時間 Transition duration.</param>
		/// <param name="next">遷移先ノード Next node.</param>
		/// <param name="process">処理イベント Processing function.</param>
		public void Initialize(float duration,FlowAINode next,ProcessingEventHandler process)
		{
			Initialize(duration, next);
			onProcess += process;

			_initial = Copy();
		}

		public void Initialize(float duration, FlowAINode next, ProcessingEventHandler process,string summary)
		{
			Initialize(duration, next);
			onProcess += process;
			_summary = summary;

			_initial = Copy();
		}

		/// <summary>処理ノードのコピーインスタンスを生成する create ProcessNode copy instance.</summary>
		/// <returns></returns>
		public ProcessNode Copy()
		{
			var node = new ProcessNode();

			node._duration = this._duration;
			node._localId = this._localId;
			node._nextNode = this._nextNode;
			node._summary = this._summary;
			node.onProcess = this.onProcess;

			return node;
		}

		/// <summary>振る舞いをコピーする copy behaivior.</summary>
		/// <param name="node">コピー元</param>
		public void Imitate(ProcessNode node)
		{
			this._duration = node._duration;
			this._summary = node._summary;
		//	this._localId = node._localId;
			this.onProcess = node.onProcess;
		}

		/// <summary>このノードをInitialize直後の状態に巻き戻す</summary>
		public void Revert()
		{
			this._duration = _initial._duration;
			this._summary = _initial._summary;
		//	this._localId = _initial._localId;
			this.onProcess = _initial.onProcess;
		//	this._nextNode = _initial._nextNode;
		}

		#endregion

		#region overrides
		/// <summary>処理 Processing.</summary>
		public override void Processing()
		{
			onProcess();
		}

		/// <summary>次の遷移ノードを取得 Next transition node.</summary>
		/// <returns></returns>
		public override FlowAINode GetNextNode()
		{
			return _nextNode;
		}
		#endregion
	}
}