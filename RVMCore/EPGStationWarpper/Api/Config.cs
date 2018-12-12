namespace RVMCore.EPGStationWarpper.Api
{
    class Config
    {
        /// <summary>
        /// エンコードが有効か
        /// </summary>
        public bool enableEncode { get; set; }
        /// <summary>
        /// ライブ視聴が有効か
        /// </summary>
        public bool enableLiveStreaming { get; set; }
        /// <summary>
        /// エンコードオプション名
        /// </summary>
        public string[] encodeOption { get; set; }
        /// <summary>
        /// 手動予約時のエンコード設定番号
        /// </summary>
        public int defaultEncode { get; set; }
        /// <summary>
        /// 手動予約時にデフォルトで ts を削除するか
        /// </summary>
        public bool delTs { get; set; }
        /// <summary>
        /// 有効になっている放送波
        /// </summary>
        public Boradcast boradcast { get; set; }
        /// <summary>
        /// 録画ファイル視聴アプリ設定
        /// </summary>
        public Recordedviewer recordedViewer { get; set; }
        /// <summary>
        /// 録画ファイルダウンロードアプリ設定
        /// </summary>
        public Recordeddownloader recordedDownloader { get; set; }
        /// <summary>
        /// mpeg ts streaming ライブ視聴の設定
        /// </summary>
        public string[] mpegTsStreaming { get; set; }
        /// <summary>
        /// mpeg ts streaming の 視聴アプリ設定
        /// </summary>
        public Mpegtsviewer mpegTsViewer { get; set; }
        /// <summary>
        /// 録画ストリーミング配信
        /// </summary>
        public Recordedstreaming recordedStreaming { get; set; }
        /// <summary>
        /// HLS での録画済みファイルの配信設定
        /// </summary>
        public string[] recordedHLS { get; set; }
        /// <summary>
        /// HLS ライブ配信の設定
        /// </summary>
        public string[] liveHLS { get; set; }
        /// <summary>
        /// HLS 視聴アプリ設定
        /// </summary>
        public Hlsviewer HLSViewer { get; set; }
        /// <summary>
        /// WebM ライブ配信の設定
        /// </summary>
        public string[] liveWebM { get; set; }
        /// <summary>
        /// MP4 ライブ配信の設定
        /// </summary>
        public string[] liveMP4 { get; set; }
        /// <summary>
        /// kodi host 名の一覧
        /// </summary>
        public string[] kodiHosts { get; set; }

        public class Boradcast
        {
            public bool GR { get; set; }
            public bool BS { get; set; }
            public bool CS { get; set; }
            public bool SKY { get; set; }
        }

        public class Recordedviewer
        {
            public string ios { get; set; }
            public string android { get; set; }
            public string mac { get; set; }
            public string win { get; set; }
        }

        public class Recordeddownloader
        {
            public string ios { get; set; }
            public string android { get; set; }
            public string mac { get; set; }
            public string win { get; set; }
        }

        public class Mpegtsviewer
        {
            public string ios { get; set; }
            public string android { get; set; }
            public string mac { get; set; }
            public string win { get; set; }
        }

        public class Recordedstreaming
        {
            public string[] mpegTs { get; set; }
            public string[] webm { get; set; }
            public string[] mp4 { get; set; }
        }

        public class Hlsviewer
        {
            public string ios { get; set; }
            public string android { get; set; }
            public string mac { get; set; }
            public string win { get; set; }
        }


    }
}
