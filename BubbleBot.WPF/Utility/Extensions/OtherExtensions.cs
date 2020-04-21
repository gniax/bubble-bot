using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using CrashReporterDotNET;
using Newtonsoft.Json.Linq;

namespace BubbleBot.Utility.Extensions
{
    public static class OtherExtensions
    {
        public static async Task<JObject> ReadAsJsonAsync(this HttpContent content)
        {
            var txtContent = await content.ReadAsStringAsync().ConfigureAwait(false);
            var jObj = JObject.Parse(txtContent);
            return jObj;
        }

        public static T? ReadNullable<T>(this BinaryReader br, Func<T?> func) where T : struct
        {
            var hasValue = br.ReadBoolean();
            return hasValue ? func() : null;
        }

        public static void WriteNullable<T>(this BinaryWriter bw, T? value) where T : struct
        {
            bw.Write(value.HasValue);
            if (value.HasValue) bw.Write((dynamic) value.Value);
        }

        public static void SendCrashReport(this Exception exception, string developperMessage = "")
        {
            var reportCrash = new ReportCrash("crash-reports@example.com")
            {
                DeveloperMessage = developperMessage,
                ToEmail = "crash-reports@example.com",
                DoctorDumpSettings = new DoctorDumpSettings
                {
                    ApplicationID = new Guid("2d91c39b-99f9-4eab-bd9f-a7730f2289d2")
                }
            };

            reportCrash.Send(exception);
        }

        public static byte[] GetAllBytes(this BinaryWriter writer)
        {
            var pos = writer.BaseStream.Position;

            var data = new byte[writer.BaseStream.Length];
            writer.BaseStream.Position = 0;
            writer.BaseStream.Read(data, 0, (int) writer.BaseStream.Length);

            writer.BaseStream.Position = pos;

            return data;
        }
    }
}