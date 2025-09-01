using System.Data;
using Dapper;

using Laboratoire.Infrastructure.DbContext;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;

namespace Laboratoire.Infrastructure.Repository;

public sealed class ChemicalRepository(DataContext dapper) : IChemicalRepository
{
    #region SQL queries
    private readonly string _getAllChemicalsSql =
    $"""
    SELECT 
        chemical_id AS {nameof(Chemical.ChemicalId)},
        chemical_name AS {nameof(Chemical.ChemicalName)},
        concentration AS {nameof(Chemical.Concentration)},
        quantity AS {nameof(Chemical.Quantity)},
        unit AS {nameof(Chemical.Unit)},
        is_police_controlled AS {nameof(Chemical.IsPoliceControlled)},
        is_army_controlled AS {nameof(Chemical.IsArmyControlled)},
        entry_date AS {nameof(Chemical.EntryDate)},
        expire_Date AS {nameof(Chemical.ExpireDate)}
    FROM
        inventory.chemical;
    """;
    private readonly string _getChemicalByIdSql =
    $"""
    SELECT 
        chemical_id AS {nameof(Chemical.ChemicalId)},
        chemical_name AS {nameof(Chemical.ChemicalName)},
        concentration AS {nameof(Chemical.Concentration)},
        quantity AS {nameof(Chemical.Quantity)},
        unit AS {nameof(Chemical.Unit)},
        is_police_controlled AS {nameof(Chemical.IsPoliceControlled)},
        is_army_controlled AS {nameof(Chemical.IsArmyControlled)},
        entry_date AS {nameof(Chemical.EntryDate)},
        expire_Date AS {nameof(Chemical.ExpireDate)}
    FROM 
        inventory.chemical
    WHERE 
        chemical_id = @ChemicalIdParameter;
    """;
    private readonly string _getChemicalByNameAndConcentrationSql =
    $"""
    SELECT 
        chemical_id AS {nameof(Chemical.ChemicalId)},
        chemical_name AS {nameof(Chemical.ChemicalName)},
        concentration AS {nameof(Chemical.Concentration)},
        quantity AS {nameof(Chemical.Quantity)},
        unit AS {nameof(Chemical.Unit)},
        is_police_controlled AS {nameof(Chemical.IsPoliceControlled)},
        is_army_controlled AS {nameof(Chemical.IsArmyControlled)},
        entry_date AS {nameof(Chemical.EntryDate)},
        expire_Date AS {nameof(Chemical.ExpireDate)}
    FROM 
        inventory.chemical
    WHERE 
        chemical_name = @ChemicalNameParameter
        AND concentration = @ConcentrationParameter;
    """;
    private readonly string _addChemicalSql =
    $"""
    INSERT INTO inventory.chemical(
        chemical_name,
        concentration,
        quantity,
        unit,
        is_police_controlled,
        is_army_controlled,
        entry_date,
        expire_date
    )
    VALUES(
        @ChemicalNameParameter,
        @ConcentrationParameter,
        @QuantityParameter,
        @UnitParameter,
        @PoliceControlledParameter,
        @ArmyControlledParameter,
        @EntryDateParameter,
        @ExpireDateParameter
    )
    RETURNING chemical_id;
    """;
    private readonly string _updateChemicalSql =
    $"""
    UPDATE inventory.chemical
    SET
        chemical_name = @ChemicalNameParameter,
        concentration = @ConcentrationParameter,
        quantity = @QuantityParameter,
        unit = @UnitParameter,
        is_police_controlled = @PoliceControlledParameter,
        is_army_controlled = @ArmyControlledParameter,
        entry_date = @EntryDateParameter,
        expire_date = @ExpireDateParameter
    WHERE 
        chemical_id = @ChemicalIdParameter;
    """;
    #endregion
    public async Task<int?> AddChemicalAsync(Chemical chemical)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@ChemicalNameParameter", chemical.ChemicalName, DbType.String);
        parameters.Add("@ConcentrationParameter", chemical.Concentration, DbType.String);
        parameters.Add("@QuantityParameter", chemical.Quantity, DbType.Decimal);
        parameters.Add("@UnitParameter", chemical.Unit, DbType.String);
        parameters.Add("@PoliceControlledParameter", chemical.IsPoliceControlled, DbType.Boolean);
        parameters.Add("@ArmyControlledParameter", chemical.IsArmyControlled, DbType.Boolean);
        parameters.Add("@EntryDateParameter", chemical.EntryDate, DbType.Date);
        parameters.Add("@ExpireDateParameter", chemical.ExpireDate, DbType.Date);

        return await dapper.LoadDataSingleAsync<int>(_addChemicalSql, parameters);
    }

    public async Task<bool> DoesChemicalExistByIdAsync(Chemical chemical)
    => await GetChemicalByIdAsync(chemical.ChemicalId) is not null;
    public async Task<bool> DoesChemicalExistByNameAndConcentrationAsync(Chemical chemical)
    => await GetChemicalByNameAndConcentrationAsync(chemical) is not null;

    public async Task<IEnumerable<Chemical>> GetAllChemicalsAsync()
    => await dapper.LoadDataAsync<Chemical>(_getAllChemicalsSql);

    public async Task<Chemical?> GetChemicalByIdAsync(int? chemicalId)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@ChemicalIdParameter", chemicalId, DbType.Int32);

        return await dapper.LoadDataSingleAsync<Chemical>(_getChemicalByIdSql, parameters);
    }

    public async Task<Chemical?> GetChemicalByNameAndConcentrationAsync(Chemical chemical)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@ChemicalNameParameter", chemical.ChemicalName, DbType.String);
        parameters.Add("@ConcentrationParameter", chemical.Concentration, DbType.String);

        return await dapper.LoadDataSingleAsync<Chemical>(_getChemicalByNameAndConcentrationSql, parameters);
    }

    public async Task<bool> UpdateChemicalAsync(Chemical chemical)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@ChemicalNameParameter", chemical.ChemicalName, DbType.String);
        parameters.Add("@ConcentrationParameter", chemical.Concentration, DbType.String);
        parameters.Add("@QuantityParameter", chemical.Quantity, DbType.Decimal);
        parameters.Add("@UnitParameter", chemical.Unit, DbType.String);
        parameters.Add("@PoliceControlledParameter", chemical.IsPoliceControlled, DbType.Boolean);
        parameters.Add("@ArmyControlledParameter", chemical.IsArmyControlled, DbType.Boolean);
        parameters.Add("@EntryDateParameter", chemical.EntryDate, DbType.Date);
        parameters.Add("@ExpireDateParameter", chemical.ExpireDate, DbType.Date);
        parameters.Add("@ChemicalIdParameter", chemical.ChemicalId, DbType.Int32);

        return await dapper.ExecuteSqlAsync(_updateChemicalSql, parameters);

    }

}
