﻿using System;
using System.Drawing;
using System.IO;

namespace RVMCore.EPGStationWarpper
{
    public class EPGMetaFile
    {
        public Api.Program Meta {
            get
            {
                return Api.Program.Deserialize(this.mMeta);
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
                using (MemoryStream st = new MemoryStream())
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
                using (MemoryStream st = new MemoryStream())
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
            byte[] tmp = null;
            int i = 0;
            while((tmp is null) & (i <=3))
            {
                System.Threading.Tasks.Task.Delay(1000);
                tmp = access.GetRecordedThumbnailBytesByID(rid);
                i++;
            }
            this.mThumb = tmp;
        }

        public EPGMetaFile(EPGAccess access, Api.Program obj)
        {
            this.Meta = obj;
            this.mLogo = access.GetChannelLogoBytesByID(this.Meta.channelId);
            byte[] tmp = null;
            int i = 0;
            while ((tmp is null) & (i <= 3))
            {
                System.Threading.Tasks.Task.Delay(1000);
                tmp = access.GetRecordedThumbnailBytesByID(obj.id);
                i++;
            }
            this.mThumb = tmp;
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

        /// <summary>
        /// Write ".meta" file.
        /// </summary>
        /// <param name="path">full file path</param>
        /// <returns></returns>
        public bool WtiteFile(string path)
        {
            path = Path.GetExtension(path).ToLower()==(".meta") ? path : Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path)+".meta");
            try { 
                if (File.Exists(path))
                {
                    File.Move(path, path + ".old");
                }
            }
            catch
            {
                "Meta file is already exits, and unable to overwrite!\"{0}\"".ErrorLognConsole(path);
                return false;
            }
            try
            { 
            using (FileStream sw = new FileStream(path,FileMode.OpenOrCreate,FileAccess.ReadWrite))
            {
                var tmp = this.GetBytes();
                sw.Write(tmp,0,tmp.Length);
            }
            File.SetAttributes(path, File.GetAttributes(path) | FileAttributes.Hidden);
            return true;
            }
            catch(Exception ex)
            {
                "Unable to create or write meta file:\"{0}\"".ErrorLognConsole(path);
                ex.Message.InfoLognConsole();
                return false;
            }
        }

        /// <summary>
        /// Read ".meta" file to <see cref="EPGMetaFile"/> object.
        /// </summary>
        /// <param name="path">full file path.</param>
        /// <returns></returns>
        public static EPGMetaFile ReadFile(string path)
        {
            if (File.Exists(path)) { 
                using (FileStream ssr = new FileStream(path, FileMode.Open, FileAccess.Read))
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
