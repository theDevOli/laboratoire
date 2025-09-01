using Laboratoire.Domain.RepositoryContracts;
using Laboratoire.Domain.Entity;
using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.DTO;
using Laboratoire.Application.Utils;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services;

public class ReportGetterPDFService
(
    IReportRepository reportRepository,
    IParameterRepository parameterRepository,
    ICropsNormalizationGetterByReportIdService cropsNormalizationGetter,
    ICropRepository cropRepository,
    IFertilizerGetterService fertilizerGetterService,
    ILogger<ReportGetterPDFService> logger
)
: IReportGetterPDFService
{

    public async Task<ReportPDF?> GetReportPDFAsync(Guid? reportId)
    {
        logger.LogInformation("Starting to fetch PDF report for report ID: {ReportId}", reportId);

        var reportPdfTask = reportRepository.GetReportPDFAsync(reportId);
        var parametersTask = parameterRepository.GetParametersByReportIdAsync(reportId);
        var cropsNormalizationTask = cropsNormalizationGetter.GetCropByReportIdAsync(reportId);

        await Task.WhenAll(reportPdfTask, parametersTask, cropsNormalizationTask);

        var reportPdf = await reportPdfTask;
        var parameters = await parametersTask;
        var cropsNormalization = await cropsNormalizationTask;

        if (reportPdf is null || parameters is null)
        {
            logger.LogWarning("Report PDF or parameters not found for report ID: {ReportId}", reportId);
            return null;
        }

        logger.LogInformation("Parsing outputs for report ID: {ReportId}", reportId);
        reportPdf!.Outputs = TableOutput.Parse(reportPdf.Results, parameters);

        if (cropsNormalization?.Count() > 0)
        {
            logger.LogInformation("Fetching crops and fertilizers for report ID: {ReportId}", reportId);

            var cropsTask = cropRepository.GetAllCropsAsync();
            var fertilizersTask = fertilizerGetterService.GetAllFertilizersAsync();

            await Task.WhenAll(cropsTask, fertilizersTask);

            var crops = await cropsTask;
            var fertilizers = await fertilizersTask;
            var cropIds = cropsNormalization.Select(crop => crop.CropId);
            var reportCrops = FilterCropsById(crops, cropIds);
            logger.LogInformation("Calculating fertilizer suggestions for report ID: {ReportId}", reportId);
            reportPdf.Suggestions = GetFertilizer(reportPdf.Outputs, reportCrops!, fertilizers);
        }

        logger.LogInformation("Completed fetching PDF report for report ID: {ReportId}", reportId);
        return reportPdf;
    }
    private IEnumerable<Crop>? FilterCropsById(IEnumerable<Crop>? crops, IEnumerable<int?>? cropIds)
          => cropIds?.Select(cropId =>
                {
                    var crop = crops?.FirstOrDefault(firstCrop => firstCrop.CropId.Equals(cropId));
                    return new Crop()
                    {
                        CropId = cropId,
                        CropName = crop?.CropName,
                        NitrogenCover = crop?.NitrogenCover,
                        NitrogenFoundation = crop?.NitrogenFoundation,
                        Phosphorus = crop?.Phosphorus,
                        Potassium = crop?.Potassium,
                        MinV = crop?.MinV,
                    };
                });
    private IEnumerable<FertilizerSuggestion>? GetFertilizer(IEnumerable<TableOutput> tableOutput, IEnumerable<Crop> crops, IEnumerable<FertilizerDtoGet> fertilizers)
    {
        var phosphorusResult = tableOutput.FirstOrDefault(output => output.ParameterName == "Fósforo")?.Result;
        var potassiumResult = tableOutput.FirstOrDefault(output => output.ParameterName == "Potássio")?.Result;
        var vResult = tableOutput.FirstOrDefault(output => output.ParameterName == "V-Índice de Saturação de Bases")?.Result;
        var ctcResult = tableOutput.FirstOrDefault(output => output.ParameterName == "CTC")?.Result;
        var suggestions = new List<FertilizerSuggestion>();

        foreach (var crop in crops)
        {
            double? calagemTon = null;
            double? calagemKg = null;
            int? nitrogen = crop?.NitrogenFoundation;
            int? phosphorus = phosphorusResult > 20 ? crop?.Phosphorus?.Max
                : phosphorusResult > 10 ? crop?.Phosphorus?.Med
                : crop?.Phosphorus?.Min;
            int? potassium = potassiumResult > 60 ? crop?.Potassium?.Max
                : potassiumResult > 30 ? potassium = crop?.Potassium?.Med
                : crop?.Potassium?.Min;

            if (crop?.MinV > vResult)
                (calagemTon, calagemKg) = GetCalagem(vResult, crop.MinV, ctcResult);

            int? minValue = new[] { nitrogen, phosphorus, potassium }
            .Where(x => x != 0)
            .DefaultIfEmpty(null)
            .Min();

            string proportion = $"{nitrogen / minValue}-{phosphorus / minValue}-{potassium / minValue}";

            string? formulation = GetBestFormulationAndProportion(proportion, fertilizers);

            int nitrogenFactor = int.Parse(formulation!.Split("-")[0]);
            double haQuantity = (double)nitrogen! * 100 / nitrogenFactor;
            double tarefaQuantity = haQuantity / Constants.TAREFA_FACTOR;

            var baseFormulation = new NPKFormulation()
            {
                Formulation = formulation!,
                QuantityInHa = haQuantity,
                QuantityInTarefa = tarefaQuantity
            };
            var coverFormulation = "30-00-10";
            double? coverHa = crop?.NitrogenCover * 100 / 30;
            var cover = new NPKFormulation()
            {
                Formulation = coverFormulation,
                QuantityInHa = coverHa,
                QuantityInTarefa = coverHa / Constants.TAREFA_FACTOR
            };

            var fertilizerSuggestion = new FertilizerSuggestion()
            {
                CropName = crop?.CropName!,
                CalagemTon = calagemTon,
                CalagemKg = calagemKg,
                NitrogenNecessity = nitrogen,
                PhosphorusNecessity = phosphorus,
                PotassiumNecessity = potassium,
                Base = baseFormulation,
                Cover = cover
            };
            suggestions.Add(fertilizerSuggestion);
        }
        return suggestions;
    }

    private (double?, double?) GetCalagem(double? v, double? minV, double? ctc)
    {
        if (v is null || minV is null) return (null, null);

        var vTon = (minV - v) * ctc * Constants.CALAGEM_FACTOR / 100;
        var vKg = vTon * Constants.KILOGRAM_FACTOR / Constants.TAREFA_FACTOR;

        return (vTon, vKg);
    }

    private string? GetBestFormulationAndProportion(string proportion, IEnumerable<FertilizerDtoGet> fertilizers)
    {
        var bestFormulation = fertilizers.FirstOrDefault(fertilizer => fertilizer.Proportion == proportion);
        if (bestFormulation is not null) return bestFormulation.Formulation;
        var arr = proportion.Split("-");
        var cropNitrogen = arr[0];
        var cropPhosphorus = arr[1];
        var cropPotassium = arr[2];
        foreach (var fertilizer in fertilizers)
        {
            var currentProportion = fertilizer.Formulation;
            var proportionArr = currentProportion?.Split("-");
            var nitrogen = proportionArr?[0];
            var phosphorus = proportionArr?[1];
            var potassium = proportionArr?[2];

            if ((cropPhosphorus == phosphorus && cropPotassium == potassium) ||
            (cropNitrogen == nitrogen && cropPhosphorus == phosphorus) ||
             (cropNitrogen == nitrogen && cropPotassium == potassium))
                return fertilizer.Formulation;
        }

        var firstFertilizer = fertilizers.First();
        return firstFertilizer.Formulation;
    }
}
