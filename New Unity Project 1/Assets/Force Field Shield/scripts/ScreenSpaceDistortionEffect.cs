using UnityEngine;
using System.Collections;

public class ScreenSpaceDistortionEffect : MonoBehaviour {
	
	public RenderTexture shieldRT;
	public RenderTexture screenRT;
	public RenderTexture finalRT;
	public RenderTexture depthRT;
	
	public RenderTexture distortedRT;
	Camera distortCam;
	Camera mainCam;
	Camera shieldCam;
	
	Shader shieldReplacement;
	Material effectMaterial;
	
	void Awake()
	{
		screenRT = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.Default);
		screenRT.wrapMode = TextureWrapMode.Repeat;
	
		finalRT = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.Default);
		finalRT.wrapMode = TextureWrapMode.Repeat;
		
		depthRT = new RenderTexture(Screen.width, Screen.height, 16, RenderTextureFormat.Depth);
		depthRT.wrapMode = TextureWrapMode.Repeat;
		
		
		shieldRT = new RenderTexture(Screen.width/4,Screen.height/4,16, RenderTextureFormat.Default);
		shieldRT.wrapMode = TextureWrapMode.Repeat;
		
		shieldReplacement = Shader.Find("KH/Distortion/ShieldReplacement");
		effectMaterial = new Material(Shader.Find("KH/Distortion/ShieldComposite"));
		
		mainCam = GetComponent<Camera>();
		mainCam.SetTargetBuffers(screenRT.colorBuffer, depthRT.depthBuffer);
		mainCam.cullingMask &= ~(1 << LayerMask.NameToLayer("Shield"));
		
		distortCam = new GameObject("DistortionCam").AddComponent<Camera>();
		distortCam.enabled = false;
		
		shieldCam = new GameObject("Shield Cam").AddComponent<Camera>();	
		shieldCam.SetTargetBuffers(finalRT.colorBuffer, depthRT.depthBuffer);
		var blit = shieldCam.gameObject.AddComponent<BlitToScreen>();
		blit.effect = this;
		
	}
	
	void Update()
	{
		shieldCam.cullingMask = distortCam.cullingMask;
		shieldCam.clearFlags = CameraClearFlags.Nothing;
		shieldCam.depth = mainCam.depth + 1;
		shieldCam.transform.position = mainCam.transform.position;
		shieldCam.transform.rotation = mainCam.transform.rotation;
		shieldCam.cullingMask = 1 << LayerMask.NameToLayer("Shield");
		shieldCam.fieldOfView = mainCam.fieldOfView;
		shieldCam.orthographic = mainCam.orthographic;
		shieldCam.orthographicSize = mainCam.orthographicSize;
	}
	
	void OnPostRender()
	{
		distortCam.depth = mainCam.depth + 0.5f;
		distortCam.clearFlags = CameraClearFlags.SolidColor;
		distortCam.transform.position = mainCam.transform.position;
		distortCam.transform.rotation = mainCam.transform.rotation;
		distortCam.fieldOfView = mainCam.fieldOfView;
		distortCam.orthographic = mainCam.orthographic;
		distortCam.orthographicSize = mainCam.orthographicSize;	
		distortCam.backgroundColor = Color.grey;
		distortCam.cullingMask = 1 << LayerMask.NameToLayer("Shield");
		
		distortCam.targetTexture = shieldRT;
		distortCam.RenderWithShader(shieldReplacement, null);
		
		Shader.SetGlobalTexture("_DistortionBuffer", shieldRT);
		Shader.SetGlobalTexture("_ScreenBuffer", screenRT);
		
		Graphics.Blit(screenRT, finalRT);
	}
	
	public void BlitToScreen()
	{
		Graphics.Blit(finalRT, (RenderTexture)null);
		
		screenRT.DiscardContents();
		finalRT.DiscardContents();
		shieldRT.DiscardContents();
		
	}
}
