using TimeTracker.Data;

namespace TimeTracker
{
    public partial class App : Application
    {
        IServiceProvider _serviceProvider;
        AppShell _shell;
        public App(IServiceProvider serviceProvider)
        //public App(AppShell shell) // this breaks color
        {
            _serviceProvider = serviceProvider;
            _shell = _serviceProvider.GetRequiredService<AppShell>();
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
             return new Window(_shell);
        }
    }
}