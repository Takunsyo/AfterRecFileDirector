# AfterRecFileDirector
Windowsで作動する録画ファイルを類別、シリーズごとに分別するアプリケーションである。
未完成ではありますが、一応使えます。

以下のようにEPGStationの設定にいればEPGStationと合わせて使うことができます。
"recordedEndCommand": "E:\\AfterRecFileDirector.exe -epgstation"

グーグルドライブへのアップロード機能を追加しました。
AfterRecFileDirector.exe -upload
で呼び出せばアップロード管理画面が出ます。

MirakurunのAPIへの完全対応を追加しました。
管理機能がまだ開発中ですが
var service = new MirakurunService(serviceAddress);
でMirakurunのすべての機能にアクセスできます。例えば録画としますと
CancellationTokenSource ct = new CancellationTokenSource();
service.StreamServiceToFile(サービスID, @"E:\1.ts", ct.Token).Start();
でE:\1.tsへ録画が始まります。止まる時は　ct.Cancel();　で止まります。

マイクロソフトのOneDriveへの対応も開発していますが、時間がかかりそうです。
