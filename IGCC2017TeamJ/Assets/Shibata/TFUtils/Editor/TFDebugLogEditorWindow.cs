using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>ログウィンドウ</summary>
public class TFDebugLogEditorWindow : EditorWindow
{
	[MenuItem("Window/TFDebug LogWindow")]
	static void ShowEditor()
	{
		var window = GetWindow<TFDebugLogEditorWindow>();
		window.Show();
	}

	/// <summary>描画関数</summary>
	void OnGUI()
	{
		Color[] colors = {TFColor.GetColorFromCode(0x2a2a2a),TFColor.GetColorFromCode(0x303030) };

		//Assets側からデータを取得
		var columns = TFDebug.columns;
		//カラム幅の計算
		var width = this.position.width / columns.Count;
		
		using (var h1 = new EditorGUILayout.HorizontalScope())
		{
			foreach (var elem in columns)
			{
				var prop = TFDebug.GetLogColumnProperty(elem.Key);

				using (var v1 = new EditorGUILayout.VerticalScope())
				{
					using (var h2 = new EditorGUILayout.HorizontalScope())
					{
						//カラムヘッダーの描画
						EditorGUILayout.LabelField(elem.Key, GUILayout.Width(width / 2f));
					}

					using (var h2 = new EditorGUILayout.HorizontalScope())
					{
						//ログ消去ボタン
						if (GUILayout.Button("Clear", EditorStyles.miniButtonLeft, GUILayout.Width(width / 2f)))
						{
							TFDebug.ClearLogColumn(elem.Key);
						}
						prop.collapsed = GUILayout.Toggle(prop.collapsed, "Collapse", EditorStyles.miniButtonRight);
					}

					prop.scrollPosition = EditorGUILayout.BeginScrollView(prop.scrollPosition, GUILayout.Height(v1.rect.height));

					//ログ配列の取得
					var logs = elem.Value.ToArray();
					for (int f2 = logs.Length - 1; f2 >= 0; f2--)
					{
						//GUIStyleをラップ(折り返し)/リッチテキスト(カラータグが使えるようになる等)に設定
						GUIStyle style = new GUIStyle();
						style.wordWrap = true;
						style.richText = true;

						using (var h2 = new EditorGUILayout.HorizontalScope())
						{
							//ログ背景の描画
							EditorGUI.DrawRect(h2.rect, colors[f2 % 2]);
							//ログ本文の描画
							EditorGUILayout.LabelField(logs[f2].text, style, GUILayout.Width(width));
						}
					}

					EditorGUILayout.EndScrollView();
				}
			}
		}
	}

	void Update()
	{
		//再描画
		Repaint();
	}
}
