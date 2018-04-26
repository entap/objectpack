# ObjectPack
## 概要
C#用のJSON/MessagePackのエンコーダー、デコーダー。ORマッパー機能を搭載。
そのうちチューニング頑張る。あ、MessagePackはまだ実装してないんだけどね。

https://entap.github.io/objectpack/

## DLLのダウンロード(.NET framework 3.5)
https://entap.github.io/objectpack/ObjectPack.dll

## 簡単な使い方

JSONの文字列をモデルにマッピングする。

    Json.Decode<TestModel>("{\"i\":123,\"d\":1.0,\"s\":\"xyz\",\"b\":true}");
    class TestModel {
        public int i;
        public double d;
        public string s;
        public bool b;
    }

配列とか型を指定してマッピングする。

    Json.Decode<List<int>>("[1,2,3]");

C#の標準のコレクション(objectを要素に取ったHashMapとArrayList)として、デコードする。

    Json.Decode("[1,2,3]");

マッピングがデフォルトでゆるい。
int[]とかのArray型、Dictionary型に対応。
JSONで文字列でも、マッピング先が数値なら、int.Parseとかしてくれる。
