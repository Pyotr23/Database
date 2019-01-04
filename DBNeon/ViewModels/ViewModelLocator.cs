/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:DBNeon"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using System.Windows;
using CommonServiceLocator;
using DBNeon.ViewModels;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;

namespace DBNeon.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            ////if (ViewModelBase.IsInDesignModeStatic)
            ////{
            ////    // Create design time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DesignDataService>();
            ////}
            ////else
            ////{
            ////    // Create run time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DataService>();
            ////}

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<VMTypes>();
            SimpleIoc.Default.Register<VMLocations>();
            SimpleIoc.Default.Register<VMProcurements>();
            SimpleIoc.Default.Register<VMAddRename>(); 
            SimpleIoc.Default.Register<VMBlocks>();
            SimpleIoc.Default.Register<VMBlockChange>();
            SimpleIoc.Default.Register<VMAddMoving>();
            SimpleIoc.Default.Register<VMJournal>();
            SimpleIoc.Default.Register<VMTable>();

            Messenger.Default.Register<NotificationMessageAction<MessageBoxResult>>(this, Asking);
        }

        public void Asking(NotificationMessageAction<MessageBoxResult> notificationMessageAction)
        {
            string notificationMessage = notificationMessageAction.Notification;
            if (notificationMessage.StartsWith("Точно удалить блок с номером"))
                notificationMessageAction.Execute(MessageBox.Show(notificationMessage, "Подтверждение удаления", MessageBoxButton.OKCancel));
        }

        public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();

        public static void Cleanup()
        {
            //SimpleIoc.Default.Reset();            
        }
    }
}