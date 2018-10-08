
namespace RVMCore
{
    public enum ProgramGenre
    {
        /// <summary> ニュース・報道 </summary>
        News        = 0b1,
        /// <summary> スポーツ </summary>
        Sports      = 0b10,
        /// <summary> ドラマ </summary>
        Infomation  = 0b100,
        /// <summary> ドキュメンタリー </summary>
        Drama       = 0b1000,
        /// <summary> 音楽 </summary>
        Music       = 0b10000,
        /// <summary> バラエティー </summary>
        Variety     = 0b100000,
        /// <summary> 映画 </summary>
        Movie       = 0b1000000,
        /// <summary> アニメ・特撮 </summary>
        Anime       = 0b10000000,
        /// <summary> 情報・ワイドショー </summary>
        Documantry  = 0b100000000,
        /// <summary> 劇場・公演 </summary>
        Live        = 0b1000000000,
        /// <summary> 趣味・教育 </summary>
        Education   = 0b10000000000,
        /// <summary> その他 </summary>
        Others,
        Default = Others
    }
    /// <summary>
    /// Sub genre under <see cref="ProgramGenre.News"/>
    /// </summary>
    public enum NewsGenre
    {
        /// <summary>
        /// 定時・総合
        /// </summary>
        Comprehensive   = 0b1,
        /// <summary>
        /// 天気
        /// </summary>
        Weather         = 0b10,
        /// <summary>
        /// 特集・ドキュメント
        /// </summary>
        Documental      = 0b100,
        /// <summary>
        /// 政治・国会
        /// </summary>
        Political       = 0b1000,
        /// <summary>
        /// 経済・市況
        /// </summary>
        Economic        = 0b10000,
        /// <summary>
        /// 海外・国際
        /// </summary>
        International   = 0b100000,
        /// <summary>
        /// 解説
        /// </summary>
        Commentary      = 0b1000000,
        /// <summary>
        /// 討論・会談
        /// </summary>
        Discussion      = 0b10000000,
        /// <summary>
        /// 報道特集
        /// </summary>
        Special         = 0b100000000,
        /// <summary>
        /// ローカル・地域
        /// </summary>
        Local           = 0b1000000000,
        /// <summary>
        /// 交通
        /// </summary>
        Traffic         = 0b10000000000,
        /// <summary>
        /// その他
        /// </summary>
        Others
    }
    /// <summary>
    /// Sub genre under <see cref="ProgramGenre.Sports"/>
    /// </summary>
    public enum SportsGenre
    {
        /// <summary>
        /// スポーツニュース
        /// </summary>
        SportNews       = 0b1,
        /// <summary>
        /// 野球
        /// </summary>
        BaseBall        = 0b10,
        /// <summary>
        /// サッカー
        /// </summary>
        Soccer          = 0b100,
        /// <summary>
        /// ゴルフ
        /// </summary>
        Golf            = 0b1000,
        /// <summary>
        /// その他の球技
        /// </summary>
        OtherBalls      = 0b10000,
        /// <summary>
        /// 相撲・格闘技
        /// </summary>
        Fighting        = 0b100000,
        /// <summary>
        /// オリンピック・国際大会
        /// </summary>
        Olympics        = 0b1000000,
        /// <summary>
        /// マラソン・陸上・水泳
        /// </summary>
        Athletics       = 0b10000000,
        /// <summary>
        /// モータースポーツ
        /// </summary>
        Motor           = 0b100000000,
        /// <summary>
        /// マリン・ウィンタースポーツ
        /// </summary>
        Marine          = 0b1000000000,
        /// <summary>
        /// 競馬・公営競技
        /// </summary>
        Horseracing     = 0b10000000000,
        /// <summary> その他 </summary>
        Others
    }
    /// <summary>
    /// Sub genre under <see cref="ProgramGenre.Infomation"/>
    /// </summary>
    public enum InfomationGenre
    {
        /// <summary>
        /// 芸能・ワイドショー
        /// </summary>
        Performing      = 0b1,
        /// <summary>
        /// ファッション
        /// </summary>
        Faison          = 0b10,
        /// <summary>
        /// 暮らし・住まい
        /// </summary>
        Dwelling        = 0b100,
        /// <summary>
        /// 健康・医療
        /// </summary>
        Medical         = 0b1000,
        /// <summary>
        /// ショッピング・通販
        /// </summary>
        OnlineBusiness  = 0b10000,
        /// <summary>
        /// グルメ・料理
        /// </summary>
        Gourmet         = 0b100000,
        /// <summary>
        /// イベント
        /// </summary>
        Event           = 0b1000000,
        /// <summary>
        /// 番組紹介・お知らせ
        /// </summary>
        Notification    = 0b10000000,
        /// <summary> その他 </summary>
        Others

    }
    /// <summary>
    /// Sub genre under <see cref="ProgramGenre.Drama"/>
    /// </summary>
    public enum DramaGenre
    {
        /// <summary>
        /// 国内ドラマ
        /// </summary>
        DomesticDrama   =0b1,
        /// <summary>
        /// 海外ドラマ
        /// </summary>
        ForeignDrama    =0b10,
        /// <summary>
        /// 時代劇
        /// </summary>
        HistoricalDrama =0b100,
        /// <summary> その他 </summary>
        Others
    }
    /// <summary>
    /// Sub genre under <see cref="ProgramGenre.Music"/>
    /// </summary>
    public enum MusicGenre
    {
        /// <summary>
        /// 国内ロック・ポップス
        /// </summary>
        DomesticPops    = 0b1,
        /// <summary>
        /// 海外ロック・ポップス
        /// </summary>
        ForeignPops     = 0b10,
        /// <summary>
        /// クラシック・オペラ
        /// </summary>
        Classic         = 0b100,
        /// <summary>
        /// ジャズ・フュージョン
        /// </summary>
        Jazz            = 0b1000,
        /// <summary>
        /// 歌謡曲・演歌
        /// </summary>
        Ballad          = 0b10000,
        /// <summary>
        /// ライブ・コンサート
        /// </summary>
        Concert         = 0b100000,
        /// <summary>
        /// ランキング・リクエスト
        /// </summary>
        Ranks           = 0b1000000,
        /// <summary>
        /// カラオケ・のど自慢
        /// </summary>
        Karaoke         = 0b10000000,
        /// <summary>
        /// 民謡・邦楽
        /// </summary>
        FolkSong        = 0b100000000,
        /// <summary>
        /// 童謡・キッズ
        /// </summary>
        Kids            = 0b1000000000,
        /// <summary>
        /// 民族音楽・ワールドミュージック
        /// </summary>
        EthnicMusic     = 0b10000000000,
        /// <summary> その他 </summary>
        Others
    }
    /// <summary>
    /// Sub genre under <see cref="ProgramGenre.Variety"/>
    /// </summary>
    public enum VarietyGenre
    {
        /// <summary>
        /// クイズ
        /// </summary>
        Quiz    = 0b1,
        /// <summary>
        /// ゲーム
        /// </summary>
        Game = 0b10,
        /// <summary>
        /// トークバラエティ
        /// </summary>
        Talk = 0b100,
        /// <summary>
        /// お笑い・コメディ
        /// </summary>
        Comedy = 0b1000,
        /// <summary>
        /// 音楽バラエティ
        /// </summary>
        Musical = 0b10000,
        /// <summary>
        /// 旅バラエティ
        /// </summary>
        Travel = 0b100000,
        /// <summary>
        /// 料理バラエティ
        /// </summary>
        Gourmet = 0b1000000,
        /// <summary> その他 </summary>
        Others
    }
    /// <summary>
    /// Sub genre under <see cref="ProgramGenre.Movie"/>
    /// </summary>
    public enum MovieGenre
    {
        /// <summary>
        /// 洋画
        /// </summary>
        ForeignMovie=0b1,
        /// <summary>
        /// 邦画
        /// </summary>
        DomesticMovie = 0b10,
        /// <summary>
        /// アニメ
        /// </summary>
        AnimeMovie = 0b100,
        /// <summary> その他 </summary>
        Other
    }
    /// <summary>
    /// Sub genre under <see cref="ProgramGenre.Anime"/>
    /// </summary>
    public enum AnimeGenre
    {
        /// <summary>
        /// 国内アニメ
        /// </summary>
        DomesticAnime=0b1,
        /// <summary>
        /// 海外アニメ
        /// </summary>
        ForeignAnime = 0b10,
        /// <summary>
        /// 特撮
        /// </summary>
        SFX = 0b100,
        /// <summary> その他 </summary>
        Others
    }
    /// <summary>
    /// Sub genre under <see cref="ProgramGenre.Documantry"/>
    /// </summary>
    public enum DocumantryGenre
    {
        /// <summary>
        /// 社会・時事
        /// </summary>
        Society     = 0b1,
        /// <summary>
        /// 歴史・紀行
        /// </summary>
        History = 0b10,
        /// <summary>
        /// 自然・動物・環境
        /// </summary>
        Nature      = 0b100,
        /// <summary>
        /// 宇宙・科学・医学
        /// </summary>
        Science = 0b1000,
        /// <summary>
        /// カルチャー・伝統文化
        /// </summary>
        Culture = 0b10000,
        /// <summary>
        /// 文学・文芸
        /// </summary>
        Literature = 0b100000,
        /// <summary>
        /// スポーツ
        /// </summary>
        Sports = 0b1000000,
        /// <summary>
        /// ドキュメンタリー全般
        /// </summary>
        General = 0b10000000,
        /// <summary>
        /// インタビュー・討論
        /// </summary>
        Interview = 0b100000000,
        /// <summary> その他 </summary>
        Others
    }
    /// <summary>
    /// Sub genre under <see cref="ProgramGenre.Live"/>
    /// </summary>
    public enum LiveGenre
    {
        /// <summary>
        /// 現代劇・新劇
        /// </summary>
        Contemporary    = 0b1,
        /// <summary>
        /// ミュージカル
        /// </summary>
        Musical = 0b10,
        /// <summary>
        /// ダンス・バレエ
        /// </summary>
        Dance = 0b100,
        /// <summary>
        /// 落語・演芸
        /// </summary>
        Entertainment = 0b1000,
        /// <summary>
        /// 歌舞伎・古典
        /// </summary>
        JClassic = 0b10000,
        /// <summary> その他 </summary>
        Others
    }
    /// <summary>
    /// Sub genre under <see cref="ProgramGenre.Education"/>
    /// </summary>
    public enum EducationGenre
    {
        /// <summary>
        /// 旅・釣り・アウトドア
        /// </summary>
        Outdoor             = 0b1,
        /// <summary>
        /// 園芸・ペット・手芸
        /// </summary>
        Handicraft = 0b10,
        /// <summary>
        /// 音楽・美術・工芸
        /// </summary>
        Art = 0b100,
        /// <summary>
        /// 囲碁・将棋
        /// </summary>
        Go = 0b1000,
        /// <summary>
        /// 麻雀・パチンコ
        /// </summary>
        Gambling = 0b10000,
        /// <summary>
        /// 車・オートバイ
        /// </summary>
        Motor = 0b100000,
        /// <summary>
        /// コンピューター・ＴＶゲーム
        /// </summary>
        Computer = 0b1000000,
        /// <summary>
        /// 会話・語学
        /// </summary>
        Language = 0b10000000,
        /// <summary>
        /// 幼児・小学生
        /// </summary>
        ElementaryStudent = 0b100000000,
        /// <summary>
        /// 中学生・高校生
        /// </summary>
        JuniorHighStudent = 0b1000000000,
        /// <summary>
        /// 大学生・受験
        /// </summary>
        CollegeStudent = 0b10000000000,
        /// <summary>
        /// 生涯教育・資格
        /// </summary>
        AdultEducation = 0b100000000000,
        /// <summary>
        /// 教育問題
        /// </summary>
        EducationProblems = 0b1000000000000,
        /// <summary> その他 </summary>
        Others
    }
}