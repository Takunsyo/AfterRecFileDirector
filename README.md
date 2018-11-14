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

マイクロソフトのOneDriveへの対応も開発していますが、時間がかかりそうです。
