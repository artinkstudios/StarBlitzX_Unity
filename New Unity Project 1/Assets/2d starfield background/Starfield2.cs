using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


public class Starfield2 : MonoBehaviour
{
	public int		MaxStars = 100;
	public float	StarSize = 0.1f;
	public float	StarSizeRange = 0.5f;
	public float	FieldWidth = 20f;
	public float	FieldHeight = 25f;
	public float	ParallaxFactor = 0f;
	public bool		Colorize = false;
	
	float 				xOffset;
	float 				yOffset;

	ParticleSystem						Particles;
	ParticleSystem.Particle[] Stars;
	Transform 								theCamera;

	Vector3				theCameraLastPos;

	void Awake ()
	{
		theCamera = Camera.main.transform;
		theCameraLastPos = theCamera.position;

		Stars = new ParticleSystem.Particle[ MaxStars ];
		Particles = GetComponent<ParticleSystem>();

		Assert.IsNotNull( Particles, "Particle system missing from object!" );

		xOffset = FieldWidth * 0.5f;																										// Offset the coordinates to distribute the spread
		yOffset = FieldHeight * 0.5f;																										// around the object's center

		for ( int i=0; i<MaxStars; i++ )
		{
			float randSize = Random.Range( 1f - StarSizeRange, StarSizeRange + 1f );			// Randomize star size within parameters
			float scaledColor = ( true == Colorize ) ? randSize - StarSizeRange : 1f;			// If coloration is desired, color based on size

			Stars[ i ].position = GetRandomInRectangle( FieldWidth, FieldHeight ) + transform.position;
			Stars[ i ].startSize = StarSize * randSize;
			Stars[ i ].startColor = new Color( 1f, scaledColor, scaledColor, 1f );

//			float speed = Random.Range( 0.05f, 0.15f );
			float speed = Random.Range( ParallaxFactor * 0.5f, ParallaxFactor * 1.5f );
			Stars[ i ].velocity = new Vector3 ( speed, speed, 0 );
		}
		Particles.SetParticles( Stars, Stars.Length );  																// Write data to the particle system
	}
	
	void Update ()
	{
		Vector3 cameraDelta = ( theCamera.position - theCameraLastPos )  * ParallaxFactor;
		theCameraLastPos = theCamera.position;

		for ( int i=0; i<MaxStars; i++ )
		{
			Vector3 pos = Stars[ i ].position + transform.position;

			Vector3 speed = Stars[ i ].velocity.y * cameraDelta;
			pos -= speed;

			if ( pos.x < ( theCamera.position.x - xOffset ) )
			{
				pos.x += FieldWidth;
			}
			else if ( pos.x > ( theCamera.position.x + xOffset ) )
			{
				pos.x -= FieldWidth;
			}

			if ( pos.y < ( theCamera.position.y - yOffset ) )
			{
				pos.y += FieldHeight;
			}
			else if ( pos.y > ( theCamera.position.y + yOffset ) )
			{
				pos.y -= FieldHeight;
			}

			Stars[ i ].position = pos - transform.position;
		}
		Particles.SetParticles( Stars, Stars.Length );

		Vector3 newPos = theCamera.position * ParallaxFactor;														// Calculate the position of the object
		newPos.z = 0;																																		// Force Z-axis to zero, since we're in 2D
		transform.position = newPos;
	}

	// GetRandomInRectangle
	//----------------------------------------------------------
	// Get a random value within a certain rectangle area
	//
	Vector3 GetRandomInRectangle ( float width, float height )
	{
		float x = Random.Range( 0, width );
		float y = Random.Range( 0, height );
		return new Vector3 ( x - xOffset , y - yOffset, 0 );
	}
}
