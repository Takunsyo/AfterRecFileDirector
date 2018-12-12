using RVMCore.EPGStationWarpper.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RVMCore.EPGStationWarpper
{
    public static class Helper
    {
        /// <summary>
        /// Get <see cref="StreamFile"/> object from EPGStation's recorded file <see cref="Program"/>.
        /// </summary>
        /// <param name="body"></param>
        /// <param name="mEPG">A working access to EPGStation.</param>
        /// <returns></returns>
        public static StreamFile GetStreamFileObj(this Program body, EPGAccess mEPG)
        {
            if (body == null) return null;
            StreamFile mFile = new StreamFile();
            mFile.ChannelName = mEPG.GetChannelNameByID(body.channelId);
            mFile.Content = body.description;
            mFile.Infomation = body.extended;
            mFile.Title = body.name;
            mFile.recTitle = body.name;
            mFile.recKeyWord = "EPGStation " + body.id.ToString();
            mFile.recSubTitle = body.videoResolution + body.videoResolution + body.videoType;
            mFile.recKeywordInfo = string.Format("Error:E({0})D({1})S({2})", body.errorCnt, body.dropCnt, body.scramblingCnt);
            mFile.Genre = body.Genre;
            mFile.StartTime = GetTimeFromUNIXTime(body.startAt);
            mFile.EndTime = GetTimeFromUNIXTime(body.endAt);
            mFile.FilePath = System.IO.Path.Combine(mEPG.BaseFolder, System.Web.HttpUtility.UrlDecode(body.filename));
            mFile.EPGStation = new EPGMetaFile(mEPG,  body);
            return mFile;
        }

        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> items, Action<T> action){
            foreach (T i in items)
            {
                action(i);
            }
            return items;
        }

        public static string ToRecString(this ProgramGenre genre)
        {
            switch (genre)
            {
                case ProgramGenre.News: return "ニュース・報道"; 
                case ProgramGenre.Sports: return "スポーツ"; 
                case ProgramGenre.Drama: return "ドラマ"; 
                case ProgramGenre.Music: return "音楽"; 
                case ProgramGenre.Variety: return "バラエティー"; 
                case ProgramGenre.Movie: return "映画"; 
                case ProgramGenre.Anime: return "アニメ・特撮"; 
                case ProgramGenre.Infomation: return "情報・ワイドショー"; 
                case ProgramGenre.Documantry: return "ドキュメンタリー";
                case ProgramGenre.Live: return "劇場・公演"; 
                case ProgramGenre.Education: return "趣味・教育"; 
                default: return "その他"; 
            }
        }

        /// <summary>
        /// Get Channel Name <see cref="string"/> by id in a list of <see cref="EPGchannel"/>
        /// </summary>
        /// <param name="c_list">Channel list.</param>
        /// <param name="cid">Channel id</param>
        /// <returns></returns>
        public static string GetChannelNameByID(this IEnumerable<EPGchannel> c_list, long cid)
        {
            try
            {
                return c_list.First(x => x.id == cid).name;
            }
            catch
            {
                return null;
            }
        }

        public static DateTime GetTimeFromUNIXTime(long time)
        {
            System.DateTime dateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
            dateTime = dateTime.AddSeconds(time / 1000);
            return dateTime;
        }

        public static long GetUNIXTimeFromDatetime(this DateTime time)
        {
            return (long)(time.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }

        public static T[] AppendArray<T>(this T[] source, T[] array)
        {
            if (source == null & array == null) return null;
            if (source == null) return array;
            if (array == null) return source;
            int ltmp = source.Length;
            var tmp = source;
            Array.Resize(ref tmp, ltmp + array.Length);
            Array.Copy(array, 0, tmp, ltmp, array.Length);
            return tmp;
        }
    }
}
