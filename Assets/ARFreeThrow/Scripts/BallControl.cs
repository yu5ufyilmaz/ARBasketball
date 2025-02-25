using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(Rigidbody))]
public class BallControl : MonoBehaviour
{
	// Atış gücünü belirten parametre
	public float m_ThrowForce = 100f;

	// X ve Y ekseninde atış yönü için yazılan parametreler
	public float m_ThrowDirectionX = 0.17f;
	public float m_ThrowDirectionY = 0.67f;

	// Kamera ve top arasındaki bağlantıyı dengelemek için yazılan kod
	public Vector3 m_BallCameraOffset = new Vector3(0f, -1.4f, 3f);

	private Vector3 startPosition;
	private Vector3 direction;
	private float startTime;
	private float endTime;
	private float duration;
	private bool directionChosen = false;
	private bool throwStarted = false;

	[SerializeField]
	GameObject ARCam;

	[SerializeField]
	ARSessionOrigin m_SessionOrigin;

	Rigidbody rb;

	private void Start(){
		rb = gameObject.GetComponent<Rigidbody>();
		m_SessionOrigin = GameObject.Find("AR Session Origin").GetComponent<ARSessionOrigin>();
		ARCam = m_SessionOrigin.transform.Find("AR Camera").gameObject;
		transform.parent = ARCam.transform;
		ResetBall();
	}

	private void Update(){

		// Topun atışını başlatan ve buna göre potaya gönderecek fonksiyon
		if(Input.GetMouseButtonDown(0)){ // Mouse ve dokunmayla çalışmasına olanak sağlar
			startPosition = Input.mousePosition;
			startTime = Time.time;
			throwStarted = true;
			directionChosen = false;
		} 
		// Topun atışını bitiren ve buna göre sonraki komutu çalıştıran fonksiyon
		else if (Input.GetMouseButtonUp(0)) { // Mouse ve dokunmayla çalışmasına olanak sağlar
			endTime = Time.time;
			duration = endTime - startTime;
			direction = Input.mousePosition - startPosition;
			directionChosen = true;
		}

		// Topun atışı sırasında yönü seçmemizi sağlayan fonksiyon
		if (directionChosen) {
			rb.mass = 1;
			rb.useGravity = true;

			rb.AddForce(
				ARCam.transform.forward * m_ThrowForce / duration +
				ARCam.transform.up * direction.y * m_ThrowDirectionY +
				ARCam.transform.right * direction.x * m_ThrowDirectionX);

			startTime = 0.0f;
			duration = 0.0f;

			startPosition = new Vector3(0, 0, 0);
			direction = new Vector3(0, 0, 0);

			throwStarted = false;
			directionChosen = false;
		}

		// 5 saniyeden sonra topun resetlenmesini sağlar. Aşağıdaki ResetBall fonksiyonunu kullanır
		if(Time.time - endTime >= 5 && Time.time - endTime <= 6)
			ResetBall();

	}

	public void ResetBall(){
		rb.mass = 0;
		rb.useGravity = false;
		rb.linearVelocity = Vector3.zero;
		rb.angularVelocity = Vector3.zero;
		endTime = 0.0f;

		Vector3 ballPos = ARCam.transform.position + ARCam.transform.forward * m_BallCameraOffset.z + ARCam.transform.up * m_BallCameraOffset.y;
		transform.position = ballPos;
	}

}
