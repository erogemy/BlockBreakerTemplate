# BlockBreakerTemplate

## これはなに
Unityで誰でもブロック崩しが作れるテンプレートプロジェクトです。

## 使い方
### Unityのインストール
1. https://unity.com/ja/download からUnity Hubをダウンロードしてください。このツールがUnityをインストールしたり起動したりするものです。
1. Unity Hubを起動して、Installs > Install Editor > 任意のバージョン（UNITY 6系がオススメ）と進んでください。
1. 「Add modules」はインストールするモジュールの選択画面です。少し下にスクロールして、「Web Build Support」にチェックを入れてください。

### Unityプロジェクトの作成
1. Unity HubのProjectsで「+ New Project」ボタンを押してください。
1. 画面真ん中で利用するテンプレートを選択します。「Universal 2D」を選んでください。
1. 画面左の「Project Name」でプロジェクトに名前を付けられます。自由に名前を付けましょう。
1. 「+ Create Project」ボタンを押してプロジェクトを作成しましょう。自動でUnityが立ち上がります。

### BlockBreakerTemplateのインストール
1. Unityの画面上のメニューから、Window > Package Managerを選択してください。
1. 小さなウィンドウが出てきたら、左上の「＋▼」を押して「Install package from git URL...」を押してください。
1. 出てきたテキスト入力欄に以下をコピペして、入力欄右の「install」を押してください。
  ```
  https://github.com//erogemy/BlockBreakerTemplate.git?path=/Assets/#main
  ```

### ツールの使い方
1.　Unityの画面上のメニューから、Tools > Erogemy > BlockBreaker > BlockBreakerBuilderWindow を選択してください。
1. あとはウィンドウに使い方が書いてあります。

## 凝った使い方
### 複数ステージを作りたい
* Assets/Images/にPhase_1、Phase_2...と順番にフォルダを作るとステージクリア型になります。

### 文字の色を変えたい
* ヒエラルキーで`TMP_Text`と検索すると文字オブジェクトにアクセスできます。詳しくは「Unity TextMeshPro」などでweb検索してみてください。
