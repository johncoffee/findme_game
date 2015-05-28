using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class HttpContentParser
{
	
	public HttpContentParser (string content)
	{
		this.Parse (content);
	}

	public HttpContentParser (byte[] data)
	{
		this.Parse (data, Encoding.UTF8);
	}
	
	public HttpContentParser (byte[] data, Encoding encoding)
	{
		this.Parse (data, encoding);
	}

	private void Parse(string content) {
		string name = string.Empty;
		string value = string.Empty;
		bool lookForValue = false;
		int charCount = 0;
		
		foreach (var c in content) {
			if (c == '=') {
				lookForValue = true;
			} else if (c == '&') {
				lookForValue = false;
				AddParameter (name, value);
				name = string.Empty;
				value = string.Empty;
			} else if (!lookForValue) {
				name += c;
			} else {
				value += c;
			}
			
			if (++charCount == content.Length) {
				AddParameter (name, value);
				break;
			}
		}
		
		// Get the start & end indexes of the file contents
		//int startIndex = nameMatch.Index + nameMatch.Length + "\r\n\r\n".Length;
		//Parameters.Add(name, s.Substring(startIndex).TrimEnd(new char[] { '\r', '\n' }).Trim());
		
		// If some data has been successfully received, set success to true
		if (Parameters.Count != 0)
			this.Success = true;

	}

	private void Parse (byte[] data, Encoding encoding)
	{
		this.Success = false;

		
		// Copy to a string for header parsing
		string content = encoding.GetString (data);
		Parse (content);
	}
	
	private void AddParameter (string name, string value)
	{
		if (!string.IsNullOrEmpty (name) && !string.IsNullOrEmpty (value))
			Parameters.Add (name.Trim (), value.Trim ());
	}
	
	public IDictionary<string, string> Parameters = new Dictionary<string, string> ();
	
	public bool Success {
		get;
		private set;
	}
}
