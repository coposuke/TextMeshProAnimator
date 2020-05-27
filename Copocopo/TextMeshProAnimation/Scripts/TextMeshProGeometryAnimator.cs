// ===================================
//
// Copyright(c) 2020 Copocopo All rights reserved.
// https://github.com/coposuke/TextMeshProAnimator
//
// ===================================


using UnityEngine;
using TMPro;


/// <summary>
/// TextMeshPro ジオメトリアニメーション
/// <note>テキスト内容をランタイムで変更し続けるとGCを多く発生しますので、ご留意ください</note>
/// </summary>
public class TextMeshProGeometryAnimator : MonoBehaviour
{
	/// <summary>
	/// アニメーションプログレス(エディタ確認用)
	/// </summary>
	[SerializeField, Range(0.0f, 1.0f)]
	private float progress = 0.0f;

	/// <summary>
	/// Enable時に再生するかどうか
	/// </summary>
	[SerializeField]
	private bool playOnEnable = false;

	/// <summary>
	/// progressで再生するかどうか
	/// </summary>
	[SerializeField]
	private bool playByProgress = false;

	/// <summary>
	/// アニメーション中かどうか
	/// </summary>
	public bool isAnimating { get { return time < maxTime; } }

	/// <summary>
	/// 文字送りアニメーションデータ
	/// </summary>
	[SerializeField]
	public TextMeshProGeometryAnimation animationData;


	/// <summary>TextMeshPro Textコンポーネント</summary>
	private TMP_Text textComponent;
	/// <summary>textComponent.textInfoのキャッシュ</summary>
	private TMP_TextInfo textInfo;
	/// <summary>textComponent.textに変更があったかどうか</summary>
	private bool hasTextChanged;
	/// <summary>アニメーション時間</summary>
	private float time;
	/// <summary>アニメーション最大時間</summary>
	private float maxTime;

	/// <summary>頂点座標のキャッシュ</summary>
	private Vector3[][] baseVertices = default;
	/// <summary>頂点カラーのキャッシュ</summary>
	private Color32[][] baseColors = default;
	/// <summary>頂点座標のアニメーション後</summary>
	private Vector3[][] animatedVertices = default;
	/// <summary>頂点カラーのアニメーション後</summary>
	private Color32[][] animatedColors = default;


#if UNITY_EDITOR
	/// <summary>
	/// Unity Event OnValidate
	/// </summary>
	private void OnValidate()
	{
		if (this.textComponent == null)
		{
			this.textComponent = GetComponent<TMP_Text>();
		}

		this.time = this.maxTime * this.progress;
		UpdateText(true);
		UpdateAnimation();
	}
#endif

	/// <summary>
	/// Unity Event Awake
	/// </summary>
	private void Awake()
	{
		this.textComponent = GetComponent<TMP_Text>();
		ON_TEXT_CHANGED(textComponent);
	}

	/// <summary>
	/// Unity Event OnEnable
	/// </summary>
	private void OnEnable()
	{
		// Subscribe to event fired when text object has been regenerated.
		TMPro_EventManager.TEXT_CHANGED_EVENT.Add(ON_TEXT_CHANGED);

		if (this.playOnEnable)
			Play();
		else
			Finish();
	}

	/// <summary>
	/// Unity Event OnDisable
	/// </summary>
	private void OnDisable()
	{
		TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(ON_TEXT_CHANGED);

		Finish();
	}

	/// <summary>
	/// Unity Event Update
	/// </summary>
	private void Update()
	{
		if (null == animationData) { return; }
		if (null == textInfo) { return; }

		if (playByProgress)
		{
			time = maxTime * progress;
		}
		else
		{
			if (maxTime <= time) { return; }
			if (maxTime <= 0.0f) { return; }
			time += Time.deltaTime;
		}

		if (maxTime <= time)
		{
			time = maxTime;
		}

#if UNITY_EDITOR
		progress = time / maxTime;
#endif

		UpdateAnimation();
	}

	/// <summary>
	/// TextMesh Proのtext変更時に呼び出されるメソッドです
	/// OnEnableとOnDisableにてTMPro_EventManagerに登録しています
	/// </summary>
	/// <param name="obj"></param>
	private void ON_TEXT_CHANGED(Object obj)
	{
		if (obj == this.textComponent)
			this.hasTextChanged = true;
	}

	/// <summary>
	/// アニメーションデータの上書き設定
	/// </summary>
	public void SetAnimation(TextMeshProGeometryAnimation animation)
	{
		this.animationData = animation;
	}

	/// <summary>
	/// 再生
	/// </summary>
	public void Play()
	{
		this.time = 0.0f;
		UpdateText(true);
		UpdateAnimation();
	}

	/// <summary>
	/// 強制終了
	/// </summary>
	public void Finish()
	{
		this.time = this.maxTime;
		UpdateAnimation();
	}

	/// <summary>
	/// TMPro Textの情報更新
	/// </summary>
	private void UpdateText(bool forceUpdate)
	{
		if (this.hasTextChanged || forceUpdate)
		{
			// ForceMeshUpdate は GCAlloc が発生するので取り扱い注意！！
			this.textComponent.ForceMeshUpdate(true);
			this.textInfo = textComponent.textInfo;

			// 各アニメーション要素で、一番時間がかかるものを最大時間として計算
			maxTime = Mathf.Max(
				CalcAnimationTotalTime(this.textInfo.characterCount, this.animationData.position),
				CalcAnimationTotalTime(this.textInfo.characterCount, this.animationData.rotation),
				CalcAnimationTotalTime(this.textInfo.characterCount, this.animationData.scale),
				CalcAnimationTotalTime(this.textInfo.characterCount, this.animationData.alpha),
				CalcAnimationTotalTime(this.textInfo.characterCount, this.animationData.color),
				CalcAnimationTotalTime(this.textInfo.characterCount, this.animationData.positionNoise),
				CalcAnimationTotalTime(this.textInfo.characterCount, this.animationData.rotationNoise),
				CalcAnimationTotalTime(this.textInfo.characterCount, this.animationData.scaleNoise),
				CalcAnimationTotalTime(this.textInfo.characterCount, this.animationData.alphaNoise),
				CalcAnimationTotalTime(this.textInfo.characterCount, this.animationData.colorNoise)
			);

			// ForceMeshUpdate にて ON_TEXT_CHANGED がコールされるので最後にフラグを下す
			this.hasTextChanged = false;
		}
	}

	/// <summary>
	/// 頂点データのキャッシュ
	/// </summary>
	/// <returns>成功判定</returns>
	private bool UpdateCachedVertex()
	{
		// 頂点キャッシュの確保
		if (this.baseVertices == null)
			this.baseVertices = new Vector3[textInfo.materialCount][];

		if (this.baseColors == null)
			this.baseColors = new Color32[textInfo.materialCount][];

		if (this.animatedVertices == null)
			this.animatedVertices = new Vector3[textInfo.materialCount][];

		if (this.animatedColors == null)
			this.animatedColors = new Color32[textInfo.materialCount][];

		// 頂点キャッシュの内容更新
		for (int i = 0; i < textInfo.materialCount; i++)
		{
			TMP_MeshInfo meshInfo = textInfo.meshInfo[i];

			if (meshInfo.vertices == null || meshInfo.colors32 == null)
				return false;

			// 要素数に変更があった場合は配列再確保
			if (this.animatedVertices[i] == null || this.animatedVertices[i].Length != meshInfo.vertices.Length)
				this.animatedVertices[i] = new Vector3[meshInfo.vertices.Length];

			if (this.animatedColors[i] == null || this.animatedColors[i].Length != meshInfo.colors32.Length)
				this.animatedColors[i] = new Color32[meshInfo.colors32.Length];

			// MeshInfo 内の配列がごっそり変わった場合は参照切り替え & コピー
			if (this.baseVertices[i] != meshInfo.vertices)
			{
				this.baseVertices[i] = meshInfo.vertices;
				System.Array.Copy(meshInfo.vertices, this.animatedVertices[i], meshInfo.vertices.Length);
			}

			if (this.baseColors[i] != meshInfo.colors32)
			{
				this.baseColors[i] = meshInfo.colors32;
				System.Array.Copy(meshInfo.colors32, this.animatedColors[i], meshInfo.colors32.Length);
			}
		}

		return true;
	}

	/// <summary>
	/// TMPro Textの頂点情報の編集
	/// </summary>
	private void UpdateAnimation()
	{
		UpdateText(false);

		// マーカーや下線等の追加描画物はどうしても描画されてしまうので、
		// 表示最大数も合わせてアニメーションすることで対応しています。
		// 恐らくtextInfo.meshInfoのどこかにありますが、未調査の為この実装になります。
		if (this.animationData.useMaxVisibleCharacter)
		{
			var maxVisibleCharacters = CalcAnimationCharacterCount(time, this.animationData.alpha);
			if (this.textComponent.maxVisibleCharacters != maxVisibleCharacters)
			{
				// ForceMeshUpdate は GCAlloc が発生するので取り扱い注意！！
				this.textComponent.maxVisibleCharacters = maxVisibleCharacters;
				this.textComponent.ForceMeshUpdate(true);
				this.textInfo = textComponent.textInfo;
			}
		}

		// アニメーション用の頂点キャッシュ更新
		if (!UpdateCachedVertex())
			return;

		// 開始時等MeshInfoの生成が遅れるケースがあったため小さい数値をforに使用
		var count = Mathf.Min(this.textInfo.characterCount, this.textInfo.characterInfo.Length);
		for (int i = 0; i < count; i++)
		{
			var charInfo = this.textInfo.characterInfo[i];

			// Skip characters that are not visible and thus have no geometry to manipulate.
			if (!charInfo.isVisible)
				continue;

			// Get the index of the material used by the current character.
			int materialIndex = this.textInfo.characterInfo[i].materialReferenceIndex;

			// Get the index of the first vertex used by this text element.
			int vertexIndex = this.textInfo.characterInfo[i].vertexIndex;

			// Determine the center point of each character at the baseline.
			//Vector2 charMidBasline = new Vector2((sourceVertices[vertexIndex + 0].x + sourceVertices[vertexIndex + 2].x) / 2, charInfo.baseLine);
			// Determine the center point of each character.
			//Vector2 charMidBasline = (sourceVertices[vertexIndex + 0] + sourceVertices[vertexIndex + 2]) / 2;

			// Need to translate all 4 vertices of each quad to aligned with middle of character / baseline.
			// This is needed so the matrix TRS is applied at the origin for each character.
			//Vector3 offset = charMidBasline;

			if (animationData.position.use || animationData.rotation.use || animationData.scale.use || animationData.positionNoise.use || animationData.rotationNoise.use)
			{
				// Get the cached vertices of the mesh used by this text element (character or sprite).
				Vector3[] verts = this.baseVertices[materialIndex];
				var vertex0 = verts[vertexIndex];
				var vertex1 = verts[vertexIndex + 1];
				var vertex2 = verts[vertexIndex + 2];
				var vertex3 = verts[vertexIndex + 3];

				if (animationData.position.use)
				{
					var ratio = animationData.position.curve.Evaluate(CalcAnimationTime(time, i, animationData.position));
					var delta = Vector3.LerpUnclamped(animationData.position.from, animationData.position.to, ratio);
					vertex0 += delta;
					vertex1 += delta;
					vertex2 += delta;
					vertex3 += delta;
				}

				if (animationData.rotation.use)
				{
					var ratio = animationData.rotation.curve.Evaluate(CalcAnimationTime(time, i, animationData.rotation));
					var delta = Vector3.LerpUnclamped(animationData.rotation.from, animationData.rotation.to, ratio);
					var center = Vector3.Scale(vertex2 - vertex0, animationData.pivot) + vertex0;
					var matrix = Matrix4x4.Rotate(Quaternion.Euler(delta));
					vertex0 = matrix.MultiplyPoint(vertex0 - center) + center;
					vertex1 = matrix.MultiplyPoint(vertex1 - center) + center;
					vertex2 = matrix.MultiplyPoint(vertex2 - center) + center;
					vertex3 = matrix.MultiplyPoint(vertex3 - center) + center;
				}

				if (animationData.scale.use)
				{
					var ratio = animationData.scale.curve.Evaluate(CalcAnimationTime(time, i, animationData.scale));
					var delta = Vector3.LerpUnclamped(animationData.scale.from, animationData.scale.to, ratio);
					var center = Vector3.Scale(vertex2 - vertex0, animationData.pivot) + vertex0;
					vertex0 = Vector3.Scale(vertex0 - center, delta) + center;
					vertex1 = Vector3.Scale(vertex1 - center, delta) + center;
					vertex2 = Vector3.Scale(vertex2 - center, delta) + center;
					vertex3 = Vector3.Scale(vertex3 - center, delta) + center;
				}

				if (animationData.positionNoise.use)
				{
					var tex = animationData.positionNoise.noiseTexture;
					var uv = new Vector2(i / (float)count, 0.0f) * animationData.positionNoise.tiling + animationData.positionNoise.offset;
					uv = uv + animationData.positionNoise.speed * Time.timeSinceLevelLoad;
					var noise = tex.GetPixel(Mathf.FloorToInt(uv.x % 1.0f * tex.width), Mathf.FloorToInt(uv.y % 1.0f * tex.height));

					var ratio = animationData.positionNoise.curve.Evaluate(CalcAnimationTime(time, i, animationData.positionNoise));
					var delta = new Vector3(noise.r, noise.g, noise.b) * 2.0f - Vector3.one;
					delta = delta * ratio;
					vertex0 += delta;
					vertex1 += delta;
					vertex2 += delta;
					vertex3 += delta;
				}

				if (animationData.rotationNoise.use)
				{
					var tex = animationData.rotationNoise.noiseTexture;
					var uv = new Vector2(i / (float)count, 0.0f) * animationData.rotationNoise.tiling + animationData.rotationNoise.offset;
					uv = uv + animationData.rotationNoise.speed * Time.timeSinceLevelLoad;
					var noise = tex.GetPixel(Mathf.FloorToInt(uv.x % 1.0f * tex.width), Mathf.FloorToInt(uv.y % 1.0f * tex.height));

					var ratio = animationData.rotationNoise.curve.Evaluate(CalcAnimationTime(time, i, animationData.rotationNoise));
					var delta = new Vector3(noise.r, noise.g, noise.b) * 2.0f - Vector3.one;
					var center = Vector3.Scale(vertex2 - vertex0, animationData.pivot) + vertex0;
					var matrix = Matrix4x4.Rotate(Quaternion.Euler(delta * ratio));
					vertex0 = matrix.MultiplyPoint(vertex0 - center) + center;
					vertex1 = matrix.MultiplyPoint(vertex1 - center) + center;
					vertex2 = matrix.MultiplyPoint(vertex2 - center) + center;
					vertex3 = matrix.MultiplyPoint(vertex3 - center) + center;
				}

				if (animationData.scaleNoise.use)
				{
					var tex = animationData.scaleNoise.noiseTexture;
					var uv = new Vector2(i / (float)count, 0.0f) * animationData.scaleNoise.tiling + animationData.scaleNoise.offset;
					uv = uv + animationData.scaleNoise.speed * Time.timeSinceLevelLoad;
					var noise = tex.GetPixel(Mathf.FloorToInt(uv.x % 1.0f * tex.width), Mathf.FloorToInt(uv.y % 1.0f * tex.height));

					var ratio = animationData.scaleNoise.curve.Evaluate(CalcAnimationTime(time, i, animationData.scaleNoise));
					var delta = Vector3.Lerp(Vector2.one, new Vector3(noise.r, noise.g, noise.b) * 2.0f - Vector3.one, ratio);
					var center = Vector3.Scale(vertex2 - vertex0, animationData.pivot) + vertex0;
					vertex0 = Vector3.Scale(vertex0 - center, delta) + center;
					vertex1 = Vector3.Scale(vertex1 - center, delta) + center;
					vertex2 = Vector3.Scale(vertex2 - center, delta) + center;
					vertex3 = Vector3.Scale(vertex3 - center, delta) + center;
				}

				Vector3[] animatedVerts = this.animatedVertices[materialIndex];
				animatedVerts[vertexIndex] = vertex0;
				animatedVerts[vertexIndex + 1] = vertex1;
				animatedVerts[vertexIndex + 2] = vertex2;
				animatedVerts[vertexIndex + 3] = vertex3;
			}

			if (animationData.color.use || animationData.alpha.use || animationData.colorNoise.use || animationData.alphaNoise.use)
			{
				// Get the cached vertices of the mesh used by this text element (character or sprite).
				Color32[] colors = this.baseColors[materialIndex];
				var color0 = colors[vertexIndex];
				var color1 = colors[vertexIndex + 1];
				var color2 = colors[vertexIndex + 2];
				var color3 = colors[vertexIndex + 3];

				if (animationData.color.use)
				{
					var ratio = animationData.color.curve.Evaluate(CalcAnimationTime(time, i, animationData.color));
					color0 = animationData.color.gradient.Evaluate(ratio);
					color1 = color2 = color3 = color0;
				}

				if (animationData.alpha.use)
				{
					var ratio = animationData.alpha.curve.Evaluate(CalcAnimationTime(time, i, animationData.alpha));
					float alpha = Mathf.Lerp(animationData.alpha.from, animationData.alpha.to, ratio);
					color0.a = (byte)(color0.a * Mathf.Clamp01(alpha));
					color1 = color2 = color3 = color0;
				}

				if (animationData.colorNoise.use)
				{
					var tex = animationData.colorNoise.noiseTexture;
					var uv = new Vector2(i / (float)count, 0.0f) * animationData.colorNoise.tiling + animationData.colorNoise.offset;
					uv = uv + animationData.colorNoise.speed * Time.timeSinceLevelLoad;
					var noise = tex.GetPixel(Mathf.FloorToInt(uv.x % 1.0f * tex.width), Mathf.FloorToInt(uv.y % 1.0f * tex.height));

					var ratio = animationData.colorNoise.curve.Evaluate(CalcAnimationTime(time, i, animationData.colorNoise));
					color0 = Color32.Lerp(color0, color0 * noise, ratio);
					color1 = Color32.Lerp(color1, color0 * noise, ratio);
					color2 = Color32.Lerp(color2, color0 * noise, ratio);
					color3 = Color32.Lerp(color3, color0 * noise, ratio);
				}

				if (animationData.alphaNoise.use)
				{
					var tex = animationData.alphaNoise.noiseTexture;
					var uv = new Vector2(i / (float)count, 0.0f) * animationData.alphaNoise.tiling + animationData.alphaNoise.offset;
					uv = uv + animationData.alphaNoise.speed * Time.timeSinceLevelLoad;
					var noise = tex.GetPixel(Mathf.FloorToInt(uv.x % 1.0f * tex.width), Mathf.FloorToInt(uv.y % 1.0f * tex.height));

					var ratio = animationData.alphaNoise.curve.Evaluate(CalcAnimationTime(time, i, animationData.alphaNoise));
					color0.a = (byte)Mathf.Lerp(color0.a, color0.a * Mathf.Clamp01(noise.r), ratio);
					color1 = color2 = color3 = color0;
				}

				Color32[] animatedColors = this.animatedColors[materialIndex];
				animatedColors[vertexIndex] = color0;
				animatedColors[vertexIndex + 1] = color1;
				animatedColors[vertexIndex + 2] = color2;
				animatedColors[vertexIndex + 3] = color3;
			}
		}

		// 表示しているマテリアルの数だけ頂点を更新します
		// <Material>や<Font>でロードした情報はmeshInfoにキャッシュされていることに注意が必要です
		for (int i = 0; i < textInfo.materialCount; i++)
		{
#if UNITY_EDITOR
			// OnValidateにてMeshの生成が遅れるケースがあったためNullチェック
			if (textInfo.meshInfo[i].mesh == null) { continue; }
#endif
			// Push changes into meshes
			textInfo.meshInfo[i].mesh.vertices = this.animatedVertices[i];
			textInfo.meshInfo[i].mesh.colors32 = this.animatedColors[i];
			textComponent.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
		}
	}

	/// <summary>
	/// 文字送りアニメーションデータの要素の最大時間を算出
	/// </summary>
	static private float CalcAnimationTotalTime(int characterCount, TextMeshProGeometryAnimation.ItemBase item)
	{
		if (characterCount <= 0) { return 0.0f; }
		if (item == null) { return 0.0f; }
		if (!item.use) { return 0.0f; }
		return item.delay + (characterCount - 1) * item.wave + item.time;
	}

	/// <summary>
	/// 文字送りアニメーションデータの要素の特定文字の相対時間を算出
	/// </summary>
	static private float CalcAnimationTime(float time, int characterIndex, TextMeshProGeometryAnimation.ItemBase item)
	{
		if (time < item.delay) { return 0.0f; }
		if (item.time <= 0.0f) { return 1.0f; }
		return Mathf.Clamp01(((time - item.delay) - (characterIndex * item.wave)) / item.time);
	}

	/// <summary>
	/// 文字送りアニメーションデータの要素と絶対時間から何文字目までWaveしているか算出
	/// </summary>
	static private int CalcAnimationCharacterCount(float time, TextMeshProGeometryAnimation.ItemBase item)
	{
		if (item.wave <= 0.0f) { return int.MaxValue; }
		return (int)((time - item.delay) / item.wave);
	}
}
