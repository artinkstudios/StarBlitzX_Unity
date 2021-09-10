using UnityEngine;
using System.Collections;

/*
	DetonatorBurstEmitter is an interface for DetonatorComponents to use to create particles
	
	- Handles common tasks for Detonator... almost every DetonatorComponent uses this for particles
	- Builds the gameobject with emitter, animator, renderer
	- Everything incoming is automatically scaled by size, timeScale, color
	- Enable oneShot functionality

	You probably don't want to use this directly... though you certainly can.
*/

public class DetonatorBurstEmitter : DetonatorComponent
{
    private ParticleSystem _particleSystem;
    private ParticleSystemRenderer _particleRenderer;
    //private ParticleAnimator _particleAnimator;

	private float _baseDamping = 0.1300004f;
	private float _baseSize = 1f;
	private Color _baseColor = Color.white;
	
	public float damping = 1f;
	public float startRadius = 1f;
	public float maxScreenSize = 2f;
	public bool explodeOnAwake = false;
	public bool oneShot = true;
	public float sizeVariation = 0f;
	public float particleSize = 1f;
	public float count = 1;
	public float sizeGrow = 20f;
	public bool exponentialGrowth = true;
	public float durationVariation = 0f;
	public bool useWorldSpace = true;
	public float upwardsBias = 0f;
	public float angularVelocity = 20f;
	public bool randomRotation = true;
	
	public ParticleSystemRenderMode renderMode;
	
	//TODO make this based on some var
	/*
	_sparksRenderer.particleRenderMode = ParticleRenderMode.Stretch;
	_sparksRenderer.lengthScale = 0f;
	_sparksRenderer.velocityScale = 0.7f;
	*/
	
	public bool useExplicitColorAnimation = false;
	public Color[] colorAnimation = new Color[5];
	
	private bool _delayedExplosionStarted = false;
	private float _explodeDelay;
	
	public Material material;
	
	//unused
	override public void Init() 
	{
		print ("UNUSED");
	} 
	
    public void Awake()
    {
        _particleSystem = (gameObject.AddComponent<ParticleSystem>()) as ParticleSystem;
        _particleRenderer = (gameObject.AddComponent< ParticleSystemRenderer > ()) as ParticleSystemRenderer;
        //_particleAnimator = (gameObject.AddComponent("ParticleAnimator")) as ParticleAnimator;

		_particleSystem.hideFlags = HideFlags.HideAndDontSave;
		_particleRenderer.hideFlags = HideFlags.HideAndDontSave;
		//_particleAnimator.hideFlags = HideFlags.HideAndDontSave;
		
		//_particleAnimator.damping = _baseDamping;
        var pa = _particleSystem.limitVelocityOverLifetime;
        pa.dampen = _baseDamping;

        _particleSystem.Stop();
		_particleRenderer.maxParticleSize = maxScreenSize;
        _particleRenderer.material = material;
		_particleRenderer.material.color = Color.white; //workaround for this not being settable elsewhere
		//_particleAnimator.sizeGrow = sizeGrow;
        var ps = _particleSystem.sizeOverLifetime;
        ps.sizeMultiplier = sizeGrow;


        if (explodeOnAwake)
		{
			Explode();
		}
    }
	
	private float _emitTime;
	private float speed = 3.0f;
	private float initFraction = 0.1f;
	static float epsilon = 0.01f;
	
	void Update () 
	{
		//do exponential particle scaling once emitted
		if (exponentialGrowth)
		{
			float elapsed = Time.time - _emitTime;
			float oldSize = SizeFunction(elapsed - epsilon);
			float newSize = SizeFunction(elapsed);
			float growth = ((newSize / oldSize) - 1) / epsilon;
            //_particleAnimator.sizeGrow = growth;
            var ps = _particleSystem.sizeOverLifetime;
            ps.sizeMultiplier = growth;
        }
		else
		{
            //_particleAnimator.sizeGrow = sizeGrow;
            var ps = _particleSystem.sizeOverLifetime;
            ps.sizeMultiplier = sizeGrow;
        }
		
		//delayed explosion
		if (_delayedExplosionStarted)
		{
			_explodeDelay = (_explodeDelay - Time.deltaTime);
			if (_explodeDelay <= 0f)
			{
				Explode();
			}
		}
	}
	
	private float SizeFunction (float elapsedTime) 
	{
		float divided = 1 - (1 / (1 + elapsedTime * speed));
		return initFraction + (1 - initFraction) * divided;
	}
	
    public void Reset()
    {
		size = _baseSize;
		color = _baseColor;
		damping = _baseDamping;
    }

	
	private float _tmpParticleSize; //calculated particle size... particleSize * randomized size (by sizeVariation)
	private Vector3 _tmpPos; //calculated position... randomized inside sphere of incoming radius * size
	private Vector3 _tmpDir; //calculated velocity - randomized inside sphere - incoming velocity * size
	private Vector3 _thisPos; //handle on this gameobject's position, set inside
	private float _tmpDuration; //calculated duration... incoming duration * incoming timescale
	private float _tmpCount; //calculated count... incoming count * incoming detail
	private float _scaledDuration; //calculated duration... duration * timescale
	private float _scaledDurationVariation; 
	private float _scaledStartRadius; 
	private float _scaledColor; //color with alpha adjusted according to detail and duration
	private float _randomizedRotation;
	private float _tmpAngularVelocity; //random angular velocity from -angularVelocity to +angularVelocity, if randomRotation is true;
	
    override public void Explode()
    {
		if (on)
		{
			//_particleSystem.useWorldSpace = useWorldSpace;
			
			_scaledDuration = timeScale * duration;
			_scaledDurationVariation = timeScale * durationVariation;
			_scaledStartRadius = size * startRadius;

			_particleRenderer.renderMode = renderMode;
			
			if (!_delayedExplosionStarted)
			{
				_explodeDelay = explodeDelayMin + (Random.value * (explodeDelayMax - explodeDelayMin));
			}
			if (_explodeDelay <= 0) 
			{
                //Color[] modifiedColors = _particleAnimator.colorAnimation;
                Color[] modifiedColors = new Color[5];
                Gradient grad = new Gradient();
                GradientColorKey[] colourKey = new GradientColorKey[5];
                GradientAlphaKey[] alphaKey = new GradientAlphaKey[5];
				
				if (useExplicitColorAnimation)
				{
					modifiedColors[0] = colorAnimation[0];
					modifiedColors[1] = colorAnimation[1];
					modifiedColors[2] = colorAnimation[2];
					modifiedColors[3] = colorAnimation[3];
					modifiedColors[4] = colorAnimation[4];
				}
				else //auto fade
				{
					modifiedColors[0] = new Color(color.r, color.g, color.b, (color.a * .7f));
					modifiedColors[1] = new Color(color.r, color.g, color.b, (color.a * 1f));
					modifiedColors[2] = new Color(color.r, color.g, color.b, (color.a * .5f));
					modifiedColors[3] = new Color(color.r, color.g, color.b, (color.a * .3f));
					modifiedColors[4] = new Color(color.r, color.g, color.b, (color.a * 0f));
				}

                colourKey[0].color = modifiedColors[0];
                colourKey[0].time = 0.0f;
                colourKey[1].color = modifiedColors[1];
                colourKey[1].time = 0.25f;
                colourKey[2].color = modifiedColors[2];
                colourKey[2].time = 0.5f;
                colourKey[3].color = modifiedColors[3];
                colourKey[3].time = 0.75f;
                colourKey[4].color = modifiedColors[4];
                colourKey[4].time = 1.0f;

                alphaKey[0].alpha = modifiedColors[0].a;
                alphaKey[0].time = 0.0f;
                alphaKey[1].alpha = modifiedColors[1].a;
                alphaKey[1].time = 0.25f;
                alphaKey[2].alpha = modifiedColors[2].a;
                alphaKey[2].time = 0.5f;
                alphaKey[3].alpha = modifiedColors[3].a;
                alphaKey[3].time = 0.75f;
                alphaKey[4].alpha = modifiedColors[4].a;
                alphaKey[4].time = 1.0f;
                grad.SetKeys(colourKey, alphaKey);


                //_particleAnimator.colorAnimation = modifiedColors;
                var pa = _particleSystem.colorOverLifetime;
                pa.color = grad;
                _particleRenderer.material = material;
				//_particleAnimator.force = force;
                var ps = _particleSystem.forceOverLifetime;
                ps.xMultiplier = force.x;
                ps.yMultiplier = force.y;
                ps.zMultiplier = force.z;
                _tmpCount = count * detail;
				if (_tmpCount < 1) _tmpCount = 1;
				
				if (useWorldSpace == true)
				{
					_thisPos = this.gameObject.transform.position;
				}
				else
				{
					_thisPos = new Vector3(0,0,0);
				}

				for (int i = 1; i <= _tmpCount; i++)
				{
					_tmpPos =  Vector3.Scale(Random.insideUnitSphere, new Vector3(_scaledStartRadius, _scaledStartRadius, _scaledStartRadius)); 
					_tmpPos = _thisPos + _tmpPos;
									
					_tmpDir = Vector3.Scale(Random.insideUnitSphere, new Vector3(velocity.x, velocity.y, velocity.z)); 
					_tmpDir.y = (_tmpDir.y + (2 * (Mathf.Abs(_tmpDir.y) * upwardsBias)));
					
					if (randomRotation == true)
					{
						_randomizedRotation = Random.Range(-1f,1f);
						_tmpAngularVelocity = Random.Range(-1f,1f) * angularVelocity;
						
					}
					else
					{
						_randomizedRotation = 0f;
						_tmpAngularVelocity = angularVelocity;
					}
					
					_tmpDir = Vector3.Scale(_tmpDir, new Vector3(size, size, size));
					
					 _tmpParticleSize = size * (particleSize + (Random.value * sizeVariation));
					
					_tmpDuration = _scaledDuration + (Random.value * _scaledDurationVariation);
					//_particleSystem.Emit(_tmpPos, _tmpDir, _tmpParticleSize, _tmpDuration, color, _randomizedRotation, _tmpAngularVelocity);
                    _particleSystem.Emit(_tmpPos, _tmpDir, _tmpParticleSize, _tmpDuration, color);
                }
					
				_emitTime = Time.time;
				_delayedExplosionStarted = false;
				_explodeDelay = 0f;
			}
			else
			{
				//tell update to start reducing the start delay and call explode again when it's zero
				_delayedExplosionStarted = true;
			}
		}
    }

}
