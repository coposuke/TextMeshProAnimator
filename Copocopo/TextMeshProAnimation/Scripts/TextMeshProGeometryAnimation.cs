// ===================================
//
// Copyright(c) 2020 Copocopo All rights reserved.
// https://github.com/coposuke/TextMeshProAnimator
//
// ===================================


using UnityEngine;


/// <summary>
/// 文字送りアニメーションデータ
/// </summary>
[System.Serializable]
public class TextMeshProGeometryAnimation
{
	[System.Serializable]
	public class ItemBase
	{
		/// <summary>この要素を使用するかどうか</summary>
		[Tooltip("この要素を使用するかどうか")]
		public bool use;

		/// <summary>開始遅延</summary>
		[Tooltip("開始遅延")]
		public float delay;

		/// <summary>1文字のウェーブ遅延時間(/s)</summary>
		[Tooltip("1文字のウェーブ遅延時間(/s)")]
		public float wave;

		/// <summary>1文字のカーブ時間(/s)</summary>
		[Tooltip("1文字のカーブ時間(/s)")]
		public float time;

		/// <summary>値の変動カーブ</summary>
		[Tooltip("値の変動カーブ")]
		public AnimationCurve curve;
	}

	[System.Serializable]
	public class ItemFromTo<TType> : ItemBase
	{
		/// <summary>開始の値</summary>
		[Tooltip("開始の値")]
		public TType from;

		/// <summary>終了の値</summary>
		[Tooltip("終了の値")]
		public TType to;
	}

	[System.Serializable]
	public class ItemGradient<TType> : ItemBase
	{
		/// <summary>段階的変化のある値</summary>
		[Tooltip("段階的変化のある値")]
		public TType gradient;
	}

	[System.Serializable]
	public class ItemNoize : ItemBase
	{
		/// <summary>ノイズテクスチャ</summary>
		[Tooltip("ノイズテクスチャ")]
		public Texture2D noiseTexture;

		/// <summary>ノイズUVオフセット</summary>
		[Tooltip("ノイズUVオフセット")]
		public Vector2 offset;

		/// <summary>ノイズUVタイリング</summary>
		[Tooltip("ノイズUVタイリング")]
		public Vector2 tiling;

		/// <summary>ノイズUVスクロール速度</summary>
		[Tooltip("ノイズUVスクロール速度")]
		public Vector2 speed;
	}

	[System.Serializable]
	public class ItemVector3 : ItemFromTo<Vector3> { }

	[System.Serializable]
	public class ItemVector2 : ItemFromTo<Vector2> { }

	[System.Serializable]
	public class ItemGradiet : ItemGradient<Gradient> { }

	[System.Serializable]
	public class ItemFloat : ItemFromTo<float> { }


	/// <summary>
	/// 再生速度(1chara/s)
	/// </summary>
	public float speed = 0.1f;

	/// <summary>
	/// 最大表示文字数をアニメーションさせるかどうか(下線等の描画を防ぐ)
	/// </summary>
	public bool useMaxVisibleCharacter = false;

	/// <summary>
	/// 回転/伸縮の軸
	/// </summary>
	public Vector2 pivot = new Vector2(0.5f, 0.5f);

	/// <summary>
	/// 座標アニメーション情報
	/// </summary>
	public ItemVector3 position;

	/// <summary>
	/// 座標アニメーション情報
	/// </summary>
	public ItemVector3 rotation;

	/// <summary>
	/// 拡縮アニメーション情報
	/// </summary>
	public ItemVector2 scale;

	/// <summary>
	/// 半透明アニメーション情報
	/// </summary>
	public ItemFloat alpha;

	/// <summary>
	/// 色アニメーション情報
	/// </summary>
	public ItemGradiet color;

	/// <summary>
	/// 座標ノイズ情報
	/// </summary>
	public ItemNoize positionNoise;

	/// <summary>
	/// 回転ノイズ情報
	/// </summary>
	public ItemNoize rotationNoise;

	/// <summary>
	/// 拡縮ノイズ情報
	/// </summary>
	public ItemNoize scaleNoise;

	/// <summary>
	/// 半透明ノイズ情報
	/// </summary>
	public ItemNoize alphaNoise;

	/// <summary>
	/// 色ノイズ情報
	/// </summary>
	public ItemNoize colorNoise;
}


/// <summary>
/// 文字送りアニメーションデータ
/// </summary>
[CreateAssetMenu(fileName = "Geometry Animation Asset", menuName = "TextMeshPro/Geometry Animation Asset")]
public class TextMeshProGeometryAnimationAsset : ScriptableObject
{
	/// <summary>
	/// 文字送りアニメーションデータ
	/// </summary>
	public TextMeshProGeometryAnimation animation;
}


