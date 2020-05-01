using System.IO;
using System.Text;
using SharpCompress.Compressors.LZMA;
using Tachyon.Game.Beatmaps;
using Tachyon.Game.IO;

namespace Tachyon.Game.Scoring
{
    public class ScoreEncoder
    {
        public const int LATEST_VERSION = 128;

        private readonly Score score;
        private readonly IBeatmap beatmap;

        public ScoreEncoder(Score score, IBeatmap beatmap)
        {
            this.score = score;
            this.beatmap = beatmap;
        }

        public void Encode(Stream stream)
        {
            using (SerializationWriter sw = new SerializationWriter(stream))
            {
                sw.Write(LATEST_VERSION);
                sw.Write(score.ScoreInfo.Beatmap.MD5Hash);
                sw.Write((ushort)(score.ScoreInfo.GetCountPerfect() ?? 0));
                sw.Write((ushort)(score.ScoreInfo.GetCountGood() ?? 0));
                sw.Write((ushort)(score.ScoreInfo.GetCountMiss() ?? 0));
                sw.Write((int)score.ScoreInfo.TotalScore);
                sw.Write((ushort)score.ScoreInfo.MaxCombo);
                sw.Write(score.ScoreInfo.Combo == score.ScoreInfo.MaxCombo);

                sw.Write(score.ScoreInfo.Date.DateTime);
                sw.WriteByteArray(createReplayData());
                sw.Write((long)0);
                writeModSpecificData(score.ScoreInfo, sw);
            }
        }

        private void writeModSpecificData(ScoreInfo score, SerializationWriter sw)
        {
        }

        private byte[] createReplayData()
        {
            var content = new ASCIIEncoding().GetBytes(replayStringContent);

            using (var outStream = new MemoryStream())
            {
                using (var lzma = new LzmaStream(new LzmaEncoderProperties(false, 1 << 21, 255), false, outStream))
                {
                    outStream.Write(lzma.Properties);

                    long fileSize = content.Length;
                    for (int i = 0; i < 8; i++)
                        outStream.WriteByte((byte)(fileSize >> (8 * i)));

                    lzma.Write(content);
                }

                return outStream.ToArray();
            }
        }

        private string replayStringContent
        {
            get
            {
                StringBuilder replayData = new StringBuilder();

                replayData.AppendFormat(@"{0}|{1}|{2}|{3},", -12345, 0, 0, 0);
                return replayData.ToString();
            }
        }
    }
}
