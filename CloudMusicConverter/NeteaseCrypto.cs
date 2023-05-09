using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TagLib.Id3v2;

namespace CloudMusicConverter
{
    class NotNeteaseFileException : IOException
    {
        public NotNeteaseFileException(string name)
            : base(string.Format(@"""{0}"" not netease music file", name))
        {

        }
    }

    class NeteaseCrypto : IComparable<NeteaseCrypto>
    {
        private static readonly byte[] _flag = new byte[8] { 0x43, 0x54, 0x45, 0x4e, 0x46, 0x44, 0x41, 0x4d };

        private static byte[] _id3Flag = new byte[3] { 0x49, 0x44, 0x33 };
        private static byte[] _pngFlag = new byte[8] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };

        private static byte[] _coreBoxKey = new byte[16] { 0x68, 0x7A, 0x48, 0x52, 0x41, 0x6D, 0x73, 0x6F, 0x35, 0x6B, 0x49, 0x6E, 0x62, 0x61, 0x78, 0x57 };
        private static byte[] _modifyBoxKey = new byte[16] { 0x23, 0x31, 0x34, 0x6C, 0x6A, 0x6B, 0x5F, 0x21, 0x5C, 0x5D, 0x26, 0x30, 0x55, 0x3C, 0x27, 0x28 };

        private NeteaseCopyrightData _cdata = null;

        public string FullFileName { get; set; }

        private readonly byte[] _imageCover;

        public Bitmap Cover { get; } = null;
        public double Progress { get; private set; }

        public string FullFilePath { get; set; }


        List<string> artist = null;
        public string[] Artist
        {
            get
            {
                if (_cdata != null && artist == null)
                {
                    if (_cdata.Artist != null)
                    {
                        artist = new List<string>();
                        foreach (var item in _cdata.Artist)
                        {
                            artist.Add(item[0].ToString());
                        }
                    }
                }

                if (artist != null)
                    return artist.ToArray();
                return null;
            }
        }

        public string Format
        {
            get
            {
                if (_cdata != null)
                {
                    return _cdata.Format;
                }
                return null;
            }
        }

        public int Bitrate
        {
            get
            {
                if (_cdata != null)
                {
                    return _cdata.Bitrate;
                }
                return 0;
            }
        }

        public int Duration
        {
            get
            {
                if (_cdata != null)
                {
                    return _cdata.Duration;
                }
                return 0;
            }
        }

        public string Name
        {
            get
            {
                if (_cdata != null)
                {
                    return _cdata.MusicName;
                }
                return null;
            }
        }

        public string FileName { get; internal set; }

        private readonly byte[] _keyBox;

        private readonly FileStream _fs;

        public NeteaseCrypto(FileStream fs)
        {
            _fs = fs;

            byte[] flag = new byte[8];
            fs.Read(flag, 0, flag.Length);

            if (!flag.SequenceEqual(_flag))
            {
                throw new NotNeteaseFileException(fs.Name);
            }

            // not use less
            fs.Seek(2, SeekOrigin.Current);

            //获取了4字节的key长度
            byte[] coreKeyChunk = ReadChunk(fs);
            for (int i = 0; i < coreKeyChunk.Length; i++)
            {
                //每个字节和0x64进行异或
                coreKeyChunk[i] ^= 0x64;
            }

            //用之前的core_key创建了AES_ECB的解密器
            int ckcLen = AesDecrypt(coreKeyChunk, _coreBoxKey);

            //减去的17是开头的neteasecloudmusic
            byte[] finalKey = new byte[ckcLen - 17];
            Array.Copy(coreKeyChunk, 17, finalKey, 0, finalKey.Length);

            _keyBox = new byte[256];
            for (int i = 0; i < _keyBox.Length; i++)
            {
                _keyBox[i] = (byte)i;
            }

            byte swap = 0;
            byte c = 0;
            byte last_byte = 0;
            byte key_offset = 0;

            for (int i = 0; i < _keyBox.Length; i++)
            {
                swap = _keyBox[i];
                c = (byte)((swap + last_byte + finalKey[key_offset++]) & 0xff);
                if (key_offset >= finalKey.Length) key_offset = 0;
                _keyBox[i] = _keyBox[c];
                _keyBox[c] = swap;
                last_byte = c;
            }

            byte[] dontModifyChunk = ReadChunk(fs);

            if (dontModifyChunk != null)
            {

                int startIndex = 0;
                for (int i = 0; i < dontModifyChunk.Length; i++)
                {
                    dontModifyChunk[i] ^= 0x63;
                    if (dontModifyChunk[i] == 58 && startIndex == 0)
                    {
                        startIndex = i + 1;
                    }
                }

                byte[] dontModifyDecryptChunk = Convert.FromBase64String(Encoding.UTF8.GetString(dontModifyChunk, startIndex, dontModifyChunk.Length - startIndex));
                int mdcLen = AesDecrypt(dontModifyDecryptChunk, _modifyBoxKey);

                DataContractJsonSerializer d = new DataContractJsonSerializer(typeof(NeteaseCopyrightData));
                // skip `music:`
                using (MemoryStream reader = new MemoryStream(dontModifyDecryptChunk, 6, mdcLen - 6))
                {
                    _cdata = d.ReadObject(reader) as NeteaseCopyrightData;
                }
            }

            // skip crc & some use less chunk
            fs.Seek(9, SeekOrigin.Current);

            _imageCover = ReadChunk(fs);

            if (_imageCover != null)
            {
                using (MemoryStream imageStream = new MemoryStream(_imageCover))
                {
                    Cover = Image.FromStream(imageStream) as Bitmap;
                }
            }
        }

        private byte[] ReadChunk(FileStream fs)
        {
            uint len = fs.ReadUInt32();

            if (len > 0)
            {

                byte[] chunk = new byte[len];

                // unsafe
                fs.Read(chunk, 0, (int)len);

                return chunk;
            }

            return null;
        }

        private int AesDecrypt(byte[] data, byte[] key)
        {
            var aes = Aes.Create();
            aes.Mode = CipherMode.ECB;
            aes.Key = key;
            aes.Padding = PaddingMode.PKCS7;

            using (MemoryStream stream = new MemoryStream(data))
            {
                using (CryptoStream cs = new CryptoStream(stream, aes.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    return cs.Read(data, 0, data.Length);
                }
            }
        }

        private bool ByteCompare(byte[] src, byte[] dst)
        {
            if (src.Length > dst.Length)
                return false;

            for (int i = 0; i < src.Length; i++)
            {
                if (dst[i] != src[i])
                    return false;
            }

            return true;
        }

        private string GetMime(byte[] imgData)
        {
            if (ByteCompare(_pngFlag, imgData))
            {
                return "image/png";
            }

            return "image/jpeg";
        }

        public void Dump()
        {
            int n = 0x8000;
            double totalLen = _fs.Length - _fs.Position;
            double alreadyProcess = 0;

            //char[] ignore = Path.GetInvalidFileNameChars();
            //var convertName = Name;

            //foreach (var i in ignore)
            //{
            //    convertName = convertName.Replace(i.ToString(), "");
            //}

            string filePath = null;

            FileStream stream = null;

            while (n > 1)
            {
                byte[] chunk = new byte[n];
                n = _fs.Read(chunk, 0, n);

                for (int i = 0; i < n; i++)
                {
                    int j = (i + 1) & 0xff;
                    chunk[i] ^= _keyBox[(_keyBox[j] + _keyBox[(_keyBox[j] + j) & 0xff]) & 0xff];
                }

                if (stream == null)
                {
                    if (ByteCompare(_id3Flag, chunk))
                    {
                        _cdata.Format = "mp3";
                    }
                    else
                    {
                        _cdata.Format = "flac";
                    }

                    filePath = Path.Combine(FullFilePath, string.Format("{0}.{1}", FileName, Format));

                    stream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);
                }

                if (stream != null)
                {
                    stream.Write(chunk, 0, n);
                }
                else
                {
                    break;
                }

                alreadyProcess += n;

                Progress = (alreadyProcess / totalLen) * 100d;
            }


            //文件流解密并write完成，使用taglib写入封面图片
            if (stream != null)
            {
                stream.Close();

                TagLib.File f = null;
                TagLib.Tag tag = null;
                TagLib.ByteVector imgCoverData = null;
                if (_imageCover != null)
                {
                    imgCoverData = new TagLib.ByteVector(_imageCover, _imageCover.Length);
                }
                if (Format.ToLower() == "mp3")
                {
                    f = TagLib.File.Create(filePath);
                    tag = f.GetTag(TagLib.TagTypes.Id3v2);
                    if (imgCoverData != null)
                    {
                        AttachmentFrame frame = new AttachmentFrame();
                        frame.MimeType = GetMime(_imageCover);
                        frame.Data = imgCoverData;
                        ((Tag)tag).AddFrame(frame);
                    }
                }
                else if (Format.ToLower() == "flac")
                {
                    f = TagLib.File.Create(filePath);
                    tag = f.Tag;

                    if (imgCoverData != null)
                    {
                        TagLib.Picture picture = new TagLib.Picture(imgCoverData);
                        picture.MimeType = GetMime(_imageCover);
                        picture.Type = TagLib.PictureType.FrontCover;

                        TagLib.IPicture[] pics = new TagLib.IPicture[tag.Pictures.Length + 1];
                        for (int i = 0; i < tag.Pictures.Length; i++)
                        {
                            pics[i] = tag.Pictures[i];
                        }
                        pics[pics.Length - 1] = picture;

                        tag.Pictures = pics;
                    }
                }
                tag.Title = Name;
                tag.Performers = Artist;
                tag.Album = _cdata.Album;
                tag.Comment = "Create by netease copyright protected dump tool gui. author 5L";

                f.Save();
            }
        }

        public int CompareTo(NeteaseCrypto other)
        {
            if (Progress == 100) return -1;
            if (Progress > other.Progress) return 0;
            return 1;
        }

        public void CloseFile()
        {
            if (_fs != null)
                _fs.Dispose();
        }
    }
}
