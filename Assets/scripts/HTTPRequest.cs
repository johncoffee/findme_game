using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HTTPRequest : MonoBehaviour
{

	public delegate void RequestFinishedCallback (HTTP.Response reponse,Exception exception);

	private string method;
	private string url;
	private WWWForm form = new WWWForm ();
	private int maxRetries = 0;
	private float waitTimeTillRetry = 1f;
	public RequestFinishedCallback RequestFinished;
	private int retries;
	
	public static HTTPRequest CreateRequest ()
	{
		GameObject requestObject = new GameObject ();
		return requestObject.AddComponent<HTTPRequest> ();
	}

	public void StartRequest ()
	{
		StartCoroutine (StartRequestInternal ());
	}

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
	
	private IEnumerator StartRequestInternal ()
	{
		HTTP.Request r;
		if (method == "GET") {


			r = new HTTP.Request ("GET", url);
		} else {
			r = new HTTP.Request (url, form);
		}

		yield return r.Send ();
		if (r.exception == null) {
			if (RequestFinished != null) {
				RequestFinished (r.response, null);
			}
			
		} else {
			Debug.LogError (r.exception);
			while (retries < maxRetries) {
				retries++;
				yield return new WaitForSeconds (waitTimeTillRetry);
				StartCoroutine (StartRequestInternal ());
			}
			if (RequestFinished != null) {
				RequestFinished (null, r.exception);
			}
		}
		Destroy (gameObject);
	}

	public string Method {
		get {
			return this.method;
		}
		set {
			method = value;
		}
	}

	public string Url {
		get {
			return this.url;
		}
		set {
			url = value;
		}
	}

	public int MaxRetries {
		get {
			return this.maxRetries;
		}
		set {
			maxRetries = value;
		}
	}

	public float WaitTimeTillRetry {
		get {
			return this.waitTimeTillRetry;
		}
		set {
			waitTimeTillRetry = value;
		}
	}

	public WWWForm Arguments {
		get {
			return this.form;
		}
	}

}
