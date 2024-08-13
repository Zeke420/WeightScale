using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Windows;
using System.Windows.Markup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WeightScale.BusinessLogicLayer.Models;
using WeightScale.BusinessLogicLayer.Services;
using WeightScale.BusinessLogicLayer.Utils;
using WeightScale.DataAccessLayer.Contexts;
using WeightScale.DataAccessLayer.Repository;
using WeightScale.DataAccessLayer.Repository.Implementation;
using WeightScale.Integration.Fixtures.Scale;
using WeightScale.Presentation.Helpers;
using WeightScale.Presentation.Resources.Constants;
using WeightScale.Presentation.Services;
using WeightScale.Presentation.Services.Interfaces;
using WeightScale.Presentation.ViewModel;

namespace WeightScale.Presentation
{
    public partial class App
    {
#if DEBUG
        private static readonly string AppSettingsFileName = GlobalResource.DevSettings;
#else
    private static readonly string  AppSettingsFileName = GlobalResource.ProdSettings;
#endif

        public IServiceProvider ServiceProvider { get; private set; }
        private const string CurrentCultureInfo = "en-US";

        protected override void OnStartup(StartupEventArgs e)
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(CurrentCultureInfo);
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfoByIetfLanguageTag(CurrentCultureInfo);
            FrameworkElement.LanguageProperty.OverrideMetadata(
                                                               typeof(FrameworkElement),
                                                               new FrameworkPropertyMetadata(
                                                                XmlLanguage.GetLanguage(CultureInfo.CurrentUICulture
                                                                        .IetfLanguageTag)));
            var buildConfiguration = BuildConfiguration();
            var serviceProvider = ConfigureServices(buildConfiguration);

            serviceProvider.GetRequiredService<MainWindow>()
                           .Show();
        }

        private IConfiguration BuildConfiguration()
        {
            var builder = new ConfigurationBuilder()
                          .SetBasePath(Directory.GetCurrentDirectory())
                          .AddJsonFile(AppSettingsFileName, false, true);

            return builder.Build();
        }

        private IServiceProvider ConfigureServices(IConfiguration configuration)
        {
            var services = new ServiceCollection();
            var applicationSettings = configuration.Get<ApplicationSettings>() ??
                                      throw new ArgumentNullException(nameof(ApplicationSettings));

            var resourceManager = new ResourceManager("WeightScale.BusinessLogicLayer.Resources",
                                                      Assembly.GetExecutingAssembly());

            services.Configure<ApplicationSettings>(configuration);

            services.AddSingleton(new WeightScaleDbContext());

            services.AddSingleton<ViewModelLocator>();
            services.AddTransient<MainWindow>();

            services.AddSingleton<MainViewModel>();
            services.AddTransient<HeaderViewModel>();
            services.AddTransient<CourierViewModel>();
            services.AddTransient<ShipmentViewModel>();
            services.AddSingleton<WeightViewModel>();
            services.AddTransient<ReportViewModel>();
            services.AddSingleton<FooterViewModel>();

            services.AddSingleton<Func<Type, ViewModelBase>>(serviceProvider =>
                                                                     viewModelType =>
                                                                             (ViewModelBase)serviceProvider
                                                                                     .GetRequiredService(viewModelType));

            services.AddTransient<IPackageRepository, PackageRepository>();
            services.AddTransient<ICouriersRepository, CouriersRepository>();
            services.AddTransient<IShipmentRepository, ShipmentRepository>();
            services.AddTransient<IDialogService, DialogService>();

            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<IFileExportService, FileExportService>();
            services.AddSingleton<IMessenger, Messenger>();

            services.AddSingleton<IDeviceManager, DeviceManager>();
            services.AddSingleton<IPackageService, PackageService>();

            services.AddTransient<IScaleDevice, ScaleDevice>();
            services.AddTransient<ICourierService, CourierService>();
            services.AddTransient<IShipmentService, ShipmentService>();
            services.AddTransient<IWeightService, WeightService>();

            ServiceProvider = services.BuildServiceProvider();
            return ServiceProvider;
        }
    }
}