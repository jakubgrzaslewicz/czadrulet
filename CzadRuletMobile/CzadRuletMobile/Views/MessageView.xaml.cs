using System;
using System.ComponentModel;
using CzadRuletMobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CzadRuletMobile.Views
{
    public partial class MessageView : Label
    {
       // public MessageViewModel _viewModel;

        public MessageView(string username, string message)
        {
           // BindingContext = _viewModel = new MessageViewModel(username, message);
            
        }

    }
}