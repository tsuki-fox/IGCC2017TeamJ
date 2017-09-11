using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>モニタウィンドウ</summary>
public class TFDebugMonitorEditorWindow : EditorWindow
{
	[MenuItem("Window/TFDebug MonitorWindow")]
	static void ShowEditor()
	{
		var window=GetWindow<TFDebugMonitorEditorWindow>();
		window.Show();
	}

	/// <summary>描画関数</summary>
	void OnGUI()
	{
		//Assets側からデータを取得
		var monitors = TFDebug.monitors;
		//カラム幅を計算
		var width = position.width/monitors.Count;

		EditorGUILayout.BeginHorizontal();

		foreach(var elem in monitors)
		{
			EditorGUILayout.BeginVertical();

			//カラムヘッダの描画
			var headerRect=EditorGUILayout.BeginHorizontal();
			//カラムヘッダ背景の描画
			EditorGUI.DrawRect(headerRect, TFColor.Monotone.black);
			//カラムヘッダ本文の描画
			EditorGUILayout.LabelField(elem.Key, GUILayout.Width(width));
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal();

			//GUIStyleを設定
			GUIStyle style = new GUIStyle();
			style.wordWrap = true;
			style.richText = true;

			//本文の描画
			EditorGUILayout.LabelField(elem.Value,style, GUILayout.Width(width));
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.EndVertical();
		}

		EditorGUILayout.EndHorizontal();
	}

	void Update()
	{
		//再描画
		Repaint();	
	}
}
