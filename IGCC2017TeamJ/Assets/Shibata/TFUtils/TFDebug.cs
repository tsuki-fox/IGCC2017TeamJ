using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class TFDebug
{
	#region structs
	/// <summary>カラムプロパティ</summary>
	public class ColumnProperty
	{
		/// <summary>着色する単語</summary>
		public struct ColoredWord
		{
			public string word;
			public Color color;
		}

		public Color _titleBackColor;
		public Color _logBackColor1;
		public Color _logBackColor2;

		public bool collapsed = false;

		public List<ColoredWord> _coloredWords = new List<ColoredWord>();

		public Vector2 scrollPosition = new Vector2();

		/// <summary>着色する単語を追加する</summary>
		/// <param name="word">単語</param>
		/// <param name="color">色</param>
		public void AddColoredWord(string word,Color color)
		{
			ColoredWord item = new ColoredWord();
			item.word = word;
			item.color = color;
			_coloredWords.Add(item);
		}
	}

	/// <summary>ログデータ</summary>
	public struct LogData
	{
		/// <summary>本文</summary>
		public string text;
		/// <summary>タイムスタンプ</summary>
		public float timeStamp;

		/// <summary>コンストラクタ</summary>
		/// <param name="text">本文</param>
		/// <param name="timeStamp">タイプスタンプ</param>
		public LogData(string text,float timeStamp)
		{
			this.text = text;
			this.timeStamp = timeStamp;
		}
	}
	#endregion

	#region private fields
	static Dictionary<string,Queue<LogData>> _columns = new Dictionary<string, Queue<LogData>>();
	static Dictionary<string, ColumnProperty> _columnProps = new Dictionary<string, ColumnProperty>();

	static int _capacity = 32;

	static Dictionary<string,string> _monitors=new Dictionary<string, string>();

	#endregion

	#region properties
	/// <summary>カラムのデータ</summary>
	public static Dictionary<string,Queue<LogData>> columns
	{
		get { return _columns; }
	}

	/// <summary>モニタのデータ</summary>
	public static Dictionary<string,string> monitors
	{
		get { return _monitors; }
	}
	#endregion

	#region public methods
	/// <summary>デバッグログを追加する</summary>
	/// <param name="columnName">カラム名</param>
	/// <param name="format">本文</param>
	public static void Log(string columnName,string format,params object[] args)
	{
		//キーが存在しない場合は新規にカラムを作成
		if(!_columns.ContainsKey(columnName))
		{
			_columns.Add(columnName, new Queue<LogData>());
			ColumnProperty prop = new ColumnProperty();
			SetLogColumnProperty(columnName, prop);
		}

		format = string.Format(format, args);

		StringBuilder builder = new StringBuilder(1024);
		builder.AppendFormat("<color=#c0c0c0>{0}</color>", format);
		
		//色を置き換える
		if(_columnProps.ContainsKey(columnName))
		{
			foreach(var elem in _columnProps[columnName]._coloredWords)
			{
				builder.Replace(elem.word, string.Format("<color={0}>{1}</color>",TFColor.GetCodeFromColor(elem.color),elem.word));
			}
		}

		//溢れたデータをポップする
		LogData logData = new LogData(builder.ToString(),Time.time);
		_columns[columnName].Enqueue(logData);
		if (_columns[columnName].Count > _capacity)
		{
			_columns[columnName].Dequeue();
		}
	}

	/// <summary>指定カラムのログを消去する</summary>
	/// <param name="columnName">カラム名</param>
	/// <returns>指定カラムが存在しない場合はfalseを返す</returns>
	public static bool ClearLogColumn(string columnName)
	{
		if(!_columns.ContainsKey(columnName))
		{
			return false;
		}

		_columns[columnName].Clear();
		return true;
	}

	/// <summary>指定カラムを除去する</summary>
	/// <param name="columnName">カラム名</param>
	/// <returns>指定カラムが存在しない場合はfalseを返す</returns>
	public static bool RemoveLogColumn(string columnName)
	{
		if(!_columns.ContainsKey(columnName))
		{
			return false;
		}

		_columns.Remove(columnName);
		return true;
	}

	public static void SetLogColumnProperty(string columnName,ColumnProperty prop)
	{
		_columnProps.Add(columnName, prop);
	}

	public static ColumnProperty GetLogColumnProperty(string columnName)
	{
		return _columnProps[columnName];
	}

	/// <summary>デバッグモニタに書き込む</summary>
	/// <param name="monitorName">モニタ名</param>
	/// <param name="format">テキスト</param>
	public static void Write(string monitorName, string format,params object[] args)
	{
		if(!_monitors.ContainsKey(monitorName))
		{
			_monitors.Add(monitorName, "");
		}

		format = string.Format(format, args);

		_monitors[monitorName] += string.Format("<color=#c0c0c0>{0}</color>",format);
	}

	/// <summary>指定デバッグモニタをクリアする</summary>
	/// <param name="monitorName">モニタ名</param>
	/// <returns>指定モニタが存在しない場合はfalse</returns>
	public static bool ClearMonitor(string monitorName)
	{
		if(!_monitors.ContainsKey(monitorName))
		{
			return false;
		}

		_monitors[monitorName] = "";
		return true;
	}

	/// <summary>指定モニタを削除する</summary>
	/// <param name="monitorName">モニタ名</param>
	/// <returns>指定モニタが存在しない場合はfalse</returns>
	public static bool RemoveMonitor(string monitorName)
	{
		if(!_monitors.ContainsKey(monitorName))
		{
			return false;
		}

		_monitors.Remove(monitorName);
		return true;
	}
	#endregion
}
