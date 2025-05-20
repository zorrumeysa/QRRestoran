using Microsoft.AspNetCore.Mvc;
using QRCoder;
using DinkToPdf;
using DinkToPdf.Contracts;
using QRRestoran.Services;

namespace QRRestoran.Controllers
{
    public class QRGeneratorController : Controller
    {
        private readonly IConverter _converter;
        private readonly IViewRenderService _viewRender;

        public QRGeneratorController(IConverter converter, IViewRenderService viewRender)
        {
            _converter = converter;
            _viewRender = viewRender;
        }

        // Tek bir masa için QR SVG olarak döner
        [HttpGet]
        public IActionResult MasaQR(int masaNo)
        {
            string url = Url.Action("Index", "Menu", new { masaNo }, Request.Scheme);

            using var qrGenerator = new QRCodeGenerator();
            using var qrData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
            var svgQr = new SvgQRCode(qrData).GetGraphic(6);

            return Content(svgQr, "image/svg+xml");
        }

        // HTML View'dan PDF çıktısı üretir
        [HttpGet]
        public async Task<IActionResult> TumMasalarPDF()
        {
            var html = await _viewRender.RenderToStringAsync("TumMasalarPDF", null);

            var doc = new HtmlToPdfDocument
            {
                GlobalSettings = new GlobalSettings
                {
                    PaperSize = PaperKind.A4,
                    Orientation = Orientation.Portrait,
                    DocumentTitle = "Masa QR Kodları"
                },
                Objects = {
                    new ObjectSettings
                    {
                        HtmlContent = html
                    }
                }
            };

            var pdf = _converter.Convert(doc);
            return File(pdf, "application/pdf", "TumMasalar.pdf");
        }

        // QR Kodları listesi (görsel)
        public IActionResult TumMasalar()
        {
            return View(); // Views/QRGenerator/TumMasalar.cshtml
        }
    }
}
