﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventToCommandBehavior.cs" company="The Silly Company">
//   The Silly Company 2016. All rights reserved.
// </copyright>
// <summary>
//   https://github.com/davidbritch/xamarin-forms/blob/master/ItemSelectedBehavior/ItemSelectedBehavior/Behaviors/EventToCommandBehavior.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SillyCompany.Mobile.Practices.Behaviors
{
    using System;
    using System.Reflection;
    using System.Windows.Input;

    using Xamarin.Forms;

    /// <summary>
    /// The event to command behavior.
    /// </summary>
    public class EventToCommandBehavior : BehaviorBase<View>
    {
        /// <summary>
        /// The event name property.
        /// </summary>
        public static readonly BindableProperty EventNameProperty = BindableProperty.Create(
            "EventName", 
            typeof(string), 
            typeof(EventToCommandBehavior), 
            null, 
            propertyChanged: OnEventNameChanged);

        /// <summary>
        /// The command property.
        /// </summary>
        public static readonly BindableProperty CommandProperty = BindableProperty.Create(
            "Command", 
            typeof(ICommand), 
            typeof(EventToCommandBehavior), 
            null);

        /// <summary>
        /// The command parameter property.
        /// </summary>
        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(
            "CommandParameter", 
            typeof(object), 
            typeof(EventToCommandBehavior), 
            null);

        /// <summary>
        /// The input converter property.
        /// </summary>
        public static readonly BindableProperty InputConverterProperty = BindableProperty.Create(
            "Converter", 
            typeof(IValueConverter), 
            typeof(EventToCommandBehavior), 
            null);

        /// <summary>
        /// The event handler.
        /// </summary>
        private Delegate eventHandler;

        /// <summary>
        /// Gets or sets the event name.
        /// </summary>
        public string EventName
        {
            get { return (string)this.GetValue(EventNameProperty); }
            set { this.SetValue(EventNameProperty, value); }
        }

        /// <summary>
        /// Gets or sets the command.
        /// </summary>
        public ICommand Command
        {
            get { return (ICommand)this.GetValue(CommandProperty); }
            set { this.SetValue(CommandProperty, value); }
        }

        /// <summary>
        /// Gets or sets the command parameter.
        /// </summary>
        public object CommandParameter
        {
            get { return this.GetValue(CommandParameterProperty); }
            set { this.SetValue(CommandParameterProperty, value); }
        }

        /// <summary>
        /// Gets or sets the converter.
        /// </summary>
        public IValueConverter Converter
        {
            get { return (IValueConverter)this.GetValue(InputConverterProperty); }
            set { this.SetValue(InputConverterProperty, value); }
        }

        /// <summary>
        /// The on attached to.
        /// </summary>
        /// <param name="bindable">
        /// The bindable.
        /// </param>
        protected override void OnAttachedTo(View bindable)
        {
            base.OnAttachedTo(bindable);
            this.RegisterEvent(this.EventName);
        }

        /// <summary>
        /// The on detaching from.
        /// </summary>
        /// <param name="bindable">
        /// The bindable.
        /// </param>
        protected override void OnDetachingFrom(View bindable)
        {
            base.OnDetachingFrom(bindable);
            this.DeregisterEvent(this.EventName);
        }

        /// <summary>
        /// The register event.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <exception cref="ArgumentException">
        /// </exception>
        private void RegisterEvent(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return;
            }

            EventInfo eventInfo = this.AssociatedObject.GetType().GetRuntimeEvent(name);
            if (eventInfo == null)
            {
                throw new ArgumentException($"EventToCommandBehavior: Can't register the '{this.EventName}' event.");
            }

            MethodInfo methodInfo = typeof(EventToCommandBehavior).GetTypeInfo().GetDeclaredMethod("OnEvent");
            this.eventHandler = methodInfo.CreateDelegate(eventInfo.EventHandlerType, this);
            eventInfo.AddEventHandler(this.AssociatedObject, this.eventHandler);
        }

        /// <summary>
        /// The deregister event.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <exception cref="ArgumentException">
        /// </exception>
        private void DeregisterEvent(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return;
            }

            if (this.eventHandler == null)
            {
                return;
            }

            EventInfo eventInfo = this.AssociatedObject.GetType().GetRuntimeEvent(name);
            if (eventInfo == null)
            {
                throw new ArgumentException($"EventToCommandBehavior: Can't de-register the '{this.EventName}' event.");
            }

            eventInfo.RemoveEventHandler(this.AssociatedObject, this.eventHandler);
            this.eventHandler = null;
        }        

        /// <summary>
        /// The on event name changed.
        /// </summary>
        /// <param name="bindable">
        /// The bindable.
        /// </param>
        /// <param name="oldValue">
        /// The old value.
        /// </param>
        /// <param name="newValue">
        /// The new value.
        /// </param>
        private static void OnEventNameChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var behavior = (EventToCommandBehavior)bindable;
            if (behavior.AssociatedObject == null)
            {
                return;
            }

            string oldEventName = (string)oldValue;
            string newEventName = (string)newValue;

            behavior.DeregisterEvent(oldEventName);
            behavior.RegisterEvent(newEventName);
        }
    }
}