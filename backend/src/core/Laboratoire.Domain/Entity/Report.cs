using System.ComponentModel.DataAnnotations;

namespace Laboratoire.Domain.Entity;

public class Report
{
    [Required]
    public Guid? ReportId { get; set; }
    [Required]
    public ReportResult[]? Results { get; set; }
    public static ReportResult[]? AddEquations(ReportResult[]? Results, IEnumerable<Parameter>? parameter)
    {
        var newReportResults = Results?.Select(r =>
        {
            var equation = parameter?.FirstOrDefault(p => p.ParameterId == r.ParameterId)?.Equation;
            return new ReportResult()
            {
                ParameterId = r.ParameterId,
                ValueA = r.ValueA,
                ValueB = r.ValueB,
                ValueC = r.ValueC,
                Equation = equation,
            };
        });
        return newReportResults?.ToArray();
    }
    public void AddEquations(IEnumerable<Parameter>? parameter)
    {
        var newReportResults = Results?.Select(r =>
        {
            var equation = parameter?.FirstOrDefault(p => p.ParameterId == r.ParameterId)?.Equation;
            return new ReportResult()
            {
                ParameterId = r.ParameterId,
                ValueA = r.ValueA,
                ValueB = r.ValueB,
                ValueC = r.ValueC,
                Equation = equation,
            };
        });
        Results = newReportResults?.ToArray();
    }

    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != this.GetType())
            return false;

        Report other = (Report)obj;

        return this.ReportId == other.ReportId;
    }

    public override int GetHashCode()
    => HashCode.Combine(this.ReportId);
}
