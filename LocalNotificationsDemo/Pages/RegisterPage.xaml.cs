using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace LocalNotificationsDemo.Pages;

public partial class RegisterPage : ContentPage
{
    public string Username { get; set; }
    
    public  string Password { get; set; }
    
    public  string Email { get; set; }
    
    public string ConfirmPassword { get; set; }
    
    public RegisterPage()
    {
        InitializeComponent();
        BindingContext = this;
    }

    private void Register_OnClicked(object? sender, EventArgs e)
    {
        
    }
}