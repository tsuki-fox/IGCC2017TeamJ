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
		/// <summary>次に遷移するノード</summary>
		public FlowAINode nextNode
		{
			get { return _nextNode; }
			set { _nextNode = value; }
		}

		#endregion

		#region public methods
		/// <summary>初期化</summary>
		/// <param name="duration">遷移時間</param>
		/// <param name="next">遷移先ノード</param>
		public void Initialize(float duration,FlowAINode next)
		{
			this.duration = duration;
			_nextNode = next;
		}

		/// <summary>初期化</summary>
		/// <param name="duration">遷移時間</param>
		/// <param name="next">遷移先ノード</param>
		/// <param name="process">処理イベント</param>
		public void Initialize(float duration,FlowAINode next,ProcessingEventHandler process)
		{
			Initialize(duration, next);
			onProcess += process;
		}
		#endregion

		#region overrides
		/// <summary>処理</summary>
		public override void Processing()
		{
			onProcess();
		}

		/// <summary>次の遷移ノードを取得</summary>
		/// <returns></returns>
		public override FlowAINode GetNextNode()
		{
			return _nextNode;
		}
		#endregion
	}
}