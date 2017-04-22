using UnityEngine;
using System.Collections;

public class EyesController : MonoBehaviour {
	public SkinnedMeshRenderer eyes;
	public float weight = 100.0f;
	public bool isAngry = false;
	public bool isSuprise = false;

	private bool setValues = false;
	private bool suprise_previous = false;
	private bool angry_previous = false;

	[Header("Index of BlendShapes")]
	[SerializeField] private int brow_mid_down_left = 15;
	[SerializeField] private int brow_mid_down_right = 16;
	[SerializeField] private int brow_mid_up_left = 17;
	[SerializeField] private int brow_mid_up_right = 18;
	[SerializeField] private int brow_outer_down_left = 19;
	[SerializeField] private int brow_outer_down_right = 20;
	[SerializeField] private int brow_outer_up_left = 21;
	[SerializeField] private int brow_outer_up_right = 22;
	[SerializeField] private int brow_squeeze = 23;

	// Update is called once per frame
	void Update () {
		if (suprise_previous != isSuprise || angry_previous != isAngry) {
			setValues = false;
			suprise_previous = isSuprise;
			angry_previous = isAngry;
		}
		if (isAngry && isSuprise == false && setValues == false) {
			setValues = true;
			SetAngry ();
		} else if (isAngry == false && isSuprise && setValues == false) {
			setValues = true;
			SetSuprise ();
		} else if(isAngry == false && isSuprise == false && setValues == false) {
			setValues = true;
			SetNormal ();
		}
	}

	void SetSuprise() {
		SetNormal ();
		eyes.SetBlendShapeWeight (brow_mid_up_left, weight);
		eyes.SetBlendShapeWeight (brow_mid_up_right, weight);
		eyes.SetBlendShapeWeight (brow_outer_up_left, weight);
		eyes.SetBlendShapeWeight (brow_outer_up_right, weight);
	}
	void SetAngry() {
		SetNormal ();
		eyes.SetBlendShapeWeight (brow_mid_down_left, weight);
		eyes.SetBlendShapeWeight (brow_mid_down_right, weight);
		eyes.SetBlendShapeWeight (brow_outer_down_left, weight);
		eyes.SetBlendShapeWeight (brow_outer_down_right, weight);
		eyes.SetBlendShapeWeight (brow_squeeze, weight);
	}
	void SetNormal() {
		eyes.SetBlendShapeWeight (brow_mid_down_left, 0);
		eyes.SetBlendShapeWeight (brow_mid_down_right, 0);
		eyes.SetBlendShapeWeight (brow_outer_down_left, 0);
		eyes.SetBlendShapeWeight (brow_outer_down_right, 0);
		eyes.SetBlendShapeWeight (brow_mid_up_left, 0);
		eyes.SetBlendShapeWeight (brow_mid_up_right, 0);
		eyes.SetBlendShapeWeight (brow_outer_up_left, 0);
		eyes.SetBlendShapeWeight (brow_outer_up_right, 0);
		eyes.SetBlendShapeWeight (brow_squeeze, 0);
	}
}
