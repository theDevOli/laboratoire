using System.ComponentModel.DataAnnotations;
using System.Globalization;
using NCalc;

namespace Laboratoire.Domain.Entity;

public class TableOutput
{
    [Required]
    public string? ParameterName { get; set; }
    [Required]
    public string? Unit { get; set; }
    public string? OfficialDoc { get; set; }
    public string? Vmp { get; set; }
    public double? Result { get; set; }
    public string? ResultStr { get; set; }

    public static IEnumerable<TableOutput> Parse(IEnumerable<ReportResult>? results, IEnumerable<Parameter> parameters)
    {
        var res = new List<TableOutput>();

        foreach (var parameter in parameters)
        {
            string? resultStr = null;
            double? resultVal = null;

            var result = results?.FirstOrDefault(result => result.ParameterId == parameter?.ParameterId);
            var isEfficiency = string.Equals(parameter.ParameterName, "Eficiência da DBO", StringComparison.OrdinalIgnoreCase);
            if (result is not null && !isEfficiency)
                resultVal = double.Parse(parameter.Equation is null ? result.GetResult().ToString() : GetCalculus(result)!);

            if (result is null || isEfficiency) resultVal = SetCalculus(parameter?.ParameterName, results, parameters, out resultStr);

            var tableOutput = new TableOutput()
            {
                ParameterName = parameter?.ParameterName,
                Unit = parameter?.Unit,
                OfficialDoc = parameter?.OfficialDoc,
                Vmp = parameter?.Vmp,
                Result = resultVal,
                ResultStr = resultStr
            };
            res.Add(tableOutput);
        }
        var catalogId = parameters?.FirstOrDefault()?.CatalogId;

        if (catalogId > 3)
            return res.OrderBy(r => r.Unit is String unit ? unit.Contains("UFC") ? 1 : 0 : 0)
              .ThenBy(r => r.ParameterName);

        if (catalogId <= 3)
        {
            Dictionary<string, int> parameterOrder = new()
            {
                ["ALUMÍNIO"] = 0,
                ["CÁLCIO + MAGNÉSIO"] = 1,
                ["CÁLCIO"] = 2,
                ["MAGNÉSIO"] = 3,
                ["FÓSFORO"] = 4,
                ["HIDROGÊNIO + ALUMÍNIO"] = 5,
                ["MATÉRIA ORGÂNICA"] = 6,
                ["PH EM ÁGUA"] = 7,
                ["SÓDIO"] = 8,
                ["POTÁSSIO"] = 9,
                ["SB- SOMA DE BASES TROCÁVEIS"] = 10,
                ["CTC"] = 11,
                ["V-ÍNDICE DE SATURAÇÃO DE BASES"] = 12,
                ["ÁGUA DISPONÍVEL (AD)"] = 13,
                ["AREIA"] = 14,
                ["ARGILA"] = 15,
                ["SILTE"] = 16,
                ["CLASSIFICAÇÃO ÁGUA DISPONÍVEL"] = 17,
                ["CLASSIFICAÇÃO TEXTURAL (TRIÂNGULO AMERICANO)"] = 18
            };
            return res
            .OrderBy(r =>
        {
            var key = r.ParameterName?.ToUpper();
            return parameterOrder.ContainsKey(key!) ? parameterOrder[key!] : int.MaxValue;
        })
        .ThenBy(r => r.ParameterName);
        }

        return res;
    }

    // private static string? GetCalculus(ReportResult? result, Parameter? parameter)
    private static string? GetCalculus(ReportResult? result)
    {
        // var equation = parameter?.Equation!;
        var equation = result?.Equation;
        var tempResult = result?.GetResult();


        if (equation is null)
            return tempResult.ToString();

        Expression expr = new Expression(equation);
        expr.Parameters["x"] = tempResult;

        expr.Parameters["i"] = result?.ValueA ?? 0;
        expr.Parameters["f"] = result?.ValueB ?? 0;
        expr.Parameters["v"] = result?.ValueC ?? 0;

        var res = expr.Evaluate();
        return res?.ToString();
    }

    private static double SetCalculus(string? parameterName, IEnumerable<ReportResult>? results, IEnumerable<Parameter?>? parameters, out string resultStr)
    {
        resultStr = String.Empty;
        double resultVal = 0;
        switch (parameterName?.ToLower())
        {
            case "salinidade":
                resultStr = GetSalinidade(results, parameters);
                break;
            case "sodicidade":
                resultStr = GetSodicidade(results, parameters);
                break;
            case "v-índice de saturação de bases":
                resultVal = GetV(results, parameters);
                break;
            case "sb- soma de bases trocáveis":
                resultVal = GetSB(results, parameters);
                break;
            case "ctc":
                resultVal = GetCTC(results, parameters);
                break;
            case "ras":
                resultVal = GetRAS(results, parameters);
                break;
            case "magnésio":
                resultVal = GetMagnesium(results, parameters);
                break;
            case "silte":
                resultVal = GetSilte(results, parameters);
                break;
            case "classificação textural (triângulo americano)":
                resultStr = GetTypeClassification(results, parameters);
                break;
            case "água disponível (ad)":
                resultVal = GetAD(results, parameters);
                break;
            case "classificação água disponível":
                resultStr = GetADClassification(results, parameters);
                break;
            case "eficiência da dbo":
                resultVal = GetDBOEfficiency(results, parameters);
                break;
        }
        return resultVal;
    }

    private static double GetV(IEnumerable<ReportResult>? results, IEnumerable<Parameter?>? parameters)
    {
        double sb = GetSB(results, parameters);
        var ctc = GetCTC(results, parameters);
        var v = (sb * 100) / ctc;
        return v;

    }
    private static double GetCTC(IEnumerable<ReportResult>? results, IEnumerable<Parameter?>? parameters)
    {
        double sb = GetSB(results, parameters);
        var acidity = GetResult("hidrogênio + alumínio", results, parameters) ?? 0;

        return sb + acidity;

    }

    private static double GetSB(IEnumerable<ReportResult>? results, IEnumerable<Parameter?>? parameters)
    {
        const double POTASSIUM_MOL = 39.0983;

        var calciumAndMagnesium = GetResult("cálcio + magnésio", results, parameters) ?? 0;
        var potassium = GetResult("potássio", results, parameters) / (POTASSIUM_MOL * 10) ?? 0;

        return calciumAndMagnesium + potassium;
    }
    private static double GetMagnesium(IEnumerable<ReportResult>? results, IEnumerable<Parameter?>? parameters)
    {
        var calciumAndMagnesium = GetResult("cálcio + magnésio", results, parameters) ?? 0;
        var calcium = GetResult("cálcio", results, parameters) ?? 0;

        return calciumAndMagnesium - calcium;
    }

    private static double GetAD(IEnumerable<ReportResult>? results, IEnumerable<Parameter?>? parameters)
    {
        var areia = (decimal)GetResult("areia", results, parameters)!;
        var argila = (decimal)GetResult("argila", results, parameters)!;
        var silte = 100 - (areia + argila);

        decimal ad = (1 + (0.3591m * (
            (-0.02128887m * areia) +
            (-0.01005814m * silte) +
            (-0.01901894m * argila) +
            (0.0001171219m * areia * silte) +
            (0.0002073924m * areia * argila) +
            (0.00006118707m * silte * argila) +
            (-0.000006373789m * areia * silte * argila)
         )));

        return Math.Pow((double)ad, 2.78474) * 10;

    }
    private static string GetADClassification(IEnumerable<ReportResult>? results, IEnumerable<Parameter?>? parameters)
    {
        var ad = GetAD(results, parameters);

        if (ad <= 0.46)
            return "AD1";

        if (ad <= 0.61)
            return "AD2";

        if (ad <= 0.8)
            return "AD3";

        if (ad <= 1.06)
            return "AD4";

        if (ad <= 1.4)
            return "AD5";

        return "AD6";
    }
    private static string GetTypeClassification(IEnumerable<ReportResult>? results, IEnumerable<Parameter?>? parameters)
    {
        var argila = GetResult("argila", results, parameters) ?? 0;

        if (argila <= 15)
            return "Tipo 1";

        if (argila <= 35)
            return "Tipo 2";

        return "Tipo 3";
    }

    private static double GetSilte(IEnumerable<ReportResult>? results, IEnumerable<Parameter?>? parameters)
    {
        var areia = GetResult("areia", results, parameters) ?? 0;
        var argila = GetResult("argila", results, parameters) ?? 0;

        return 100 - (areia + argila);
    }
    private static string GetSalinidade(IEnumerable<ReportResult>? results, IEnumerable<Parameter?>? parameters)
    {
        var ce = GetResult("condutividade", results, parameters);
        if (ce < 250)
            return "C1";
        if (ce < 750)
            return "C2";
        if (ce < 2250)
            return "C3";
        return "C4";
    }

    private static string GetSodicidade(IEnumerable<ReportResult>? results, IEnumerable<Parameter?>? parameters)
    {
        var ce = GetResult("condutividade", results, parameters) ?? 0;
        var ras = GetRAS(results, parameters);
        var firstLog = 18.87 - 4.44 * Math.Log10(ce);

        if (ras <= firstLog)
            return "S1";
        if (firstLog < ras && ras <= 31.31)
            return "S2";
        if (31.31 - 6.66 * Math.Log10(ce) < ras && ras <= 43.75)
            return "S3";
        return "S4";
    }

    private static double GetRAS(IEnumerable<ReportResult>? results, IEnumerable<Parameter?>? parameters)
    {
        const double SODIUM_FACTOR = 22.99;
        const double HARDNESS_FACTOR = 16.09;

        var sodium = GetResult("sódio", results, parameters);
        var hardness = GetResult("dureza", results, parameters);
        var elSodium = sodium / SODIUM_FACTOR ?? 0;
        var elHardness = hardness / HARDNESS_FACTOR ?? 0;

        return elSodium / Math.Sqrt(elHardness / 2);
    }
    private static double GetDBOEfficiency(IEnumerable<ReportResult>? results, IEnumerable<Parameter?>? parameters)
    {
        var dbo = GetResult("dbo", results, parameters);
        var efficiencyId = parameters?
        .FirstOrDefault(parameter => string.Equals(parameter?.ParameterName, "Eficiência da DBO", StringComparison.OrdinalIgnoreCase))?
        .ParameterId;
        var efficiency = results?.FirstOrDefault(result => result.ParameterId == efficiencyId);
        efficiency!.ValueB = dbo;

        return double.Parse(GetCalculus(efficiency)!);
    }

    private static double? GetResult(string parameterName, IEnumerable<ReportResult>? results, IEnumerable<Parameter?>? parameters)
    {
        var parameter = parameters?.FirstOrDefault(parameter => parameter!.ParameterName!.ToLower() == parameterName);
        var result = results?.FirstOrDefault(result => result.ParameterId == parameter?.ParameterId);
        return double.Parse(GetCalculus(result)!);
    }
    public string GetUnit()
    {
        if (Unit is null)
            return "-";

        return Unit;
    }

    public string GetOfficialDoc()
    {
        if (OfficialDoc is null)
            return "-";

        return OfficialDoc;
    }

    public string GetVmp()
    {
        if (Vmp is null)
            return "-";

        return $"{Vmp} {Unit}";
    }

    public string GetResult()
    {
        if (!String.IsNullOrEmpty(ResultStr))
            return ResultStr;

        if (Result is null)
            return "ND";

        if (Result <= 0)
            return "ND";

        return Result?.ToString("F2", new CultureInfo("pt-BR"))!;
    }

    public bool IsParameterOutVmp()
    {
        if (Vmp is not null && Vmp.Contains('>'))
        {
            var vmp = double.Parse(Vmp.Trim().Split('>')[1]);
            return Result < vmp;
        }
        // if (ParameterName!.ToLower().Contains("cloro"))
        //     return Result < 0.2;

        var ok = double.TryParse(Vmp, out var val);
        if (!ok)
            return false;

        return Result > val;
    }
}
