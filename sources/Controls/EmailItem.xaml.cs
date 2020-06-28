using System.Windows;
using System.Windows.Input;

namespace EmailClientApplication.Controls
{
    /// <summary>
    /// Логика взаимодействия для EmailItem.xaml
    /// </summary>
    public partial class EmailItem
    {
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title { get { return GetValue(TitleProperty) as string; } set { SetValue(TitleProperty, value); } }

        /// <summary>
        /// The title property
        /// </summary>
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(EmailItem), new FrameworkPropertyMetadata(null, PropertyChangedCallback));

        /// <summary>
        /// Properties the changed callback.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="dependencyPropertyChangedEventArgs">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var item = ((EmailItem)dependencyObject);

            if (string.IsNullOrEmpty(dependencyPropertyChangedEventArgs.NewValue as string))
            {
                item.RaiseEvent(new RoutedEventArgs(OnRemoveItemEvent));
                return;
            }

            if (dependencyPropertyChangedEventArgs.OldValue as string != dependencyPropertyChangedEventArgs.NewValue as string)
                item.RaiseEvent(new RoutedPropertyChangedEventArgs<string>(dependencyPropertyChangedEventArgs.OldValue as string, dependencyPropertyChangedEventArgs.NewValue as string,
                    TitleChangedEvent));            
        }
           
        /// <summary>
        /// Initializes a new instance of the <see cref="EmailItem"/> class.
        /// </summary>
        public EmailItem()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the OnClick event of the RemoveButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void RemoveButton_OnClick(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(OnRemoveItemEvent));
        }

        /// <summary>
        /// Handles the OnMouseLeftButtonDown event of the UIElement control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
           if (e.ClickCount == 2)
               RaiseEvent(new RoutedEventArgs(OnSelectedEvent));                
        }

        /// <summary>
        /// The title changed event
        /// </summary>
        public static readonly RoutedEvent TitleChangedEvent = EventManager.RegisterRoutedEvent("TitleChanged", RoutingStrategy.Direct, typeof(RoutedPropertyChangedEventHandler<string>), typeof(EmailItem));

        /// <summary> 
        /// Occurs when the Value property changes. 
        /// </summary> 
        public event RoutedPropertyChangedEventHandler<string> TitleChanged
        {
            add { AddHandler(TitleChangedEvent, value); }
            remove { RemoveHandler(TitleChangedEvent, value); }
        }

        /// <summary>
        /// The on selected event
        /// </summary>
        public static readonly RoutedEvent OnSelectedEvent = EventManager.RegisterRoutedEvent("OnSelected", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(EmailItem));

        /// <summary>
        /// Occurs when [on selected].
        /// </summary>
        public event RoutedEventHandler OnSelected
        {
            add { AddHandler(OnSelectedEvent, value); }
            remove { RemoveHandler(OnSelectedEvent, value); }
        }



        /// <summary>
        /// The on remove item event
        /// </summary>
        public static readonly RoutedEvent OnRemoveItemEvent = EventManager.RegisterRoutedEvent("OnRemoveItem", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(EmailItem));

        /// <summary>
        /// Occurs when [on remove item].
        /// </summary>
        public event RoutedEventHandler OnRemoveItem
        {
            add { AddHandler(OnRemoveItemEvent, value); }
            remove { RemoveHandler(OnRemoveItemEvent, value); }
        }
    }
}
