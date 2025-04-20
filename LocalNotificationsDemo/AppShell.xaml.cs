using LocalNotificationsDemo.Service;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace LocalNotificationsDemo
{
    public partial class AppShell : Shell
    {
        private readonly IAuthService _authService;

        public AppShell()
        {
            _authService = IPlatformApplication.Current.Services.GetRequiredService<IAuthService>();;
            InitializeComponent();
        
            // Подписываемся на событие навигации
            Navigating += OnNavigating;
        }

        private void OnNavigating(object sender, ShellNavigatingEventArgs e)
        {
            // Получаем целевой маршрут (убираем начальные //)
            var route = e.Target.Location.OriginalString.TrimStart('/');
        
            // Если пользователь не авторизован и пытается попасть на главную страницу
            if (!_authService.IsAuthenticated && route == "MainPage")
            {
                // Отменяем навигацию
                e.Cancel();
            
                // Перенаправляем на страницу входа
                Shell.Current.GoToAsync("//Login");
            }
        
            // Если пользователь авторизован и пытается попасть на страницу входа/регистрации
            if (_authService.IsAuthenticated && (route == "Login" || route == "Register"))
            {
                e.Cancel();
                Shell.Current.GoToAsync("//MainPage");
            }
        }
    }
}
