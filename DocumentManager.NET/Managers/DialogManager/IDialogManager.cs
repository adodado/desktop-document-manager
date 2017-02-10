using System;

namespace DocumentManager.NET.Managers.DialogManager
{
    /// <summary>
    /// Interface IDialogManager
    /// </summary>
    public interface IDialogManager
    {
        #region Methods of IDialogManager

        /// <summary>
        /// Shows the dialog.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="caption">The caption.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool? ShowDialog(string message, string caption);

        /// <summary>
        /// Shows the settings view.
        /// </summary>
        /// <param name="callback">The callback.</param>
        void ShowSettings(Action callback);

        /// <summary>
        /// Shows the create new collections view.
        /// </summary>
        /// <param name="callback">The callback.</param>
        void ShowNewCollections(Action callback);

        /// <summary>
        /// Shows the backup all indexed documents view.
        /// </summary>
        /// <param name="callback">The callback.</param>
        void ShowBackupAllIndexedDocuments(Action callback);

        /// <summary>
        /// Shows the backup collections view.
        /// </summary>
        /// <param name="callback">The callback.</param>
        void ShowBackupCollections(Action callback);

        /// <summary>
        /// Shows the backup documents view.
        /// </summary>
        /// <param name="callback">The callback.</param>
        void ShowBackupDocuments(Action callback);

        #endregion
    }
}