# TextMeshPro Animator

TextMesh Proのテキストをアニメーションさせることができます。

![サンプル１](https://github.com/coposuke/TextMeshProAnimator/blob/image/TMPA1.gif)
![サンプル２](https://github.com/coposuke/TextMeshProAnimator/blob/image/TMPA2.gif)
![サンプル３](https://github.com/coposuke/TextMeshProAnimator/blob/image/TMPA3.gif)
![サンプル４](https://github.com/coposuke/TextMeshProAnimator/blob/image/TMPA4.gif)

## ■導入方法

### 方法１ 手動
`Clone or Download` から `Download ZIP` を選択してダウンロードしてください。  
解凍したら、Scriptsフォルダ を Assetsフォルダ の中に配置してください。

### 方法２ PackageManager
Packages/manifest.json に下記のように追記してください。
```
{
  "dependencies": {
    ...
    ...
    "com.copocopo.textmeshpro.animator": "https://github.com/coposuke/TextMeshProAnimator.git"
  }
}
```
追記後に Unity を起動すると自動的にインポートされます。

## ■使い方～はじめに～
TextMeshProGeometryAnimator は、TextMesh Proがアタッチされている GameObject にアタッチして使います。

コンポーネントにある `AnimationData` のパラメータが、文字に対するアニメーション内容になります。

![GameObjectにAttach](https://github.com/coposuke/TextMeshProAnimator/blob/image/HowToUse1.png)

## ■使い方～コンポーネント～

### - Progress
アニメーション進行度を調整できます。  
`Play By Progress` が True の時のみ機能します。

### - Play On Enable
コンポーネントが有効になったタイミングで自動的に開始します。  
`Play By Progress` が False の時のみ機能します。

### - Play By Progress
`Progress` で アニメーション進行度を調整できるようになります。  
Animator と連携したい場合などに有効です。

### - AnimationData
アニメーションとなるタイミングや動きに関するデータ群です。
データの種類は下記の通りです。

#### Speed
アニメーション再生速度です。

#### Use Max Visible Character
アニメーションデータの Alpha の時間に合わせて、  
TextMesh Pro の機能の MaxVisibleCharacter を連動させます。  
これにより、マーカーや下線などの表示タイミングを合わせることが出来ます。

#### Pivot
回転（Rotation）や 拡縮（Scale）の基準点を調節できます。

#### 各種パターン
|パターン名|内容|
|-|-|
|Position|座標を動かします|
|Rotation|回転します|
|Scale|拡大/縮小します|
|Alpha|透明度を変更します|
|Color|色を変更します|
|PositionNoise|座標にノイズ（ばらつき）を加えます|
|RotationNoise|回転にノイズ（ばらつき）を加えます|
|ScaleNoise|拡縮にノイズ（ばらつき）を加えます|
|AlphaNoise|透明度にノイズ（ばらつき）を加えます|
|ColorNoise|色にノイズ（ばらつき）を加えます|

## ■使い方～パターン～
### 通常パターンの使い方
#### Use
使用するフラグです。  
使用しない場合は False にすることで、処理負荷を抑えられます。

#### Delay
アニメーションの開始タイミングを遅らせます。

#### Wave
テキスト毎の開始タイミングを遅らせます。  

#### Time
アニメーションが終了する時間を設定します。  

#### Curve
From - To のパラメータの影響度を設定します。  
横軸は アニメーション時間（0.0 ～ 1.0）  
縦軸は 影響度（0.0 = From、1.0 = To）

#### From
開始時点の値（差分）を設定します。

#### To
終了時点の値（差分）を設定します。

### ノイズパターンの使い方
機能が上記と同じ部分は省略いたします。

#### Curve
ノイズの影響度を設定します。  
横軸は アニメーション時間（0.0 ～ 1.0）  
縦軸は 影響度（テクスチャのXYZ(RGB)値に乗算）

#### Noise Texture
XYZ（RGB）が入力されたテクスチャを設定します。  
Black(0,0,0) は -0.5  
Gray(0.5,0.5,0.5) は 0.0  
White(1,1,1) は +0.5  
となります。

#### Offset
ノイズテクスチャのUVに対するオフセットを設定します。

#### Tiling
ノイズテクスチャのUVに対するタイリング（繰り返し）を設定します。

#### Speed
ノイズテクスチャのUVスクロールする速度を設定します。

## ■License
[MIT](https://github.com/coposuke/TextMeshProAnimator/blob/master/LICENSE)
