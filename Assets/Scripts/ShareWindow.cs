﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Soomla.Profile;

public class ShareWindow : MonoBehaviour
{
	public InputField messageInputField;
	public RawImage rawImageControl;

	private Window window;
	private RenderTexture renderTexture;
	private Texture2D texture;

	void Awake()
	{
		window = GetComponent<Window>();
	}

	void OnEnable()
	{
		var cam = Camera.main;

		float aspectRatio = 9.0f / 16.0f;
		int width = Screen.width;
		int height = (int)(width * aspectRatio);

		if (renderTexture == null)
		{
			renderTexture = new RenderTexture(width, height, 0);
			renderTexture.useMipMap = false;
		}

		if (texture == null)
		{
			texture = new Texture2D(width, height, TextureFormat.ARGB32, false, false);
			texture.filterMode = FilterMode.Point;
			texture.wrapMode = TextureWrapMode.Clamp;
		}

		cam.targetTexture = renderTexture;
		cam.Render();

		RenderTexture.active = renderTexture;

		texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
		texture.Apply();

		cam.targetTexture = null;

		if (rawImageControl.texture != texture)
			rawImageControl.texture = texture;
	}

	public void Share(string providerName)
	{
		string sharePayload = "uploadImage";
		Provider shareProvider = null;

		switch (providerName)
		{
			default:
			case "Facebook":
				shareProvider = Provider.FACEBOOK;
				break;
			case "Twitter":
				shareProvider = Provider.TWITTER;
				break;
		}

		ProfileEvents.OnSocialActionStarted = (Provider provider, SocialActionType action, string payload) =>
		{
			if (payload != sharePayload)
				return;

			//loadingControl.SetActive(true);
		};

		ProfileEvents.OnSocialActionFinished = (Provider provider, SocialActionType action, string payload) =>
		{
			if (payload != sharePayload)
				return;

			//loadingControl.SetActive(false);
			window.Close();
		};

		SoomlaProfile.UploadImage(shareProvider, messageInputField.text, "image.png", texture, sharePayload);
	}
}
