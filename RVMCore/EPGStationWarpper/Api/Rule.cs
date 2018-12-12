using System;

namespace RVMCore.EPGStationWarpper.Api
{
    public class Rule
    {
        public int id { get; set; }
        /// <summary> 検索文字列 </summary>
        public string keyword { get; set; }
        /// <summary> 検索除外文字列 </summary>
        public string ignoreKeyword { get; set; }
        /// <summary> 大文字小文字を区別する </summary>
        public bool keyCS { get; set; }
        /// <summary> 正規表現 </summary>
        public bool keyRegExp { get; set; }
        /// <summary> タイトルを検索範囲に含む </summary>
        public bool title { get; set; }
        /// <summary> 詳細を検索範囲に含む </summary>
        public bool description { get; set; }
        /// <summary> 拡張を検索範囲に含む </summary>
        public bool extended { get; set; }
        public bool GR { get; set; }
        public bool BS { get; set; }
        public bool CS { get; set; }
        public bool SKY { get; set; }
        public long station { get; set; }
        public int genrelv1 { get; set; }
        public int genrelv2 { get; set; }
        /// <summary> 開始時刻 </summary>
        public int startTime { get; set; }
        /// <summary> 時刻範囲 </summary>
        public int timeRange { get; set; }
        /// <summary> 曜日 </summary>
        public WeekFlag week { get; set; }
        /// <summary> 無料放送だけか </summary>
        public bool isFree { get; set; }
        /// <summary> 最小長 </summary>
        public long durationMin { get; set; }
        /// <summary> 最大長 </summary>
        public long durationMax { get; set; }
        /// <summary> ルールが有効か </summary>
        public bool enable { get; set; }
        /// <summary> 録画データの保存場所 </summary>
        public string directory { get; set; }
        /// <summary> 録画ファイル名のフォーマット </summary>
        public string recordedFormat { get; set; }
        /// <summary> 録画モード 1 </summary>
        public int mode1 { get; set; }
        /// <summary> 録画モード 1 の保存場所 </summary>
        public string directory1 { get; set; }
        /// <summary> 録画モード 2 </summary>
        public int mode2 { get; set; }
        /// <summary> 録画モード 2 の保存場所 </summary>
        public string directory2 { get; set; }
        /// <summary> 録画モード 3 </summary>
        public int mode3 { get; set; }
        /// <summary> 録画モード 3 の保存場所 </summary>
        public string directory3 { get; set; }
        /// <summary> エンコード後にオリジナルファイルを削除するか </summary>
        public bool delTs { get; set; }
        [Flags]
        public enum WeekFlag
        {
            Sunday = 0x01,
            Monday =0x02,
            Tuesday = 0x04,
            Wednesday = 0x08,
            Thursday = 0x10,
            Friday = 0x20,
            Saturday = 0x40
        }
    }
}
