using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Net.Http;

namespace mvamobiletodo
{
    sealed partial class MainPage: Page
    {
        private List<TodoItem> items = new List<TodoItem>();

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async Task InsertTodoItem(TodoItem todoItem)
        {
            // This code inserts a new TodoItem into the database. When the operation completes
            // and Mobile Services has assigned an Id, the item is added to the CollectionView
            await App.MobileService.InvokeApiAsync<TodoItem, string>("tasks/", todoItem, HttpMethod.Post, null);
            
            items.Add(todoItem);

            TextInput.Text = "";
            TextInput.Focus(FocusState.Programmatic);
            await RefreshTodoItems();
        }

        private async Task RefreshTodoItems()
        {
            MobileServiceInvalidOperationException exception = null;
            try
            {
                // This code refreshes the entries in the list view by querying the TodoItems table.
                // The query excludes completed TodoItems
                items = await App.MobileService.InvokeApiAsync<List<TodoItem>>("tasks/", HttpMethod.Get, null);
            }
            catch (MobileServiceInvalidOperationException e)
            {
                exception = e;
            }

            if (exception != null)
            {
                await new MessageDialog(exception.Message, "Error loading items").ShowAsync();
            }
            else
            {
                ListItems.ItemsSource = items;
                this.ButtonSave.IsEnabled = true;
            }
        }

        private async Task UpdateCheckedTodoItem(TodoItem item)
        {
            item.Complete = true;

            await App.MobileService.InvokeApiAsync<TodoItem, TodoItem>(string.Format("tasks/{0}", item.Id), item, new HttpMethod("PATCH"), null);	

            items.Remove(item);
            ListItems.Focus(Windows.UI.Xaml.FocusState.Unfocused);
            await RefreshTodoItems();
        }

        private async void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            ButtonRefresh.IsEnabled = false;

            await RefreshTodoItems();

            ButtonRefresh.IsEnabled = true;
        }

        private async void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            var todoItem = new TodoItem { Description = TextInput.Text };
            await InsertTodoItem(todoItem);
        }

        private async void CheckBoxComplete_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            TodoItem item = cb.DataContext as TodoItem;
            await UpdateCheckedTodoItem(item);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            ButtonRefresh.IsEnabled = false;
            ButtonSave.IsEnabled = false;
            TextInput.IsEnabled = false;

            await RefreshTodoItems();

            ButtonRefresh.IsEnabled = true;
            ButtonSave.IsEnabled = true;
            TextInput.IsEnabled = true;
            TextInput.Focus(FocusState.Keyboard);
        }
    }
}
