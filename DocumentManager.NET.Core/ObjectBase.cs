using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DocumentManager.NET.Core
{
    public class ObjectBase : Object, INotifyPropertyChanged
    {
        #region Classes

        public sealed class ObjectIsChangedEventHolder
        {
            #region Events

            public event EventHandler IsChanged;

            #endregion

            #region Fields

            readonly ObjectBase _obj;

            #endregion

            #region Constructor

            /// <summary>
            /// Initializes a new instance of the <see cref="ObjectIsChangedEventHolder"/> class.
            /// </summary>
            /// <param name="obj">The object.</param>
            public ObjectIsChangedEventHolder(ObjectBase obj)
            {
                _obj = obj;
            }

            #endregion

            #region Methods

            /// <summary>
            /// Raises the ischanged event.
            /// </summary>
            public void RaiseIsChanged()
            {
                var tmp = IsChanged;
                if (tmp != null) tmp(_obj, EventArgs.Empty);
            }

            #endregion
        }

        #endregion

        #region Events

        public virtual event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Fields

        readonly Dictionary<string, ObjectIsChangedEventHolder> _eventHandler = new Dictionary<string, ObjectIsChangedEventHolder>();

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectBase"/> class.
        /// </summary>
        public ObjectBase()
        {
            PropertyChanged += Object_PropertyChanged;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Extracts the name of the property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyExpression">The property expression.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="System.ArgumentNullException">propertyExpression</exception>
        /// <exception cref="System.ArgumentException">
        /// The expression is not a member access expression
        /// or
        /// The member access expression does not access a property
        /// or
        /// The referenced property is a static property
        /// </exception>
        static string ExtractPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            if (propertyExpression == null)
            {
                throw new ArgumentNullException("propertyExpression");
            }
            var body = propertyExpression.Body as MemberExpression;
            if (body == null)
            {
                throw new ArgumentException("The expression is not a member access expression");
            }
            var member = body.Member as PropertyInfo;
            if (member == null)
            {
                throw new ArgumentException("The member access expression does not access a property");
            }
            var getMethod = member.GetGetMethod(true);
            if (getMethod.IsStatic)
            {
                throw new ArgumentException("The referenced property is a static property");
            }
            return body.Member.Name;
        }

        /// <summary>
        /// Handles the PropertyChanged event of the NotificationObjectWithEvent control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        void Object_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            foreach (var item in _eventHandler.Where(s => s.Key == e.PropertyName))
                item.Value.RaiseIsChanged();
        }

        /// <summary>
        /// Properties the specified property expression.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyExpression">The property expression.</param>
        /// <returns>XObjectIsChangedEventHolder.</returns>
        public ObjectIsChangedEventHolder Property<T>(Expression<Func<T>> propertyExpression)
        {
            var propName = ExtractPropertyName(propertyExpression);
            if (_eventHandler.ContainsKey(propName)) return _eventHandler[propName];

            var nh = new ObjectIsChangedEventHolder(this);
            _eventHandler.Add(propName, nh);
            return nh;
        }

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected virtual void RaisePropertyChanged(string propertyName)
        {
            var propertyChanged = PropertyChanged;
            if (propertyChanged != null)
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <param name="propertyNames">The property names.</param>
        /// <exception cref="System.ArgumentNullException">propertyNames</exception>
        protected void RaisePropertyChanged(params string[] propertyNames)
        {
            if (propertyNames == null)
                throw new ArgumentNullException("propertyNames");
            var strArrays = propertyNames;
            foreach (var str in strArrays)
                RaisePropertyChanged(str);
        }

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyExpression">The property expression.</param>
        protected void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            var str = ExtractPropertyName<T>(propertyExpression);
            RaisePropertyChanged(str);
        }

        #endregion
    }
}
