# AfterRecFileDirector
Windowsで作動する録画ファイルを類別、シリーズごとに分別するアプリケーションである。
未完成ではありますが、一応使えます。

以下のように[EPGStation](https://github.com/l3tnun/EPGStation/)の設定にいればEPGStationと合わせて使うことができます。
```
"recordedEndCommand": "E:\\AfterRecFileDirector.exe -epgstation"
```
グーグルドライブへのアップロード機能を追加しました。
```
AfterRecFileDirector.exe -upload
```
で呼び出せばアップロード管理画面が出ます。

[Mirakurun](https://github.com/Chinachu/Mirakurun)のAPIへの完全対応を追加しました。
管理機能がまだ開発中ですが
```c#
var service = new MirakurunService(serviceAddress);
```
でMirakurunのすべての機能にアクセスできます。例えば録画としますと
```c#
CancellationTokenSource ct = new CancellationTokenSource();
service.StreamServiceToFile(サービスID, @"E:\1.ts", ct.Token).Start();
```
でE:\1.tsへ録画が始まります。止まる時は　```ct.Cancel();```　で止まります。

デモとしてMirakuruViewerを作りました。
```
AfterRecFileDirector.exe -mirakurun
```
で立ち上げます、一応テレビは見れますが、そもそもプログラミングなど趣味でやっているようなものですから、すごーい機能や綺麗なUIは期待できない。

マイクロソフトのOneDriveへの対応も開発していますが、時間がかかりそうです。

本プロジェクトで引用したライブラリーは以下の通りです：
DLLをできるだけ一つに纏まるため引用[Fody/Costura](https://github.com/Fody/Costura)
ログを作るた[apache/log4net](https://logging.apache.org/log4net/)
メディアを動画で表示するための[Sascha-L/WPF-MediaKit](https://github.com/Sascha-L/WPF-MediaKit)
DirectShowでいろいろ試そうとして引用した(こちらはDllではなくコードをコピペして少し編集しました)[DirectShowNET Library
](http://directshownet.sourceforge.net)
その他グーグルドライブ用に[Google.Apis.Drive.v3]、マイクロソフトワンドライブ用に[Microsoft.OneDrive.Sdk]等をも引用しました。
バイナリやコードを引用したわけではないが、作動に欠けないものとして[Chinachu/Mirakurun](https://github.com/Chinachu/Mirakurun)の働きがすべてのデータを提供しています。
