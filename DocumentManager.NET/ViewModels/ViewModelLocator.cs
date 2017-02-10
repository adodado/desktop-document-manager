using DocumentManager.NET.Logging;
using DocumentManager.NET.Managers.DialogManager;
using DocumentManager.NET.Utils;
using DocumentManager.NET.Utils.Managers.Indexing;
using DocumentManager.NET.ViewModels.Backups;
using DocumentManager.NET.ViewModels.Collections;
using DocumentManager.NET.ViewModels.Indexing;
using DocumentManager.NET.ViewModels.MainWindow;
using DocumentManager.NET.ViewModels.Settings;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace DocumentManager.NET.ViewModels
{
    public class ViewModelLocator
    {
        /// <summary>
        /// Gets the main window viewmodel.
        /// </summary>
        /// <value>The main window viewmodel.</value>
        public MainViewModel MainWindow
        {
            get { return ServiceLocator.Current.GetInstance<MainViewModel>(); }
        }
        /// <summary>
        /// Gets the settings viewmodel.
        /// </summary>
        /// <value>The settings viewmodel.</value>
        public SettingsViewModel Settings
        {
            get { return ServiceLocator.Current.GetInstance<SettingsViewModel>(); }
        }

        public IndexControlViewModel IndexingControl
        {
            get { return ServiceLocator.Current.GetInstance<IndexControlViewModel>(); }
        }

        /// <summary>
        /// Gets the new collection viewmodel.
        /// </summary>
        /// <value>The new collection viewmodel.</value>
        public NewCollectionViewModel NewCollection
        {
            get { return ServiceLocator.Current.GetInstance<NewCollectionViewModel>(); }
        }

        public BackupAllIndexedDocumentsViewModel BackupAllIndexedDocuments
        {
            get { return ServiceLocator.Current.GetInstance<BackupAllIndexedDocumentsViewModel>(); }
        }

        public BackupCollectionViewModel BackupCollections
        {
            get { return ServiceLocator.Current.GetInstance<BackupCollectionViewModel>(); }
        }

        public BackupDocumentsViewModel BackupDocuments
        {
            get { return ServiceLocator.Current.GetInstance<BackupDocumentsViewModel>(); }
        }

        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            if (!SimpleIoc.Default.IsRegistered<IDialogManager>())
                SimpleIoc.Default.Register<IDialogManager, DialogManager>();

            if (!SimpleIoc.Default.IsRegistered<ILoger>())
                SimpleIoc.Default.Register<ILoger, Loger>();

            if (!SimpleIoc.Default.IsRegistered<MainViewModel>())
                SimpleIoc.Default.Register<MainViewModel>();

            if (!SimpleIoc.Default.IsRegistered<IndexControlViewModel>())
                SimpleIoc.Default.Register<IndexControlViewModel>();

            if (!SimpleIoc.Default.IsRegistered<SettingsViewModel>())
                SimpleIoc.Default.Register<SettingsViewModel>();

            if (!SimpleIoc.Default.IsRegistered<NewCollectionViewModel>())
                SimpleIoc.Default.Register<NewCollectionViewModel>();

            if (!SimpleIoc.Default.IsRegistered<BackupAllIndexedDocumentsViewModel>())
                SimpleIoc.Default.Register<BackupAllIndexedDocumentsViewModel>();

            if (!SimpleIoc.Default.IsRegistered<BackupCollectionViewModel>())
                SimpleIoc.Default.Register<BackupCollectionViewModel>();

            if (!SimpleIoc.Default.IsRegistered<BackupDocumentsViewModel>())
                SimpleIoc.Default.Register<BackupDocumentsViewModel>();
        }

        /// <summary>
        /// Cleans up all the resources.
        /// </summary>
        public static void Cleanup()
        {
            ServiceLocator.Current.GetInstance<MainViewModel>().Cleanup();
            ServiceLocator.Current.GetInstance<SettingsViewModel>().Cleanup();
            ServiceLocator.Current.GetInstance<NewCollectionViewModel>().Cleanup();
        }
    }
}