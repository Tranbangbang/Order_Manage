using QRCoder;
using SkiaSharp;
using System;

namespace Order_Manage.Service.Impl
{
    public class QrCodeServiceImpl : IQrCodeService
    {
        public byte[] GenerateQrCode(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                throw new ArgumentException("Content cannot be null or empty.");
            }
            using (var qrGenerator = new QRCodeGenerator())
            {
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.L);
                return RenderQrCodeWithSkia(qrCodeData, 300, 300);
            }
        }

        private byte[] RenderQrCodeWithSkia(QRCodeData qrCodeData, int width, int height)
        {
            var info = new SKImageInfo(width, height);
            using (var surface = SKSurface.Create(info))
            {
                var canvas = surface.Canvas;

                canvas.Clear(SKColors.White);
                var paint = new SKPaint
                {
                    Color = SKColors.Black,
                    Style = SKPaintStyle.Fill
                };
                // Lấy dữ liệu bit từ QRCodeData
                int moduleCount = qrCodeData.ModuleMatrix.Count;
                float moduleSize = (float)width / moduleCount;
                for (int x = 0; x < moduleCount; x++)
                {
                    for (int y = 0; y < moduleCount; y++)
                    {
                        if (qrCodeData.ModuleMatrix[x][y])
                        {
                            canvas.DrawRect(x * moduleSize, y * moduleSize, moduleSize, moduleSize, paint);
                        }
                    }
                }
                canvas.Flush();
                using (var image = surface.Snapshot())
                using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
                {
                    return data.ToArray();
                }
            }
        }
    }
}
