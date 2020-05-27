// ===================================
//
// Copyright(c) 2020 Copocopo All rights reserved.
// https://github.com/coposuke/TextMeshProAnimator
//
// ===================================


using UnityEngine;


/// <summary>
/// 文字送りアニメーション
/// </summary>
public class TextMeshProSimpleAnimator : MonoBehaviour
{
	/// <summary>
	/// アニメーション中かどうか
	/// </summary>
	[System.NonSerialized]
	public bool isAnimating = false;

	/// <summary>
	/// ループするかどうか
	/// </summary>
	public bool isLoop = false;

	/// <summary>
	/// 1文字あたりの表示速度
	/// </summary>
	public float speedPerCharacter = 0.1f;
	
	/// <summary>
	/// 自動再生
	/// </summary>
	[SerializeField]
	private bool isAuto = false;
	
	/// <summary>
	/// TextMeshPro
	/// </summary>
	private TMPro.TextMeshProUGUI text = default;

	/// <summary>
	/// アニメーション時間
	/// </summary>
	private float time = 0.0f;
	

	/// <summary>
	/// Override Unity Function
	/// </summary>
	private void Awake()
	{
		text = GetComponent<TMPro.TextMeshProUGUI>();
	}

	/// <summary>
	/// Override Unity Function
	/// </summary>
	private void Start()
	{
		if (this.isAuto) { Play(); }
	}

	/// <summary>
	/// Override Unity Function
	/// </summary>
	private void OnEnable()
	{
		if (this.isAuto) { Play(); }
	}

	/// <summary>
	/// Override Unity Function
	/// </summary>
	private void Update()
	{
		if(this.isAnimating)
			UpdateAnimation(Time.deltaTime);
	}

	/// <summary>
	/// アニメーション再生開始
	/// </summary>
	public void Play()
	{
		if (this.isAnimating)
			return;
		//if(!isAnimating)
		//	text.ForceMeshUpdate();

		this.isAnimating = true;
		UpdateAnimation(0.0f);
	}

	/// <summary>
	/// アニメーション強制終了
	/// </summary>
	public void Finish()
	{
		if (!this.isAnimating)
			return;

		this.isAnimating = false;
		this.text.maxVisibleCharacters = this.text.textInfo.characterCount;
		this.time = 0.0f;
	}

	/// <summary>
	/// アニメーション更新
	/// </summary>
	private void UpdateAnimation(float deltaTime)
	{
		int maxVisibleCharacters = this.text.textInfo.characterCount;
		float maxTime = maxVisibleCharacters * speedPerCharacter;

		this.time += deltaTime;
		if (this.time < maxTime)
		{
			int visibleCharacters = Mathf.FloorToInt(time / speedPerCharacter);
			if (text.maxVisibleCharacters != visibleCharacters)
				text.maxVisibleCharacters = visibleCharacters;
		}
		else
		{
			if (this.isLoop)
				this.time = 0.0f;
			else
				Finish();
		}
	}
}
