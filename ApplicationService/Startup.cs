using Amazon;
using Amazon.SecretsManager;
using Newtonsoft.Json;
using Persistence;
using Services.Abstractions;
using Services;
using Amazon.S3;
using Microsoft.AspNetCore.Http.Features;
using Services.Users;
using Amazon.CognitoIdentityProvider;
using Domain.Models.Users;
using Services.Helpers;
using Domain.Models;
using Services.Dayforce;
using IdentityModel.Client;
using Domain.Models.Dayforce;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;
using ApplicationService.Middlewares;

namespace ApplicationService;

public class Startup
{
    private const string DayforceApiUrlConfig = "DayforceApiUrl";
    private readonly IWebHostEnvironment _env;

    public Startup(IConfiguration configuration, IWebHostEnvironment env)
    {
        Configuration = configuration;
        _env = env;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container
    public void ConfigureServices(IServiceCollection services)
    {
        var configSettings = GetConfigSettings();

        services.AddAuthentication("Bearer")
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters { ValidateAudience = false };
                    options.Authority = configSettings.JwtIssuer;
                    options.RequireHttpsMetadata = false;
                });
        services.AddResponseCompression(o => {
            o.Providers.Add<GzipCompressionProvider>();
            o.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/json" });
        });
        services.Configure<GzipCompressionProviderOptions>(o => {
            o.Level = CompressionLevel.Fastest;
        });

        services.AddPersistence(configSettings.DbConnectionString);
        // This step is to register AwsCognito or other config into DI
        services.AddSingleton(configSettings.AwsCognito);
        services.AddSingleton(configSettings.DayforceConfig);
        services.AddSingleton(configSettings.LegacyDayforceConfig);
        services.AddSingleton(configSettings.SftpDayforceConfig);
        services.AddSingleton(configSettings.TestSftpDayforceConfig);
        services.AddSingleton(configSettings.OtherSecrets);
        services.AddSingleton(configSettings.Config);
        services.AddMemoryCache(); // Add memory cache service
        services.AddSingleton<LocalCacheService>();

        AddRoleBasedServices(services);
        services.AddScoped<IDateTimeHelper, DateTimeHelper>();
        services.AddSingleton<IEncrytionHelper, EncryptionHelper>();
        services.AddScoped<ICognitoService, CognitoService>();
        services.AddScoped<ILoggedInUserService, LoggedInUserService>();
        services.AddScoped<ILoggedInUserRoleService, LoggedInUserRoleService>();
        services.AddTransient<IDayforceSftpClient, DayforceSftpClient>();
        services.AddHttpClient<IDayforceApiClient, DayforceApiClient>()
            .ConfigureHttpClient((sp, httpClient) =>
            {
                var legacyDayforceApiConfig = configSettings.LegacyDayforceConfig;
                httpClient.BaseAddress = new Uri(legacyDayforceApiConfig.LegacyDayforceApiUrl);
                httpClient.Timeout = TimeSpan.FromSeconds(60); // Default it to 60 seconds, Is it too much? Revisit!
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                httpClient.SetBasicAuthentication(legacyDayforceApiConfig.LegacyDayforceApiUsername, legacyDayforceApiConfig.LegacyDayforceApiPassword);
            })
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler()
            {
                AllowAutoRedirect = false,
            })
            .SetHandlerLifetime(TimeSpan.FromMinutes(5)); // Override handler lifetime to 5 mins from 2 mins

        services.AddAWSService<IAmazonS3>();
        services.AddAWSService<IAmazonCognitoIdentityProvider>();
        services.AddLogging();
        services.Configure<FormOptions>(o =>
        {
            o.ValueLengthLimit = int.MaxValue;
            o.MultipartBodyLengthLimit = int.MaxValue;
            o.MemoryBufferThreshold = int.MaxValue;
        });

        services.AddControllers();
        // Form options Configuration for large files
        services.Configure<FormOptions>(options =>
        {
            options.MultipartBodyLengthLimit = long.MaxValue;
            options.MemoryBufferThreshold = int.MaxValue;
            options.ValueLengthLimit = int.MaxValue;
        });
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Application MicroService", Version = "v1" });
            c.ResolveConflictingActions(apiDescription => apiDescription.First());
            c.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Scheme = "bearer"
            });
            c.OperationFilter<AuthenticationRequirementsOperationFilter>();
        });
    }

    private void AddRoleBasedServices(IServiceCollection services)
    {
        AddSuperUserServices(services);
        AddPowerUserServices(services);
        AddCAM(services);
        AddCM(services);
        AddIOA(services);
        AddIIA(services);
        AddUnAuthorizedUserServices(services);
    }

    private void AddIIA(IServiceCollection services)
    {
        services.AddTransient<IReportServices, Services.IIA.ReportServices>();
        services.AddTransient<IReportDeltaService, Services.IIA.ReportDeltaService>();
        services.AddTransient<IGPRIService, Services.IIA.GPRIService>();
        services.AddTransient<IProviderServices, Services.IIA.ProviderServices>();
        services.AddTransient<IPayGroupServices, Services.IIA.PayGroupServices>();
        services.AddTransient<ILegalEntityServices, Services.IIA.LegalEntityServices>();
        services.AddTransient<IClientServices, Services.IIA.ClientServices>();
        services.AddTransient<IPayrollElementServices, Services.IIA.PayrollElementServices>();
        services.AddTransient<ITaxAuthorityServices, Services.IIA.TaxAuthorityServices>();
        services.AddTransient<IPayCalendarService, Services.IIA.PayCalendarService>();
        services.AddTransient<IConfigServices, Services.IIA.ConfigServices>();
        services.AddTransient<IEmployeeService, Services.IIA.EmployeeService>();
        services.AddTransient<IPayslipsService, Services.IIA.PayslipsService>();
        services.AddTransient<ILookupService, Services.IIA.LookupService>();
        services.AddTransient<IJsonReviewService, Services.IIA.JsonReviewService>();
        services.AddTransient<ICspListServices, Services.IIA.CspListServices>();
        // services.AddTransient<IS3Handling, Services.IIA.S3Handling>();
        // services.AddTransient<ISQSHandling, Services.IIA.SQSHandling>();
        services.AddTransient<IDataImportService, Services.IIA.DataImportService>();
        // services.AddTransient<ISelectListHelper, Services.IIA.SelectListHelper>();
        services.AddScoped<IUserService, Services.IIA.UserService>();
        services.AddTransient<IGenericListServices, Services.IIA.GenericListServices>();
        services.AddScoped<IChangeLogBatchService, Services.IIA.ChangeLogBatchService>();
    }

    private void AddIOA(IServiceCollection services)
    {
        services.AddTransient<IReportServices, Services.IOA.ReportServices>();
        services.AddTransient<IReportDeltaService, Services.IOA.ReportDeltaService>();
        services.AddTransient<IGPRIService, Services.IOA.GPRIService>();
        services.AddTransient<IProviderServices, Services.IOA.ProviderServices>();
        services.AddTransient<IPayGroupServices, Services.IOA.PayGroupServices>();
        services.AddTransient<ILegalEntityServices, Services.IOA.LegalEntityServices>();
        services.AddTransient<IClientServices, Services.IOA.ClientServices>();
        services.AddTransient<IPayrollElementServices, Services.IOA.PayrollElementServices>();
        services.AddTransient<ITaxAuthorityServices, Services.IOA.TaxAuthorityServices>();
        services.AddTransient<IPayCalendarService, Services.IOA.PayCalendarService>();
        services.AddTransient<IConfigServices, Services.IOA.ConfigServices>();
        services.AddTransient<IEmployeeService, Services.IOA.EmployeeService>();
        services.AddTransient<IPayslipsService, Services.IOA.PayslipsService>();
        services.AddTransient<ILookupService, Services.IOA.LookupService>();
        services.AddTransient<IJsonReviewService, Services.IOA.JsonReviewService>();
        services.AddTransient<ICspListServices, Services.IOA.CspListServices>();
        // services.AddTransient<IS3Handling, Services.IOA.S3Handling>();
        // services.AddTransient<ISQSHandling, Services.IOA.SQSHandling>();
        services.AddTransient<IDataImportService, Services.IOA.DataImportService>();
        // services.AddTransient<ISelectListHelper, Services.IOA.SelectListHelper>();
        services.AddScoped<IUserService, Services.IOA.UserService>();
        services.AddTransient<IGenericListServices, Services.IOA.GenericListServices>();
        services.AddScoped<IChangeLogBatchService, Services.IOA.ChangeLogBatchService>();
    }

    private void AddCM(IServiceCollection services)
    {
        services.AddTransient<IReportServices, Services.CM.ReportServices>();
        services.AddTransient<IReportDeltaService, Services.CM.ReportDeltaService>();
        services.AddTransient<IGPRIService, Services.CM.GPRIService>();
        services.AddTransient<IProviderServices, Services.CM.ProviderServices>();
        services.AddTransient<IPayGroupServices, Services.CM.PayGroupServices>();
        services.AddTransient<ILegalEntityServices, Services.CM.LegalEntityServices>();
        services.AddTransient<IClientServices, Services.CM.ClientServices>();
        services.AddTransient<IPayrollElementServices, Services.CM.PayrollElementServices>();
        services.AddTransient<ITaxAuthorityServices, Services.CM.TaxAuthorityServices>();
        services.AddTransient<IPayCalendarService, Services.CM.PayCalendarService>();
        services.AddTransient<IConfigServices, Services.CM.ConfigServices>();
        services.AddTransient<IEmployeeService, Services.CM.EmployeeService>();
        services.AddTransient<IPayslipsService, Services.CM.PayslipsService>();
        services.AddTransient<ILookupService, Services.CM.LookupService>();
        services.AddTransient<IJsonReviewService, Services.CM.JsonReviewService>();
        services.AddTransient<ICspListServices, Services.CM.CspListServices>();
        // services.AddTransient<IS3Handling, Services.CM.S3Handling>();
        // services.AddTransient<ISQSHandling, Services.CM.SQSHandling>();
        services.AddTransient<IDataImportService, Services.CM.DataImportService>();
        // services.AddTransient<ISelectListHelper, Services.CM.SelectListHelper>();
        services.AddScoped<IUserService, Services.CM.UserService>();
        services.AddTransient<IGenericListServices, Services.CM.GenericListServices>();
        services.AddScoped<IChangeLogBatchService, Services.CM.ChangeLogBatchService>();
    }

    private void AddCAM(IServiceCollection services)
    {
        services.AddTransient<IReportServices, Services.CAM.ReportServices>();
        services.AddTransient<IReportDeltaService, Services.CAM.ReportDeltaService>();
        services.AddTransient<IGPRIService, Services.CAM.GPRIService>();
        services.AddTransient<IProviderServices, Services.CAM.ProviderServices>();
        services.AddTransient<IPayGroupServices, Services.CAM.PayGroupServices>();
        services.AddTransient<ILegalEntityServices, Services.CAM.LegalEntityServices>();
        services.AddTransient<IClientServices, Services.CAM.ClientServices>();
        services.AddTransient<IPayrollElementServices, Services.CAM.PayrollElementServices>();
        services.AddTransient<ITaxAuthorityServices, Services.CAM.TaxAuthorityServices>();
        services.AddTransient<IPayCalendarService, Services.CAM.PayCalendarService>();
        services.AddTransient<IConfigServices, Services.CAM.ConfigServices>();
        services.AddTransient<IEmployeeService, Services.CAM.EmployeeService>();
        services.AddTransient<IPayslipsService, Services.CAM.PayslipsService>();
        services.AddTransient<ILookupService, Services.CAM.LookupService>();
        services.AddTransient<IJsonReviewService, Services.CAM.JsonReviewService>();
        services.AddTransient<ICspListServices, Services.CAM.CspListServices>();
        // services.AddTransient<IS3Handling, Services.CAM.S3Handling>();
        // services.AddTransient<ISQSHandling, Services.CAM.SQSHandling>();
        services.AddTransient<IDataImportService, Services.CAM.DataImportService>();
        // services.AddTransient<ISelectListHelper, Services.CAM.SelectListHelper>();
        services.AddScoped<IUserService, Services.CAM.UserService>();
        services.AddTransient<IGenericListServices, Services.CAM.GenericListServices>();
        services.AddScoped<IChangeLogBatchService, Services.CAM.ChangeLogBatchService>();
    }

    private void AddUnAuthorizedUserServices(IServiceCollection services)
    {
        services.AddTransient<IReportServices, Services.UnAuthorized.ReportServices>();
        services.AddTransient<IReportDeltaService, Services.UnAuthorized.ReportDeltaService>();
        services.AddTransient<IGPRIService, Services.UnAuthorized.GPRIService>();
        services.AddTransient<IProviderServices, Services.UnAuthorized.ProviderServices>();
        services.AddTransient<IPayGroupServices, Services.UnAuthorized.PayGroupServices>();
        services.AddTransient<ILegalEntityServices, Services.UnAuthorized.LegalEntityServices>();
        services.AddTransient<IClientServices, Services.UnAuthorized.ClientServices>();
        services.AddTransient<IPayrollElementServices, Services.UnAuthorized.PayrollElementServices>();
        services.AddTransient<ITaxAuthorityServices, Services.UnAuthorized.TaxAuthorityServices>();
        services.AddTransient<IPayCalendarService, Services.UnAuthorized.PayCalendarService>();
        services.AddTransient<IConfigServices, Services.UnAuthorized.ConfigServices>();
        services.AddTransient<IEmployeeService, Services.UnAuthorized.EmployeeService>();
        services.AddTransient<IPayslipsService, Services.UnAuthorized.PayslipsService>();
        services.AddTransient<ILookupService, Services.UnAuthorized.LookupService>();
        services.AddTransient<IJsonReviewService, Services.UnAuthorized.JsonReviewService>();
        services.AddTransient<ICspListServices, Services.UnAuthorized.CspListServices>();
        // services.AddTransient<IS3Handling, Services.UnAuthorized.S3Handling>();
        // services.AddTransient<ISQSHandling, Services.UnAuthorized.SQSHandling>();
        services.AddTransient<IDataImportService, Services.UnAuthorized.DataImportService>();
        services.AddScoped<IUserService, Services.UnAuthorized.UserService>();
        services.AddTransient<IGenericListServices,Services.UnAuthorized.GenericListServices>();
        services.AddScoped<IChangeLogBatchService, Services.UnAuthorized.ChangeLogBatchService>();
    }

    private void AddSuperUserServices(IServiceCollection services)
    {
        services.AddTransient<IReportServices, ReportServices>();
        services.AddTransient<IReportDeltaService, ReportDeltaService>();
        services.AddTransient<IGPRIService, GPRIService>();
        services.AddTransient<IProviderServices, ProviderServices>();
        services.AddTransient<IPayGroupServices, PayGroupServices>();
        services.AddTransient<ILegalEntityServices, LegalEntityServices>();
        services.AddTransient<IClientServices, ClientServices>();
        services.AddTransient<IPayrollElementServices, PayrollElementServices>();
        services.AddTransient<ITaxAuthorityServices, TaxAuthorityServices>();
        services.AddTransient<IPayCalendarService, PayCalendarService>();
        services.AddTransient<IConfigServices, ConfigServices>();
        services.AddTransient<IEmployeeService, EmployeeService>();
        services.AddTransient<IPayslipsService, PayslipsService>();
        services.AddTransient<ILookupService, LookupService>();
        services.AddTransient<IJsonReviewService, JsonReviewService>();
        services.AddTransient<IS3Handling, S3Handling>();
        services.AddTransient<ISQSHandling, SQSHandling>();
        services.AddTransient<IDataImportService, DataImportService>();
        services.AddTransient<ISelectListHelper, SelectListHelper>();
        services.AddTransient<IReportServiceHelper, ReportServiceHelper>();
        services.AddScoped<IUserService, UserService>();
        services.AddTransient<IGenericListServices, GenericListServices>();
        services.AddTransient<ICspListServices, CspListServices>();
        services.AddScoped<IChangeLogBatchService, ChangeLogBatchService>();
    }

    private void AddPowerUserServices(IServiceCollection services)
    {

        services.AddTransient<IReportServices, Services.PowerUser.ReportServices>();
        services.AddTransient<IReportDeltaService, Services.PowerUser.ReportDeltaService>();
        services.AddTransient<IGPRIService, Services.PowerUser.GPRIService>();
        services.AddTransient<IProviderServices, Services.PowerUser.ProviderServices>();
        services.AddTransient<IPayGroupServices, Services.PowerUser.PayGroupServices>();
        services.AddTransient<ILegalEntityServices, Services.PowerUser.LegalEntityServices>();
        services.AddTransient<IClientServices, Services.PowerUser.ClientServices>();
        services.AddTransient<IPayrollElementServices, Services.PowerUser.PayrollElementServices>();
        services.AddTransient<ITaxAuthorityServices, Services.PowerUser.TaxAuthorityServices>();
        services.AddTransient<IPayCalendarService, Services.PowerUser.PayCalendarService>();
        services.AddTransient<IConfigServices, Services.PowerUser.ConfigServices>();
        services.AddTransient<IEmployeeService, Services.PowerUser.EmployeeService>();
        services.AddTransient<IPayslipsService, Services.PowerUser.PayslipsService>();
        services.AddTransient<ILookupService, Services.PowerUser.LookupService>();
        services.AddTransient<IJsonReviewService, Services.PowerUser.JsonReviewService>();
        services.AddTransient<ICspListServices, Services.PowerUser.CspListServices>();
        // services.AddTransient<IS3Handling, Services.PowerUser.S3Handling>();
        // services.AddTransient<ISQSHandling, Services.PowerUser.SQSHandling>();
        services.AddTransient<IDataImportService, Services.PowerUser.DataImportService>();
        services.AddScoped<IUserService, Services.PowerUser.UserService>();
        services.AddScoped<IGenericListServices, Services.PowerUser.GenericListServices>();
        services.AddScoped<IChangeLogBatchService, Services.PowerUser.ChangeLogBatchService>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        app.UseMiddleware<LoggingMiddleware>();
        app.UseResponseCompression();
        app.UseHttpsRedirection();
        app.UseCors(x => x
            .WithMethods("GET", "POST", "DELETE", "PUT", "OPTIONS")
            .AllowAnyOrigin()
            .AllowAnyHeader());

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("../swagger/v1/swagger.json", "Application MicroService");
        });

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();

        });
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    private ConfigSettings GetConfigSettings()
    {
        if (_env.IsDevelopment())
        {
            var configSettings = new ConfigSettings
            {
                DayforceConfig = Configuration.GetSection("Dayforce").Get<DayforceConfig>(),
                LegacyDayforceConfig = Configuration.GetSection("LegacyDayforce").Get<LegacyDayforceConfig>(),
                SftpDayforceConfig = Configuration.GetSection("SftpDayforce").Get<SftpDayforceConfig>(),
                TestSftpDayforceConfig = Configuration.GetSection("sftpDayforceTest").Get<TestSftpDayforceConfig>(),
                DbConnectionString = Configuration.GetConnectionString("DbConnectionString") ?? throw new ArgumentNullException(),
                AwsCognito = Configuration.GetSection("AwsCognito").Get<AwsCognito>(),
                Config = Configuration.GetSection("Config").Get<Config>(),
                OtherSecrets = Configuration.GetSection("Other").Get<OtherSecrets>(),
            };

            configSettings.JwtIssuer = $"{Configuration["Jwt:IssuerBase"]}{configSettings.AwsCognito.UserPoolId}";

            return configSettings;
        }

        return GetSecretsAndAmendConfig();
    }

    private ConfigSettings GetSecretsAndAmendConfig()
    {
        var secretId = Environment.GetEnvironmentVariable(Domain.Constants.ENVIRONMENT_VARIABLES_SECRET_KEY);

        if (string.IsNullOrEmpty(secretId))
        {
            throw new ArgumentNullException(secretId);
        }

        var awsSecretManager = new AwsSecretManager(new AmazonSecretsManagerClient(RegionEndpoint.USEast1));

        // This may create thread contention/deadlock but this is designed to be invoked only once so it should be okay.
        var secrets = awsSecretManager.GetSecrets(secretId).Result;

        string databaseConfigJson = Convert.ToString(secrets["DatabaseConfig"]) ?? string.Empty;
        string awsCognitoJson = Convert.ToString(secrets["AwsCognitoConfig"]) ?? string.Empty;
        string dayforceConfigJson = Convert.ToString(secrets["dayforce"]) ?? string.Empty;
        string legacyDayforceConfigJson = Convert.ToString(secrets["legacyDayforce"]) ?? string.Empty;
        string sftpDayforceConfigJson = Convert.ToString(secrets["sftpDayforce"]) ?? string.Empty;
        string testSftpDayforceConfigJson = Convert.ToString(secrets["sftpDayforceTest"]) ?? string.Empty;
        string otherConfigJson = Convert.ToString(secrets["other"]) ?? string.Empty;

        var postgresDatabaseConfig = JsonConvert.DeserializeObject<PostgresDatabaseConfig>(databaseConfigJson) ?? throw new ArgumentNullException(databaseConfigJson);
        var awsCognitoConfig = JsonConvert.DeserializeObject<AwsCognito>(awsCognitoJson) ?? throw new ArgumentNullException(awsCognitoJson);
        var dayforceConfig = JsonConvert.DeserializeObject<DayforceConfig>(dayforceConfigJson) ?? throw new ArgumentNullException(dayforceConfigJson);
        var legacyDayforceConfig = JsonConvert.DeserializeObject<LegacyDayforceConfig>(legacyDayforceConfigJson) ?? throw new ArgumentNullException(legacyDayforceConfigJson);
        var sftpDayforceConfig = JsonConvert.DeserializeObject<SftpDayforceConfig>(sftpDayforceConfigJson) ?? throw new ArgumentNullException(sftpDayforceConfigJson);
        var testSftpDayforceConfig = JsonConvert.DeserializeObject<TestSftpDayforceConfig>(testSftpDayforceConfigJson) ?? throw new ArgumentNullException(testSftpDayforceConfigJson);
        var otherSecretConfig = JsonConvert.DeserializeObject<OtherSecrets>(otherConfigJson) ?? throw new ArgumentNullException(otherConfigJson);

        var S3ReportOutputBucketName = Environment.GetEnvironmentVariable("Config__S3ReportOutputBucketName");
        var S3PayslipBucketName = Environment.GetEnvironmentVariable("Config__S3PayslipBucketName");
        var S3DataImportBucketName = Environment.GetEnvironmentVariable("Config__S3DataImportBucketName");
        var S3ErrorDetailsBucketName = Environment.GetEnvironmentVariable("Config__S3ErrorDetailsBucketName");
        var S3SerializedGpriBucketName = Environment.GetEnvironmentVariable("Config__S3SerializedGpriBucketName");
        var S3TempPayslipBucketName = Environment.GetEnvironmentVariable("Config__S3TempPayslipBucketName");
        var defaultTimeZone = $"{Configuration["Config:DefaultTimeZone"]}";

        return new ConfigSettings()
        {
            DayforceConfig = dayforceConfig,
            LegacyDayforceConfig = legacyDayforceConfig,
            SftpDayforceConfig = sftpDayforceConfig,
            TestSftpDayforceConfig = testSftpDayforceConfig,
            DbConnectionString = postgresDatabaseConfig.ToString(),
            AwsCognito = awsCognitoConfig,
            OtherSecrets = otherSecretConfig,
            Config = new Config()
            {
                S3ReportOutputBucketName = !string.IsNullOrEmpty(S3ReportOutputBucketName)
                                        ? S3ReportOutputBucketName
                                        : "dev-reports-output",
                S3PayslipBucketName = !string.IsNullOrEmpty(S3PayslipBucketName)
                                        ? S3PayslipBucketName
                                        : "ultipay-payslips",
                S3DataImportBucketName = !string.IsNullOrEmpty(S3DataImportBucketName)
                                        ? S3DataImportBucketName
                                        : "ultipay-dataimport-default",
                S3ErrorDetailsBucketName = !string.IsNullOrEmpty(S3ErrorDetailsBucketName)
                                        ? S3ErrorDetailsBucketName
                                        : "ultipay-errordetails-default",
                S3SerializedGpriBucketName = !string.IsNullOrEmpty(S3SerializedGpriBucketName)
                                        ? S3SerializedGpriBucketName
                                        : "ultipay-gprijson-default",
                DefaultTimeZone = string.IsNullOrEmpty(defaultTimeZone) ? "America/Bogota" : defaultTimeZone,
                S3TempPayslipBucketName = !string.IsNullOrEmpty(S3TempPayslipBucketName)
                                        ? S3TempPayslipBucketName
                                        : "dev-ultipay-tempupload-payslips",
            },
            JwtIssuer = $"{Configuration["Jwt:IssuerBase"]}{awsCognitoConfig.UserPoolId}"
        };
    }
}

public class ConfigSettings
{
    public Config Config { get; set; }
    public DayforceConfig DayforceConfig { get; set; }
    public LegacyDayforceConfig LegacyDayforceConfig { get; set; }
    public SftpDayforceConfig SftpDayforceConfig { get; set; }
    public TestSftpDayforceConfig TestSftpDayforceConfig { get; set; }
    public OtherSecrets OtherSecrets { get; set; }
    public string DbConnectionString { get; set; }
    public AwsCognito AwsCognito { get; set; }
    public string JwtIssuer { get; set; }
}
