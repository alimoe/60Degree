﻿using UnityEngine;
using System.Collections;

public class InputControl : Core.MonoSingleton<InputControl> {


	private Vector3 currentPressedPosition;
	private bool valideTap;
    public void ChangePressedPosition(Vector3 position)
    {
        currentPressedPosition = position;
    }

	void Update () {
		if (AppControl.Instance.IsPlaying ()) {

				if (Input.GetMouseButtonDown (0)) {
		
						currentPressedPosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
						AppControl.Instance.HandleTap (currentPressedPosition);
						valideTap = true;
				}

				if (Input.GetMouseButton (0)) {
						AppControl.Instance.HandleDrag (Camera.main.ScreenToWorldPoint (Input.mousePosition));
				}

				if (Input.GetMouseButtonUp (0)) {
						//Debug.Log("Input.mousePosition"+Input.mousePosition);
						if (!valideTap)return;
								
						Vector3 delta = Camera.main.ScreenToWorldPoint (Input.mousePosition) - currentPressedPosition;
						if (delta.magnitude < 0.15f)return;
								
						float angle = (Mathf.Atan2 (delta.y, delta.x) / Mathf.PI) * 180f;
						if (angle < 0)angle = 360f + angle;
								
						float thred = 30f;
						if (angle < thred || angle > 360f - thred) {
								AppControl.Instance.HandleSwipe (currentPressedPosition, BoardDirection.Right);
			
						} else if (angle < 60f + thred && angle > 60f - thred) {
								AppControl.Instance.HandleSwipe (currentPressedPosition, BoardDirection.TopRight);
			
						} else if (angle < 120f + thred && angle > 120f - thred) {
								AppControl.Instance.HandleSwipe (currentPressedPosition, BoardDirection.TopLeft);
			
						} else if (angle < 180f + thred && angle > 180f - thred) {
								AppControl.Instance.HandleSwipe (currentPressedPosition, BoardDirection.Left);
			
						} else if (angle < 240f + thred && angle > 240f - thred) {
								AppControl.Instance.HandleSwipe (currentPressedPosition, BoardDirection.BottomLeft);
			
						} else if (angle < 300f + thred && angle > 300f - thred) {
								AppControl.Instance.HandleSwipe (currentPressedPosition, BoardDirection.BottomRight);
			
						}
		
				}

			}
			else {
				valideTap = false;
			}
	}
}
