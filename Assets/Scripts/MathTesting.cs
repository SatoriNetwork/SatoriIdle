using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MathTesting : MonoBehaviour
{
    [SerializeField] TMP_InputField AValue;
    [SerializeField] TMP_InputField BValue;
    [SerializeField] TextMeshProUGUI Output;
    [SerializeField] Button Format;
    [SerializeField] Button ADD;
    [SerializeField] Button Subtract;
    [SerializeField] Button Multiply;
    [SerializeField] Button GreaterThan;
    [SerializeField] Button LessThan;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Format.onClick.AddListener(() => {

			BGN A = new BGN(Convert.ToInt32(AValue.text));
            Output.text = A.ToString();
		});
        ADD.onClick.AddListener(() => {
            BGN A = new BGN(Convert.ToInt32(AValue.text));
            BGN B = new BGN(Convert.ToInt32(BValue.text));
            BGN Total = A + B;
            Output.text = Total.ToString();
            
		});
		Subtract.onClick.AddListener(() => {
			BGN A = new BGN(Convert.ToInt32(AValue.text));
			BGN B = new BGN(Convert.ToInt32(BValue.text));
			BGN Total = A - B;
			Output.text = Total.ToString();

		});
		Multiply.onClick.AddListener(() => {
			BGN A = new BGN(Convert.ToInt32(AValue.text));
			BGN B = new BGN(Convert.ToInt32(BValue.text));
			BGN Total = A * B;
			Output.text = Total.ToString();

		});
		GreaterThan.onClick.AddListener(() => {
			BGN A = new BGN(Convert.ToInt32(AValue.text));
			BGN B = new BGN(Convert.ToInt32(BValue.text));
			bool Total = A > B;
			Output.text = Total.ToString();

		});
		LessThan.onClick.AddListener(() => {
			BGN A = new BGN(Convert.ToInt32(AValue.text));
			BGN B = new BGN(Convert.ToInt32(BValue.text));
			bool Total = A < B;
			Output.text = Total.ToString();

		});
	}
}
