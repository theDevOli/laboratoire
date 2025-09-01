using Microsoft.AspNetCore.Mvc;

using Laboratoire.Domain.Entity;
using Laboratoire.Application.Utils;
using Laboratoire.Application.DTO;
using Laboratoire.Application.ServicesContracts;
using Microsoft.AspNetCore.Authorization;
using Rotativa.AspNetCore;
using QRCoder;
using Laboratoire.Application.Mapper;
using Laboratoire.Domain.Utils;

namespace Laboratoire.UI.Controllers;

[ApiController]
[Route("v1/api/[controller]")]
public class ReportController
(
    IReportAdderService reportAdderService,
    IReportGetterByIdService reportGetterByIdService,
    IReportGetterService reportGetterService,
    IReportGetterPDFService reportGetterPDFService,
    IReportPatchService reportPatchService
) : Controller
{
    [HttpGet]
    [Authorize(Policy =Policy.Workers)]
    public async Task<IActionResult> GetAllReportsAsync()
    {
        var reports = await reportGetterService.GetAllReportsAsync();
        return Ok(ApiResponse<IEnumerable<Report>>.Success(reports));
    }

    [HttpGet("{reportId}")]
    [Authorize(Policy =Policy.Workers)]
    public async Task<IActionResult> GetReportByIdAsync([FromRoute] Guid reportId)
    {
        var report = await reportGetterByIdService.GetReportByIdAsync(reportId);
        if (report is null)
            return NotFound(ApiResponse<object>.Failure("The record was not found.", 404));

        return Ok(ApiResponse<Report>.Success(report));
    }

    [HttpPost]
    [Authorize(Policy =Policy.Workers)]
    public async Task<IActionResult> AddReportAsync([FromBody] ReportDtoAdd reportDto)
    {
        var report = reportDto.ToReport();

        var addError = await reportAdderService.AddReportAsync(report);
        if (addError.IsNotSuccess())
            return StatusCode(addError.StatusCode, ApiResponse<object>.Failure(addError.Message!, addError.StatusCode));

        return Ok(ApiResponse<string>.Success(SuccessMessage.Added));
    }

    [HttpPatch("{reportId}")]
    [Authorize(Policy =Policy.Workers)]
    public async Task<IActionResult> PatchReportAsync([FromRoute] Guid reportId, [FromBody] ReportDtoPatch reportDto)
    {
        var report = reportDto.ToReport();
        if (reportId != report.ReportId)
            return BadRequest(ApiResponse<object>.Failure("The ID from route do not match within ID from body request", 400));

        var updateError = await reportPatchService.PatchReportAsync(report);
        if (updateError.IsNotSuccess())
            return StatusCode(updateError.StatusCode, ApiResponse<object>.Failure(updateError.Message!, updateError.StatusCode));

        return Ok(ApiResponse<string>.Success(SuccessMessage.Updated));
    }

    [AllowAnonymous]
    [HttpGet("GenerateReport/{reportId}")]
    public async Task<IActionResult> GenerateReport([FromRoute] Guid reportId)
    {
        var pdf = await reportGetterPDFService.GetReportPDFAsync(reportId);
        if (pdf is null)
            return NotFound(ApiResponse<object>.Failure(ErrorMessage.NotFound, 404));

        var url = $"{Request.Scheme}://{Request.Host}{Request.Path}";

        var protocol = Request.IsHttps ? "https" : "http";
        string customSwitches = string.Format(" --allow {0} --header-html {0} --header-spacing 2 --footer-html {1} --footer-spacing 4",
       Url.Action("header", "report", new { area = "" }, protocol), Url.Action("footer", "report", new { area = "" }, protocol));

        using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
        using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q))
        using (PngByteQRCode qrCode = new PngByteQRCode(qrCodeData))
        {
            byte[] qrCodeImage = qrCode.GetGraphic(20);
            string base64Image = Convert.ToBase64String(qrCodeImage);
            pdf.QRCode = "data:image/png;base64," + base64Image;
        }
        return new ViewAsPdf("Report", pdf)
        {
            FileName = $"Relatorio_LabSolo_{pdf?.ProtocolId}.pdf",
            PageSize = Rotativa.AspNetCore.Options.Size.A4,
            PageMargins = new Rotativa.AspNetCore.Options.Margins()
            {
                Top = 40,
                Bottom = 30,
                Left = 5,
                Right = 5
            },
            CustomSwitches = customSwitches,
        };
    }

    [AllowAnonymous]
    [HttpGet("Header")]
    public ActionResult Header()
    {
        return View();
    }

    [AllowAnonymous]
    [HttpGet("Footer")]
    public ActionResult Footer()
    {
        return View();
    }
}