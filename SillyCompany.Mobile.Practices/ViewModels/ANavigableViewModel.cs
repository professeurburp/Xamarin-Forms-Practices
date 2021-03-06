// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ANavigableViewModel.cs" company="The Silly Company">
//   The Silly Company 2016. All rights reserved.
// </copyright>
// <summary>
//   The a navigable view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SillyCompany.Mobile.Practices.ViewModels
{
    using System;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Runtime.CompilerServices;

    using SillyCompany.Mobile.Practices.Annotations;
    using SillyCompany.Mobile.Practices.Services.Navigables;

    /// <summary>
    /// The a navigable view model.
    /// </summary>
    public abstract class ANavigableViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// The navigation service.
        /// </summary>
        protected readonly INavigationService NavigationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ANavigableViewModel"/> class.
        /// </summary>
        /// <param name="navigationService">
        /// The navigation service.
        /// </param>
        protected ANavigableViewModel(INavigationService navigationService)
        {
            this.NavigationService = navigationService;
        }

        /// <summary>
        /// The property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// The load.
        /// </summary>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        public virtual void Load(object parameter)
        {
        }

        /// <summary>
        /// The raise property changed.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        [NotifyPropertyChangedInvocator]
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// The raise property changed.
        /// </summary>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <exception cref="ArgumentException">
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// </exception>
        public void RaisePropertyChanged<T>(Expression<Func<T>> expression)
        {
            if (expression == null)
            {
                throw new ArgumentException("Getting property name form expression is not supported for this type.");
            }

            var lamda = expression as LambdaExpression;
            if (lamda == null)
            {
                throw new NotSupportedException("Getting property name form expression is not supported for this type.");
            }

            var memberExpression = lamda.Body as MemberExpression;
            if (memberExpression != null)
            {
                this.RaisePropertyChanged(memberExpression.Member.Name);
                return;
            }

            var unary = lamda.Body as UnaryExpression;
            var member = unary?.Operand as MemberExpression;
            if (member != null)
            {
                this.RaisePropertyChanged(member.Member.Name);
                return;
            }

            throw new NotSupportedException("Getting property name form expression is not supported for this type.");
        }

        /// <summary>
        /// The set and raise.
        /// </summary>
        /// <param name="property">
        /// The property.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        protected bool SetAndRaise<T>(ref T property, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(property, value))
            {
                return false;
            }

            property = value;
            this.RaisePropertyChanged(propertyName);
            return true;
        }
    }
}