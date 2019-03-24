
var args = WScript.Arguments;
var shell = new ActiveXObject("WScript.Shell");

if( args.length < 1 ) {
	WScript.Echo("postBehavior.js  <対象ファイル> ");
	WScript.Quit(1);
}

// ファイル名をコマンドラインから取得
var targetFile = args(0);

//  ファイル関連の操作を提供するオブジェクトを取得
var fs = new ActiveXObject( "ADODB.Stream" );
fs.Type = 2;
fs.Charset = "UTF-8" ;

//  ファイルを読み取り専用で開く
fs.Open();
fs.LoadFromFile(targetFile);


//var file = fs.OpenTextFile( targetfile, 1, true, -1 );

while ( !fs.EOS ) {

  // 書き出しファイルの指定 (新規作成する)
  var output = new ActiveXObject("ADODB.Stream");
  output.Type = 2;
  output.Charset = "UTF-8";
  output.Open();

  // ファイルから一行読み込み -1：全行読み込み・-2：一行読み込み
  var lineStr = fs.ReadText(-2);    
  var flds = lineStr.split('\t');   // 

  //  0：文字列のみ書き込み・1：文字列 + 改行を書き込み
  output.WriteText(flds[1], 1);    

  // 書き出しファイルの保存
  //  1：指定ファイルがなければ新規作成・2：ファイルがある場合は上書き
  var outfile = "json.txt" ;
  output.SaveToFile(outfile, 2);
  output.Close();

  var cmd = "curl -H \"Content-type: application/json\" -XPUT \"http://localhost:9200/simba-ea-master/behaviorChunk/"+ flds[0] +"\" -d @json.txt" ;
  WScript.Echo ("command: " + cmd);
  var e = shell.Exec(cmd); // コマンドを実行
  while (e.Status == 0) {
    WScript.Sleep(100);
  }
  
}

//  ファイルを閉じる
fs.Close();

//  オブジェクトを解放
fs = null;

WScript.Echo( "終了" );



