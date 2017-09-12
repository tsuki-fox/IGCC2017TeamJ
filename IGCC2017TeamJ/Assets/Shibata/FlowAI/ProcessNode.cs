using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowAI
{
	public class ProcessNode : FlowAINode
	{
		#region events
		public delegate void ProcessingEventHandler();
		public event ProcessingEventHandler onProcess = delegate { };
		#endregion

		#region private fields
		FlowAINode _nextNode = null;
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