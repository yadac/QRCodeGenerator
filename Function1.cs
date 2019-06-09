
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using ZXing.QrCode;
using ZXing.Common;

namespace QRCodeGenerator1
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]HttpRequest req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");
            string url = req.Query["url"];
            ZXing.ZKWeb.BarcodeWriter barcodeWriter = new ZXing.ZKWeb.BarcodeWriter
            {
                Format = ZXing.BarcodeFormat.QR_CODE,
                Encoder = new QRCodeWriter(),
                Options = new EncodingOptions { Height = 150, Width = 150 },
                Renderer = new ZXing.ZKWeb.Rendering.BitmapRenderer()
            };
            System.DrawingCore.Bitmap qrCodeImage = barcodeWriter.Write(url);

            return qrCodeImage != null
                ? (ActionResult)new FileContentResult(ImageToByteArray(qrCodeImage), "image/jpeg")
                : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }

        private static byte[] ImageToByteArray(System.DrawingCore.Bitmap qrCodeImage)
        {
            byte[] byteArray = new byte[0];
            using(MemoryStream memoryStream = new MemoryStream())
            {
                qrCodeImage.Save(memoryStream, System.DrawingCore.Imaging.ImageFormat.Jpeg);
                byteArray = memoryStream.ToArray();
            }
            return byteArray;
        }
    }
}
