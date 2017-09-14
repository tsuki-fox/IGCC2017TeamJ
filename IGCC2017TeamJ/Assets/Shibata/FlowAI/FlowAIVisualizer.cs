using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;
using System.Linq;
using FlowAI;

namespace FlowAI
{
	public class FlowAIVisualizer : MonoBehaviour
	{
		struct PrepareData
		{
			public FlowAINode node;
			public int depthX;
			public int depthY;
			public bool isActive;
			public bool isFocus;
			public Rect rect;

			public PrepareData(FlowAINode node, int depthX, int depthY, bool isActive, bool isFocus, Rect rect)
			{
				this.node = node;
				this.depthX = depthX;
				this.depthY = depthY;
				this.isActive = isActive;
				this.rect = rect;
				this.isFocus = isFocus;
			}
		}

		#region private fields
		[SerializeField]
		FlowAIHolder _target;       //ターゲットAIホルダ
		FlowAIBasis _targetBasis;   //ターゲットAI

		[SerializeField, Header("<Node visuals>")]
		Texture2D _processTex;      //処理ノードテクスチャ
		[SerializeField]
		Texture2D _branchTex;       //分岐ノードテクスチャ
		[SerializeField]
		Texture2D _arrowHeadTex;    //矢印テクスチャ
		[SerializeField]
		Texture2D _lineTex;         //線テクスチャ
		[SerializeField]
		Color _nodeColor = Color.white;
		[SerializeField]
		Color _activeNodeColor = Color.red;
		[SerializeField]
		float _nodeVerticalSpace = 20f;     //ノード間の垂直方向の間隔
		[SerializeField]
		float _nodeHorizontalSpace = 20f;   //ノード間の水平方向の間隔
		[SerializeField]
		Vector2 _nodeSize = new Vector2(100f, 33f); //ノードの大きさ


		[SerializeField, Header("<Focuses>")]
		Texture2D _focusNodeTex;     //フォーカス中のノードのテクスチャ
		[SerializeField]
		Color _focusNodeTexColor = Color.green;   //フォーカス中のノードの色

		[SerializeField, Header("<Selected node>")]
		Texture2D _selectedNodeTex;
		[SerializeField]
		Color _selectedNodeTexColor = Color.green;


		[SerializeField, Header("<Window settings>")]
		Vector2 _windowPosition;    //ウィンドウの位置
		[SerializeField]
		Vector2 _windowSize;        //ウィンドウのサイズ
		[SerializeField]
		Vector2 _globalOffset;      //全体のオフセット

		[SerializeField, Header("<Line settings>")]
		float _lineWidth = 3f;                  //線の太さ
		[SerializeField]
		Color _lineColor = Color.white;         //線の色
		[SerializeField]
		Color _activeLineColor = Color.red;     //アクティブ時の線の色
		[SerializeField]
		int _lineDetailLevel = 100;
		[SerializeField]
		float _wayPointSize = 10f;

		[SerializeField, Header("<Font settings>")]
		Font _font;
		[SerializeField]
		int _fontSize;
		[SerializeField]
		Color _fontColor;

		List<PrepareData> _prepares = new List<PrepareData>();  //準備済みリスト

		[SerializeField]
		bool _isVisible = true; //表示フラグ
		[SerializeField]
		bool _swapMode = false;

		FlowAINode _from = null;
		FlowAINode _to = null;
		bool _isInDrag = false;

		int _maxDepthX;
		int _maxDepthY;

		[SerializeField]
		float _swappingDuration = 0.5f;

		bool _isInSwapping = false;
		int _swappingFromId = -1;
		int _swappingToId = -1;
		float _swappingElapsed = 0f;

		[SerializeField, Range(0f, 1600f),Header("Exit button settings")]
		float _exitButtonPosX;
		[SerializeField, Range(0, 960f)]
		float _exitButtonPosY;
		[SerializeField, Range(0, 500f)]
		float _exitButtonWidth;
		[SerializeField, Range(0, 500f)]
		float _exitButtonHeight;

		[SerializeField]
		float _afterDuration = 5f;

		bool _isInHacking = false;
		float _showDuration;
		float _showElapsed = 0f;

		#endregion

		#region properties
		public FlowAIHolder target
		{
			get { return _target; }
			set
			{
				_target = value;
				if (_target != null)
					_targetBasis = _target.flowAI;
				else
					_targetBasis = null;
			}
		}

		public bool isVisible { get { return _isVisible; } set { _isVisible = value; } }
		#endregion

		void Prepare(FlowAINode node)
		{
			_prepares.Clear();
			Prepare(node, 0, 0);
		}

		void Prepare(FlowAINode node, int depthX, int depthY)
		{
			if (node == null)
				return;

			//準備済みノードならば終了
			if (_prepares.Exists(item => item.node == node))
				return;

			_maxDepthX = Mathf.Max(depthX, _maxDepthX);
			_maxDepthY = Mathf.Max(depthY, _maxDepthY);

			//準備済みリストに追加
			//位置
			Vector2 pos = new Vector2(depthX * _nodeHorizontalSpace, depthY * _nodeVerticalSpace);
			pos += _globalOffset;

			//矩形
			Rect nodeRect = new Rect(Vector2.zero, _nodeSize);
			nodeRect.center = pos;

			Vector2 mousePos = Input.mousePosition;
			mousePos.y = Screen.height - mousePos.y;

			_prepares.Add(new PrepareData(node, depthX, depthY, _targetBasis.currentNode == node, nodeRect.Contains(mousePos), nodeRect));

			//処理ノードかエントリポイントノードならば
			if (node is ProcessNode || node is FlowAIBasis.EntryPointNode)
			{
				//Y方向の深さを1つ掘る
				Prepare(node.GetNextNode(), depthX, depthY + 1);
				return;
			}

			//分岐ノードならば
			else if (node is BranchNode)
			{
				var branch = node as BranchNode;

				//Y方向の深さを1つ掘る
				Prepare(branch.trueNode, depthX, depthY + 1);
				//X方向とY方向の深さを1つ掘る
				Prepare(branch.falseNode, depthX + 1, depthY + 1);
				return;
			}
		}

		/// <summary>ノードを描画する</summary>
		void DrawNodes()
		{
			foreach (var item in _prepares)
			{
				//スワップ中ならスキップする
				if (item.node.localId == _swappingFromId || item.node.localId == _swappingToId)
					continue;

				//位置
				Vector2 pos = new Vector2(item.depthX * _nodeHorizontalSpace, item.depthY * _nodeVerticalSpace);
				pos += _globalOffset;

				//矩形
				Rect nodeRect = new Rect(Vector2.zero, _nodeSize);
				nodeRect.center = pos;

				//テクスチャ描画
				//使用色
				Color usedColor = _nodeColor;
				//使用テクスチャ
				Texture2D usedTex = item.node is BranchNode ? _branchTex : _processTex;
				//アスペクト比
				float aspect = nodeRect.width / nodeRect.height;
				//描画
				GUI.DrawTexture(nodeRect, usedTex, ScaleMode.ScaleToFit, true, aspect, usedColor, 0f, 0f);

				if (item.isActive)
				{
					GUIStyle progStyle = new GUIStyle();
					progStyle.font = _font;
					progStyle.fontSize = _fontSize;
					GUIStyleState progState = new GUIStyleState();
					progState.textColor = _fontColor;
					progStyle.normal = progState;

					Rect progRect = nodeRect;
					progRect.position = new Vector2(progRect.position.x, progRect.position.y - _fontSize * 1.1f);

					float progress = _targetBasis.elapsed / item.node.duration;
					string bar = "";
					for (int f1 = 0; f1 < (int)(progress * 20f); f1++)
						bar += "|";

					GUI.Label(progRect, "progress:" + bar, progStyle);

					GUI.DrawTexture(nodeRect, usedTex, ScaleMode.ScaleToFit, true, aspect, _activeNodeColor, 0f, 0f);
				}

				//summaryの描画
				GUIStyle style = new GUIStyle();
				style.font = _font;
				style.fontSize = _fontSize;
				style.alignment = TextAnchor.MiddleCenter;
				GUIStyleState state = new GUIStyleState();
				state.textColor = _fontColor;
				style.normal = state;

				GUI.Label(nodeRect, item.node.summary, style);
			}

			//スワップアニメーション
			if (_isInSwapping)
			{
				var swpFrom = _prepares.Find(item => item.node.localId == _swappingFromId);
				var swpTo = _prepares.Find(item => item.node.localId == _swappingToId);

				float t = _swappingElapsed / _swappingDuration;

				var swpFromRect = swpFrom.rect;
				var swpToRect = swpTo.rect;

				swpFromRect.position = Vector2.Lerp(swpFrom.rect.position, swpTo.rect.position, t);
				swpToRect.position = Vector2.Lerp(swpTo.rect.position, swpFrom.rect.position, t);

				TFDebug.Write("visualizer", "lerp t:{0}\n", t);

				GUI.DrawTexture(swpFromRect, _processTex, ScaleMode.ScaleToFit, true, _nodeSize.x / _nodeSize.y, _nodeColor, 0f, 0f);
				GUI.DrawTexture(swpToRect, _processTex, ScaleMode.ScaleToFit, true, _nodeSize.x / _nodeSize.y, _nodeColor, 0f, 0f);

				//summaryの描画
				GUIStyle style = new GUIStyle();
				style.font = _font;
				style.fontSize = _fontSize;
				style.alignment = TextAnchor.MiddleCenter;
				GUIStyleState state = new GUIStyleState();
				state.textColor = _fontColor;
				style.normal = state;

				GUI.Label(swpFromRect, swpTo.node.summary, style);
				GUI.Label(swpToRect, swpFrom.node.summary, style);
			}
		}

		Vector2 GetBezierRoute(Vector2 begin,Vector2 end,Vector2 beginTan,Vector2 endTan,float t)
		{
			Vector2 res = new Vector2();
			res.x = (1 - t) * (1 - t) * (1 - t) * begin.x + 3 * (1 - t) * (1 - t) * t * beginTan.x + 3 * (1 - t) * t * t * endTan.x + t * t * t * end.x;
			res.y = (1 - t) * (1 - t) * (1 - t) * begin.y + 3 * (1 - t) * (1 - t) * t * beginTan.y + 3 * (1 - t) * t * t * endTan.y + t * t * t * end.y;
			return res;
		}

		/// <summary>
		/// from : http://ft-lab.ne.jp/cgi-bin-unity/wiki.cgi?page=unity_script_opengl
		/// </summary>
		void DrawLine2D(Vector3 v0, Vector3 v1, float lineWidth,Color color)
		{
			Vector3 n = ((new Vector3(v1.y, v0.x, 0.0f)) - (new Vector3(v0.y, v1.x, 0.0f))).normalized * lineWidth;
			GL.Vertex3(v0.x - n.x, v0.y - n.y, 0.0f);
			GL.Vertex3(v0.x + n.x, v0.y + n.y, 0.0f);
			GL.Vertex3(v1.x + n.x, v1.y + n.y, 0.0f);
			GL.Vertex3(v1.x - n.x, v1.y - n.y, 0.0f);
		}

		void DrawBezierCurve(Vector2 begin,Vector2 end,Vector2 beginTan,Vector2 endTan,Color color,Texture2D tex,float width)
		{
			GL.PushMatrix();
			GL.Begin(GL.QUADS);
			GL.Color(color);

			for(int f1=0;f1<_lineDetailLevel;f1++)
			{
				var point = GetBezierRoute(begin, end, beginTan, endTan, (float)f1 / _lineDetailLevel);
				var next = GetBezierRoute(begin, end, beginTan, endTan, (float)(f1 + 1) / _lineDetailLevel);
				var rect = new Rect(point, new Vector2(width, width));

				DrawLine2D(point, next, width, color);
			}

			GL.End();
			GL.PopMatrix();

			
			for (int f1 = 0; f1 < 10; f1++)
			{
				var point = GetBezierRoute(begin, end, beginTan, endTan, (float)f1 / 10f);
				var rect = new Rect(point, new Vector2(_wayPointSize, _wayPointSize));
				rect.position -= new Vector2(rect.width / 2f, rect.height / 2f);
				GUI.DrawTexture(rect, tex, ScaleMode.ScaleToFit, true, 1f, color, 0f, 0f);
			}
			
			
		}

		void DrawBezier(Vector2 begin, Vector2 end, PrepareData from, PrepareData to, Color color)
		{

			//同じノードなら
			if (from.node == to.node)
			{
				DrawBezierCurve(begin, end,
						begin + new Vector2(_nodeSize.x, _nodeSize.y * 3f),
						end + new Vector2(_nodeSize.x, -_nodeSize.y * 3f),
						color, _lineTex, _lineWidth);
				return;
			}

			//始点よりもYが深ければ
			if (to.depthY > from.depthY)
			{
				//Xが同じ深さ
				if (from.depthX == to.depthX)
				{
					DrawBezierCurve(begin, end,
							begin - new Vector2(_nodeSize.x, 0f),
							end + new Vector2(_nodeSize.x, 0f),
							color, _lineTex, _lineWidth);
				}
				//右
				else if (from.depthX < to.depthY)
				{
					DrawBezierCurve(begin, end,
						begin + new Vector2(_nodeSize.x, 0f),
						end,
						color, _lineTex, _lineWidth);
				}
				//左
				else
				{
					DrawBezierCurve(begin, end,
						begin,
						end - new Vector2(_nodeSize.x, 0f),
						color, _lineTex, _lineWidth);
				}

			}
			//始点よりもYが浅ければ
			else
			{
				//Xが同じ深さ
				if (from.depthX == to.depthX)
				{
					DrawBezierCurve(begin, end,
						begin - new Vector2(_nodeSize.x, -_nodeSize.y * 3f),
						end - new Vector2(_nodeSize.x, _nodeSize.y * 3f),
						color, _lineTex, _lineWidth);
				}
				//右
				else if (from.depthX < to.depthY)
				{
					DrawBezierCurve(begin, end,
						begin - new Vector2(_nodeSize.x, 0f),
						end - new Vector2(_nodeSize.x, 0f),
						color, _lineTex, _lineWidth);
				}
				//左
				else
				{
					DrawBezierCurve(begin, end,
						begin + new Vector2(_nodeSize.x * 2f, _nodeSize.y * 3f),
						end + new Vector2(_nodeSize.x, -_nodeSize.y * 3f),
						color, _lineTex, _lineWidth);
				}
			}
		}

		void DrawLines()
		{
			foreach (var item in _prepares)
			{
				//位置
				Vector2 pos = new Vector2(item.depthX * _nodeHorizontalSpace, item.depthY * _nodeVerticalSpace);
				pos += _globalOffset;

				if (item.node is ProcessNode || item.node is FlowAIBasis.EntryPointNode)
				{
					var next = _prepares.Find(elem => elem.node == item.node.GetNextNode());
					var nextPos = new Vector2(next.depthX * _nodeHorizontalSpace, next.depthY * _nodeVerticalSpace);
					nextPos += _globalOffset;

					Vector2 begin = new Vector2(pos.x, pos.y + _nodeSize.y / 2f);
					Vector2 end = new Vector2(nextPos.x, nextPos.y - _nodeSize.y / 2f);

					if (item.isActive)
						DrawBezier(begin, end, item, next, _activeLineColor);
					else
						DrawBezier(begin, end, item, next, _lineColor);
				}
				else if (item.node is BranchNode)
				{
					var branch = item.node as BranchNode;

					{
						var trueNext = _prepares.Find(elem => elem.node == branch.trueNode);
						var trueNextPos = new Vector2(trueNext.depthX * _nodeHorizontalSpace, trueNext.depthY * _nodeVerticalSpace);
						trueNextPos += _globalOffset;

						Vector2 begin = new Vector2(pos.x, pos.y + _nodeSize.y / 2f);
						Vector2 end = new Vector2(trueNextPos.x, trueNextPos.y - _nodeSize.y / 2f);

						if (item.isActive && branch.GetNextNode() == branch.trueNode)
							DrawBezier(begin, end, item, trueNext, _activeLineColor);
						else
							DrawBezier(begin, end, item, trueNext, _lineColor);
					}
					{
						var falseNext = _prepares.Find(elem => elem.node == branch.falseNode);
						var falseNextPos = new Vector2(falseNext.depthX * _nodeHorizontalSpace, falseNext.depthY * _nodeVerticalSpace);
						falseNextPos += _globalOffset;

						Vector2 begin = new Vector2(pos.x + _nodeSize.x / 2f, pos.y);
						Vector2 end = new Vector2(falseNextPos.x, falseNextPos.y - _nodeSize.y / 2f);

						if (item.isActive && branch.GetNextNode() == branch.falseNode)
							DrawBezier(begin, end, item, falseNext, _activeLineColor);
						else
							DrawBezier(begin, end, item, falseNext, _lineColor);
					}
				}
			}
		}

		void DrawFocus()
		{
			foreach (var item in _prepares)
			{
				if (item.isFocus&&item.node is ProcessNode)
				{
					TFDebug.Log("x", "LID:{0}", item.node.localId);
					var temp = GUI.color;
					GUI.color = _focusNodeTexColor;
					GUI.DrawTexture(item.rect, _focusNodeTex);
					GUI.color = temp;
				}
			}
		}

		void OnSwap()
		{
			var mousePos = Input.mousePosition;
			mousePos.y = Screen.height - mousePos.y;

			var from = _prepares.Find(item => item.node == _from);

			var temp = GUI.color;
			GUI.color = _selectedNodeTexColor;
			GUI.DrawTexture(from.rect, _selectedNodeTex);
			GUI.color = temp;

			//	Handles.DrawBezier(from.rect.center, mousePos, from.rect.center, mousePos, Color.red, null, 5f);

			GL.Begin(GL.QUADS);
			DrawLine2D(from.rect.center, mousePos, _lineWidth, Color.red);
			GL.End();
		}

		void RevertButton(Rect pos)
		{
			if(GUI.Button(pos, "Revert All"))
			{
				var procs = _targetBasis.nodes.OfType<ProcessNode>();

				foreach (var item in procs)
					item.Revert();
			}
		}

		void ExitButton(Rect pos)
		{
			if(GUI.Button(pos,("Exit Hacking")))
			{
				EndHacking(_afterDuration);
			}
		}

		void Start()
		{
			if (_target != null)
				_targetBasis = _target.GetComponent<FlowAIHolder>().flowAI;
		}

		void Update()
		{
			TFDebug.ClearMonitor("visualizer");

			if (_isInSwapping)
			{
				_swappingElapsed += Time.unscaledDeltaTime;
				if(_swappingElapsed>_swappingDuration)
				{
					_isInSwapping = false;
					_swappingFromId = -1;
					_swappingToId = -1;
				}
			}

			if(!_isInHacking)
			{
				_showElapsed += Time.deltaTime;
				if (_showElapsed > _showDuration)
					_isVisible = false;
			}
		}

		void OnValidate()
		{
			if (_target != null)
				_targetBasis = _target.GetComponent<FlowAIHolder>().flowAI;
			else
				_targetBasis = null;
		}

		void OnGUI()
		{
			/*if (!_isInHacking)
			{
				if (GUI.Button(new Rect(0, 0, 100, 33), "hack begin"))
				{
					BeginHacking(0.05f);
				}
			}*/

			TFDebug.Write("visualizer", "show time:{0}\n", _showElapsed);

			//非表示
			if (!_isVisible)
				return;

			GUI.Box(new Rect(_windowPosition, _windowSize), "");

			//ターゲットにするAIがnull
			if (_targetBasis == null)
			{
				var temp = GUI.color;
				GUI.color = Color.red;
				GUI.Label(new Rect(_windowPosition + _globalOffset, new Vector2(100f, 100f)), "Target AI not found");
				GUI.color = temp;
				return;
			}

			Prepare(_targetBasis.entryPointNode);
			DrawLines();
			DrawNodes();
			DrawFocus();

			if (_isInHacking)
			{
				ExitButton(new Rect(_exitButtonPosX + _windowPosition.x, _exitButtonPosY + _windowPosition.y, _exitButtonWidth, _exitButtonHeight));

				PrepareData? focused = _prepares
					.Select(item => item as PrepareData?)
					.FirstOrDefault(item => item.Value.isFocus);

				if (Input.GetMouseButtonDown(0) && focused.HasValue && focused.Value.node is ProcessNode)
				{
					_isInDrag = true;
					_from = focused.Value.node;
				}

				if (Input.GetMouseButtonUp(0) && _isInDrag && focused.HasValue && focused.Value.node is ProcessNode)
				{
					_isInDrag = false;
					_to = focused.Value.node;

					if (!_isInSwapping)
					{
						_targetBasis.ImitativeSwap(_from.localId, _to.localId);

						_isInSwapping = true;
						_swappingElapsed = 0f;
						_swappingFromId = _from.localId;
						_swappingToId = _to.localId;
					}
				}
				else if (Input.GetMouseButtonUp(0) && _isInDrag)
				{
					_isInDrag = false;
					_from = null;
				}

				if (_isInDrag)
					OnSwap();
				
				TFDebug.Write("visualizer", "is in swap:{0}\n", _isInDrag.ToString());
			}
		}

		#region public methods
		public void BeginHacking(float timeScale)
		{
			Time.timeScale = timeScale;
			_isInHacking = true;
			_isVisible = true;
		//	_targetBasis.isStopped = true;
		}
		public void EndHacking(float duration)
		{
			Time.timeScale = 1f;
			_isInHacking = false;
		//	_targetBasis.isStopped = false;
			_showElapsed = 0f;
			_showDuration = duration;
		}
		#endregion
	}
}