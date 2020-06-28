using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace EmailClientApplication.Controls
{
	public class EmailCollectionTextBox : TextBox
	{
        private int _indexSelectedElement = -1;
        private const string WrapPanelName = "EmailItems";
        private const string DefaultWatermark = "Enter email...";
	    private const string ContentHostName = "PART_ContentHost";
        private ContentControl _contentControl;
        private WrapPanel _wrapPanel;


        /// <summary>
        /// The watermark text property
        /// </summary>
		public static readonly DependencyProperty WatermarkTextProperty = DependencyProperty.Register("WatermarkText", typeof(string), typeof(EmailCollectionTextBox), new UIPropertyMetadata(string.Empty));

        /// <summary>
        /// The observer property key
        /// </summary>
        public static readonly DependencyPropertyKey ObserverPropertyKey = DependencyProperty.RegisterReadOnly("Items", typeof(ObservableCollection<string>), typeof(EmailCollectionTextBox), new FrameworkPropertyMetadata(new ObservableCollection<string>()));

        /// <summary>
        /// The items property
        /// </summary>
        public static readonly DependencyProperty ItemsProperty = ObserverPropertyKey.DependencyProperty;

        /// <summary>
        /// Get or set the watermark text.
        /// </summary>
        /// <value>
        /// The watermark text.
        /// </value>
        public string WatermarkText { get { return (string)GetValue(WatermarkTextProperty); } set { SetValue(WatermarkTextProperty, value); } }

        /// <summary>
        /// Get the email items.
        /// </summary>
        /// <value>
        /// The email items.
        /// </value>
        public ObservableCollection<string> Items { get { return (ObservableCollection<string>)GetValue(ItemsProperty); } }


		/// <summary>
		/// Initializes a new instance of the <see cref="EmailCollectionTextBox"/> class with default watermark text.
		/// </summary>
		public EmailCollectionTextBox(): this(DefaultWatermark) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="EmailCollectionTextBox"/> class.
		/// </summary>
		/// <param name="watermark">The watermark to show when value is <c>null</c> or empty.</param>
		public EmailCollectionTextBox(string watermark)
		{
			WatermarkText = watermark;
            SetValue(ObserverPropertyKey, new ObservableCollection<string>());

            Loaded += delegate 
            {
                _wrapPanel = Template.FindName(WrapPanelName, this) as WrapPanel;
                if (_wrapPanel == null)
                    throw new NullReferenceException(string.Format("WrapPanel '{0}' not founded", WrapPanelName));

                _contentControl = Template.FindName(ContentHostName, this) as ContentControl;
                if (_contentControl == null)
                    throw new NullReferenceException(string.Format("ContentControl '{0}' not founded", ContentHostName));

            };

            LostFocus += EmailCollectionTextBoxLostFocus;
		}

        /// <summary>
        /// Emails the collection text box lost focus.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
	    private void EmailCollectionTextBoxLostFocus(object sender, RoutedEventArgs e)
	    {
	        Check(Key.Enter);
	        Text = string.Empty;
            _indexSelectedElement = -1;
	    }

	    /// <summary>
        /// Вызывается, когда необработанное маршрутизируемое вложенное событие <see cref="E:System.Windows.Input.Keyboard.KeyUp" /> достигает на своем пути элемента, производного от этого класса. Реализуйте этот метод, чтобы добавить для данного события обработчик класса.
        /// </summary>
        /// <param name="e">Предоставляет данные о событии.</param>
	    protected override void OnKeyUp(KeyEventArgs e)
	    {
            base.OnKeyUp(e);
            Check(e.Key);
	    }

        /// <summary>
        /// Check the key pressed.
        /// </summary>
        /// <param name="key">The key.</param>
	    private void Check(Key key)
	    {
            if (key == Key.Enter && !Validation.GetHasError(this))
            {
                CreateNewEmailItem();
                _indexSelectedElement = -1;
            }
	    }


        /// <summary>
        /// Create the new EmailItem.
        /// </summary>
        private void CreateNewEmailItem()
	    {
            if (_wrapPanel.Children.IndexOf(_contentControl) < _wrapPanel.Children.Count - 1)
            {
                _wrapPanel.Children.Remove(_contentControl);
                _wrapPanel.Children.Add(_contentControl);
            }

            if (string.IsNullOrEmpty(Text))
                return;

            //multiply email lines 
            //
            var emails = Text.Trim().Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).Select(s=> s.Trim()).Where(s=> !string.IsNullOrWhiteSpace(s)).ToList();
            foreach (var email in emails)
            {
                var inputTextToUpper = email.ToUpper();
                if (Items.Any(s => s.ToUpper() == inputTextToUpper))
                    continue;
                    
                //Add new email item
                //
                Items.Add(email);
                var emailItem = new EmailItem
                {
                    Title = email
                };

                //subscribe to events
                //
                emailItem.OnRemoveItem += EmailItemOnRemoveItem;
                emailItem.TitleChanged += EmailItemValueChanged;
                emailItem.OnSelected   += EmailItemOnSelected;

                _wrapPanel.Children.Insert(_indexSelectedElement >= 0 ?_indexSelectedElement : _wrapPanel.Children.Count > 0 ? _wrapPanel.Children.Count - 2 : 0, emailItem);
            }

            
            Text = string.Empty;
	    }

	    
        /// <summary>
        /// Emails the item on selected.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void EmailItemOnSelected(object sender, RoutedEventArgs e)
        {
            
            Check(Key.Enter);

            var item = (EmailItem)sender;
            Text += item.Title;

            _indexSelectedElement = _wrapPanel.Children.IndexOf(item);
            _wrapPanel.Children.Remove(_contentControl);
            _wrapPanel.Children.Insert(_indexSelectedElement, _contentControl);

            RemoveItem(item);
        }

        /// <summary>
        /// Email the item value changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="string"/> instance containing the event data.</param>
	    private void EmailItemValueChanged(object sender, RoutedPropertyChangedEventArgs<string> e)
	    {
	        var item = (EmailItem) sender;
	        if (string.IsNullOrEmpty(e.NewValue))
	            RemoveItem(item);

            //Duplicate
            //
	        var newValueTitle = e.NewValue.Trim().ToUpper();
	        if (Items.Any(i => i.ToUpper() == newValueTitle))
	        {
	            RemoveItem(item, e.OldValue);
	            return;
	        }

            //Update item from items 
            //
	        var itemIndex = Items.IndexOf(e.OldValue);
            if (itemIndex >= 0)
                Items[itemIndex] = e.NewValue.Trim();
	    }
        /// <summary>
        /// Emails the item on remove item.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void EmailItemOnRemoveItem(object sender, RoutedEventArgs e)
        {
            RemoveItem((EmailItem)sender);
        }

        /// <summary>
        /// Removes the item.
        /// </summary>
        /// <param name="emailItem">The email item.</param>
	    private void RemoveItem(EmailItem emailItem, string oldValue = null)
	    {
            //unsubscribe from events
            //
            emailItem.OnRemoveItem -= EmailItemOnRemoveItem;
            emailItem.TitleChanged -= EmailItemValueChanged;
            emailItem.OnSelected   -= EmailItemOnSelected;

            //removes
            //
            if (!string.IsNullOrEmpty(oldValue))
            {
                var itemIndex = Items.IndexOf(oldValue);
                if (itemIndex >= 0)
                    Items.Remove(oldValue);
            }
            else
            {
                Items.Remove(emailItem.Title);
            }
            
            _wrapPanel.Children.Remove(emailItem);
	    }
	}
}