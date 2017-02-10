using System.IO;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DocumentManager.NET.Core.Documents
{
    public class Document : ObjectBase
    {
        #region Fields

        /// <summary>
        /// The _path
        /// </summary>
        private string _path;
        /// <summary>
        /// The _name
        /// </summary>
        private string _name;
        /// <summary>
        /// The _is selected
        /// </summary>
        private bool _isSelected;
        /// <summary>
        /// The _tags
        /// </summary>
        private string _tag;
        /// <summary>
        /// The _icon
        /// </summary>
        private ImageSource _icon;

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the icon.
        /// </summary>
        /// <value>The icon.</value>
        public ImageSource Icon
        {
            get
            {
                if (_icon == null && File.Exists(_path))
                {
                    using (var sysicon = System.Drawing.Icon.ExtractAssociatedIcon(_path))
                    {
                        _icon = Imaging.CreateBitmapSourceFromHIcon(
                            sysicon.Handle,
                            Int32Rect.Empty,
                            BitmapSizeOptions.FromEmptyOptions());
                    }
                }

                return _icon;
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

        /// <summary>
        /// Gets or sets a value indicating whether this instance is selected.
        /// </summary>
        /// <value><c>true</c> if this instance is selected; otherwise, <c>false</c>.</value>
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                RaisePropertyChanged(() => IsSelected);
            }
        }
        /// <summary>
        /// Gets or sets the path.
        /// </summary>
        /// <value>The path.</value>
        public string Path
        {
            get { return _path; }
            set
            {
                _path = value;
                RaisePropertyChanged(() => Path);
            }
        }
        /// <summary>
        /// Gets or sets the tags.
        /// </summary>
        /// <value>The tags.</value>
        public string Tag
        {
            get { return _tag; }
            set
            {
                _tag = value;
                RaisePropertyChanged(() => Tag);
            }
        }

        #endregion
    }
}
