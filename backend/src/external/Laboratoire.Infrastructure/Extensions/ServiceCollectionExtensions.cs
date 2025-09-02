
using Microsoft.Extensions.DependencyInjection;

using Laboratoire.Infrastructure.Repository;
using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.Services;
using Laboratoire.Application.Services.AuthServices;
using Laboratoire.Application.Services.CashFlowServices;
using Laboratoire.Application.Services.CatalogServices;
using Laboratoire.Application.Services.ChemicalServices;
using Laboratoire.Application.Services.ClientServices;
using Laboratoire.Application.Services.CropServices;
using Laboratoire.Application.Services.HazardServices;
using Laboratoire.Application.Services.ParameterServices;
using Laboratoire.Application.Services.PartnerServices;
using Laboratoire.Application.Services.PermissionServices;
using Laboratoire.Application.Services.PropertyServices;
using Laboratoire.Application.Services.ProtocolServices;
using Laboratoire.Application.Services.ReportServices;
using Laboratoire.Application.Services.RoleServices;
using Laboratoire.Application.Services.TransactionServices;
using Laboratoire.Application.Services.UserServices;
using Laboratoire.Domain.RepositoryContracts;
using Laboratoire.Application.IUtils;
using Laboratoire.Application.Utils;

namespace Laboratoire.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUtils(this IServiceCollection services)
    {
        services.AddScoped<IToken, Token>();
        return services;
    }
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IAuthRepository, AuthRepository>();
        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IHazardRepository, HazardRepository>();
        services.AddScoped<IChemicalRepository, ChemicalRepository>();
        services.AddScoped<IPartnerRepository, PartnerRepository>();
        services.AddScoped<IFertilizerRepository, FertilizerRepository>();
        services.AddScoped<IPropertyRepository, PropertyRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<ICashFlowRepository, CashFlowRepository>();
        services.AddScoped<ICatalogRepository, CatalogRepository>();
        services.AddScoped<IParameterRepository, ParameterRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUtilsRepository, UtilsRepository>();
        services.AddScoped<ICropRepository, CropRepository>();
        services.AddScoped<IReportRepository, ReportRepository>();
        services.AddScoped<IProtocolRepository, ProtocolRepository>();
        services.AddScoped<ICropsNormalizationRepository, CropsNormalizationRepository>();
        services.AddScoped<IChemicalsNormalizationRepository, ChemicalsNormalizationRepository>();

        return services;
    }
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthRegistrationService, AuthRegistrationService>();
        services.AddScoped<IAuthLoginService, AuthLoginService>();
        services.AddScoped<IAuthTokenRefresherService, AuthTokenRefresherService>();
        services.AddScoped<IAuthTokenValidatorService, AuthTokenValidatorService>();
        services.AddScoped<IAuthChangePasswordService, AuthChangePasswordService>();
        services.AddScoped<IAuthResetPasswordService, AuthResetPasswordService>();

        services.AddScoped<IAuthenticationGetterService, AuthenticationGetterService>();

        services.AddScoped<ICashFlowAdderService, CashFlowAdderService>();
        // services.AddScoped<ICashFlowPatchService, CashFlowPatchService>();
        services.AddScoped<ICashFlowGetterByIdService, CashFlowGetterByIdService>();
        services.AddScoped<ICashFlowGetterByYearAndMonthService, CashFlowGetterByYearAndMonthService>();
        services.AddScoped<ICashFlowGetterService, CashFlowGetterService>();
        services.AddScoped<ITotalAmountGetterService, TotalAmountGetterService>();
        services.AddScoped<ICashFlowUpdatableService, CashFlowUpdatableService>();

        services.AddScoped<ICatalogAdderService, CatalogAdderService>();
        services.AddScoped<ICatalogGetterByIdService, CatalogGetterByIdService>();
        services.AddScoped<ICatalogGetterService, CatalogGetterService>();
        services.AddScoped<ICatalogUpdatableService, CatalogUpdatableService>();

        services.AddScoped<IChemicalAdderService, ChemicalAdderService>();
        services.AddScoped<IChemicalGetterByIdService, ChemicalGetterByIdService>();
        services.AddScoped<IChemicalGetterService, ChemicalGetterService>();
        services.AddScoped<IChemicalUpdatableService, ChemicalUpdatableService>();

        services.AddScoped<IChemicalsNormalizationAdderService, ChemicalsNormalizationAdderService>();
        services.AddScoped<IChemicalsNormalizationDeleterService, ChemicalsNormalizationDeleterService>();
        services.AddScoped<IChemicalsNormalizationGetterByIdService, ChemicalsNormalizationGetterByIdService>();
        services.AddScoped<IChemicalsNormalizationGetterService, ChemicalsNormalizationGetterService>();

        services.AddScoped<IClientAdderService, ClientAdderService>();
        services.AddScoped<IClientGetterByIdService, ClientGetterByIdService>();
        services.AddScoped<IClientGetterByTaxIdService, ClientGetterByTaxIdService>();
        services.AddScoped<IClientGetterByLikeTaxIdService, ClientGetterByLikeTaxIdService>();
        services.AddScoped<IClientGetterService, ClientGetterService>();
        services.AddScoped<IClientUpdatableService, ClientUpdatableService>();

        services.AddScoped<ICropAdderService, CropAdderService>();
        services.AddScoped<ICropGetterByIdService, CropGetterByIdService>();
        services.AddScoped<ICropGetterService, CropGetterService>();
        services.AddScoped<ICropUpdatableService, CropUpdatableService>();

        services.AddScoped<ICropsNormalizationAdderService, CropsNormalizationAdderService>();
        services.AddScoped<ICropsNormalizationGetterByReportIdService, CropsNormalizationGetterByReportIdService>();
        services.AddScoped<ICropsNormalizationGetterService, CropsNormalizationGetterService>();
        services.AddScoped<ICropsNormalizationDeleterService, CropsNormalizationDeleterService>();

        services.AddScoped<IHazardAdderService, HazardAdderService>();
        services.AddScoped<IHazardGetterByIdService, HazardGetterByIdService>();
        services.AddScoped<IHazardGetterService, HazardGetterService>();
        services.AddScoped<IHazardUpdatableService, HazardUpdatableService>();

        services.AddScoped<IParameterAdderService, ParameterAdderService>();
        services.AddScoped<IParameterGetterByIdService, ParameterGetterByIdService>();
        services.AddScoped<IParameterGetterService, ParameterGetterService>();
        services.AddScoped<IParameterInputGetterService, ParameterInputGetterService>();
        services.AddScoped<IParameterUpdatableService, ParameterUpdatableService>();

        services.AddScoped<IPartnerAdderService, PartnerAdderService>();
        services.AddScoped<IPartnerGetterByIdService, PartnerGetterByIdService>();
        services.AddScoped<IPartnerGetterService, PartnerGetterService>();
        services.AddScoped<IPartnerUpdatableService, PartnerUpdatableService>();

        services.AddScoped<IPermissionAdderService, PermissionAdderService>();
        services.AddScoped<IPermissionGetterService, PermissionGetterService>();
        services.AddScoped<IPermissionUpdatableService, PermissionUpdatableService>();

        services.AddScoped<IPropertyAdderService, PropertyAdderService>();
        services.AddScoped<IPropertyGetterService, PropertyGetterService>();
        services.AddScoped<IPropertyGetterByPropertyIdService, PropertyGetterByPropertyIdService>();
        services.AddScoped<IPropertyGetterByClientIdService, PropertyGetterByClientIdService>();
        // services.AddScoped<IPropertyGetterByClientTaxIdService, PropertyGetterByClientTaxIdService>();
        services.AddScoped<IPropertyGetterToDisplayService, PropertyGetterToDisplayService>();
        services.AddScoped<IPropertyUpdatableService, PropertyUpdatableService>();

        services.AddScoped<IProtocolAdderService, ProtocolAdderService>();
        // services.AddScoped<IProtocolGetterService, ProtocolGetterService>();
        services.AddScoped<IProtocolGetterToDisplayService, ProtocolGetterToDisplayService>();
        // services.AddScoped<IProtocolGetterByIdService, ProtocolGetterByIdService>();
        services.AddScoped<IProtocolYearGetterService, ProtocolYearGetterService>();
        services.AddScoped<IProtocolSpotSaverService, ProtocolSpotSaverService>();
        services.AddScoped<IProtocolPatchReportService, ProtocolPatchReportService>();
        services.AddScoped<IProtocolPatchCatalogService, ProtocolPatchCatalogService>();
        services.AddScoped<IProtocolPatchCashFlowIdService, ProtocolPatchCashFlowIdService>();
        services.AddScoped<IProtocolUpdatableService, ProtocolUpdatableService>();

        services.AddScoped<IRoleAdderService, RoleAdderService>();
        services.AddScoped<IRoleGetterByIdService, RoleGetterByIdService>();
        services.AddScoped<IRoleGetterByUserIdService, RoleGetterByUserIdService>();
        services.AddScoped<IRoleGetterService, RoleGetterService>();
        services.AddScoped<IRoleUpdatableService, RoleUpdatableService>();

        services.AddScoped<ITransactionAdderService, TransactionAdderService>();
        services.AddScoped<ITransactionGetterByIdService, TransactionGetterByIdService>();
        services.AddScoped<ITransactionGetterService, TransactionGetterService>();
        services.AddScoped<ITransactionUpdatableService, TransactionUpdatableService>();

        services.AddScoped<IFertilizerGetterService, FertilizerGetterService>();

        services.AddScoped<IReportAdderService, ReportAdderService>();
        services.AddScoped<IReportGetterByIdService, ReportGetterByIdService>();
        services.AddScoped<IReportGetterService, ReportGetterService>();
        services.AddScoped<IReportGetterPDFService, ReportGetterPDFService>();
        services.AddScoped<IReportPatchService, ReportPatchService>();

        services.AddScoped<IUserAdderService, UserAdderService>();
        services.AddScoped<IUserGetterByIdService, UserGetterByIdService>();
        services.AddScoped<IUserGetterByUsernameService, UserGetterByUsernameService>();
        services.AddScoped<IUserGetterService, UserGetterService>();
        services.AddScoped<IUserPatchService, UserPatchService>();
        services.AddScoped<IUserRenameService, UserRenameService>();
        services.AddScoped<IUserUpdatableService, UserUpdatableService>();

        services.AddScoped<IStateGetterService, StateGetterService>();

        return services;
    }

}
