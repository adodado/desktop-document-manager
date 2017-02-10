using System.Collections.Generic;

namespace DocumentManager.NET.Core.Documents
{
    public class DocumentCollection : ObjectBase
    {
        #region Fields

        /// <summary>
        /// The _name
        /// </summary>
        private string _name;
        /// <summary>
        /// The _documents
        /// </summary>
        private List<Document> _documents;

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the documents.
        /// </summary>
        /// <value>The documents.</value>
        public List<Document> Documents
        {
            get { return _documents; }
            set
            {
                _documents = value;
                RaisePropertyChanged(() => Documents);
            }
        }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChanged(() => Name);
            }
        }

        #endregion
    }
}
