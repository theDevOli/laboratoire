using System.Data;
using Dapper;

using Laboratoire.Infrastructure.DbContext;
using Laboratoire.Application.DTO;
using Laboratoire.Application.Mapper;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;

namespace Laboratoire.Infrastructure.Repository;

public sealed class CropRepository (DataContext dapper) : ICropRepository
{
    #region SQL queries
    private readonly string _getAllCropsSql =
    $"""
    SELECT
        crop_id AS {nameof(Crop.CropId)},
        crop_name AS {nameof(Crop.CropName)},
        nitrogen_cover AS {nameof(Crop.NitrogenCover)},
        nitrogen_foundation AS {nameof(Crop.NitrogenFoundation)},
        phosphorus AS {nameof(Crop.Phosphorus)},
        potassium AS {nameof(Crop.Potassium)},
        min_v AS {nameof(Crop.MinV)},
        extra_data AS {nameof(Crop.ExtraData)}
    FROM 
        document.crop;
    """;
    private readonly string _getCropByIdSql =
    $"""
    SELECT
        crop_id AS {nameof(Crop.CropId)},
        crop_name AS {nameof(Crop.CropName)},
        nitrogen_cover AS {nameof(Crop.NitrogenCover)},
        nitrogen_foundation AS {nameof(Crop.NitrogenFoundation)},
        phosphorus AS {nameof(Crop.Phosphorus)},
        potassium AS {nameof(Crop.Potassium)},
        min_v AS {nameof(Crop.MinV)},
        extra_data AS {nameof(Crop.ExtraData)}
    FROM 
        document.crop
    WHERE
        crop_id = @CropIdParameter;
    """;
    private readonly string _getCropByNameSql =
    $"""
    SELECT
        crop_id AS {nameof(Crop.CropId)},
        crop_name AS {nameof(Crop.CropName)},
        nitrogen_cover AS {nameof(Crop.NitrogenCover)},
        nitrogen_foundation AS {nameof(Crop.NitrogenFoundation)},
        phosphorus AS {nameof(Crop.Phosphorus)},
        potassium AS {nameof(Crop.Potassium)},
        min_v AS {nameof(Crop.MinV)},
        extra_data AS {nameof(Crop.ExtraData)}
    FROM 
        document.crop
    WHERE 
        crop_name = @CropNameParameter;
    """;
    private readonly string _addCropSql =
    $"""
    INSERT INTO document.crop(
        crop_name,
        nitrogen_cover,
        nitrogen_foundation,
        phosphorus,
        potassium,
        min_v,
        extra_data
    )
    VALUES
    (
        @CropNameParameter,
        @NitrogenCoverParameter,
        @NitrogenFoundationParameter,
        @PhosphorusParameter::JSONB,
        @PotassiumParameter::JSONB,
        @MinVParameter,
        @ExtraDataParameter
    );
    """;
    private readonly string _updateCropSql =
    $"""
    UPDATE document.crop
    SET
        crop_name = @CropNameParameter,
        nitrogen_cover = @NitrogenCoverParameter,
        nitrogen_foundation = @NitrogenFoundationParameter,
        phosphorus = @PhosphorusParameter::JSONB,
        potassium = @PotassiumParameter::JSONB,
        min_v = @MinVParameter,
        extra_data = @ExtraDataParameter
    WHERE 
        crop_id = @CropIdParameter;
    """;
    #endregion
    public async Task<bool> AddCropAsync(Crop crop)
    {
        var cropDB = crop.ToDb();

        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@CropNameParameter", cropDB.CropName, DbType.String);
        parameters.Add("@NitrogenCoverParameter", cropDB.NitrogenCover, DbType.Int32);
        parameters.Add("@NitrogenFoundationParameter", cropDB.NitrogenFoundation, DbType.Int32);
        parameters.Add("@PhosphorusParameter", cropDB.Phosphorus, DbType.String);
        parameters.Add("@PotassiumParameter", cropDB.Potassium, DbType.String);
        parameters.Add("@MinVParameter", cropDB.MinV, DbType.Int32);
        parameters.Add("@ExtraDataParameter", cropDB.ExtraData, DbType.String);

        return await dapper.ExecuteSqlAsync(_addCropSql, parameters);
    }
    public async Task<bool> DoesCropExistByCropIdAsync(Crop crop)
   => await GetCropByIdAsync(crop.CropId) is not null;

    public async Task<bool> DoesCropExistByNameAsync(Crop crop)
   => await GetCropByNameAsync(crop.CropName) is not null;

    public async Task<IEnumerable<Crop>> GetAllCropsAsync()
    {
        var dto = await dapper.LoadDataAsync<CropDtoDb>(_getAllCropsSql);
        return dto.Select(d => d.FromDb());
    }
    public async Task<Crop?> GetCropByIdAsync(int? cropId)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@CropIdParameter", cropId, DbType.Int32);

        var dto = await dapper.LoadDataSingleAsync<CropDtoDb>(_getCropByIdSql, parameters);
        if (dto is null)
            return null;
        return dto.FromDb();
    }
    public async Task<Crop?> GetCropByNameAsync(string? cropName)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@CropNameParameter", cropName, DbType.String);

        var dto = await dapper.LoadDataSingleAsync<CropDtoDb>(_getCropByNameSql, parameters);
        if (dto is null)
            return null;
        return dto.FromDb();
    }
    public async Task<bool> UpdateCropAsync(Crop crop)
    {
        var cropDB = crop.ToDb();

        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@CropNameParameter", cropDB.CropName, DbType.String);
        parameters.Add("@NitrogenCoverParameter", cropDB.NitrogenCover, DbType.Int32);
        parameters.Add("@NitrogenFoundationParameter", cropDB.NitrogenFoundation, DbType.Int32);
        parameters.Add("@PhosphorusParameter", cropDB.Phosphorus, DbType.String);
        parameters.Add("@PotassiumParameter", cropDB.Potassium, DbType.String);
        parameters.Add("@MinVParameter", cropDB.MinV, DbType.Int32);
        parameters.Add("@ExtraDataParameter", cropDB.ExtraData, DbType.String);
        parameters.Add("@CropIdParameter", cropDB.CropId, DbType.Int32);

        return await dapper.ExecuteSqlAsync(_updateCropSql, parameters);
    }
}
