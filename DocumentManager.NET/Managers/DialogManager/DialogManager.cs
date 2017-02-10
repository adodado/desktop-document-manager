using DocumentManager.NET.Views.Backups;
using DocumentManager.NET.Views.Collections;
using DocumentManager.NET.Views.Settings;
using System;
using System.Linq;
using System.Windows;

namespace DocumentManager.NET.Managers.DialogManager
{
    public class DialogManager : IDialogManager
    {
        #region Methods

        /// <summary>
        /// Shows a dialog.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="caption">The caption.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool? ShowDialog(string message, string caption)
        {
            bool? returnValue = null;

            var result = MessageBox.Show(message, caption, MessageBoxButton.YesNoCancel);

            switch (result)
            {
                case MessageBoxResult.Yes:
                    returnValue = true;
                    break;
                case MessageBoxResult.No:
                    returnValue = false;
                    break;
            }

            return returnValue;
        }

        /// <summary>
        /// Shows the create new collections view.
        /// </summary>
        /// <param name="callback">The callback.</param>
        public void ShowNewCollections(Action callback)
        {
            var window = TryGetWindow(typeof(NewCollectionView));

            if (window == null)
            {
                window = new NewCollectionView();

                window.Closed += (s, e) =>
                {
                    if (callback != null)
                        callback();
                };

                window.Show();
            }
            else
            {
                window.Activate();
            }
        }

        /// <summary>
        /// Shows the backup all indexed documents view.
        /// </summary>
        /// <param name="callback">The callback.</param>
        public void ShowBackupAllIndexedDocuments(Action callback)
        {
            var window = TryGetWindow(typeof(BackupAllIndexedDocumentsView));

            if (window == null)
            {
                window = new BackupAllIndexedDocumentsView();

                window.Closed += (s, e) =>
                {
                    if (callback != null)
                        callback();
                };

                window.Show();
            }
            else
            {
                window.Activate();
            }
        }

        /// <summary>
        /// Shows the backup collections view.
        /// </summary>
        /// <param name="callback">The callback.</param>
        public void ShowBackupCollections(Action callback)
        {
            var window = TryGetWindow(typeof(BackupCollectionView));

            if (window == null)
            {
                window = new BackupCollectionView();

                window.Closed += (s, e) =>
                {
                    if (callback != null)
                        callback();
                };

                window.Show();
            }
            else
            {
                window.Activate();
            }
        }

        /// <summary>
        /// Shows the backup documents view.
        /// </summary>
        /// <param name="callback">The callback.</param>
        public void ShowBackupDocuments(Action callback)
        {
            var window = TryGetWindow(typeof(BackupDocumentsView));

            if (window == null)
            {
                window = new BackupDocumentsView();

                window.Closed += (s, e) =>
                {
                    if (callback != null)
                        callback();
                };

                window.Show();
            }
            else
            {
                window.Activate();
            }
        }

        /// <summary>
        /// Shows the settings view.
        /// </summary>
        /// <param name="callback">The callback.</param>
        public void ShowSettings(Action callback)
        {
            var window = TryGetWindow(typeof(SettingsView));

            if (window == null)
            {
                window = new SettingsView();

                window.Closed += (s, e) =>
                {
                    if (callback != null)
                        callback();
                };

                window.Show();
            }
            else
            {
                window.Activate();
            }
        }

        /// <summary>
        /// Tries to get an window.
        /// </summary>
        /// <param name="windowType">Type of window.</param>
        /// <returns>Window.</returns>
        private Window TryGetWindow(Type windowType)
        {
            Window returnWindow = null;

            foreach (var window in Application.Current.Windows.Cast<Window>().Where(window => window.GetType() == windowType))
            {
                returnWindow = window;
            }

            return returnWindow;
        }

        #endregion
    }
}
