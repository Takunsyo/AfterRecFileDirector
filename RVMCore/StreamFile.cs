using System;
using System.Runtime.Serialization;
using System.Security.Cryptography;


namespace RVMCore
{
    [Serializable,Obsolete("This object is for Output infomation only.",false)]
    public class StreamFile : ISerializable
    {
        /// <summary> %1 (-i)録画したファイルのフルパース </summary>
        public string FilePath;
        /// <summary> %c (-g)番組のジャンル </summary>
        public ProgramGenre Genre;
        /// <summary> %d 番組のタイトル </summary>
        public string Title;
        /// <summary> %8 予約タイトル </summary>
        public string recTitle;
        /// <summary> %9 予約サブタイトル this may not be need.</summary>
        public string recSubTitle;
        /// <summary> %5 (-c)チャンネル名 </summary>
        public string ChannelName;
        /// <summary> %e 番組の番組内容 </summary>
        public string Content;
        /// <summary> %f 番組の番組詳細 </summary>
        public string Infomation;
        /// <summary> %i/%j %k 番組情報:開始時間 </summary>
        public DateTime StartTime;
        /// <summary> %i/%j %l　番組情報:終了時間 </summary>
        public DateTime EndTime;
        /// <summary> %a キーワード検索予約時のタイトルキーワード </summary>
        public string recKeyWord;
        /// <summary> %b キーワード検索予約時の詳細キーワード </summary>
        public string recKeywordInfo;
        /// <summary> 録画ファイル独自のID、自動生成される。 </summary>
        public Guid ID;
        public StreamFile()
        {
            ID = Guid.NewGuid();
        }

        public StreamFile(SerializationInfo info, StreamingContext context)
        {
            FilePath = (string)info.GetValue("FilePath", typeof(string));
            Genre = (ProgramGenre)info.GetValue("Genre", typeof(ProgramGenre));
            Title = (string)info.GetValue("Title", typeof(string));
            recTitle = (string)info.GetValue("rTitle", typeof(string));
            recSubTitle = (string)info.GetValue("rSubTitle", typeof(string));
            ChannelName = (string)info.GetValue("ChannelName", typeof(string));
            Content = (string)info.GetValue("Content", typeof(string));
            Infomation = (string)info.GetValue("Infomation", typeof(string));
            StartTime = (DateTime)info.GetValue("StartTime", typeof(DateTime));
            EndTime = (DateTime)info.GetValue("EndTime", typeof(DateTime));
            ID = (Guid)info.GetValue("ID", typeof(Guid));
            recKeyWord = (string)info.GetValue("rKeyword", typeof(string));
            recKeywordInfo = (string)info.GetValue("rKWInfo", typeof(string));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("FilePath", FilePath, typeof(string));
            info.AddValue("Genre", Genre, typeof(ProgramGenre));
            info.AddValue("Title", Title, typeof(string));
            info.AddValue("rTitle", recTitle, typeof(string));
            info.AddValue("rSubTitle", recSubTitle, typeof(string));
            info.AddValue("ChannelName", ChannelName, typeof(string));
            info.AddValue("Content", Content, typeof(string));
            info.AddValue("Infomation", Infomation, typeof(string));
            info.AddValue("StartTime", StartTime, typeof(DateTime));
            info.AddValue("EndTime", EndTime, typeof(DateTime));
            info.AddValue("ID", ID, typeof(Guid));
            info.AddValue("rKeyword", recKeyWord, typeof(string));
            info.AddValue("rKWInfo", recKeywordInfo, typeof(string));
        }
        /// <summary>
        /// Save current <see cref="StreamFile"/> object to a XML file.
        /// </summary>
        /// <param name="filePath">FILE PATH MEANS FILE PATH you fuck.</param>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="IO.DirectoryNotFoundException"></exception>
        /// <exception cref="IO.PathTooLongException"></exception>
        /// <exception cref="IO.IOException"></exception>
        /// <exception cref="Security.SecurityException"></exception>
        public void ToXml(string filePath)
        {
            string path = filePath.ToLower().EndsWith(".xml") ? filePath : filePath + ".xml";
            System.Xml.Serialization.XmlSerializer sr = new System.Xml.Serialization.XmlSerializer(typeof(StreamFile));

            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(path))
            {
                sr.Serialize(sw, this);
            }
            System.IO.File.SetAttributes(path, System.IO.File.GetAttributes(path) | System.IO.FileAttributes.Hidden);
        }
        /// <summary>
        /// Read <see cref="StreamFile"/> object from a XML file.
        /// </summary>
        /// <param name="filePath">the full path of the xml file including file name.</param>
        /// <exception cref="InvalidOperationException">if it fucks up, don't be mad.</exception>
        public static StreamFile FromXml(string filePath)
        {
            System.Xml.Serialization.XmlSerializer sr = new System.Xml.Serialization.XmlSerializer(typeof(StreamFile));
            StreamFile result = new StreamFile();
            using (System.IO.StreamReader ssr = new System.IO.StreamReader(filePath))
            {
                result = (StreamFile)sr.Deserialize(ssr);
            }
            return result;
        }

        public void ToXMLCrypto(string filePath, string SecretKey)
        {
            string path = filePath.ToLower().EndsWith(".exml") ? filePath : filePath + ".exml";
            System.Xml.Serialization.XmlSerializer sr = new System.Xml.Serialization.XmlSerializer(typeof(StreamFile));
            byte[] oringalData = null;
            using (System.IO.MemoryStream sw = new System.IO.MemoryStream())
            {
                sr.Serialize(sw, this);
                oringalData = sw.ToArray();
            }
            if (oringalData == null)
                throw new Exception("Unable to serialize Object!");
            // Dim encryptedData As Byte() = Nothing
            // Dim rsa As New RSACryptoServiceProvider
            // Dim RsaParas As New RSAParameters
            // Initialze Public key
            byte[] PublicKey = System.Text.Encoding.Unicode.GetBytes(SecretKey);
            // input keys
            // RsaParas.Exponent = {1, 0, 1}
            // RsaParas.Modulus = PublicKey
            RijndaelManaged RMCrypto = new RijndaelManaged();
            RMCrypto.Key = PublicKey;
            RMCrypto.IV = PublicKey;
            using (System.IO.FileStream fs = new System.IO.FileStream(path, System.IO.FileMode.OpenOrCreate))
            {
                using (CryptoStream cs = new CryptoStream(fs, RMCrypto.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(oringalData, 0, oringalData.Length);
                }
            }
            System.IO.File.SetAttributes(path, System.IO.File.GetAttributes(path) | System.IO.FileAttributes.Hidden);
        }

        public static StreamFile ReadXMLCrypto(string filePath, string SecretKey)
        {
            byte[] file;
            byte[] PublicKey = System.Text.Encoding.Unicode.GetBytes(SecretKey);
            RijndaelManaged RMCrypto = new RijndaelManaged();
            RMCrypto.Key = PublicKey;
            RMCrypto.IV = PublicKey;
            using (System.IO.FileStream fs = new System.IO.FileStream(filePath, System.IO.FileMode.Open))
            {
                using (CryptoStream cs = new CryptoStream(fs, RMCrypto.CreateEncryptor(), CryptoStreamMode.Read))
                {
                    file = new byte[cs.Length];
                    cs.Read(file, 0, (int)cs.Length);
                }
            }
            if (file == null)
                throw new Exception("Unable to serialize Object!");
            System.Xml.Serialization.XmlSerializer sr = new System.Xml.Serialization.XmlSerializer(typeof(StreamFile));
            StreamFile result = new StreamFile();
            using (System.IO.MemoryStream ssr = new System.IO.MemoryStream(file))
            {
                result = (StreamFile)sr.Deserialize(ssr);
            }
            return result;
        }

        public EPGStationWarpper.EPGMetaFile EPGStation { get; set; }
    }

}
