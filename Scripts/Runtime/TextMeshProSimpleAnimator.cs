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
	public bool isAnimating { get; private set; } = false;

	/// <summary>
	/// 1文字あたりの表示速度
	/// </summary>
	public float speedPerCharacter = 0.1f;
	
	/// <summary>
	/// 自動再生
	/// </summary>
	[SerializeField]
	private bool playOnEnable = false;

	/// <summary>
	/// ループするかどうか
	/// </summary>
	public bool isLoop = false;

	/// <summary>
	/// TextMeshPro
	/// </summary>
	private TMPro.TMP_Text text = default;

	/// <summary>
	/// アニメーション時間
	/// </summary>
	private float time = 0.0f;
	

	/// <summary>
	/// Override Unity Function
	/// </summary>
	private void Awake()
	{
		text = GetComponent<TMPro.TMP_Text>();
	}

	/// <summary>
	/// Override Unity Function
	/// </summary>
	private void Start()
	{
		if (this.playOnEnable) { Play(); }
	}

	/// <summary>
	/// Override Unity Function
	/// </summary>
	private void OnEnable()
	{
		if (this.playOnEnable) { Play(); }
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

		this.time = 0.0f;
		this.isAnimating = true;
		this.text.ForceMeshUpdate(true);
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
		float maxTime = (maxVisibleCharacters + 1) * speedPerCharacter;

		this.time += deltaTime;

		int visibleCharacters = Mathf.Clamp(Mathf.FloorToInt(time / speedPerCharacter), 0, maxVisibleCharacters);
		if (text.maxVisibleCharacters != visibleCharacters)
			text.maxVisibleCharacters = visibleCharacters;

		if (this.time > maxTime)
		{
			if (this.isLoop)
			{
				time = time % maxTime;
			}
			else
			{
				Finish();
			}
		}
	}
}
