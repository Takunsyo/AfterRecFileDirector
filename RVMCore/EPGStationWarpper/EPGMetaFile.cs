using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RVMCore.EPGStationWarpper
{
    public class EPGMetaFile
    {
        public Api.RecordedProgram Meta {
            get
            {
                return Api.RecordedProgram.Deserialize(this.mMeta);
            }
            private set
            {
                this.mMeta = value.Serialize();
            }
        }
        private byte[] mMeta { get; set; }

        private byte[] mLogo { get; set; }

        public Image Logo {
            get
            {
                using (System.IO.MemoryStream st = new System.IO.MemoryStream())
                {
                    if(mLogo ==null || mLogo.Length <=0) return null;
                    st.Write(mLogo, 0, mLogo.Length);
                    return Image.FromStream(st);
                }
            }
        }

        private byte[] mThumb { get; set; }

        public Image ThumbImage
        {
            get
            {
                using (System.IO.MemoryStream st = new System.IO.MemoryStream())
                {
                    if (mThumb == null || mThumb.Length <= 0) return null;
                    st.Write(mThumb, 0, mThumb.Length);
                    return Image.FromStream(st);
                }
            }
        }

        private static readonly byte[] m_head = new byte[]{0x54,0x56,0x41,0x46}; //TVAF in ASCII

        private byte[] Header
        {
            get
            {
                int req = (this.mMeta.Length << 12) + (this.mLogo == null? 0:this.mLogo.Length);
                var tmp = BitConverter.GetBytes(req);
                //Here have 1 byte for futher dev.
                return m_head.AppendArray(tmp);
            }
        }

        private byte[] Body
        {
            get
            {
                var tmp = this.mMeta.AppendArray(mLogo);
                return tmp.AppendArray(mThumb);
            }
        }

        public EPGMetaFile(byte[] jMeta, byte[] cLogo, byte[] thumb)
        {
            this.mMeta = jMeta;
            this.mLogo = cLogo;
            this.mThumb = thumb;    
        }

        public EPGMetaFile(EPGAccess access,int rid)
        {
            this.Meta = access.GetRecordProgramByID(rid);
            this.mLogo = access.GetChannelLogoBytesByID(this.Meta.channelId);
            this.mThumb = access.GetRecordedThumbnailBytesByID(rid);
        }

        public EPGMetaFile(EPGAccess access, Api.RecordedProgram obj)
        {
            this.Meta = obj;
            this.mLogo = access.GetChannelLogoBytesByID(this.Meta.channelId);
            this.mThumb = access.GetRecordedThumbnailBytesByID(obj.id);
        }

        public byte[] GetBytes()
        {
            return this.Header.AppendArray(Body);
        }

        public static EPGMetaFile ReadByte(byte[] data)
        {
            for (int i = 0; i <= 3; i++ )
            {
                if (!(data[i].Equals(m_head[i])))
                {
                    "Meta Data type dismatch!!".ErrorLognConsole();
                    return null;
                }
            }
            int offset = 8;
            var tmp = BitConverter.ToInt32(data,4);
            tmp = tmp& 0xFFFFFF;
            int MetaLength = tmp >> 12;
            int LogoLength = tmp & 0xFFF;
            byte[] Meta = new byte[MetaLength];
            byte[] Logo = new byte[LogoLength];
            Array.Copy(data, offset, Meta, 0, MetaLength);
            offset += MetaLength;
            Array.Copy(data, offset, Logo, 0, LogoLength);
            offset += LogoLength;
            int ThumbLength = data.Length -offset;
            byte[] Thumb = new byte[ThumbLength];
            Array.Copy(data, offset, Thumb, 0, ThumbLength);
            var rep = new EPGMetaFile(Meta,Logo,Thumb);
            return rep;
        }

        public bool WtiteFile(string path)
        {
            path = path.ToLower().EndsWith(".meta") ? path : path + ".meta";
            try { 
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Move(path, path + ".old");
                }
            }
            catch
            {
                "Meta file is already exits, and unable to overwrite!\"{0}\"".ErrorLognConsole(path);
                return false;
            }
            try
            { 
            using (System.IO.FileStream sw = new System.IO.FileStream(path,System.IO.FileMode.OpenOrCreate,System.IO.FileAccess.ReadWrite))
            {
                var tmp = this.GetBytes();
                sw.Write(tmp,0,tmp.Length);
            }
            System.IO.File.SetAttributes(path, System.IO.File.GetAttributes(path) | System.IO.FileAttributes.Hidden);
            return true;
            }
            catch(Exception ex)
            {
                "Unable to create or write meta file:\"{0}\"".ErrorLognConsole(path);
                ex.Message.InfoLognConsole();
                return false;
            }
        }

        public static EPGMetaFile ReadFile(string path)
        {
            if (System.IO.File.Exists(path)) { 
                using (System.IO.FileStream ssr = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                {
                    long len = ssr.Length;
                    byte[] tmp = new byte[len];
                    ssr.Read(tmp, 0, (int)len);
                    return EPGMetaFile.ReadByte(tmp);
                }
            }
            else
            {
                "Unable to open or read meta file:\"{0}\"".ErrorLognConsole(path);
                return null;
            }
        }
    }
}
