using UnityEngine;
using System.Collections;

public class HTTPListener : HTTP.Server.HttpRequestHandler {

	
	public delegate void GotMessageCallback(string message, HttpContentParser formData);

	public GotMessageCallback GotMessage;

	public override void POST (HTTP.Request request)
	{
		base.POST (request);

		HttpContentParser parser = new HttpContentParser (request.Bytes);
		var response = request.response;
		if (!parser.Success) {
			response.status = 400;
			response.message = "Bad Request";
			response.Text = "Bad Request, can't parse request";
			return;
		} 

		if (!parser.Parameters.ContainsKey ("message")) {
			response.status = 400;
			response.message = "Bad Request";
			response.Text = "Bad Request, missing message";
			return;
		}

		string message = parser.Parameters ["message"];


		Debug.Log ("We got a message:" + message);


		response.status = 200;
		response.message = "Ok";
		response.Text = "Everything is sunny!";

		if (GotMessage != null) {
			
			Loom.QueueOnMainThread (() => {
				GotMessage(message, parser);
			});
		}


	}
}